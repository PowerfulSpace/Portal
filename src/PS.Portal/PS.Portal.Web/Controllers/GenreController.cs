using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PS.Portal.DAL.Interfaces;
using PS.Portal.Domain.Entities;
using PS.Portal.Domain.Enums;
using PS.Portal.Domain.Models;

namespace PS.Portal.Web.Controllers
{
    public class GenreController : Controller
    {
        private readonly IGenre _genreRepository;
        private readonly IMovie _movieRepository;

        public GenreController(IGenre genreRepository, IMovie movieRepository)
        {
            _genreRepository = genreRepository;
            _movieRepository = movieRepository;
        }
        public async Task<IActionResult> Index(string sortExpression = "", string searchText = "", int currentPage = 1, int pageSize = 5)
        {
            SortModel sortModel = new SortModel();
            sortModel.AddColumn("name");
            sortModel.AddColumn("description");
            sortModel.ApplySort(sortExpression);

            PaginatedList<Genre> genres = await _genreRepository.GetItemsAsync(sortModel.SortedProperty, sortModel.SortedOrder, searchText, currentPage, pageSize);

            var pager = new PagerModel(genres.TotalRecords, currentPage, pageSize);
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

            return View(genres);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var genre = new Genre();
            PopulateViewBagsAsync().GetAwaiter().GetResult();
            return View(genre);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Genre genre)
        {
            var bolret = false;
            string errMessage = "";

            try
            {
                if (genre.Description.Length < 4 || genre.Description == null)
                    errMessage = "Genre Description Must be atleast 4 Characters";

                if (_genreRepository.IsItemNameExists(genre.Name) == true)
                    errMessage = errMessage + " " + " Genre Name " + genre.Name + " Exists Already";

                if (errMessage == "")
                {
                    genre = await _genreRepository.GreateAsync(genre);
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

                return View(genre);
            }
            else
            {
                TempData["SuccessMessage"] = "Genre " + genre.Name + " Created Successfully";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var genre = await _genreRepository.GetItemAsync(id);

            TempData.Keep("CurrentPage");
            TempData.Keep("PageSize");
            TempData.Keep("SearchText");

            if (genre != null)
            {
                return View(genre);
            }

            return NotFound();
        }


        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var genre = await _genreRepository.GetItemAsync(id);

            await PopulateViewBagsAsync();

            TempData.Keep("CurrentPage");
            TempData.Keep("PageSize");
            TempData.Keep("SearchText");

            if (genre != null)
            {
                return View(genre);
            }

            return NotFound();
        }


        [HttpPost]
        public async Task<IActionResult> Edit(Genre genre)
        {
            var bolret = false;
            string errMessage = "";

            try
            {
                if (genre.Description.Length < 4 || genre.Description == null)
                    errMessage = "Genre Description Must be atleast 4 Characters";

                if (_genreRepository.IsItemNameExists(genre.Name, genre.Id) == true)
                    errMessage = errMessage + " " + " Genre Name " + genre.Name + " Exists Already";

                if (errMessage == "")
                {
                    genre = await _genreRepository.EditAsync(genre);
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

                return View(genre);
            }
            else
            {
                TempData["SuccessMessage"] = "Genre " + genre.Name + " Created Successfully";
                return RedirectToAction(nameof(Index), new { currentPage = currentPage, pageSize = pageSize, searchText = TempData.Peek("SearchText") });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            var genre = await _genreRepository.GetItemAsync(id);

            TempData.Keep("CurrentPage");
            TempData.Keep("PageSize");
            TempData.Keep("SearchText");

            if (genre != null)
            {
                return View(genre);
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Genre genre)
        {
            try
            {
                genre = await _genreRepository.DeleteAsync(genre);
            }
            catch (Exception ex)
            {
                string errMessage = ex.Message;
                TempData["ErrorMessage"] = errMessage;
                ModelState.AddModelError("", errMessage);
                return View(genre);
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
    }
}
