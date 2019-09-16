using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ocelot.Models
{
    public class StaffModel
    {
        public int UserSystemID { get; set; }
        public int UserID { get; set; }
        public string PayrollNumber { get; set; }
        public string FirstName { get; set; }
        public string StaffName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string UserType { get; set; }
        public string DepartmentName { get; set; }
        public string PhoneNumber { get; set; }
        public string DepartmentID { get; set; }
        public string DateAdded { get; set; }
    }

    public class UserCredential : StaffModel
    {
        public int ID { get; set; }
        public byte[] Password { get; set; }
        public byte[] Keys { get; set; }
        public byte[] IV { get; set; }
    }
}
