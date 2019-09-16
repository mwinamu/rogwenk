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

namespace Ocelot.Controllers
{
    public class DriversController : Controller
    {
        DriversSQL SQL = new DriversSQL();
        [HttpGet]
        [OcelotAuthorizationAttribute(Roles = "Administrator,SuperAdministrator,CanViewDriver")]
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Driver()
        {
            return View();
        }

        [HttpPost]
        [OcelotAuthorizationAttribute(Roles = "Administrator,SuperAdministrator,CanAddNewDriver")]
        public JsonResult AddNewDriver([FromBody]DriversModel Driver)
        {
            Driver.AddedBy = Constant.GetUserID();
            var Response = SQL.AddNewDriver(Driver);
            return Json(Response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [OcelotAuthorizationAttribute(Roles = "Administrator,SuperAdministrator,CanAssignVehicle")]
        public JsonResult AssignVehicle([FromBody]AssignVehicleModel assign)
        {
            var Response = SQL.AssignBike(assign);
            return Json(Response, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [OcelotAuthorizationAttribute(Roles = "Administrator,SuperAdministrator,CanUpdateDriver")]
        public JsonResult UpdateDriver([FromBody]UpdateDriverModel Driver)
        {
            Driver.UpdatedBy = Constant.GetUserID();
            var Response = SQL.UpdateDriver(Driver);
            return Json(Response, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [OcelotAuthorizationAttribute(Roles = "Administrator,SuperAdministrator,CanAssignBike")]
        public JsonResult AssignBike([FromBody]AssignVehicleModel Driver)
        {
            Driver.AssignBy = Constant.GetUserID();
            var Response = SQL.AssignBike(Driver);
            return Json(Response, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        [OcelotAuthorizationAttribute(Roles = "Administrator,SuperAdministrator,CanGetDriver")]
        public JsonResult GetDriverList()
        {
            var DriverList = SQL.GetDriversList().ToList().Select(x => new { x.ID, x.DriverName, x.PhoneNumber, x.Email, x.ShopName,x.VehicleRegistration });
            return Json(DriverList,JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [OcelotAuthorizationAttribute(Roles = "Administrator,SuperAdministrator,CanGetDriver")]
        public JsonResult GetDriversNearOrderLocation()
        {
            var DriverList = SQL.GetDriversList().ToList();
            return Json(DriverList, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [OcelotAuthorizationAttribute(Roles = "Administrator,SuperAdministrator,CanAssignToShop")]
        public JsonResult AssignToShop([FromBody]AssignDriverToShopModel assing)
        {
            // for the purpose of re using table AssignShop
            // i have in this case assummed that user id is the driver id , hope that helps
            var d = new AssignShop();
            d.ShopsID = assing.ShopID;
            d.UserID = assing.DriverID;
            string AssignBy = Constant.GetUserID();
            return Json(ShopSQL.AssignShop(d, AssignBy));
        }

        [HttpGet]
        public JsonResult LatestLocation(double Latitude, double Longitude)
        {
            var Locations = SQL.GetLatestLocation(Latitude, Longitude).OrderBy(x => x.DistanceFromCustomer).Take(5);
            return Json(Locations, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public string  GetGoogleDistance(string origins,string destinations)
        {
            var baseAddress = "https://maps.googleapis.com/maps/api/distancematrix/json?units=imperial&origins=" + origins + "&destinations="+ destinations + "&key=AIzaSyAq_8SJ3YGt8WuNJ1rp3ipN2axtKkK8fes";

            var http = (HttpWebRequest)WebRequest.Create(new Uri(baseAddress));
            http.Accept = "application/json";
            http.ContentType = "application/json";
            http.Method = "POST";

            string parsedContent = "";
            ASCIIEncoding encoding = new ASCIIEncoding();
            Byte[] bytes = encoding.GetBytes(parsedContent);

            Stream newStream = http.GetRequestStream();
            newStream.Write(bytes, 0, bytes.Length);
            newStream.Close();

            var response = http.GetResponse();

            var stream = response.GetResponseStream();
            var sr = new StreamReader(stream);
            var content = sr.ReadToEnd();

            return content;
        }

    }

}