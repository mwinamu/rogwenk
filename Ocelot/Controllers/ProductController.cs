using Ocelot.Models;
using System.Web.Http;
using Newtonsoft.Json;
using System.Data.SqlClient;
using Ocelot.Service;
using Ocelot.Logging;
using Ocelot.SQLHELPER;
using HttpGetAttribute = System.Web.Http.HttpGetAttribute;
using HttpPostAttribute = System.Web.Http.HttpPostAttribute;
using System.Web.Mvc;
using System.Linq;

namespace Ocelot.Controllers
{
    public class ProductController : Controller
    {

        ProductSQL SQL = new ProductSQL();
        [OcelotAuthorizationAttribute(Roles = "Administrator,SuperAdministrator,CanViewProduct")]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [OcelotAuthorizationAttribute(Roles = "Administrator,SuperAdministrator,CanGetProduct")]
        public JsonResult ProductList()
        {
            var Brands = SQL.GetProductsList().ToList();
            return Json(Brands, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        [OcelotAuthorizationAttribute(Roles = "Administrator,SuperAdministrator,CanGetProduct")]
        public JsonResult ProductDropDown()
        {
            var prods = SQL.GetProductDropDown().ToList();
            return Json(prods, JsonRequestBehavior.AllowGet);
        }
    }
}