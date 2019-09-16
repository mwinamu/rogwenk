using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Ocelot.Service
{
    public static class RoleChecker
    {
        private static string DBConn = Constant.DbConn;
        public static bool HasRole(string RoleName)
        {
            var userId = Constant.GetUserID();
            if (userId != "" || userId != null)
            {
                List<Roles> roles = ClaimsManager.RolesPerUserId(userId.ToString());
                var re = roles.Find(r => r.RoleName == RoleName);
                if (re != null)
                {
                    return true;
                }
            }

            return false;
        }
        public static string UserName()
        {
            string UserName = String.Empty;
            string userId = Constant.GetUserID();
            if (userId != "" || userId != null)
            {
                using (SqlConnection conn = new SqlConnection(DBConn))
                {
                    using (SqlCommand cmd = new SqlCommand(@"select concat(FirstName,' ',LastName) as Username from users where UserID = @UserID", conn))
                    {
                        conn.Open();
                        cmd.Parameters.AddWithValue("@UserID", userId);
                        SqlDataReader rdr = cmd.ExecuteReader();
                        if (rdr.Read())
                        {
                            UserName = rdr["UserName"].ToString();
                        }
                    }
                }
            }

            return UserName;
        }
    }
}