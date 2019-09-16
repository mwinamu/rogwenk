using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Company.WebApplication1.Models;
using Ocelot.Service;
using Ocelot.Models;

namespace Ocelot.SQLHELPER
{
    public class VehicleSQL
    {
        private static string DBConn = Constant.DbConn;
        private static string querry = String.Empty;
        private static MessageResult mes = new MessageResult();
        public MessageResult AddVehicle(VehicleModel vehicle)
        {
            int i = 0;
            Guid ID = Guid.NewGuid();
            MessageResult swResponse = new MessageResult();
            try
            {
                using (SqlConnection conn = new SqlConnection(DBConn))
                {
                    string sql = "INSERT INTO [dbo].[Vehicles] ([ID],[VehicleRegistration] ,[VehicleType] ,[VehicleCapacity] ,[FuelConsumption],[AddedBy])" +
                        "VALUES (@ID,@VRegNO,@VType,@VCapacity,@FuelConsumption,@AddedBy)";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        conn.Open();
                        cmd.Parameters.AddWithValue("@ID", ID.ToString().ToUpper());
                        cmd.Parameters.AddWithValue("@VRegNO", vehicle.VehicleRegistration);
                        cmd.Parameters.AddWithValue("@VType", vehicle.VehicleType);
                        cmd.Parameters.AddWithValue("@VCapacity", vehicle.VehicleCapacity);
                        cmd.Parameters.AddWithValue("@FuelConsumption", vehicle.FuelConsumption);
                        cmd.Parameters.AddWithValue("@AddedBy", vehicle.AddedBy);
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
                swResponse.Message = "Vehicle Added Successfully";
            }
            return swResponse;
        }


        public MessageResult UpdateVehicle(EditVehicleModel vehicle,string id)
        {
            int i = 0;
            MessageResult swResponse = new MessageResult();
            try
            {
                using (SqlConnection conn = new SqlConnection(DBConn))
                {
                    string sql = "UPDATE Vehicles SET VehicleRegistration=@VRegNO,VehicleType=@VType,VehicleCapacity=@VCapacity, FuelConsumption=@FuelConsumption,UpdatedBy=@UpdatedBy,UpdatedAt=@UpdatedAt where ID=@ID";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        conn.Open();
                        cmd.Parameters.AddWithValue("@ID",vehicle.ID);
                        cmd.Parameters.AddWithValue("@VRegNO", vehicle.VehicleRegistration);
                        cmd.Parameters.AddWithValue("@VType", vehicle.VehicleType);
                        cmd.Parameters.AddWithValue("@VCapacity", vehicle.VehicleCapacity);
                        cmd.Parameters.AddWithValue("@FuelConsumption", vehicle.FuelConsumption);
                        cmd.Parameters.AddWithValue("@UpdatedBy", id);
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
                swResponse.Message = "Vehicle Updated Successfully";
            }
            return swResponse;
        }


        public List<VehicleModel> GetVehicleList()
        {
            List<VehicleModel> _Vehicle = new List<VehicleModel>();
            try
            {
                using (SqlConnection conn = new SqlConnection(DBConn))
                {
                    using (SqlCommand cmd = new SqlCommand("SELECT * FROM Vehicles Order by AddedAt desc", conn))//call Stored Procedure
                    {
                        cmd.CommandType = System.Data.CommandType.Text;
                        conn.Open();
                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            VehicleModel _data = new VehicleModel()
                            {
                                ID = reader["ID"].ToString(),
                                VehicleRegistration = reader["VehicleRegistration"].ToString(),
                                VehicleType = reader["VehicleType"].ToString(),
                                VehicleCapacity = (int)reader["VehicleCapacity"],
                                FuelConsumption = reader["FuelConsumption"].ToString(),
                                IsAvailable = Convert.ToInt32(reader["IsAvailable"]),
                                AddedBy = reader["AddedBy"].ToString(),
                            };
                            _Vehicle.Add(_data);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return _Vehicle;
        }
        public List<VehicleModel> GetOffLoadingVehicleList()
        {
            List<VehicleModel> _Vehicle = new List<VehicleModel>();
            try
            {
                using (SqlConnection conn = new SqlConnection(DBConn))
                {
                    using (SqlCommand cmd = new SqlCommand(@"select * from Vehicles as a
inner join VehicleStockLogs as b on b.VehicleID = Convert(nvarchar(50), a.id)
where b.Status = 2", conn))//call Stored Procedure
                    {
                        cmd.CommandType = System.Data.CommandType.Text;
                        conn.Open();
                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            VehicleModel _data = new VehicleModel()
                            {
                                ID = reader["ID"].ToString(),
                                VehicleRegistration = reader["VehicleRegistration"].ToString(),
                                VehicleType = reader["VehicleType"].ToString(),
                                VehicleCapacity = (int)reader["VehicleCapacity"],
                                FuelConsumption = reader["FuelConsumption"].ToString(),
                                IsAvailable = Convert.ToInt32(reader["IsAvailable"]),
                                AddedBy = reader["AddedBy"].ToString(),
                            };
                            _Vehicle.Add(_data);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return _Vehicle;
        }

        public MessageResult UpdateAvailability(VehicleModel vehicle,string UpdateBy)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(DBConn))
                {
                    querry = $@"update Vehicles set IsAvailable = @IsAvailable,
                            UpdatedBy = @UpdateBy, UpdatedAt  = @UpdatedAt where ID = @ID";
                    using(SqlCommand cmd = new SqlCommand(querry, conn))
                    {
                        conn.Open();
                        cmd.Parameters.AddWithValue("@ID", vehicle.ID);
                        cmd.Parameters.AddWithValue("@IsAvailable", vehicle.IsAvailable);
                        cmd.Parameters.AddWithValue("@UpdateBy", UpdateBy);
                        cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);
                        cmd.ExecuteScalar();
                        mes.Status = "success";
                        mes.Message = "vehicle availabilty updated successfully";
                    }
                }

            }catch(Exception e)
            {
                mes.Status = "warning";
                mes.Message = "Failed! reload and try again later";
            }
            return mes;
        }

    }
}
