using Microsoft.AspNetCore.Mvc;

namespace PS.Portal.Web.Controllers
{
    public class ActorController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
