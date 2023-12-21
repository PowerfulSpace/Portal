using Microsoft.AspNetCore.Mvc;
using PS.Portal.DAL.Interfaces;
using PS.Portal.Domain.Entities;
using PS.Portal.Domain.Models;

namespace PS.Portal.Web.Controllers
{
    public class ShowController : Controller
    {
        private readonly IMovie _movieRepository;

        public ShowController(IMovie movieRepository)
        {
            _movieRepository = movieRepository;
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
    }
}
