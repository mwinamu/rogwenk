using System.Web.Mvc;

namespace Ocelot.Controllers
{
    public class ErrorController : Controller
    {
        [HttpGet]
        public ActionResult InternalServerError()
        {
            return View();
        }

        [HttpGet]
        public ActionResult NotFound()
        {
            return View();
        }
        [HttpGet]
        public ActionResult AccesDenied()
        {
            return View();
        }
    }
}
