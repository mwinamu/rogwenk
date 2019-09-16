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

namespace Company.WebApplication1.Controllers
{
    public class UserController : Controller
    {
        [OcelotAuthorizationAttribute(Roles = "Administrator,SuperAdministrator,CanViewUser")]
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
        [OcelotAuthorizationAttribute(Roles = "Administrator,SuperAdministrator,CanViewUser")]
        [HttpGet]
        public JsonResult GetUsers()
        {
            return Json(UsersSQL.GetUsers(),JsonRequestBehavior.AllowGet);
        }
        [OcelotAuthorizationAttribute(Roles = "Administrator,SuperAdministrator,CanViewUser")]
        [HttpGet]
        public JsonResult GetNewUsers()
        {
            return Json(UsersSQL.GetNewUsers());
        }
        [OcelotAuthorizationAttribute(Roles = "Administrator,SuperAdministrator,CanAssignUserToGroup")]
        [HttpPost]
        public JsonResult AssignGroup([FromBody]UserGroup user)
        {
            string AddedBy =Constant.GetUserID();
            return Json(UsersSQL.AssignGroup(user, AddedBy), JsonRequestBehavior.AllowGet);
        }
        [OcelotAuthorizationAttribute(Roles = "Administrator,SuperAdministrator,CanEditUser")]
        [HttpPost]
        public JsonResult EditUser([FromBody]User user)
        {
            return Json(UsersSQL.EditUser(user), JsonRequestBehavior.AllowGet);
        }
    }
}