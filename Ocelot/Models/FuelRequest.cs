using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ocelot.Models
{
    public class FuelRequest
    {
        public int ID { get; set; }
        public int OrderID { get; set; }
        public int DriverID { get; set; }
        public string VRegNO { get; set; }
        public string DriverName { get; set; }
        public int VehicleID { get; set; }
        public string CurrentLevel { get; set; }
        public int RequestMileage { get; set; }
        public string DateRequested { get; set; }
        public string InvoiceNumber { get; set; }
        public string ApprovedQuantity { get; set; }
        public Decimal FuelingQuantity { get; set; }
        public string FuelingImage { get; set; }
        public string ApprovedBy { get; set; }
        public int AttendantID { get; set; }
        public int MeterNO { get; set; }
        public string FuelingStation { get; set; }
        public int StationID { get; set; }
        public Decimal DateClosed { get; set; }
        public string RejectReason { get; set; }
        public Decimal PreMileage { get; set; }
        public Decimal PreQuantity { get; set; }
        public string TillNumber { get; set; }
    }
    public class DeclineFuelRequest
    {
        public int ID { get; set; }
        public string Reason { get; set; }
    }
}