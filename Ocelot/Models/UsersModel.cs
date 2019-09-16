using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ocelot.Models
{
    public class UsersListModel
    {
        public string UserID { get; set; }
        public string PayrollNumber { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string DepartmentID { get; set; }
        public string StaffImage { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string UserType { get; set; }
        public string Status { get; set; }
        public DateTime DateAdded { get; set; }
        public string AddedBy { get; set; }

    }
}