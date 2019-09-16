using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ocelot.Models
{
    public class TerminalModel: TerminalDropDownModel
    {
       
        public Decimal? Latitude { get; set; }
        public Decimal? Longitude { get; set; }
        public string TransactionTime { get; set; }
        public string DateAdded { get; set; }
    }
    public class TerminalDropDownModel
    {
        public int TerminalID { get; set; }
        public string TerminalName { get; set; }
    }
}