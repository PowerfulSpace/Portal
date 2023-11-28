using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PS.Portal.DAL.Interfaces;
using PS.Portal.Domain.Entities;
using PS.Portal.Domain.Enums;
using PS.Portal.Domain.Extensions;
using PS.Portal.Domain.Models;

namespace PS.Portal.Web.Controllers
{
    [Authorize]
    public class MovieController : Controller
    {
        private readonly IWebHostEnvironment _webHost;
        private readonly IMovie _movieRepository;
        private readonly ICountry _countryRepository;
        private readonly IProducer _producerRepository;
        private readonly IActor _actorRepository;
        private readonly IGenre _genreRepository;
        private readonly IReview _reviewRepository;

        public MovieController(
            IWebHostEnvironment webHost,
            IMovie movieRepository,
            ICountry countryRepository,
            IProducer producerRepository,
            IActor actorRepository,
            IGenre genreRepository,
            IReview reviewRepository)
        {
            _webHost = webHost;
            _movieRepository = movieRepository;
            _countryRepository = countryRepository;
            _producerRepository = producerRepository;
            _actorRepository = actorRepository;
            _genreRepository = genreRepository;
            _reviewRepository = reviewRepository;
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

        [HttpGet]
        public IActionResult Create()
        {
            var movie = new Movie();
            PopulateViewBagsAsync().GetAwaiter().GetResult();
            return View(movie);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Movie movie)
        {
            var bolret = false;
            string errMessage = "";

            try
            {
                if (movie.Description.Length < 2 || movie.Description == null)
                    errMessage = "Movie Description Must be atleast 2 Characters";

                if (_movieRepository.IsItemNameExists(movie.Name) == true)
                    errMessage = errMessage + " " + " Movie Name " + movie.Name + " Exists Already";

                if (errMessage == "")
                {

                    string uniqueFileName = GetUploadedFileName(movie);
                    if (uniqueFileName != null)
                        movie.PhotoUrl = uniqueFileName;

                    movie = await _movieRepository.GreateAsync(movie);
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

                return View(movie);
            }
            else
            {
                TempData["SuccessMessage"] = "Movie " + movie.Name + " Created Successfully";
                return RedirectToAction(nameof(Index));
            }
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

            await PopulateViewBagsAsync();

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
            var bolret = false;
            string errMessage = "";

            try
            {
                if (movie.Description.Length < 2 || movie.Description == null)
                    errMessage = "Movie Description Must be atleast 2 Characters";

                if (_movieRepository.IsItemNameExists(movie.Name, movie.Id) == true)
                    errMessage = errMessage + " " + " Movie Name " + movie.Name + " Exists Already";

                if (errMessage == "")
                {

                    if (movie.MoviePhoto != null)
                    {
                        string oldFile = movie.PhotoUrl;

                        string uniqueFileName = GetUploadedFileName(movie);
                        if (uniqueFileName != null)
                            movie.PhotoUrl = uniqueFileName;

                        if (oldFile != "noimage.png")
                        {
                            DeleteUnusedFile(movie, oldFile);
                        }
                    }

                    movie = await _movieRepository.EditAsync(movie);
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

                return View(movie);
            }
            else
            {
                TempData["SuccessMessage"] = "Movie " + movie.Name + " Created Successfully";
                return RedirectToAction(nameof(Index), new { currentPage = currentPage, pageSize = pageSize, searchText = TempData.Peek("SearchText") });
            }
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
            catch(Exception ex)
            {
                string errMessage = ex.Message;
                TempData["ErrorMessage"] = errMessage;
                ModelState.AddModelError("", errMessage);
                return View(movie);
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
            ViewBag.Countries = await GetCountriesAsync();
            ViewBag.Reviews = await GetReviewsAsync();
            ViewBag.Genres = await GetGenresAsync();
            ViewBag.Actors = await GetActorsAsync();
            ViewBag.Producers = await GetProducersAsync();
        }

        private async Task<List<SelectListItem>> GetCountriesAsync()
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

        private async Task<List<SelectListItem>> GetReviewsAsync()
        {
            List<SelectListItem> listIItems = new List<SelectListItem>();

            PaginatedList<Review> items = await _reviewRepository.GetItemsAsync("login", SortOrder.Ascending, "", 1, 1000);

            listIItems = items.Select(x => new SelectListItem()
            {
                Text = x.Login,
                Value = x.Id.ToString()
            }).ToList();

            SelectListItem defItem = new SelectListItem()
            {
                Text = "---Select Review---",
                Value = ""
            };

            listIItems.Insert(0, defItem);
            return listIItems;
        }

        private async Task<List<SelectListItem>> GetGenresAsync()
        {
            List<SelectListItem> listIItems = new List<SelectListItem>();

            PaginatedList<Genre> items = await _genreRepository.GetItemsAsync("name", SortOrder.Ascending, "", 1, 1000);

            listIItems = items.Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Id.ToString()
            }).ToList();

            SelectListItem defItem = new SelectListItem()
            {
                Text = "---Select Genre---",
                Value = ""
            };

            listIItems.Insert(0, defItem);
            return listIItems;
        }

        private async Task<List<SelectListItem>> GetActorsAsync()
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

        private async Task<List<SelectListItem>> GetProducersAsync()
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

        private string GetUploadedFileName(Movie movie)
        {
            string uniqueFileName = string.Empty;

            if (movie.MoviePhoto != null)
            {
                string typetModel = Helper.GetTypeName(movie.GetType().ToString()).ToLower();

                string uploadsFolder = Path.Combine(_webHost.WebRootPath, "images", "photos", typetModel);
                uniqueFileName = Guid.NewGuid().ToString() + "_" + movie.MoviePhoto.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    movie.MoviePhoto.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }

        private void DeleteUnusedFile(Movie movie, string fileName)
        {
            string typetModel = Helper.GetTypeName(movie.GetType().ToString()).ToLower();

            string fullPatch = Path.Combine(_webHost.WebRootPath, "images", "photos", typetModel, fileName);

            System.IO.File.Delete(fullPatch);
        }
    }
}
