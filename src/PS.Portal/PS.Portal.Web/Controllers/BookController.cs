using Microsoft.AspNetCore.Mvc;

namespace PS.Portal.Web.Controllers
{
    public class BookController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
