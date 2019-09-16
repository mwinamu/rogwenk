using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ocelot.Models
{
    public class MaintenanceModel
    {
        public int ID { get; set; }
        public int Status { get; set; }
        public int DriverID { get; set; }
        public string VRegNO { get; set; }
        public string DriverName { get; set; }
        public string RepairType { get; set; }
        public string Description { get; set; }
        public string DriverInfo { get; set; }
        public string ClosingImage { get; set; }
        public Decimal? Amount { get; set; }
        public string DateClosed { get; set; }
    }
}