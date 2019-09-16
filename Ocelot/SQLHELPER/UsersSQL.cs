using Ocelot.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Ocelot.Logging;
using Ocelot.Service;


namespace Ocelot.SQLHELPER
{
    public static class UsersSQL
    {
        public static readonly string DBConn = Constant.DbConn;
        public static  string querry = String.Empty;
        public static MessageResult mes = new MessageResult();
        public static MessageResult AddUser(UsersListModel user)
        {
            var mes = new MessageResult();
            
                using (SqlConnection conn = new SqlConnection(DBConn))
                {
                    conn.Open();
                    SqlCommand command = conn.CreateCommand();
                    SqlTransaction transaction = conn.BeginTransaction("ADDUSER");
                    command.Connection = conn;
                    command.Transaction = transaction;

                try
                {
                    Byte[] Password;
                    Byte[] Keys;
                    Byte[] IV;
                    AccountService.EncryptStringToBytes(user.Password.Trim(),
            out Password, out Keys, out IV);
                    command.Parameters.AddWithValue("@FirstName", user.FirstName);
                    command.Parameters.AddWithValue("@LastName", user.LastName);
                    command.Parameters.AddWithValue("@Email", user.Email);
                    command.Parameters.AddWithValue("@PhoneNumber", user.PhoneNumber);
                    command.Parameters.AddWithValue("@DateAdded", DateTime.Now);
                    command.Parameters.AddWithValue("@Position", 1);
                    command.CommandText = $@"insert into Users(FirstName,LastName,Email,PhoneNumber,DateAdded,Position)
			        OUTPUT Inserted.UserID values(@FirstName,@LastName,@Email,@PhoneNumber,@DateAdded,@Position)";
                    int UserID = Convert.ToInt32(command.ExecuteScalar());
                    command.Parameters.AddWithValue("@UserID", UserID);
                    command.Parameters.AddWithValue("@Password", Password);
                    command.Parameters.AddWithValue("@Keys", Keys);
                    command.Parameters.AddWithValue("@IV", IV);
                    command.Parameters.AddWithValue("@LoginType", 1);
                    command.Parameters.AddWithValue("@Status", 1);
                    command.Parameters.AddWithValue("@AddedBy", Constant.GetUserID());
                    command.CommandText = $@"Insert into UserCredential(UserID,Password,Keys,IV,LoginType,Status,DateAdded,AddedBy)
                    values(@UserID, @Password, @Keys, @IV, @LoginType, @Status, @DateAdded, @AddedBy)";
                    command.ExecuteNonQuery();
                    transaction.Commit();
                    OcelotLog.AuditLogs($"{UserID} at {DateTime.Now} added.", "UserSQL", "AddUser");
                    mes.Status = "success";
                    mes.Message = "User registered successfully";
                }
                catch (Exception e)
                {
                    transaction.Rollback();

                    OcelotLog.ErrorLogs(e.Message.ToString(), "UserSQL", "AddUser");
                    mes.Status = "warning";
                    mes.Message = "Failed! this normaly works";
                }

            }

            
            return mes;
        }
        public static List<User> GetUsers()
        {
            var users = new List<User>();
            try
            {
                using (SqlConnection conn = new SqlConnection(DBConn))
                {
                    querry = $@"
                       select * from Users ";
                    using (SqlCommand cmd = new SqlCommand(querry, conn))
                    {
                        conn.Open();
                        SqlDataReader rdr = cmd.ExecuteReader();
                        while (rdr.Read())
                        {
                            var user = new User();
                            user.Id = rdr["Id"].ToString();
                            user.UserName = rdr["FirstName"].ToString() + " "+ rdr["LastName"].ToString();
                            user.Email = rdr["Email"].ToString();
                            user.PhoneNumber = rdr["PhoneNumber"].ToString();

                            users.Add(user);

                        }
                    }
                }

            }
            catch (Exception e)
            {
                users = null;
            }

            return users;
        }
        public static List<User> GetNewUsers()
        {
            var users = new List<User>();
            try
            {
                using (SqlConnection conn = new SqlConnection(DBConn))
                {
                    querry = $@"
                         select Distinct a.Id,UserName,Email,PhoneNumber
                          from AspNetUsers as a 
                          left join UserGroup as b on b.UserId = a.Id
                          left join Groups as c on c.Id = b.GroupId  where c.Name Is null ";
                    using (SqlCommand cmd = new SqlCommand(querry, conn))
                    {
                        conn.Open();
                        SqlDataReader rdr = cmd.ExecuteReader();
                        while (rdr.Read())
                        {
                            var user = new User();
                            user.Id = rdr["Id"].ToString();
                            user.UserName = rdr["UserName"].ToString();
                            user.Email = rdr["Email"].ToString();
                            user.PhoneNumber = rdr["PhoneNumber"].ToString();

                            users.Add(user);

                        }
                    }
                }

            }
            catch (Exception e)
            {
                users = null;
            }

            return users;
        }
        public static MessageResult AssignGroup(UserGroup user, string AddedBy)
        {
            try
            {
                UpdateGroupStatus(user.UserId);
                using (SqlConnection conn = new SqlConnection(DBConn))
                {

                    querry = $@"Insert into UserGroup(Id,UserId,GroupId,Status,AddedAt,AddedBy)
                                values(@Id,@UserId,@GroupId,@Status,@AddedAt,@AddedBy)";
                    using (SqlCommand cmd = new SqlCommand(querry, conn))
                    {
                        conn.Open();
                        cmd.Parameters.AddWithValue("@Id", Guid.NewGuid());
                        cmd.Parameters.AddWithValue("@UserId", user.UserId);
                        cmd.Parameters.AddWithValue("@GroupId", user.GroupId);
                        cmd.Parameters.AddWithValue("@Status", 1);
                        cmd.Parameters.AddWithValue("@AddedAt", DateTime.Now);
                        cmd.Parameters.AddWithValue("@AddedBy", AddedBy);
                        cmd.ExecuteScalar();
                        mes.Status = "success";
                        mes.Message = "User assigned to group successfully";

                    };
                }
                if (mes.Status == "success")
                {
                    UpdateUserRoles(user.UserId);
                }

            }
            catch (Exception e)
            {
                mes.Status = "warning";
                mes.Message = "Failed! relaod and tryb aian later";
            }

            return mes;
        }
        public static void UpdateUserRoles(string userID)
        {
            StageUserRoles(userID);
            using (SqlConnection conn = new SqlConnection(DBConn))
            {
                querry = $@"Declare @UserId nvarchar(50)
                        set @UserId = '{userID}'                        
                        insert into AspNetUserRoles
                        select b.UserId,a.RoleId from GroupRoles as a
                        left join UserGroup as b on b.UserId = @UserId 
                         where a.GroupId = (select GroupId from UserGroup where UserId = @UserId and Status = 1)
                        and a.Status = 1 and b.Status = 1";
                using (SqlCommand cmd = new SqlCommand(querry, conn))
                {
                    conn.Open();
                    cmd.ExecuteScalar();
                }
            }
        }
        public static void StageUserRoles(string UserID)
        {
            using (SqlConnection conn = new SqlConnection(DBConn))
            {
                querry = $@"delete AspNetUserRoles where UserId = '{UserID}'";
                using (SqlCommand cmd = new SqlCommand(querry, conn))
                {
                    conn.Open();
                    cmd.ExecuteScalar();
                }
            }
        }
        public static void UpdateGroupStatus(string UserID)
        {
            using (SqlConnection conn = new SqlConnection(DBConn))
            {
                querry = $@"Update UserGroup set Status = @StatusBefore where UserId = @UserId";
                using (SqlCommand cmd = new SqlCommand(querry, conn))
                {
                    conn.Open();
                    cmd.Parameters.AddWithValue("@StatusBefore", 0);
                    cmd.Parameters.AddWithValue("@UserId", UserID);
                    cmd.ExecuteScalar();
                }
            }

        }
        public static MessageResult EditUser(User user)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(DBConn))
                {
                    querry = $@"update AspNetUsers
                        set Email = @Email,NormalizedEmail = @NormalizedEmail,PhoneNumber = @PhoneNumber,UserName = @UserName,
                        NormalizedUserName = @NormalizedUserName where Id = @Id";
                    using (SqlCommand cmd = new SqlCommand(querry, conn))
                    {
                        conn.Open();
                        cmd.Parameters.AddWithValue("@Id", user.Id);
                        cmd.Parameters.AddWithValue("@NormalizedEmail", user.Email.ToUpper());
                        cmd.Parameters.AddWithValue("@Email", user.Email);
                        cmd.Parameters.AddWithValue("@PhoneNumber", user.PhoneNumber);
                        cmd.Parameters.AddWithValue("@UserName", user.UserName);
                        cmd.Parameters.AddWithValue("@NormalizedUserName", user.UserName.ToUpper());
                        cmd.ExecuteScalar();
                        mes.Status = "success";
                        mes.Message = "User edited successfully";
                    }
                }

            }
            catch (Exception e)
            {
                mes.Status = "warning";
                mes.Message = "Failed! reload and try again later";
            }
            return mes;
        }
    }
}