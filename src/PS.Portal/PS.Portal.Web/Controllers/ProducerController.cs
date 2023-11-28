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
    public class ProducerController : Controller
    {
        private readonly IWebHostEnvironment _webHost;
        private readonly IMovie _movieRepository;
        private readonly ICountry _countryRepository;
        private readonly IProducer _producerRepository;

        public ProducerController(
            IWebHostEnvironment webHost,
            IMovie movieRepository,
            ICountry countryRepository,
            IProducer producerRepository)
        {
            _webHost = webHost;
            _movieRepository = movieRepository;
            _countryRepository = countryRepository;
            _producerRepository = producerRepository;
        }
        public async Task<IActionResult> Index(string sortExpression = "", string searchText = "", int currentPage = 1, int pageSize = 5)
        {
            SortModel sortModel = new SortModel();
            sortModel.AddColumn("firstName");
            sortModel.AddColumn("lastName");
            sortModel.AddColumn("birthDate");
            sortModel.AddColumn("country");
            sortModel.ApplySort(sortExpression);

            PaginatedList<Producer> producers = await _producerRepository.GetItemsAsync(sortModel.SortedProperty, sortModel.SortedOrder, searchText, currentPage, pageSize);

            var pager = new PagerModel(producers.TotalRecords, currentPage, pageSize);
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

            return View(producers);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var producer = new Producer();
            PopulateViewBagsAsync().GetAwaiter().GetResult();
            return View(producer);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Producer producer)
        {
            var bolret = false;
            string errMessage = "";

            try
            {
                if (_producerRepository.IsItemNameExists(producer.LastName,producer.FirstName) == true)
                    errMessage = errMessage + " " + " Producer Name " + producer.LastName + " Exists Already";

                if (errMessage == "")
                {

                    string uniqueFileName = GetUploadedFileName(producer);
                    if (uniqueFileName != null)
                        producer.PhotoUrl = uniqueFileName;

                    producer = await _producerRepository.GreateAsync(producer);
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

                return View(producer);
            }
            else
            {
                TempData["SuccessMessage"] = "Producer " + producer.LastName + " Created Successfully";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var producer = await _producerRepository.GetItemAsync(id);

            TempData.Keep("CurrentPage");
            TempData.Keep("PageSize");
            TempData.Keep("SearchText");

            if (producer != null)
            {
                return View(producer);
            }

            return NotFound();
        }


        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var producer = await _producerRepository.GetItemAsync(id);

            await PopulateViewBagsAsync();

            TempData.Keep("CurrentPage");
            TempData.Keep("PageSize");
            TempData.Keep("SearchText");

            if (producer != null)
            {
                return View(producer);
            }

            return NotFound();
        }


        [HttpPost]
        public async Task<IActionResult> Edit(Producer producer)
        {
            var bolret = false;
            string errMessage = "";

            try
            {

                if (_producerRepository.IsItemNameExists(producer.LastName, producer.FirstName, producer.Id) == true)
                    errMessage = errMessage + " " + " Producer Name " + producer.LastName + " Exists Already";

                if (errMessage == "")
                {

                    if (producer.ProducerPhoto != null)
                    {
                        string oldFile = producer.PhotoUrl;

                        string uniqueFileName = GetUploadedFileName(producer);
                        if (uniqueFileName != null)
                            producer.PhotoUrl = uniqueFileName;

                        if (oldFile != "noimage.png")
                        {
                            DeleteUnusedFile(producer, oldFile);
                        }
                    }

                    producer = await _producerRepository.EditAsync(producer);
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

                return View(producer);
            }
            else
            {
                TempData["SuccessMessage"] = "Producer " + producer.LastName + " Created Successfully";
                return RedirectToAction(nameof(Index), new { currentPage = currentPage, pageSize = pageSize, searchText = TempData.Peek("SearchText") });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            var producer = await _producerRepository.GetItemAsync(id);

            TempData.Keep("CurrentPage");
            TempData.Keep("PageSize");
            TempData.Keep("SearchText");

            if (producer != null)
            {
                return View(producer);
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Producer producer)
        {
            try
            {
                producer = await _producerRepository.DeleteAsync(producer);
            }
            catch (Exception ex)
            {
                string errMessage = ex.Message;
                TempData["ErrorMessage"] = errMessage;
                ModelState.AddModelError("", errMessage);
                return View(producer);
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
            ViewBag.Movies = await GetMoviesAsync();
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

        private string GetUploadedFileName(Producer producer)
        {
            string uniqueFileName = string.Empty;

            if (producer.ProducerPhoto != null)
            {
                string typetModel = Helper.GetTypeName(producer.GetType().ToString()).ToLower();

                string uploadsFolder = Path.Combine(_webHost.WebRootPath, "images", "photos", typetModel);
                uniqueFileName = Guid.NewGuid().ToString() + "_" + producer.ProducerPhoto.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    producer.ProducerPhoto.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }

        private void DeleteUnusedFile(Producer producer, string fileName)
        {
            string typetModel = Helper.GetTypeName(producer.GetType().ToString()).ToLower();

            string fullPatch = Path.Combine(_webHost.WebRootPath, "images", "photos", typetModel, fileName);

            System.IO.File.Delete(fullPatch);
        }
    }
}
