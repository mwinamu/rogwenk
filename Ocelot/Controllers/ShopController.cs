using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ocelot.Models;
using System.Web.Http;
using Newtonsoft.Json;
using System.Data.SqlClient;
using Ocelot.Service;
using Ocelot.Logging;
using Ocelot.SQLHELPER;
using HttpGetAttribute = System.Web.Http.HttpGetAttribute;
using HttpPostAttribute = System.Web.Http.HttpPostAttribute;

namespace Ocelot.Controllers
{
    public class ShopController : Controller
    {
        [OcelotAuthorizationAttribute(Roles = "Administrator,SuperAdministrator,CanViewShop")]
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
        [OcelotAuthorizationAttribute(Roles = "Administrator,SuperAdministrator,CanGetShop")]
        [HttpGet]
        public JsonResult GetsShop()
        {
            return Json(ShopSQL.GetAssignedShop().ToList(),JsonRequestBehavior.AllowGet);
        }
        [OcelotAuthorizationAttribute(Roles = "Administrator,SuperAdministrator,CanGetShop")]
        [HttpGet]
        public JsonResult GetUnAssignedShop()
        {
            return Json(ShopSQL.GetUnAssignedShop(), JsonRequestBehavior.AllowGet);
        }
        [OcelotAuthorizationAttribute(Roles = "Administrator,SuperAdministrator,CanAddShop")]
        [HttpPost]
        public JsonResult AddShop([FromBody]ShopModel shop)
        {
            string AddedBy = Constant.GetUserID();
            return Json(ShopSQL.AddShop(shop,AddedBy), JsonRequestBehavior.AllowGet);
        }
        [OcelotAuthorizationAttribute(Roles = "Administrator,SuperAdministrator,CanAssignShop")]
        [HttpPost]
        public JsonResult AssignShop([FromBody]AssignShop shop)
        {
            string AddedBy = Constant.GetUserID();
            return Json(ShopSQL.AssignShop(shop, AddedBy), JsonRequestBehavior.AllowGet);
        }
        [OcelotAuthorizationAttribute(Roles = "Administrator,SuperAdministrator,CanEditShop")]
        [HttpPost]
        public JsonResult EditShop([FromBody]ShopModel shop)
        {
            string UpdatedBy = Constant.GetUserID();
            return Json(ShopSQL.EditShop(shop, UpdatedBy), JsonRequestBehavior.AllowGet);
        }
    }
}