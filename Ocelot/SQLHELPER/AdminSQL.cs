using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Ocelot.Models;
using Ocelot.Service;

namespace Ocelot.SQLHELPER
{
    public static class AdminSQL
    {
        private static string DBConn = Constant.DbConn;
        private static string querry = String.Empty;

        public static List<UserDropDownModel> userDropDowns()
        {
            var users = new List<UserDropDownModel>();
            try
            {
                
                using (SqlConnection conn = new SqlConnection(DBConn))
                {
                    querry = $@"select UserID,Email from Users";
                    using (SqlCommand cmd = new SqlCommand(querry, conn))
                    {
                        conn.Open();
                        SqlDataReader rdr = cmd.ExecuteReader();
                        while (rdr.Read())
                        {
                            var user = new UserDropDownModel();
                            user.Email = rdr["Email"].ToString();
                            user.Id = rdr["UserID"].ToString();
                            users.Add(user);
                        }
                    }
                }
            }catch(Exception e)
            {
                users = null;
            }
            return users;
        }
    }
}
