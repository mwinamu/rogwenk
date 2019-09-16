using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ocelot.Models
{
    public class DepartmetnModel
    {
        public string Id { get; set; }
        public string Department { get; set; }
        public string DateAdded { get; set; }


    }
    public class HeadOfDepartment
    {
        public string DepartmentID { get; set; }
        public string UserID { get; set; }
    }
}