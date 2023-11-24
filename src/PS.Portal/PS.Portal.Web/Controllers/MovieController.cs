using Microsoft.AspNetCore.Mvc;
using PS.Portal.DAL.Interfaces;
using PS.Portal.Domain.Entities;
using PS.Portal.Domain.Models;

namespace PS.Portal.Web.Controllers
{
    public class MovieController : Controller
    {

        private readonly IMovie _movieRepository;

        public MovieController(IMovie movieRepository)
        {
            _movieRepository = movieRepository;
        }
        public async Task<IActionResult> Index(string sortExpression = "", string searchText = "", int currentPage = 1, int pageSize = 5)
        {
            SortModel sortModel = new SortModel();
            sortModel.AddColumn("name");
            sortModel.AddColumn("description");
            sortModel.AddColumn("rating");
            sortModel.ApplySort(sortExpression);

            PaginatedList<Movie> movies = await _movieRepository.GetItemsAsync(sortModel.SortedProperty, sortModel.SortedOrder, searchText, currentPage, pageSize);

            var pager = new PagerModel(movies.TotalRecords, currentPage, pageSize);
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

            return View(movies);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var movie = new Movie();
            return View(movie);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Movie movie)
        {
            try
            {
                movie = await _movieRepository.GreateAsync(movie);
            }
            catch { }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var movie = await _movieRepository.GetItemAsync(id);
            TempData.Keep("CurrentPage");
            TempData.Keep("PageSize");
            TempData.Keep("SearchText");

            if (movie != null)
            {
                return View(movie);
            }

            return NotFound();
        }


        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var movie = await _movieRepository.GetItemAsync(id);
            TempData.Keep("CurrentPage");
            TempData.Keep("PageSize");
            TempData.Keep("SearchText");

            if (movie != null)
            {
                return View(movie);
            }

            return NotFound();
        }


        [HttpPost]
        public async Task<IActionResult> Edit(Movie movie)
        {
            try
            {
                movie = await _movieRepository.EditAsync(movie);
            }
            catch { }

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

        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            var movie = await _movieRepository.GetItemAsync(id);
            TempData.Keep("CurrentPage");
            TempData.Keep("PageSize");
            TempData.Keep("SearchText");

            if (movie != null)
            {
                return View(movie);
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Movie movie)
        {
            try
            {
                movie = await _movieRepository.DeleteAsync(movie);
            }
            catch { }

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
    }
}
