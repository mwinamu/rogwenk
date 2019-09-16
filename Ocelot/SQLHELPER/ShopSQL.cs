using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Ocelot.Models;
using Ocelot.Service;

namespace Ocelot.SQLHELPER
{
    public static class ShopSQL
    {
        private static string DBConn = Constant.DbConn;
        private static string querry =  String.Empty;
        private static MessageResult mes = new MessageResult();
        public static List<AssignedShopsModel> GetAssignedShop()
        {
            var shops = new List<AssignedShopsModel>();
            try
            {
                using (SqlConnection conn = new SqlConnection(DBConn))
                {
                     querry = $@"select a.ShopID,a.ShopName,a.Route,a.Territory,a.Location,a.AddedAt,
a.AddedBy,ISNULL( b.Email,'NOT ASSIGNED') AS Email
from Shops as a
left join AssignShop as c on c.ShopsID = a.ShopID
left join Users as b on convert(nvarchar(50),b.UserID) = c.UserID 
where c.Status  = 1";
                    using (SqlCommand cmd = new SqlCommand(querry, conn))
                    {
                        conn.Open();
                        SqlDataReader rdr = cmd.ExecuteReader();
                        while (rdr.Read())
                        {
                            var shop = new AssignedShopsModel();
                            shop.ShopID = rdr["ShopID"].ToString();
                            shop.ShopName = rdr["ShopName"].ToString();
                            shop.Route = rdr["Route"].ToString();
                            shop.Territory = rdr["Territory"].ToString();
                            shop.Location = rdr["Location"].ToString();
                            shop.AddedAt = rdr["AddedAt"].ToString();
                            shop.AddedBy = rdr["AddedBy"].ToString();
                            shop.Email = rdr["Email"].ToString();
                            shops.Add(shop);
                        }
                    }
                }
            }catch(Exception e)
            {
                shops = null;
            }
            return shops;
        }
        public static List<AssignedShopsModel> ShopsDropDown()
        {
            var shops = new List<AssignedShopsModel>();
            try
            {
                using (SqlConnection conn = new SqlConnection(DBConn))
                {
                     querry = $@"select a.ShopID,a.ShopName,a.Route,a.Territory,a.Location,a.AddedAt,
                        a.AddedBy,ISNULL( b.NormalizedEmail,'NOT ASSIGNED') AS Email
                    from Shops as a
                    left join AssignShop as c on c.ShopsID = a.ShopID
                    left join AspNetUsers as b on b.Id = c.UserID 
                    where c.Status  = 1
                    and c.UserID not in (select ID from Drivers)
                    order by AddedAt desc";
                    using (SqlCommand cmd = new SqlCommand(querry, conn))
                    {
                        conn.Open();
                        SqlDataReader rdr = cmd.ExecuteReader();
                        while (rdr.Read())
                        {
                            var shop = new AssignedShopsModel();
                            shop.ShopID = rdr["ShopID"].ToString();
                            shop.ShopName = rdr["ShopName"].ToString();
                            shop.Route = rdr["Route"].ToString();
                            shop.Territory = rdr["Territory"].ToString();
                            shop.Location = rdr["Location"].ToString();
                            shop.AddedAt = rdr["AddedAt"].ToString();
                            shop.AddedBy = rdr["AddedBy"].ToString();
                            shop.Email = rdr["Email"].ToString();
                            shops.Add(shop);
                        }
                    }
                }
            }catch(Exception e)
            {
                shops = null;
            }
            return shops;
        }
        public static List<ShopModel> GetUnAssignedShop()
        {
            var shops = new List<ShopModel>();
            try
            {
                using (SqlConnection conn = new SqlConnection(DBConn))
                {
                     querry = $@"
                                select * from Shops as a
                                where a.ShopID not in (select ShopsID from AssignShop )";
                    using (SqlCommand cmd = new SqlCommand(querry, conn))
                    {
                        conn.Open();
                        SqlDataReader rdr = cmd.ExecuteReader();
                        while (rdr.Read())
                        {
                            var shop = new ShopModel();
                            shop.ShopID = rdr["ShopID"].ToString();
                            shop.ShopName = rdr["ShopName"].ToString();
                            shop.Route = rdr["Route"].ToString();
                            shop.Territory = rdr["Territory"].ToString();
                            shop.Location = rdr["Location"].ToString();
                            shop.AddedAt = rdr["AddedAt"].ToString();
                            shop.AddedBy = rdr["AddedBy"].ToString();
                            shops.Add(shop);
                        }
                    }
                }
            }catch(Exception e)
            {
                shops = null;
            }
            return shops;
        }
        public static MessageResult AddShop(ShopModel shop,string AddedBy)
        {
            try
            {
                using(SqlConnection conn = new SqlConnection(DBConn))
                {
                    querry = $@"Insert into Shops(ShopID,ShopName,Route,Territory,Location,AddedAt,AddedBy)
                        values(@ShopID,@ShopName,@Route,@Territory,@Location,@AddedAt,@AddedBy)";
                    using(SqlCommand cmd = new SqlCommand(querry,conn))
                    {
                        conn.Open();
                        cmd.Parameters.AddWithValue("@ShopID", Guid.NewGuid());
                        cmd.Parameters.AddWithValue("@ShopName", shop.ShopName);
                        cmd.Parameters.AddWithValue("@Route", shop.Route);
                        cmd.Parameters.AddWithValue("@Territory", shop.Territory);
                        //cmd.Parameters.AddWithValue("@ZoneID", shop.ZoneID);
                        cmd.Parameters.AddWithValue("@Location", shop.Location);
                        cmd.Parameters.AddWithValue("@AddedAt", DateTime.Now);
                        cmd.Parameters.AddWithValue("@AddedBy", AddedBy);
                        cmd.ExecuteReader();
                        mes.Status = "success";
                        mes.Message = "Shop added successfully";
                    }
                }

            }catch(Exception e)
            {
                mes.Status = "success";
                mes.Message = "Failed! reload and try again";
            }
            return mes;
        }
        public static MessageResult EditShop(ShopModel shop, string UpdatedBy)
        {
            try
            {
                using(SqlConnection conn = new SqlConnection(DBConn))
                {
                    querry = $@"Update Shops set ShopName = @ShopName,Route =  @Route,Territory = @Territory,
                               Location = @Location,UpdatedAt = @UpdatedAt,UpdatedBy = @UpdatedBy where
                                ShopID = @ShopID";
                    using(SqlCommand cmd = new SqlCommand(querry,conn))
                    {
                        conn.Open();
                        cmd.Parameters.AddWithValue("@ShopID", shop.ShopID);
                        cmd.Parameters.AddWithValue("@ShopName", shop.ShopName);
                        cmd.Parameters.AddWithValue("@Route", shop.Route);
                        cmd.Parameters.AddWithValue("@Territory", shop.Territory);
                        cmd.Parameters.AddWithValue("@Location", shop.Location);
                        cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);
                        cmd.Parameters.AddWithValue("@UpdatedBy", UpdatedBy);
                        cmd.ExecuteScalar();
                        mes.Status = "success";
                        mes.Message = "Shop updated successfully";
                    }
                }
            }catch(Exception e)
            {
                mes.Status = "warning";
                mes.Message = "Failed! reload and try again later";
            }
            return mes;
        }
        public static MessageResult AssignShop(AssignShop assign,string addedBy)
        {
            try
            {
                using(SqlConnection conn = new SqlConnection(DBConn))
                {
                    querry = $@"update AssignShop set Status = '{0}' where UserID = @UserID 
                    insert into AssignShop(ID,ShopsID,UserID,Status,AddedAt,AddedBy)
                    values(@ID,@ShopsID,@UserID,@Status,@AddedAt,@AddedBy)";
                    using(SqlCommand cmd = new SqlCommand(querry, conn))
                    {
                        conn.Open();
                        cmd.Parameters.AddWithValue("@ID",Guid.NewGuid());
                        cmd.Parameters.AddWithValue("@ShopsID",assign.ShopsID);
                        cmd.Parameters.AddWithValue("@UserID", assign.UserID);
                        cmd.Parameters.AddWithValue("@Status", 1);
                        cmd.Parameters.AddWithValue("@AddedAt", DateTime.Now);
                        cmd.Parameters.AddWithValue("@AddedBy", addedBy);
                        cmd.ExecuteReader();
                        conn.Close();
                        mes.Status = "success";
                        mes.Message = "Shop assidned to user successfully";
                    }
                }
            }
            catch (Exception e)
            {
                mes.Status = "warning";
                mes.Message = "Failed! reload and try again";
            }
            return mes;
        }
    }
}
