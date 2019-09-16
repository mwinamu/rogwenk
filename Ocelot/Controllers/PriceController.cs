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

namespace Ocelot.Controllers
{
    public class PriceController : Controller
    {
        [OcelotAuthorizationAttribute(Roles = "Administrator,SuperAdministrator,CanViewPrice")]
        public ActionResult Index()
        {
            return View();
        }
        [OcelotAuthorizationAttribute(Roles = "Administrator,SuperAdministrator,CanGetPrice")]
        public JsonResult GetPrices()
        {
            return Json(PriceSQL.GetPrices(), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [OcelotAuthorizationAttribute(Roles = "Administrator,SuperAdministrator,CanAddPrice")]
        public JsonResult AddPrice([FromBody]PriceModel price)
        {
            string AddedBy = Constant.GetUserID();
            return Json(PriceSQL.AddPrice(price, AddedBy), JsonRequestBehavior.AllowGet);
        }
    }
}