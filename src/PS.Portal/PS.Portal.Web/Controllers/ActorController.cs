using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PS.Portal.DAL.Interfaces;
using PS.Portal.Domain.Entities;
using PS.Portal.Domain.Enums;
using PS.Portal.Domain.Models;

namespace PS.Portal.Web.Controllers
{
    public class ActorController : Controller
    {
        private readonly IWebHostEnvironment _webHost;
        private readonly IMovie _movieRepository;
        private readonly ICountry _countryRepository;
        private readonly IActor _actorRepository;

        public ActorController(
            IWebHostEnvironment webHost,
            IMovie movieRepository,
            ICountry countryRepository,
            IActor actorRepository)
        {
            _webHost = webHost;
            _movieRepository = movieRepository;
            _countryRepository = countryRepository;
            _actorRepository = actorRepository;
        }
        public async Task<IActionResult> Index(string sortExpression = "", string searchText = "", int currentPage = 1, int pageSize = 5)
        {
            SortModel sortModel = new SortModel();
            sortModel.AddColumn("name");
            sortModel.AddColumn("description");
            sortModel.AddColumn("rating");
            sortModel.AddColumn("yearShown");
            sortModel.AddColumn("filmDuration");
            sortModel.AddColumn("acceptableAge");
            sortModel.AddColumn("isReaded");
            sortModel.AddColumn("partFilm");
            sortModel.AddColumn("country");
            sortModel.AddColumn("producer");
            sortModel.ApplySort(sortExpression);

            PaginatedList<Actor> actors = await _actorRepository.GetItemsAsync(sortModel.SortedProperty, sortModel.SortedOrder, searchText, currentPage, pageSize);

            var pager = new PagerModel(actors.TotalRecords, currentPage, pageSize);
            pager.SortExpression = sortExpression;
            pager.SearchText = searchText;

            ViewBag.Pager = pager;

            ViewData["SortModel"] = sortModel;
            ViewBag.SearchText = searchText;

            TempData["SearchText"] = searchText;
            TempData.Keep("SearchText");

            TempData["PageSize"] = pageSize;
            TempData.Keep("PageSize");

            TempData["CurrentPage"] = currentPage;
            TempData.Keep("CurrentPage");

            return View(actors);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var actor = new Actor();
            PopulateViewBags();
            return View(actor);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Actor actor)
        {
            var bolret = false;
            string errMessage = "";

            try
            {
                if (_actorRepository.IsItemNameExists(actor.FirstName, actor.LastName) == true)
                    errMessage = errMessage + " " + " Actor Name " + actor.LastName + " Exists Already";

                if (errMessage == "")
                {

                    string uniqueFileName = GetUploadedFileName(actor);
                    if (uniqueFileName != null)
                        actor.PhotoUrl = uniqueFileName;

                    actor = await _actorRepository.GreateAsync(actor);
                    bolret = true;
                }

            }
            catch (Exception ex)
            {
                errMessage = errMessage + " " + ex.Message;
            }

            if (bolret == false)
            {
                TempData["ErrorMessage"] = errMessage;
                ModelState.AddModelError("", errMessage);

                PopulateViewBags();

                return View(actor);
            }
            else
            {
                TempData["SuccessMessage"] = "Actor " + actor.LastName + " Created Successfully";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var actor = await _actorRepository.GetItemAsync(id);

            TempData.Keep("CurrentPage");
            TempData.Keep("PageSize");
            TempData.Keep("SearchText");

            if (actor != null)
            {
                return View(actor);
            }

            return NotFound();
        }


        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var actor = await _actorRepository.GetItemAsync(id);

            PopulateViewBags();

            TempData.Keep("CurrentPage");
            TempData.Keep("PageSize");
            TempData.Keep("SearchText");

            if (actor != null)
            {
                return View(actor);
            }

            return NotFound();
        }


        [HttpPost]
        public async Task<IActionResult> Edit(Actor actor)
        {
            var bolret = false;
            string errMessage = "";

            try
            {
                if (_actorRepository.IsItemNameExists(actor.FirstName, actor.LastName, actor.Id) == true)
                    errMessage = errMessage + " " + " Actor Name " + actor.LastName + " Exists Already";

                if (errMessage == "")
                {

                    if (actor.ActorPhoto != null)
                    {
                        string uniqueFileName = GetUploadedFileName(actor);
                        if (uniqueFileName != null)
                            actor.PhotoUrl = uniqueFileName;
                    }

                    actor = await _actorRepository.EditAsync(actor);
                    bolret = true;
                }

            }
            catch (Exception ex)
            {
                errMessage = errMessage + " " + ex.Message;
            }


            var currentPage = 1;
            if (TempData["CurrentPage"] != null)
            {
                currentPage = (int)TempData["CurrentPage"]!;
            }

            var pageSize = 5;
            if (TempData["PageSize"] != null)
            {
                pageSize = (int)TempData["PageSize"]!;
            }

            if (bolret == false)
            {
                TempData["ErrorMessage"] = errMessage;
                ModelState.AddModelError("", errMessage);

                PopulateViewBags();

                return View(actor);
            }
            else
            {
                TempData["SuccessMessage"] = "Actor " + actor.LastName + " Created Successfully";
                return RedirectToAction(nameof(Index), new { currentPage = currentPage, pageSize = pageSize, searchText = TempData.Peek("SearchText") });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            var actor = await _actorRepository.GetItemAsync(id);

            TempData.Keep("CurrentPage");
            TempData.Keep("PageSize");
            TempData.Keep("SearchText");

            if (actor != null)
            {
                return View(actor);
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Actor actor)
        {
            try
            {
                actor = await _actorRepository.DeleteAsync(actor);
            }
            catch (Exception ex)
            {
                string errMessage = ex.Message;
                TempData["ErrorMessage"] = errMessage;
                ModelState.AddModelError("", errMessage);
                return View(actor);
            }

            var currentPage = 1;
            if (TempData["CurrentPage"] != null)
            {
                currentPage = (int)TempData["CurrentPage"]!;
            }

            var pageSize = 5;
            if (TempData["PageSize"] != null)
            {
                pageSize = (int)TempData["PageSize"]!;
            }

            return RedirectToAction(nameof(Index), new { currentPage = currentPage, pageSize = pageSize, searchText = TempData.Peek("SearchText") });
        }

        private void PopulateViewBags()
        {
            ViewBag.Genries = GetCountries();
            ViewBag.Movies = GetMovies();
        }

        private async Task<List<SelectListItem>> GetCountries()
        {
            List<SelectListItem> listIItems = new List<SelectListItem>();

            PaginatedList<Country> items = await _countryRepository.GetItemsAsync("name", SortOrder.Ascending, "", 1, 1000);

            listIItems = items.Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Id.ToString()
            }).ToList();

            SelectListItem defItem = new SelectListItem()
            {
                Text = "---Select Country---",
                Value = ""
            };

            listIItems.Insert(0, defItem);
            return listIItems;
        }

        private async Task<List<SelectListItem>> GetMovies()
        {
            List<SelectListItem> listIItems = new List<SelectListItem>();

            PaginatedList<Movie> items = await _movieRepository.GetItemsAsync("name", SortOrder.Ascending, "", 1, 1000);

            listIItems = items.Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Id.ToString()
            }).ToList();

            SelectListItem defItem = new SelectListItem()
            {
                Text = "---Select Movie---",
                Value = ""
            };

            listIItems.Insert(0, defItem);
            return listIItems;
        }

        private string GetUploadedFileName(Actor actor)
        {
            string uniqueFileName = string.Empty;

            if (actor.ActorPhoto != null)
            {
                string uploadsFolder = Path.Combine(_webHost.WebRootPath, "images");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + actor.ActorPhoto.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    actor.ActorPhoto.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }
    }
}
