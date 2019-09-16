using Company.WebApplication1.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Ocelot.Models;
using Ocelot.Service;

namespace Ocelot.SQLHELPER
{
    public class DriversSQL
    {
        //Add New Driver
        public static string DBConn = Constant.DbConn;
        public MessageResult AddNewDriver(DriversModel Driver)
        {
            int i = 0;
            Guid ID = Guid.NewGuid(); //DriverID

            MessageResult swResponse = new MessageResult();
            try
            {
                using (SqlConnection conn = new SqlConnection(DBConn))
                {
                    string sql = "INSERT INTO [dbo].[Drivers] ([ID],[FirstName] ,[MiddleName] ,[LastName] ,[PhoneNumber] ,[Email] ,[AddedBy])" +
                        "VALUES ( @ID,@FirstName,@MiddleName,@LastName,@PhoneNumber,@Email,@AddedBy)";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        conn.Open();
                        cmd.Parameters.AddWithValue("@ID", ID.ToString().ToUpper());
                        cmd.Parameters.AddWithValue("@FirstName", Driver.FirstName);
                        cmd.Parameters.AddWithValue("@MiddleName", Driver.MiddleName);
                        cmd.Parameters.AddWithValue("@LastName", Driver.LastName);
                        cmd.Parameters.AddWithValue("@PhoneNumber", Driver.PhoneNumber);
                        cmd.Parameters.AddWithValue("@Email", Driver.Email);
                        cmd.Parameters.AddWithValue("@AddedBy", Driver.AddedBy);
                        i = cmd.ExecuteNonQuery();
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                i = 0;
            }
            if (i == 0)
            {
                swResponse.Status = "warning";
                swResponse.Message = "Failed to Insert. Try Again";
            }
            else
            {
                swResponse.Status = "success";
                swResponse.Message = "User Added Successfully";
            }
            return swResponse;
        }


        public MessageResult UpdateDriver(UpdateDriverModel Driver)
        {
            int i = 0;
            MessageResult swResponse = new MessageResult();
            try
            {
                using (SqlConnection conn = new SqlConnection(DBConn))
                {
                    string sql = "UPDATE Drivers SET FirstName=@FirstName,MiddleName=@MiddleName,LastName=@LastName, PhoneNumber=@PhoneNumber,Email=@Email,UpdatedBy=@UpdatedBy where ID=@ID";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        conn.Open();
                        cmd.Parameters.AddWithValue("@ID", Driver.ID.ToString().ToUpper());
                        cmd.Parameters.AddWithValue("@FirstName", Driver.FirstName);
                        cmd.Parameters.AddWithValue("@MiddleName", Driver.MiddleName);
                        cmd.Parameters.AddWithValue("@LastName", Driver.LastName);
                        cmd.Parameters.AddWithValue("@PhoneNumber", Driver.PhoneNumber);
                        cmd.Parameters.AddWithValue("@Email", Driver.Email);
                        cmd.Parameters.AddWithValue("@UpdatedBy", Driver.UpdatedBy);
                        cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);
                        i = cmd.ExecuteNonQuery();
                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                i = 0;
            }
            if (i == 0)
            {
                swResponse.Status = "warning";
                swResponse.Message = "Failed to Insert. Try Again";
            }
            else
            {
                swResponse.Status = "success";
                swResponse.Message = "User Updated Successfully";
            }
            return swResponse;
        }


        public List<DriverListModel> GetDriversList()
        {
            List<DriverListModel> _DriverList = new List<DriverListModel>();
            try
            {
                using (SqlConnection conn = new SqlConnection(DBConn))
                {
                    using (SqlCommand cmd = new SqlCommand($@"
                                  SELECT DISTINCT a.ID,max( a.PhoneNumber)PhoneNumber,max( a.FirstName)FirstName,max( a.LastName)LastName,
	max( a.MiddleName)MiddleName, max(a.Email)Email,max( Isnull(C.ShopName, 'NOT ASSIGNED'))  ShopName,
max(isnull(d.VehicleRegistration,'NOT ASSIGNED'))  VehicleRegistration
FROM Drivers as a
left join AssignShop as b on Convert(nvarchar(50), b.UserID) = Convert(nvarchar(50), a.ID)
left join Shops as c on c.ShopID = b.ShopsID
left join VehicleAssignmentHistory as e on e.DriverID = a.ID
left join Vehicles as d on d.ID = e.VehicleID group by a.ID", conn))//call Stored Procedure
                    {
                        cmd.CommandType = System.Data.CommandType.Text;
                        conn.Open();
                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            DriverListModel _data = new DriverListModel()
                            {
                                ID = reader["ID"].ToString(),
                                PhoneNumber = reader["PhoneNumber"].ToString(),
                                DriverName = reader["FirstName"].ToString() + " " + reader["LastName"].ToString() + " " + reader["MiddleName"].ToString(),
                                Email = reader["Email"].ToString(),
                                ShopName = reader["ShopName"].ToString(),
                                VehicleRegistration = reader["VehicleRegistration"].ToString()
                            };
                            _DriverList.Add(_data);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return _DriverList;
        }


        public static double CalculateDistance(double sl_lat, double sl_long, double poll_lat, double poll_long)
        {
            var R = 6372.8; // In kilometers

            var dLat = toRadians(poll_lat - sl_lat);
            var dLon = toRadians(poll_long - sl_long);
            sl_lat = toRadians(sl_lat);
            poll_lat = toRadians(poll_lat);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) + Math.Sin(dLon / 2) * Math.Sin(dLon / 2) * Math.Cos(sl_lat) * Math.Cos(poll_lat);
            var c = 2 * Math.Asin(Math.Sqrt(a));
            var distance= (R * 2 * Math.Asin(Math.Sqrt(a))).ToString("#.##");
            return Convert.ToDouble(distance);

        }

        public static double toRadians(double angle)
        {
            return Math.PI * angle / 180.0;
        }

        public List<DriverLatestLocation> GetLatestLocation(double Lat,double Long)
        {
            var Locations = new List<DriverLatestLocation>();
            try
            {
                using (SqlConnection conn = new SqlConnection(DBConn))
                {
                    using (SqlCommand cmd = new SqlCommand("GetDriverLatestLocation", conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        conn.Open();
                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            double Distance=CalculateDistance(Lat, Long, Convert.ToDouble(reader["Latitude"].ToString()), Convert.ToDouble(reader["Longitude"].ToString()));
                            int AssignedOrders = GetDriversAssignedOrders(reader["DriverID"].ToString());
                            int ActiverOrders = GetDriversActiveOrders(reader["DriverID"].ToString());
                            int DeliveredOrders = GetDriversDeliveredOrders(reader["DriverID"].ToString());
                            int RejectedOrders = GetDriversRejectedOrders(reader["DriverID"].ToString());
                            var VLevel = GetLevel(reader["DriverID"].ToString());
                            DriverLatestLocation _data = new DriverLatestLocation()
                            {
                                DriverID = reader["DriverID"].ToString(),
                                DriverName = reader["DriverName"].ToString(),
                                AssingedVehicle = reader["VehicleRegistration"].ToString(),
                                Latitude = reader["Latitude"].ToString(),
                                Longitude = reader["Longitude"].ToString(),
                                LastUpdatedAt = (DateTime)reader["AddedAt"],
                                DistanceFromCustomer = Distance,
                                AssignedOrders = AssignedOrders,
                                ActiveOrders = ActiverOrders,
                                DeliveredOrders = DeliveredOrders,
                                RejectedOrders = RejectedOrders,
                                Stock13Kg = VLevel.Stock13KG,
                                Stock6Kg = VLevel.Stock6KG,
                            };
                            Locations.Add(_data);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return Locations;
        }
        public VehicleStockLevels GetLevel(string DriverID)
        {
            var stock = new VehicleStockLevels();
            using(SqlConnection conn = new SqlConnection(DBConn))
            {
                string querry = $@"select * from VehicleStockLevels as a
left join VehicleAssignmentHistory as b on b.VehicleID = a.VehicleID
where a.Status = 0 and b.Status = 1 and DriverID = '{DriverID}'
 order by a.LastUpdated desc";
                using(SqlCommand cmd = new SqlCommand(querry,conn))
                {
                    conn.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        stock.Stock6KG = Convert.ToInt32(rdr["Stock6KG"]);
                        stock.Stock13KG = Convert.ToInt32(rdr["Stock13KG"]);
                        stock.Stock50KG = Convert.ToInt32(rdr["Stock50KG"]);
                        stock.StockBurners = Convert.ToInt32(rdr["StockBurners"]);
                        stock.StockGrills = Convert.ToInt32(rdr["StockGrills"]);
                    }
                }
            }
            return stock;
        }
        public int GetDriversAssignedOrders(string DriverID)
        {
            int orderCount = 0;
            using(SqlConnection conn = new SqlConnection(DBConn))
            {
                string querry = $@" select count(*) as AssignedOrders from Orders as a
                inner join OrderDeliveryAssignment  as b on b.OrderID = a.ID
                where b.Status = 1 and a.Status = 1 and cast(AddedAt as date) = cast(getdate() as date) and DriverID = '{DriverID}'";
                using(SqlCommand cmd = new SqlCommand(querry,conn))
                {
                    conn.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.Read())
                    {
                        orderCount = Convert.ToInt32(rdr["AssignedOrders"]);
                    }
                }
            }
            return orderCount;
        }
        public int GetDriversActiveOrders(string DriverID)
        {
            int orderCount = 0;
            using(SqlConnection conn = new SqlConnection(DBConn))
            {
                string querry = $@" select count(*) as ActiveOrders from Orders as a
                inner join OrderDeliveryAssignment  as b on b.OrderID = a.ID
                where b.Status = 1 and a.Status = 2 and cast(AddedAt as date) = cast(getdate() as date) and DriverID = '{DriverID}'";
                using(SqlCommand cmd = new SqlCommand(querry,conn))
                {
                    conn.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.Read())
                    {
                        orderCount = Convert.ToInt32(rdr["ActiveOrders"]);
                    }
                }
            }
            return orderCount;
        }
        public int GetDriversDeliveredOrders(string DriverID)
        {
            int orderCount = 0;
            using(SqlConnection conn = new SqlConnection(DBConn))
            {
                string querry = $@" select count(*) as DeliveredOrders from Orders as a
                inner join OrderDeliveryAssignment  as b on b.OrderID = a.ID
                where b.Status = 1 and a.Status = 3 and cast(AddedAt as date) = cast(getdate() as date) and DriverID = '{DriverID}'";
                using(SqlCommand cmd = new SqlCommand(querry,conn))
                {
                    conn.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.Read())
                    {
                        orderCount = Convert.ToInt32(rdr["DeliveredOrders"]);
                    }
                }
            }
            return orderCount;
        }
         
        public int GetDriversRejectedOrders(string DriverID)
        {
            int orderCount = 0;
            using(SqlConnection conn = new SqlConnection(DBConn))
            {
                string querry = $@" select count(*) as RejectedOrders from Orders as a
                inner join OrderDeliveryAssignment  as b on b.OrderID = a.ID
                where b.Status = 1  and cast(AddedAt as date) = cast(getdate() as date) and a.Status = 4 and DriverID = '{DriverID}'";
                using(SqlCommand cmd = new SqlCommand(querry,conn))
                {
                    conn.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.Read())
                    {
                        orderCount = Convert.ToInt32(rdr["RejectedOrders"]);
                    }
                }
            }
            return orderCount;
        }
        public MessageResult AssignBike(AssignVehicleModel Driver)
        {
            int i = 0;
            Guid ID = Guid.NewGuid(); //DriverID
            MessageResult swResponse = new MessageResult();
            SQLExecutor(string.Format("UPDATE VehicleAssignmentHistory SET Status=0 WHERE Status=1 AND DriverID='{0}'",Driver.DriverID));
            
                try
                {
                    using (SqlConnection conn = new SqlConnection(DBConn))
                    {
                        string sql = "INSERT INTO [dbo].[VehicleAssignmentHistory] ([ID],[VehicleID],[DriverID],[AddedBy],[AddedAt])" +
                            "VALUES ( @ID,@VehicleID,@DriverID,@AddedBy,@AddedAt)";
                        using (SqlCommand cmd = new SqlCommand(sql, conn))
                        {
                            conn.Open();
                            cmd.Parameters.AddWithValue("@ID", ID.ToString().ToUpper());
                            cmd.Parameters.AddWithValue("@VehicleID", Driver.VehicleID);
                            cmd.Parameters.AddWithValue("@DriverID", Driver.DriverID);
                            cmd.Parameters.AddWithValue("@AddedBy", Driver.AssignBy);
                            cmd.Parameters.AddWithValue("@AddedAt", DateTime.Now);
                        i = cmd.ExecuteNonQuery();
                            conn.Close();
                        }
                    }
                }
                catch (Exception ex)
                {
                    i = 0;
                }
            
                if (i == 0)
                {
                    swResponse.Status = "warning";
                    swResponse.Message = "Failed to Assign. Try Again";
                }
                else
                {
                    swResponse.Status = "success";
                    swResponse.Message = "Rider Assigned Vehicle Successfully";
                }
           

            return swResponse;
        }

        public int SQLExecutor(string sql)
        {
            int i = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(DBConn))
                {
                    con.Open();
                    SqlCommand com = new SqlCommand(sql, con);
                    com.CommandType = CommandType.Text;
                    com.Prepare();
                    com.ExecuteNonQuery();
                    con.Close();
                    con.Dispose();
                    i = 1;
                }

            }
            catch (Exception ex)
            {

                i = 0;

            }
            return 0;
        }
    }
}
