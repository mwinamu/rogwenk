using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ocelot.Models
{
    public class Customer: Region
    {
        public int CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string Contact { get; set; }
        public string Address { get; set; }
        public string  Longitude  { get; set; }
        public string  Latitude  { get; set; }
    }
    public class Region
    {
        public int RegionID { get; set; }
        public string RegionName { get; set; }
    }
}