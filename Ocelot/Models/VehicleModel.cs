using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ocelot.Models
{
    public class VehicleModel
    {
        public string ID { get; set; }
        public string VehicleRegistration {get;set;}
        public string	VehicleType  {get;set;}
        public int	VehicleCapacity  {get;set;}
        public string	FuelConsumption  {get;set;}
        public string	AddedBy  {get;set;} 
        public int IsAvailable { get; set; }
     
    }

    public class EditVehicleModel:VehicleModel
    {        
        public string UpdatedBy { get; set; }
    }
}
