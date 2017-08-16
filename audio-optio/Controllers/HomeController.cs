using System.Web.Mvc;

using audio_optio.Models;


namespace audio_optio.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Gallery()
        {
            return View();
        }
    }
}