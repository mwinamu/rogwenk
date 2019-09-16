using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ocelot.Models
{
    public class StorageLocation
    {
        public int id { get; set; }
        public string StorageLocationName { get; set; }
        public string PhysicalAddress { get; set; }
        public int TotalCapacity { get; set; }
        public Decimal? DistanceFromMainTerminal { get; set; }
        public string TerminalName { get; set; }
        public int TerminalID { get; set; }
        public string DateAdded { get; set; }
        public string CreatedBy { get; set; }
    }
}