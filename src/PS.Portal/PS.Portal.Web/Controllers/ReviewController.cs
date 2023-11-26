using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PS.Portal.DAL.Interfaces;
using PS.Portal.Domain.Entities;
using PS.Portal.Domain.Enums;
using PS.Portal.Domain.Models;

namespace PS.Portal.Web.Controllers
{
    public class ReviewController : Controller
    {
        private readonly IReview _reviewRepository;
        private readonly IMovie _movieRepository;

        public ReviewController(IReview reviewRepository, IMovie movieRepository)
        {
            _reviewRepository = reviewRepository;
            _movieRepository = movieRepository;
        }
        public async Task<IActionResult> Index(string sortExpression = "", string searchText = "", int currentPage = 1, int pageSize = 5)
        {
            SortModel sortModel = new SortModel();
            sortModel.AddColumn("login");
            sortModel.AddColumn("text");
            sortModel.AddColumn("movie");
            sortModel.ApplySort(sortExpression);

            PaginatedList<Review> reviews = await _reviewRepository.GetItemsAsync(sortModel.SortedProperty, sortModel.SortedOrder, searchText, currentPage, pageSize);

            var pager = new PagerModel(reviews.TotalRecords, currentPage, pageSize);
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

            return View(reviews);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var review = new Review();
            PopulateViewBagsAsync().GetAwaiter().GetResult();
            return View(review);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Review review)
        {
            var bolret = false;
            string errMessage = "";

            try
            {
                if (review.Text.Length < 3 || review.Text == null)
                    errMessage = "Review Text Must be atleast 3 Characters";

                if (errMessage == "")
                {
                    review = await _reviewRepository.GreateAsync(review);
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

                return View(review);
            }
            else
            {
                TempData["SuccessMessage"] = "Review " + review.Login + " Created Successfully";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var review = await _reviewRepository.GetItemAsync(id);

            TempData.Keep("CurrentPage");
            TempData.Keep("PageSize");
            TempData.Keep("SearchText");

            if (review != null)
            {
                return View(review);
            }

            return NotFound();
        }


        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var review = await _reviewRepository.GetItemAsync(id);

            await PopulateViewBagsAsync();

            TempData.Keep("CurrentPage");
            TempData.Keep("PageSize");
            TempData.Keep("SearchText");

            if (review != null)
            {
                return View(review);
            }

            return NotFound();
        }


        [HttpPost]
        public async Task<IActionResult> Edit(Review review)
        {
            var bolret = false;
            string errMessage = "";

            try
            {
                if (review.Text.Length < 3 || review.Text == null)
                    errMessage = "Review Text Must be atleast 3 Characters";

                if (errMessage == "")
                {
                    review = await _reviewRepository.EditAsync(review);
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

                return View(review);
            }
            else
            {
                TempData["SuccessMessage"] = "Review " + review.Login + " Created Successfully";
                return RedirectToAction(nameof(Index), new { currentPage = currentPage, pageSize = pageSize, searchText = TempData.Peek("SearchText") });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            var review = await _reviewRepository.GetItemAsync(id);

            TempData.Keep("CurrentPage");
            TempData.Keep("PageSize");
            TempData.Keep("SearchText");

            if (review != null)
            {
                return View(review);
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Review review)
        {
            try
            {
                review = await _reviewRepository.DeleteAsync(review);
            }
            catch (Exception ex)
            {
                string errMessage = ex.Message;
                TempData["ErrorMessage"] = errMessage;
                ModelState.AddModelError("", errMessage);
                return View(review);
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
