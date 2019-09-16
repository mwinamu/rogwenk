using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ocelot.Models;
using Ocelot.Service;
using Ocelot.SQLHELPER;

namespace Ocelot.Controllers
{
    public class VendorController : Controller
    {
        VendorSQL Sqlhelpers = new VendorSQL();
        [HttpGet]
        [OcelotAuthorizationAttribute(Roles = "Administrator")]
        public JsonResult GetSalesData()
        {
            DateTime StartDate = Convert.ToDateTime("2019-01-01");
            DateTime EndDate = Convert.ToDateTime("2019-08-01");
            var Data = Sqlhelpers.GetAllSales(StartDate, EndDate).Where(s => s.Status == 0).Select(s => new {
                s.VendorAttendant,
                s.UserID,
                s.BusinessID,
                s.SaleID,
                s.BusinessName,
                s.Refill_6KG,
                s.Refill_13KG,
                s.Refill_50KG,
                s.Outright_6KG,
                s.Outright_13KG,
                s.Outright_50KG,
                s.Complete6KG,
                s.Complete13KG,
                s.TranAmount,
                s.Variance,
                s.DateSold,
                s.Commission,
                s.comment,
                s.TransID,
                s.Status
            });
            return Json(Data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult ActiveVendors(DateTime StartDate, DateTime EndDate)
        {
            var Data = Sqlhelpers.GetAllSales(StartDate, EndDate).Select(s => s.BusinessID).Distinct().ToList();
            return Json(new { ActiveVendors = Data.Count }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult TotalCommission(DateTime StartDate, DateTime EndDate)
        {
            int Commission = Sqlhelpers.GetAllSales(StartDate, EndDate).AsEnumerable().Sum(s => Convert.ToInt32(s.Commission));
            return Json(new { TotalCommission = Commission }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult TotalSales(DateTime StartDate, DateTime EndDate)
        {
            int TotalSales = Sqlhelpers.GetAllSales(StartDate, EndDate).AsEnumerable().Sum(s => s.Complete6KG + s.Complete13KG + s.Refill_6KG + s.Refill_13KG + s.Refill_50KG + s.Outright_6KG + s.Outright_13KG + s.Outright_50KG);
            return Json(new { TotalSales = TotalSales }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult VendorSales()
        {
            return View();
        }
        [HttpGet]
        public ActionResult VendorsList()
        {
            ViewBag.PageTitle = "Vendor List";
            ViewBag.ActiveMenu = "Vendor List";
            return View();
        }
        [HttpGet]
        public ActionResult VendorSummary()
        {
            ViewBag.PageTitle = "Vendor Summary";
            ViewBag.ActiveMenu = "Vendor Summary";
            return View();
        }
        [HttpPost]
        public JsonResult ApproveCommision(string[] data)
        {
            var rs = Sqlhelpers.ProcessCommission(data);
            if (rs == 1)
            {
                return Json(new { Error = false, Title = "SUCCESS", Message = "Commission Approved Successfully" });

            }
            else
            {
                return Json(new { Error = true, Title = "FAILED", Message = "Failed to Approve Commission" });

            }
        }

        public ActionResult test()
        {

            return View();
        }

        [HttpGet]
        public JsonResult Week(DateTime StartDate, DateTime EndDate)
        {
            try
            {
                var Sales = Sqlhelpers.GetAllSales(StartDate, EndDate).ToList().Select(s => new { s.BusinessID, s.BusinessName, s.Refill_6KG, s.Refill_13KG, s.Outright_6KG, s.Outright_13KG });
                var results = from line in Sales
                              group line by line.BusinessID into g
                              select new VendorSalesModel
                              {
                                  BusinessID = g.First().BusinessID,
                                  Refill_6KG = g.Sum(s => s.Refill_6KG),
                                  Refill_13KG = g.Sum(s => s.Refill_13KG),
                                  Outright_6KG = g.Sum(s => s.Outright_6KG),
                                  Outright_13KG = g.Sum(s => s.Outright_13KG),
                                  // Outright_13 = g.Count().ToString(),
                              };
                var rs = results.Select(s => new { s.BusinessName, s.Refill_6KG, s.Refill_13KG, s.Outright_6KG, s.Outright_13KG });

                return Json(rs, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = true, message = "Error" + ex.Message });
            }
        }



        [HttpGet]
        public JsonResult vendorsalessummary(DateTime StartDate, DateTime EndDate)
        {
            try
            {
                var Sales = Sqlhelpers.GetAllSales(StartDate, EndDate).ToList();

                var result =
               from sales in Sales
               group sales by sales.BusinessID into SalesGroup
               select new
               {
                   BussinessID = SalesGroup.First().BusinessID,
                   BusinessName = SalesGroup.First().BusinessName,
                   Refill6KG = SalesGroup.Sum(x => x.Refill_6KG),
                   Refill13KG = SalesGroup.Sum(x => x.Refill_13KG),
                   Outright_6KG = SalesGroup.Sum(x => x.Outright_6KG),
                   Outright_13KG = SalesGroup.Sum(x => x.Outright_13KG),
                   Complete6KG = SalesGroup.Sum(x => x.Complete6KG),
                   Complete13KG = SalesGroup.Sum(x => x.Complete13KG),
                   TotalCash = SalesGroup.Sum(x => x.TranAmount),
                   LatestSale = SalesGroup.Max(x => x.DateSold),
               };


                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = true, message = "Error" + ex.Message });
            }
        }

        [HttpGet]
        public JsonResult Summary(DateTime StartDate, DateTime EndDate)
        {
            try
            {
                var Sales = Sqlhelpers.GetAllSales(StartDate, EndDate).ToList();

                var result =
               from sales in Sales
               group sales by sales.BusinessID into SalesGroup
               select new
               {
                   BusinessName = SalesGroup.First().BusinessName,
                   Refill6KG = SalesGroup.Sum(x => x.Refill_6KG),
                   Refill13KG = SalesGroup.Sum(x => x.Refill_13KG),
                   Outright_6KG = SalesGroup.Sum(x => x.Outright_6KG),
                   Outright_13KG = SalesGroup.Sum(x => x.Outright_13KG),
                   Complete6KG = SalesGroup.Sum(x => x.Complete6KG),
                   Complete13KG = SalesGroup.Sum(x => x.Complete13KG),
               };


                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = true, message = "Error" + ex.Message });
            }
        }

        public JsonResult VendorTransactions(DateTime StartDate, DateTime EndDate, string BusinessID)
        {
            var Data = Sqlhelpers.GetAllSales(StartDate, EndDate).Where(s => s.BusinessID.Trim() == BusinessID.Trim()).Select(s => new {
                s.VendorAttendant,
                s.UserID,
                s.BusinessID,
                s.SaleID,
                s.BusinessName,
                s.Refill_6KG,
                s.Refill_13KG,
                s.Refill_50KG,
                s.Outright_6KG,
                s.Outright_13KG,
                s.Outright_50KG,
                s.Complete6KG,
                s.Complete13KG,
                s.TranAmount,
                s.Variance,
                s.DateSold,
                s.Commission,
                s.comment,
                s.TransID,
                s.Status
            });
            return Json(Data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult VendorStockTransfer(DateTime StartDate, DateTime EndDate)
        {
            var Data = Sqlhelpers.StockTransfer(StartDate, EndDate).ToList();
            return Json(Data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult VendorWithdrawals(DateTime StartDate, DateTime EndDate)
        {
            var Data = Sqlhelpers.GetVendorWithdrawals(StartDate, EndDate).ToList();
            return Json(Data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult TotalWithdrawn(DateTime StartDate, DateTime EndDate)
        {
            int TotalWithdrawn = Sqlhelpers.GetVendorWithdrawals(StartDate, EndDate).ToList().Sum(s => s.TransactionAmount);
            return Json(new { TotalWithdrawn = TotalWithdrawn }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult TotalStockTransfered(DateTime StartDate, DateTime EndDate)
        {
            var Data = Sqlhelpers.StockTransfer(StartDate, EndDate).ToList().Sum(s => s.Transferred13KG + s.Transferred6KG + s.Transferred13KG);
            return Json(Data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult TransfersCount(DateTime StartDate, DateTime EndDate)
        {
            var Data = Sqlhelpers.StockTransfer(StartDate, EndDate).ToList();
            return Json(new { TotalTransfers = Data.Count }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult TotalVendors()
        {
            var Data = String.Empty;
                //Sqlhelpers.TotalVendors().ToList();
            return Json(Data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RegisteredVendors()
        {
            var Data = String.Empty; //Sqlhelpers.GetVendorList().ToList();
            return Json(Data, JsonRequestBehavior.AllowGet);
        }
        // GET: Vendor
        public ActionResult Index()
        {
            return View();
        }
    }
}