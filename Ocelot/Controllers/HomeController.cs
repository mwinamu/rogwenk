using Ocelot.Service;
using Ocelot.SQLHELPER;
using System;
using System.Web.Mvc;
using System.Web.Security;

namespace Ocelot.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        [OcelotAuthorizationAttribute(Roles = "Administrator")]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult AnotherLink()
        {
            return View("Index");
        }

       
        [HttpGet]
        public FileResult ViewImage()
        {
            try
            {
                Byte[] b;
                string filePath = @"D:\MapIcon\tuk.png";
                b = System.IO.File.ReadAllBytes(filePath);

                return File(b, "image/png");
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
