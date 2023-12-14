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

            //IFormFile file;

            //using (var fileStream = new FileStream(movie.PhotoUrl, FileMode.OpenOrCreate))
            //{
            //    file = new FormFile(fileStream, 0, fileStream.Length, "MoviePhoto", movie.PhotoUrl);
            //}

            //movie.MoviePhoto = file;

            PopulateViewBagsAsync().GetAwaiter().GetResult();
            return View(movie);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Movie movie, List<Guid> genres, List<Guid> actors, List<Guid> reviews)
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

                    if (movie.MoviePhoto != null)
                    {
                        string uniqueFileName = GetUploadedFileName(movie);
                        if (uniqueFileName != null)
                            movie.PhotoUrl = uniqueFileName;
                    }
                      
                    movie = await AddGenresAsync(movie, genres);
                    movie = await AddActorsAsync(movie, actors);

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

                await PopulateViewBagsAsync(genres, actors, reviews);

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

            //if (movie.MoviePhoto == null)
            //{
            //    GetDefaultFileName(movie);
            //}

            await PopulateViewBagsAsync(movie.Genres.Select(x => x.Id).ToList(), movie.Actors.Select(x => x.Id).ToList(), movie.Reviews.Select(x => x.Id).ToList());

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
        public async Task<IActionResult> Edit(Movie movie, List<Guid> genres, List<Guid> actors, List<Guid> reviews)
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
                        DeleteUnusedFile(movie, oldFile);
                    }

                  
                    if (genres != null)
                        movie = await EditGenresAsync(movie, genres);
                    if (actors != null)
                        movie = await EditActorsAsync(movie, actors);
                    if (reviews != null)
                        movie = await EditReviewsAsync(movie, reviews);

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

                await PopulateViewBagsAsync(genres, actors, reviews);

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
            catch (Exception ex)
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

        private async Task PopulateViewBagsAsync(List<Guid> genersId = null!, List<Guid> actorsId = null!, List<Guid> reviewsId = null!)
        {
            if (genersId == null && actorsId == null && reviewsId == null)
            {
                ViewBag.Genres = await GetGenresAsync();
                ViewBag.Actors = await GetActorsAsync();
                ViewBag.Reviews = await GetReviewsAsync();
            }
            else
            {
                ViewBag.Reviews = await GetReviewsAsync(reviewsId);
                ViewBag.Genres = await GetGenresAsync(genersId);
                ViewBag.Actors = await GetActorsAsync(actorsId);
            }


            ViewBag.Countries = await GetCountriesAsync();
            ViewBag.Producers = await GetProducersAsync();
        }



        #region Методы для подгрузки связанных данных из таблиц

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

        private async Task<List<SelectListItem>> GetReviewsAsync(List<Guid> itemId)
        {
            List<SelectListItem> listIItems = new List<SelectListItem>();

            PaginatedList<Review> items = await _reviewRepository.GetItemsAsync("login", SortOrder.Ascending, "", 1, 1000);

            listIItems = items.Select(x => new SelectListItem()
            {
                Text = x.Login,
                Value = x.Id.ToString(),
                Selected = itemId.Contains(x.Id)
            }).ToList();

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

        private async Task<List<SelectListItem>> GetGenresAsync(List<Guid> itemId)
        {
            List<SelectListItem> listIItems = new List<SelectListItem>();

            PaginatedList<Genre> items = await _genreRepository.GetItemsAsync("name", SortOrder.Ascending, "", 1, 1000);

            listIItems = items.Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Id.ToString(),
                Selected = itemId.Contains(x.Id)
            }).ToList();

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

        private async Task<List<SelectListItem>> GetActorsAsync(List<Guid> itemId)
        {
            List<SelectListItem> listIItems = new List<SelectListItem>();

            PaginatedList<Actor> items = await _actorRepository.GetItemsAsync("lastname", SortOrder.Ascending, "", 1, 1000);

            listIItems = items.Select(x => new SelectListItem()
            {
                Text = x.LastName,
                Value = x.Id.ToString(),
                Selected = itemId.Contains(x.Id)
            }).ToList();

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

        private async Task<Movie> AddGenresAsync(Movie movie, List<Guid> genres)
        {
            List<Genre> allGenres = await _genreRepository.GetItemsAsync("name", SortOrder.Ascending, "", 1, 1000);
            List<Genre> genresList = new List<Genre>();


            foreach (var genre in genres)
            {
                genresList.AddRange(allGenres.Where(x => x.Id == genre).ToList());
            }
            movie.Genres.AddRange(genresList);

            return movie;
        }

        private async Task<Movie> AddActorsAsync(Movie movie, List<Guid> actors)
        {
            List<Actor> allActors = await _actorRepository.GetItemsAsync("name", SortOrder.Ascending, "", 1, 1000);
            List<Actor> actorsList = new List<Actor>();


            foreach (var actor in actors)
            {
                actorsList.AddRange(allActors.Where(x => x.Id == actor).ToList());
            }
            movie.Actors.AddRange(actorsList);

            return movie;
        }

        #endregion






        #region Методы для Редактирование Жанров,Актёров, Рецензий во время редактирования фильма

        private async Task<Movie> EditGenresAsync(Movie movie, List<Guid> genres)
        {
            //Выбираем фильм до изменения, вытакскиваем все жанры которые были изначально
            Movie movieBeforeChange = await _movieRepository.GetItemAsync(movie.Id);
            List<Genre> genreBeforeChange = movieBeforeChange.Genres;

            //Выбираем сначала все жанры, что бы выбрать по пришедшим id жанры которые выбранны сейчас
            List<Genre> allGenres = await _genreRepository.GetItemsAsync("name", SortOrder.Ascending, "", 1, 1000);
            List<Genre> genreAfterChange = new List<Genre>();
            foreach (var genre in genres)
            {
                genreAfterChange.AddRange(allGenres.Where(x => x.Id == genre).ToList());
            }

            //Выбираю те жанры которые были до изменения, и их не стало после изменения, что бы
            //Удалить их из таблицы на для коллекции фильмов
            List<Genre> genresToDelete = genreBeforeChange.Except(genreAfterChange).ToList();

            //Выбираю жанры которые были до измения, и которые остались после, что бы ни какие
            //действия с ними не делать
            List<Genre> genresUnchanged = genreBeforeChange.Intersect(genreAfterChange).ToList();

            //Выбираю жанры которые нужно будет добавить в коллекию к фильму
            List<Genre> genresToAdd = genreAfterChange.Except(genresUnchanged).ToList();

            //Изменения ссылки на фотографию надо по нескольким причинам.
            //1)movieBeforeChange это наша модель до изменения фотографии, там старая фотография
            //2)если этого не сделать, фотографии будут пладиться. т.к не будет ссылки что удалить старую
            movieBeforeChange.PhotoUrl = movie.PhotoUrl;
            movie = movieBeforeChange;
            if (genresToDelete != null)
            {
                foreach (var item in genresToDelete)
                {
                    movie.Genres.Remove(item);
                }
            }

            if (genresToAdd != null)
            {
                movie.Genres.AddRange(genresToAdd);
            }

            return movie;
        }

        private async Task<Movie> EditActorsAsync(Movie movie, List<Guid> actros)
        {

            Movie movieBeforeChange = await _movieRepository.GetItemAsync(movie.Id);
            List<Actor> actorsBeforeChange = movieBeforeChange.Actors;


            List<Actor> allActors = await _actorRepository.GetItemsAsync("name", SortOrder.Ascending, "", 1, 1000);
            List<Actor> actorsAfterChange = new List<Actor>();
            foreach (var actor in actros)
            {
                actorsAfterChange.AddRange(allActors.Where(x => x.Id == actor).ToList());
            }


            List<Actor> actorsToDelete = actorsBeforeChange.Except(actorsAfterChange).ToList();

            List<Actor> actorsUnchanged = actorsBeforeChange.Intersect(actorsAfterChange).ToList();


            List<Actor> actorsToAdd = actorsAfterChange.Except(actorsUnchanged).ToList();

            movieBeforeChange.PhotoUrl = movie.PhotoUrl;
            movie = movieBeforeChange;
            if (actorsToDelete != null)
            {
                foreach (var item in actorsToDelete)
                {
                    movie.Actors.Remove(item);
                }
            }

            if (actorsToAdd != null)
            {
                movie.Actors.AddRange(actorsToAdd);
            }

            return movie;
        }


        private async Task<Movie> EditReviewsAsync(Movie movie, List<Guid> reviews)
        {

            Movie movieBeforeChange = await _movieRepository.GetItemAsync(movie.Id);
            List<Review> reviewsBeforeChange = movieBeforeChange.Reviews;


            List<Review> allReviews = await _reviewRepository.GetItemsAsync("name", SortOrder.Ascending, "", 1, 1000);
            List<Review> reviewsAfterChange = new List<Review>();
            foreach (var review in reviews)
            {
                reviewsAfterChange.AddRange(allReviews.Where(x => x.Id == review).ToList());
            }


            List<Review> reviewsToDelete = reviewsBeforeChange.Except(reviewsAfterChange).ToList();

            List<Review> reviewsUnchanged = reviewsBeforeChange.Intersect(reviewsAfterChange).ToList();


            List<Review> reviewsToAdd = reviewsAfterChange.Except(reviewsUnchanged).ToList();


            movieBeforeChange.PhotoUrl = movie.PhotoUrl;
            movie = movieBeforeChange;
            if (reviewsToDelete != null)
            {
                foreach (var item in reviewsToDelete)
                {
                    movie.Reviews.Remove(item);
                }
            }

            if (reviewsToAdd != null)
            {
                movie.Reviews.AddRange(reviewsToAdd);
            }

            return movie;
        }

        #endregion






        #region Методы для Редактирование фотографий

        //private void GetDefaultFileName(Movie movie)
        //{

        //    using (var stream = System.IO.File.OpenRead("noimage.png"))
        //    {
        //        var file = new FormFile(stream, 0, stream.Length, "MoviePhoto", Path.GetFileName(stream.Name))
        //        {
        //            Headers = new HeaderDictionary(),
        //            ContentType = "application/png"
        //        };

        //        movie.MoviePhoto = file;
        //    }

        //    string uniqueFileName = string.Empty;

        //    string typetModel = Helper.GetTypeName(movie.GetType().ToString()).ToLower();

        //    string uploadsFolder = Path.Combine(_webHost.WebRootPath, "images", "photos", typetModel);
        //    uniqueFileName = Guid.NewGuid().ToString() + "_" + movie.MoviePhoto.FileName;
        //    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

        //    using (var fileStream = new FileStream(filePath, FileMode.Create))
        //    {
        //        movie.MoviePhoto.CopyTo(fileStream);
        //    }
        //}


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

        #endregion


    }
}
