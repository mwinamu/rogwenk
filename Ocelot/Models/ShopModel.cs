using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ocelot.Models
{
    public class ShopModel
    {
        public string ShopID { get; set; }
        public string ShopName { get; set; }
        public string Route { get; set; }
        public string Territory { get; set; }
        public string Location { get; set; }
        public string AddedAt { get; set; }
        public string AddedBy { get; set; }
        public string ZoneID { get; set; }
    }

    public class AssignedShopsModel : ShopModel
    {
        public string Email { get; set; }
    }
    public class AssignShop
    {
        public string ShopsID { get; set; }
        public string UserID { get; set; }

        public int Status { get; set; }
    }

}
