using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ocelot.Models
{
    public class PriceModel
    {
        public string ID { get; set; }
        public string ProductID { get; set; }
        public string Price { get; set; }
        public string AddedBy { get; set; }
        public string ProductName { get; set; }
        public string DateAdded { get; set; }
        public string ZoneID { get; set; }
        public string ZoneName { get; set; }

    }
}
