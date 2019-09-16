using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ocelot.Models
{
    public class StockModels
    {
        public StockTransferredOut Unsold { get; set; }
        public List<UnloadedEmpties> Empties { get; set; }

        
    }
    public class StockTransferredOut
    {
        public string ID { get; set; }
        public string EntryID { get; set; }
        public string VehicleID { get; set; }
        public string VehicleReg { get; set; }
        public int Transferred6KG { get; set; }
        public int Transferred13KG { get; set; }
        public int Transferred50KG { get; set; }
        public string TransferredAt { get; set; }
        public string TransferredBy { get; set; }
        public int TransferredStockGrill { get; set; }
        public int TransferredStockBurner { get; set; }

        public string ShopName { get; set; }
    }
    public class UnloadedEmpties
    {
        public int BrandID { get; set; }
        public string EmptyCylindersSKU { get; set; }
        public string EmptySkuQunatity { get; set; }
    }

    public class VehicleStockUnsold
    {
        public string ID { get; set; }
        public string VehicleLogsId { get; set; }
        public int Cylinder6KG { get; set; }
        public int Cylinder13KG { get; set; }
        public int Cylinder50KG { get; set; }
        public DateTime  AddedAt { get; set; }
        public string AddedBy { get; set; }
    }
    public class VehicleUnloadingEmptiesModels
    {
        public string Id { get; set; }
        public string TripID { get; set; }
        public int BrandID { get; set; }
        public string Size { get; set; }
        public int Quantity { get; set; }
        public string AddedBy { get; set; }
        public DateTime AddedAt { get; set; }
    }
    public class TripVariance
    {
        public string ID { get; set; }
        public string TripID { get; set; }
        public int Variance13KG { get; set; }
        public int Variance50KG { get; set; }
        public int Variance6KG { get; set; }
        public int VarianceBurner { get; set; }
        public int VarianceGrill { get; set; }
        public string VehicleRegistration { get; set; }
        public string DriverName { get; set; }
    }
}
