using Microsoft.AspNetCore.Mvc;

namespace PS.Portal.Web.Controllers
{
    public class ShowController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
