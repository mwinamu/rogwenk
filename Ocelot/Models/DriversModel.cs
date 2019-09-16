using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ocelot.Models
{
    public class DriversModel
    {
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string AddedBy { get; set; }
    }


    public class CylinderReturnsModel
    {
        public string BrandID { get; set; }
        public string BrandName { get; set; }

    }
    public class DriverListModel : DriversModel
    {
        public string ID { get; set; }

        public string DriverName { get; set; }

        public string ShopName { get; set; }
        public string VehicleRegistration { get; set; }

        public double DistanceFromCustomer { get; set; }
    }

    public class DriverLatestLocation
    {
        public string DriverID { get; set; }

        public string DriverName { get; set; }

        public string AssingedVehicle { get; set; }

        public int ActiveOrders { get; set; }
        public int RejectedOrders { get; set; }
        public int DeliveredOrders { get; set; }
        public int AssignedOrders { get; set; }
        public string Latitude { get; set; }
        public double DistanceFromCustomer { get; set; }        
        public string Longitude { get; set; }

        public int Stock6Kg { get; set; }
        public int Stock13Kg { get; set; }

        public DateTime LastUpdatedAt { get; set; }
    }


    public class UpdateDriverModel : DriversModel
    {
        public string ID { get; set; }
        public string UpdatedBy { get; set; }
    }

    public class AssignVehicleModel
    {
        public string ID { get; set; }
        public string DriverID { get; set; }
        public string VehicleID { get; set; }
        public string AssignBy { get; set; }
    }
    public class OrderDelivery
    {
        public string DriverID { get; set; }
        public string OrderID { get; set; }
    }
    public class AssignDriverToShopModel
    {
        public string DriverID { get; set; }
        public string ShopID { get; set; }
    }

    public class OrderMapDelivery
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Road { get; set; }
        public string Building { get; set; }
        public int DeliveryStatus { get; set; }
    }
}
