using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PS.Portal.DAL.Interfaces;
using PS.Portal.Domain.Entities;
using PS.Portal.Domain.Enums;
using PS.Portal.Domain.Models;

namespace PS.Portal.Web.Controllers
{
    public class CountryController : Controller
    {
        private readonly ICountry _countryRepository;
        private readonly IMovie _movieRepository;
        private readonly IActor _actorRepository;
        private readonly IProducer _producerRepository;

        public CountryController(
            ICountry countryRepository,
            IMovie movieRepository,
            IActor actorRepository,
            IProducer producerRepository)
        {
            _countryRepository = countryRepository;
            _movieRepository = movieRepository;
            _actorRepository = actorRepository;
            _producerRepository = producerRepository;
        }
        public async Task<IActionResult> Index(string sortExpression = "", string searchText = "", int currentPage = 1, int pageSize = 5)
        {
            SortModel sortModel = new SortModel();
            sortModel.AddColumn("name");
            sortModel.ApplySort(sortExpression);

            PaginatedList<Country> countries = await _countryRepository.GetItemsAsync(sortModel.SortedProperty, sortModel.SortedOrder, searchText, currentPage, pageSize);

            var pager = new PagerModel(countries.TotalRecords, currentPage, pageSize);
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

            return View(countries);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var country = new Country();
            PopulateViewBagsAsync().GetAwaiter().GetResult(); ;
            return View(country);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Country country)
        {
            var bolret = false;
            string errMessage = "";

            try
            {
                if (_countryRepository.IsItemNameExists(country.Name) == true)
                    errMessage = errMessage + " " + " Country Name " + country.Name + " Exists Already";

                if (errMessage == "")
                {
                    country = await _countryRepository.GreateAsync(country);
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

                await PopulateViewBagsAsync();

                return View(country);
            }
            else
            {
                TempData["SuccessMessage"] = "Country " + country.Name + " Created Successfully";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var country = await _countryRepository.GetItemAsync(id);

            TempData.Keep("CurrentPage");
            TempData.Keep("PageSize");
            TempData.Keep("SearchText");

            if (country != null)
            {
                return View(country);
            }

            return NotFound();
        }


        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var country = await _countryRepository.GetItemAsync(id);

            await PopulateViewBagsAsync();

            TempData.Keep("CurrentPage");
            TempData.Keep("PageSize");
            TempData.Keep("SearchText");

            if (country != null)
            {
                return View(country);
            }

            return NotFound();
        }


        [HttpPost]
        public async Task<IActionResult> Edit(Country country)
        {
            var bolret = false;
            string errMessage = "";

            try
            {
                if (_countryRepository.IsItemNameExists(country.Name, country.Id) == true)
                    errMessage = errMessage + " " + " Country Name " + country.Name + " Exists Already";

                if (errMessage == "")
                {
                    country = await _countryRepository.EditAsync(country);
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

                await PopulateViewBagsAsync();

                return View(country);
            }
            else
            {
                TempData["SuccessMessage"] = "Country " + country.Name + " Created Successfully";
                return RedirectToAction(nameof(Index), new { currentPage = currentPage, pageSize = pageSize, searchText = TempData.Peek("SearchText") });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            var country = await _countryRepository.GetItemAsync(id);

            TempData.Keep("CurrentPage");
            TempData.Keep("PageSize");
            TempData.Keep("SearchText");

            if (country != null)
            {
                return View(country);
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Country country)
        {
            try
            {
                country = await _countryRepository.DeleteAsync(country);
            }
            catch (Exception ex)
            {
                string errMessage = ex.Message;
                TempData["ErrorMessage"] = errMessage;
                ModelState.AddModelError("", errMessage);
                return View(country);
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

        private async Task PopulateViewBagsAsync()
        {
            ViewBag.Movies = await GetMoviesAsync();
        }

        private async Task<List<SelectListItem>> GetMoviesAsync()
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

        private async Task<List<SelectListItem>> GetActors()
        {
            List<SelectListItem> listIItems = new List<SelectListItem>();

            PaginatedList<Actor> items = await _actorRepository.GetItemsAsync("lastName", SortOrder.Ascending, "", 1, 1000);

            listIItems = items.Select(x => new SelectListItem()
            {
                Text = x.LastName,
                Value = x.Id.ToString()
            }).ToList();

            SelectListItem defItem = new SelectListItem()
            {
                Text = "---Select Actor---",
                Value = ""
            };

            listIItems.Insert(0, defItem);
            return listIItems;
        }

        private async Task<List<SelectListItem>> GetProducers()
        {
            List<SelectListItem> listIItems = new List<SelectListItem>();

            PaginatedList<Producer> items = await _producerRepository.GetItemsAsync("lastName", SortOrder.Ascending, "", 1, 1000);

            listIItems = items.Select(x => new SelectListItem()
            {
                Text = x.LastName,
                Value = x.Id.ToString()
            }).ToList();

            SelectListItem defItem = new SelectListItem()
            {
                Text = "---Select Producer---",
                Value = ""
            };

            listIItems.Insert(0, defItem);
            return listIItems;
        }
    }
}
