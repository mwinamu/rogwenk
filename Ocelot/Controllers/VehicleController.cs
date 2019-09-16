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
using System.Net;
using System.Text;
using System.IO;

namespace Company.WebApplication1.Controllers
{
    public class VehicleController : Controller
    {
        VehicleSQL SQL = new VehicleSQL();
        [OcelotAuthorizationAttribute(Roles = "Administrator,SuperAdministrator,CanViewVehicle")]
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
        [OcelotAuthorizationAttribute(Roles = "Administrator,SuperAdministrator,CanAddVehicle")]
        [HttpPost]
        public JsonResult AddVehicle([FromBody]VehicleModel vehicle)
        {
            vehicle.AddedBy = Constant.GetUserID();
            var Response = SQL.AddVehicle(vehicle);
            return Json(Response,JsonRequestBehavior.AllowGet);
        }
        [OcelotAuthorizationAttribute(Roles = "Administrator,SuperAdministrator,CanEditVehicle")]
        [HttpPost]
        public JsonResult EditVehicle([FromBody]EditVehicleModel vehicle)
        {
            string id = Constant.GetUserID();
            var Response = SQL.UpdateVehicle(vehicle,id);
            return Json(Response, JsonRequestBehavior.AllowGet);
        }
        [OcelotAuthorizationAttribute(Roles = "Administrator,SuperAdministrator,CanGetVehicle")]
        [HttpGet]
        public JsonResult GetVehicleList()
        {
            var Vehicles = SQL.GetVehicleList().ToList();
            return Json(Vehicles, JsonRequestBehavior.AllowGet);
        }
        [OcelotAuthorizationAttribute(Roles = "Administrator,SuperAdministrator,CanGetVehicle")]
        [HttpGet]
        public JsonResult GetVehicleDropDownList()
        {
            var Vehicles = SQL.GetVehicleList().Where(s=>s.IsAvailable == 1).ToList();
            return Json(Vehicles, JsonRequestBehavior.AllowGet);
        }
        [OcelotAuthorizationAttribute(Roles = "Administrator,SuperAdministrator,CanGetVehicle")]
        [HttpGet]
        public JsonResult GetUnloadingVehicleList()
        {
            var Vehicles = SQL.GetOffLoadingVehicleList().ToList();
            return Json(Vehicles, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [OcelotAuthorizationAttribute(Roles = "Administrator,SuperAdministrator,CanUpdateAvailabilityVehicle")]
        public JsonResult UpdateAvailability([FromBody]VehicleModel vehicle)
        {
            string UpdateBy = Constant.GetUserID();
            return Json(SQL.UpdateAvailability(vehicle, UpdateBy), JsonRequestBehavior.AllowGet);
        }
    }
}
