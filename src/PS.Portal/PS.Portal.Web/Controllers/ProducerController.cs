using Microsoft.AspNetCore.Mvc;

namespace PS.Portal.Web.Controllers
{
    public class ProducerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
