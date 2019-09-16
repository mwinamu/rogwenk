using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ocelot.Models
{
    public class ProductsModel
    {
        public string ProductID { get; set; }
        public string PriceID { get; set; }
        public string ProductName { get; set; }
        public string Price { get; set; }
    }
    public class ProductDropDownModel
    {
        public string ID { get; set; }
        public string Name { get; set; }
    }
}
