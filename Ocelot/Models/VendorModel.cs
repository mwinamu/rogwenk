using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ocelot.Models
{
    public class VendorModel
    {
    }
    public class VendorSalesModel : SalesDataModels
    {
        //ResultLine
        //public ResultLine() { }
        public string BusinessName { get; set; }
        public string VendorAttendant { get; set; }
        public string BusinessID { get; set; }

        public int UserID { get; set; }
        public int Complete6KG { get; set; }
        public int Status { get; set; }
        public int Complete13KG { get; set; }
        public string ContactPerson { get; set; }
        public string Commission { get; set; }

        public string TransID { get; set; }
    }

    public class ApproveCommissionModel { public string SaleID { get; set; } }
    public class TotalVendorsModel { public int TotalVendors { get; set; } }
    public class VendorStockUpModel
    {
        public string TripID { get; set; }
        public string VRegNo { get; set; }
        public string DriverID { get; set; }
        public string VendorID { get; set; }
        public string DriverName { get; set; }
        public string BusinessName { get; set; }
        public int Transferred6KG { get; set; }
        public int Transferred13KG { get; set; }
        public int Transferred50KG { get; set; }

        public int Burners { get; set; }
        public int Grills { get; set; }
        public int Status { get; set; }

        public DateTime DateTransferred { get; set; }
    }


    public class VendorCashWithdrawals
    {

        //					

        public string BusinessID { get; set; }
        public string BusinessName { get; set; }
        public string ReceiverPartyPublicName { get; set; }
        public int TransactionAmount { get; set; }
        public string TransID { get; set; }
        public DateTime TransactionCompletedDateTime { get; set; }
    }
    public class SalesDataModels
    {

        public string SaleID { get; set; }
        public string OutletID { get; set; }
        public string DriverID { get; set; }
        public string territory { get; set; }
        public string trip_id { get; set; }
        public string OutletName { get; set; }
        public string Truckreg { get; set; }
        public string DriverName { get; set; }
        public int Refill_6KG { get; set; }
        public int Refill_13KG { get; set; }
        public int Refill_50KG { get; set; }

        public int progas_return_6kg { get; set; }
        public int progas_return_13kg { get; set; }
        public int progas_return_50kg { get; set; }

        public int comp_return_6kg { get; set; }
        public int comp_return_13kg { get; set; }
        public int comp_return_50kg { get; set; }




        public int Outright_6KG { get; set; }
        public int Outright_13KG { get; set; }
        public int Outright_50KG { get; set; }
        public int Burners { get; set; }
        public int Grills { get; set; }
        public DateTime DateSold { get; set; }
        public int week_of_the_year { get; set; }
        public string Route { get; set; }
        public int outlets_count { get; set; }
        public int day_of_month { get; set; }
        public string SoldDate { get; set; }
        public int TotalReturned { get; set; }
        public int TranAmount { get; set; }
        public int total_sales { get; set; }
        public int MpesaAmount { get; set; }
        public string Variance { get; set; }
        public string comment { get; set; }
        public int mpesa { get; set; }
        public int bank { get; set; }
        public int cheque { get; set; }
        public int invoice { get; set; }
    }

}