using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ocelot.Models
{
    public class LeaveApplicationModel
    {
        public int ID { get; set; }
        public int Status { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string DriverName { get; set; }
        public string LeaveType { get; set; }
        public string RejectReason { get; set; }
        


    }
}