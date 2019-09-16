using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Company.WebApplication1.Models
{
    public class ZoneModdel
    {
        public string ID { get; set; }
        public string Name { get; set; }
    }
    public class ZoneDetailModel:ZoneModdel
    {
        public string Description { get; set; }
        public string AddedAt { get; set; }
        public string AddedBy{ get; set; }
    }
}
