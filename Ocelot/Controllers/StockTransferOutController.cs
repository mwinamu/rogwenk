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
    public class StockTransferOutController : Controller
    {
        [OcelotAuthorizationAttribute(Roles = "Administrator,SuperAdministrator,CanViewStock")]
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
        [OcelotAuthorizationAttribute(Roles = "Administrator,SuperAdministrator,CanGetStock")]
        [HttpGet]
        public JsonResult GetStock(string type, DateTime FromDate, DateTime ToDate)
        {
            string UserID = Constant.GetUserID();
            return Json(StockTransferredOutSQL.StockTransferredOut(type,UserID,FromDate,ToDate),JsonRequestBehavior.AllowGet);
        }
        [OcelotAuthorizationAttribute(Roles = "Administrator,SuperAdministrator,CanGetStock")]
        [HttpGet]
        public JsonResult GetShopStock(DateTime FromDate, DateTime ToDate)
        {
            string AddedBy = Constant.GetUserID();
            return Json(StockTransferredOutSQL.ShopStockTransferredOut(AddedBy,FromDate,ToDate), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetShopDistributorStock(DateTime FromDate, DateTime ToDate)
        {
            string AddedBy = Constant.GetUserID();
            return Json(StockTransferredOutSQL.ShopStockFromDistributor(AddedBy,FromDate,ToDate), JsonRequestBehavior.AllowGet);
        }
        [OcelotAuthorizationAttribute(Roles = "Administrator,SuperAdministrator,CanTransferStockOut")]
        [HttpPost]
        public JsonResult TransferStockOut([FromBody]StockTransferredOut stock)
        {
            string AddedBy = Constant.GetUserID();
            return Json(StockTransferredOutSQL.TransferStockOut(stock, AddedBy));
        }
        [OcelotAuthorizationAttribute(Roles = "Administrator,SuperAdministrator,CanViewStock")]
        [HttpGet]
        public ActionResult StockIn()
        {
            return View();
        }
        [OcelotAuthorizationAttribute(Roles = "Administrator,SuperAdministrator,CanTransferStockIn")]
        [HttpPost]
        public JsonResult TransferStockIn([FromBody]StockModels stock)
        {
            string AddedBy = Constant.GetUserID();
            return Json(StockTransferredOutSQL.TransferStockIn(stock, AddedBy), JsonRequestBehavior.AllowGet);
        }
        [OcelotAuthorizationAttribute(Roles = "Administrator,SuperAdministrator,CanGetStock")]
        [HttpGet]
        public ActionResult StockFormDistributor()
        {
            return View();
        }
        [OcelotAuthorizationAttribute(Roles = "Administrator,SuperAdministrator,CanAddStockFromDistributor")]
        [HttpPost]
        public JsonResult StockFromDistributor([FromBody]StockTransferredOut stock)
        {
            string AddedBy = Constant.GetUserID();
            return Json(StockTransferredOutSQL.TransferStockFromDistributor(stock, AddedBy), JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetContainerStockLevel()
        {
            string UserID = Constant.GetUserID();
            return Json(StockTransferredOutSQL.GetContainerStockSummary(UserID), JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        [OcelotAuthorizationAttribute(Roles = "Administrator,SuperAdministrator,CanGetVehicleVariance")]
        public JsonResult GetVehicleVariances()
        {
            string UserID = Constant.GetUserID();
            return Json(StockTransferredOutSQL.GetVariances(), JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        [OcelotAuthorizationAttribute(Roles = "Administrator,SuperAdministrator,CanGetVehicleVariance")]
        public ActionResult VehicleVariances()
        {
            return View();
        }
        [HttpGet]
        [OcelotAuthorizationAttribute(Roles = "Administrator,SuperAdministrator,CanGetContainerStock")]
        public ActionResult Container()
        {
            return View();
        }
        [HttpPost]
        [OcelotAuthorizationAttribute(Roles = "CanReconcilContainerVariance")]
        public ActionResult ReconcileContainerVariance([FromBody]ContainerStockSummary stock)
        {
            string UserID = Constant.GetUserID();
            return Json(StockTransferredOutSQL.ReconcileContainerStock(stock, UserID), JsonRequestBehavior.AllowGet);
        }
    }
}