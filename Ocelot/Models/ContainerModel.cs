using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ocelot.Models
{
    public class ContainerModel
    {
    }
    public class ContainerStockSummary
    {
        public string ID { get; set; }
        public string ShopID { get; set; }
        public string ShopName  { get; set; }
        public int Stock6KG { get; set; }
        public int Stock13KG { get; set; }
        public int Stock50KG { get; set; }
        public int StockBurner { get; set; }
        public int StockGrill { get; set; }
        public string Reason { get; set; }
    }
    public class VehicleStockLevels
    {
        public int ID { get; set; }
        public string TripID { get; set; }
        public string VehicleID { get; set; }
        public int Stock6KG { get; set; }
        public int Stock13KG { get; set; }
        public int Stock50KG { get; set; }
        public int StockGrills { get; set; }
        public int StockBurners { get; set; }

        public int Status { get; set; }
    }
    public class VehicleStockLogs
    {
        public int ID { get; set; }
        public string TripID { get; set; }
        public string VehicleID { get; set; }
        public string StockFrom { get; set; }
        public int Stock6KG { get; set; }
        public int Stock13KG { get; set; }
        public int Stock50KG { get; set; }
        public int StockGrills { get; set; }
        public int StockBurners { get; set; }
    }
}
