using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ocelot.Models
{
    public class User: UserDropDownModel
    {
        public string UserName { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string UserID { get; set; }
    }
    public class UserDropDownModel
    {
        public string Email { get; set; }
        public string Id { get; set; }
    }
    public class UserGroup {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string GroupId { get; set; }
        public int Status { get; set; }

    }



}
