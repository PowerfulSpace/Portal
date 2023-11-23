using Microsoft.AspNetCore.Mvc;

namespace PS.Portal.Web.Controllers
{
    public class GenreController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
