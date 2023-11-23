using Microsoft.AspNetCore.Mvc;

namespace PS.Portal.Web.Controllers
{
    public class CountryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
