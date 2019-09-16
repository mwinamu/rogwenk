using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ocelot.Models
{
    public class PostSaleOrder
    {
        public SaleOrderModel saleOrder { get; set; }
        public List<SaleDetail> saleDetail { get; set; }
    }
    public class SaleOrderModel
    {
        public int SQLLiteID { get; set; }
        public string AppVersion { get; set; }
        public int ID { get; set; }
        public int TripID { get; set; }
        public int SaleID { get; set; }
        public int RouteID { get; set; }
        public int CustomerID { get; set; }
        public Decimal? Latitude { get; set; }
        public Decimal? Longitude { get; set; }
        public int TransAmount { get; set; }
        public string Variance { get; set; }
        public string Comment { get; set; }
        public DateTime? TransTime { get; set; }
        public DateTime? DateAdded { get; set; }
        public int Status { get; set; }
        public int ReceiptStatus { get; set; }
    }

    public class OrderModel
    {
        public string ShipmentNO { get; set; }
        public string DriverID { get; set; }
        public string DriverName { get; set; }
        public int VehicleID { get; set; }
        public string VRegNO { get; set; }
        public string CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string TripNumber { get; set; }
        public int Quantity { get; set; }
        public DateTime DeliveryDate { get; set; }

    }
    public class TripCostsModel
    {
        public string ShipmentNO { get; set; }
        public string DriverName { get; set; }
        public string VRegNO { get; set; }
        public DateTime TripStartedAt { get; set; }
        public DateTime TripClosedAt { get; set; }
        public int Quantity { get; set; }
        public float TotalDistance { get; set; }
        public float StartMileage { get; set; }
        public float ClosingMileage { get; set; }
        public float LabourCost { get; set; }
        public float LunchAllowance { get; set; }
        public float EmployeeBenefit { get; set; }
        public float TelephoneCost { get; set; }
        public float ParkingCost { get; set; }
        public float DistributionLicence { get; set; }
        public float RandM { get; set; }
        public float FinancingLeasingCost { get; set; }
        public float InsuranceCost { get; set; }
        public float NightOutCost { get; set; }

    }
    public class OrderListModel
    {
        public string ID { get; set; }
        public string ShipmentNO { get; set; }
        public string DriverID { get; set; }
        public string DriverName { get; set; }
        public string VehicleID { get; set; }
        public string VRegNO { get; set; }
        public string CustomerID { get; set; }
        public string CustomerName { get; set; }
        public int TripNumber { get; set; }
        public int Quantity { get; set; }
        public DateTime DeliveryDate { get; set; }
        public int Status { get; set; }
        public DateTime DateAdded { get; set; }
    }


    public class DeliveredOrderList:OrderListModel
    {
        public DateTime ActualDateDelivered { get; set; }
    }

    public class ActiveOrderList : OrderListModel
    {
        public DateTime DateStarted { get; set; }
    }

    public class FailedOrderList : OrderListModel
    {

    }


    public class SaleDetail {
        public int SQLLiteID { get; set; }
        public int ID { get; set; }
        public string SaleID { get; set; }
        public int ProductID { get; set; }
        public string MaterialCode { get; set; }
        public int Quantity { get; set; }
        public string DateAdded { get; set; }

    }
    public class TruckWiseSale
    {
        public string  vregno { get; set; }
        public string DriverName { get; set; }
        public string CustomerName { get; set; }
        public string RouteName { get; set; }
        public int Refill6KG { get; set; }
        public int DriverID { get; set; }
        public int Outright6KG { get; set; }
        public int Outright13KG { get; set; }
        public int Refill13KG { get; set; }
        public int Refill50KG { get; set; }
        public int Outright50KG { get; set; }
        public int ExpectedAmount { get; set; }
        public int TotalSold{ get; set; }
    }

}