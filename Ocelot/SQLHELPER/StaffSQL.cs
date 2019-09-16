using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ocelot.Models;
using Ocelot.Service;
using System.Data.SqlClient;
using Ocelot.Logging;

namespace Ocelot.SQLHELPER
{
    public class StaffSQL
    {
        private static readonly string DBConn = Constant.DbConn;
        private static MessageResult mes = new MessageResult();
        private static string querry = String.Empty;
        private static Byte[] Password;
        private static Byte[] Keys;
        private static Byte[] IV;

        public static MessageResult AddStaff(StaffModel staff, string AddedBy)
        {
            
                String ID = Guid.NewGuid().ToString();
                int UserID;
                if (ConfirmStaffDoesntExist(staff.PayrollNumber.Trim()))
                {
                    Crypting.EncryptStringToBytes("12345", out Password, out Keys, out IV);
                using (SqlConnection conn = new SqlConnection(DBConn))
                {
                    querry = $@"Insert into tblStaff(Id,FirstName,MiddleName,LastName,PayrollNumber,DateAdded,AddedBy,
                        PhoneNumber,DepartmentID) output INSERTED.UserID
                       values(@Id,@FirstName,@MiddleName,@LastName,@PayrollNumber,@DateAdded,@AddedBy,@PhoneNumber,
                        @DepartmentID)";
                    conn.Open();
                    SqlCommand cmd = conn.CreateCommand();
                    SqlTransaction transaction = conn.BeginTransaction("AddStaff");
                    cmd.Connection = conn;
                    cmd.Transaction = transaction;
                    try
                    {
                        
                        cmd.Parameters.AddWithValue("@Id", ID.ToString());
                        cmd.Parameters.AddWithValue("@FirstName", staff.FirstName);
                        cmd.Parameters.AddWithValue("@MiddleName", staff.MiddleName);
                        cmd.Parameters.AddWithValue("@LastName", staff.LastName);
                        cmd.Parameters.AddWithValue("@PayrollNumber", staff.PayrollNumber);
                        cmd.Parameters.AddWithValue("@DateAdded", DateTime.Now);
                        cmd.Parameters.AddWithValue("@AddedBy", AddedBy);
                        cmd.Parameters.AddWithValue("@Password", Password);
                        cmd.Parameters.AddWithValue("@Keys", Keys);
                        cmd.Parameters.AddWithValue("@IV", IV);
                        cmd.Parameters.AddWithValue("@PhoneNumber", staff.PhoneNumber);
                        cmd.Parameters.AddWithValue("@DepartmentID", staff.DepartmentID);
                        cmd.CommandText = string.Format(querry);
                        UserID = (int)cmd.ExecuteScalar();
                        cmd.Parameters.AddWithValue("@UserID", UserID);
                        cmd.CommandText = string.Format($@"insert into UserCredential(UserID,Password,Keys,IV)
                            values(@UserID,@Password,@Keys,@IV)");
                        cmd.ExecuteScalar();
                        transaction.Commit();
                        mes.Status = "success";
                        mes.Message = "Staff added successfully";
                        OcelotLog.AuditLogs($"{Constant.GetUserID()} at {DateTime.Now} added.", "StaffSQL", "AddStaff");

                    }
                    catch (Exception e)
                    {

                        transaction.Rollback();
                        mes.Status = "warning";
                        mes.Message = "Failed! reload and try again later";
                        OcelotLog.ErrorLogs(e.Message.ToString(), "StaffSQL", "AddStaff");
                    }
                }

                }
                else
                {
                    mes.Status = "info";
                    mes.Message = $@"Staff with staff number:{staff.PayrollNumber} already exists";
                    OcelotLog.AuditLogs($"{Constant.GetUserID()} at {DateTime.Now} tried to add.", "StaffSQL", "AddStaff");
                }



            return mes;
        }
        
        public static MessageResult Phonenumber(StaffModel staff, string AddedBy)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(DBConn))
                {
                    querry = $@"Update tblStaff  set PhoneNumber = @PhoneNumber where  UserID = @Id 
                      ";
                    using (SqlCommand cmd = new SqlCommand(querry, conn))
                    {
                        conn.Open();
                        cmd.Parameters.AddWithValue("@UserID", staff.UserID);
                        cmd.Parameters.AddWithValue("@PhoneNumber", staff.PhoneNumber);
                        cmd.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);
                        cmd.Parameters.AddWithValue("@UpdatedBy", AddedBy);
                        cmd.ExecuteScalar();
                        mes.Status = "success";
                        mes.Message = "Phone number updated successfully";
                        OcelotLog.AuditLogs($"{Constant.GetUserID()} at {DateTime.Now} updated {staff.UserID} to {staff.PhoneNumber}.", "StaffSQL", "Phonenumber");
                    }
                }

            }
            catch (Exception e)
            {
                mes.Status = "warning";
                mes.Message = "Failed! reload and try again later";

                OcelotLog.ErrorLogs(e.Message.ToString(), "StaffSQL", "Phonenumber");
            }
            return mes;
        }
        public static List<StaffModel> GetStaffs()
        {
            var staffs = new List<StaffModel>();
            try
            {
                using (SqlConnection conn = new SqlConnection(DBConn))
                {
                    querry = $@"select a.id,UPPER(CONCAT(FirstName,' ',MiddleName,' ',LastName)) as StaffName,UserID,
                FirstName,MiddleName,LastName,a.DateAdded,PhoneNumber,b.Department,a.PayrollNumber,
                ( case  a.UserType when 1 then 'DRIVER' when 2 then 'CC' else 'User' end)UserType
                from tblStaff  as a
                left join tblDepartments as b on convert(nvarchar(50),b.ID) = a.DepartmentID";
                    using (SqlCommand cmd = new SqlCommand(querry, conn))
                    {
                        conn.Open();
                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            var staff = new StaffModel();
                            staff.FirstName = rdr["FirstName"].ToString();
                            staff.StaffName = rdr["StaffName"].ToString();
                            staff.MiddleName = rdr["MiddleName"].ToString();
                            staff.PhoneNumber = rdr["PhoneNumber"].ToString();
                            staff.PayrollNumber = rdr["PayrollNumber"].ToString();
                            staff.UserType = rdr["UserType"].ToString();
                            staff.UserID = Convert.ToInt32(rdr["UserID"]);
                            staff.LastName = rdr["LastName"].ToString();
                            staff.DateAdded = rdr["DateAdded"].ToString();
                            staff.DepartmentName = rdr["Department"].ToString();
                            staffs.Add(staff);
                        }

                    }
                }

            }
            catch (Exception e)
            {

                OcelotLog.ErrorLogs(e.Message.ToString(), "StaffSQL", "GetStaffs");
            }
            OcelotLog.AuditLogs($"{Constant.GetUserID()} at {DateTime.Now} fetched", "StaffSQL", "GetStaffs");
            return staffs;
        }
        public static bool ConfirmStaffDoesntExist(string PayrollNumber)
        {
            using (SqlConnection conn = new SqlConnection(DBConn))
            {
                var querry = $@"Select top 1 PayrollNumber from tblStaff where PayrollNumber = @PayrollNumber ";
                using (SqlCommand cmd = new SqlCommand(querry, conn))
                {
                    conn.Open();
                    cmd.Parameters.AddWithValue("@PayrollNumber", PayrollNumber);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.Read())
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}