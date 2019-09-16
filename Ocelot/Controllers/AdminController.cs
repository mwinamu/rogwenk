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
    public class AdminController : Controller
    {
        [OcelotAuthorizationAttribute(Roles = "Administrator,SuperAdministrator,CanViewAdmin")]
        public ActionResult Index()
        {
            return View();
        }
        [OcelotAuthorizationAttribute(Roles = "Administrator,SuperAdministrator,CanViewUsers")]
        public JsonResult UserDropDowns()
        {
            return Json(AdminSQL.userDropDowns(),JsonRequestBehavior.AllowGet);
        }
    }
}