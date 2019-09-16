using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Ocelot.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Web;
using System.Web.Http;

namespace Ocelot.Service
{
    #region Claims manager class
    public static class ClaimsManager
    {

        private static string DBConn = Constant.DbConn;
        #region roles per user id
        public static List<Roles> RolesPerUserId(string userID)
        {
            List<Roles> roles = new List<Roles>();
            string querry = $@"select * from Role where RoleID  in
                    (select RoleID from UserRole
                    where UserId = @UserID and IsActive = 1)";
            using (SqlConnection conn = new SqlConnection(DBConn))
            {
                using (SqlCommand cmd = new SqlCommand(querry, conn))
                {
                    conn.Open();
                    cmd.Parameters.AddWithValue("@UserID", userID);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        Roles role = new Roles
                        {
                            RoleID = rdr["RoleID"].ToString(),
                            RoleName = rdr["RoleName"].ToString()
                        };
                        roles.Add(role);
                    }

                }
            }
            return roles;
        }
        #endregion
        #region is user in role 
        public static bool IsUserInRole(string roleName)
        {
            // change to claims
            var userId = Constant.GetUserID();
            if (userId != null || userId != "")
            {
                List<Roles> roles = ClaimsManager.RolesPerUserId(userId.ToString());
                //Console.WriteLine(roles);
                string[] splitrole = roleName.Split(',');
                for (int i = 0; i < splitrole.Count(); i++)
                {
                    if (splitrole[i] == "anonymous".ToUpper())
                    {
                        return true;
                    }
                    var re = roles.Find(r => r.RoleName == splitrole[i]);
                    if (re != null)
                    {
                        return true;
                    }
                }
            }

            return false;

        }
        #endregion
        #region Get all roles
        public static List<Roles> GetAllRoles()
        {
            List<Roles> roles = new List<Roles>();
            string querry = $@"select * from tblAccessRoles";
            using (SqlConnection conn = new SqlConnection(DBConn))
            {
                using (SqlCommand cmd = new SqlCommand(querry, conn))
                {
                    conn.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        Roles role = new Roles
                        {
                            RoleID = rdr["RoleID"].ToString(),
                            RoleName = rdr["RoleName"].ToString()
                        };
                        roles.Add(role);
                    }

                }
            }
            return roles;
        }
        #endregion
        #region Get all groups
        public static List<Groups> GetAllGroups()
        {
            List<Groups> groups = new List<Groups>();
            string querry = $@"select * from tblAccessGroups";
            using (SqlConnection conn = new SqlConnection(DBConn))
            {
                using (SqlCommand cmd = new SqlCommand(querry, conn))
                {
                    conn.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        Groups group = new Groups
                        {
                            GroupID = rdr["GroupID"].ToString(),
                            GroupName = rdr["GroupName"].ToString()
                        };
                        groups.Add(group);
                    }

                }
            }
            return groups;
        }
        #endregion
        #endregion
        #region add role
        public static MessageResult AddRole(string roleNAme)
        {
            MessageResult mes = new MessageResult();
            try
            {
                using (SqlConnection conn = new SqlConnection(DBConn))
                {
                    var querry = $@"Insert into tblAccessRoles(RoleID,RoleName)
                            Values(NEWID(),'{roleNAme.ToUpper()}')";
                    using (SqlCommand cmd = new SqlCommand(querry, conn))
                    {
                        conn.Open();
                        cmd.ExecuteScalar();
                        conn.Close();
                        mes.Status = "success";
                        mes.Message = "Role added successfully";
                    }
                }
            }
            catch (Exception e)
            {
                mes.Status = "warning";
                mes.Message = "Failed reload and try again later";
            }
            return mes;
        }
        #endregion
        #region Add group
        public static MessageResult AddGroup(string GroupName)
        {
            MessageResult mes = new MessageResult();
            try
            {
                using (SqlConnection conn = new SqlConnection(DBConn))
                {
                    var querry = $@"Insert into tblAccessGroups(GroupID,GroupName)
                            Values(@GroupID,'{GroupName.ToUpper()}')";
                    using (SqlCommand cmd = new SqlCommand(querry, conn))
                    {
                        conn.Open();
                        cmd.Parameters.AddWithValue("@GroupID", Guid.NewGuid());
                        cmd.CommandType = CommandType.Text;
                        cmd.ExecuteScalar();
                        conn.Close();
                        mes.Status = "success";
                        mes.Message = "Group added successfully";
                    }
                }
            }
            catch (Exception e)
            {
                mes.Status = "warning";
                mes.Message = "Failed reload and try again later";
            }
            return mes;
        }
        #endregion
        #region add group roles
        public static MessageResult AddGroupRoles(List<GroupRoles> groupRoles)
        {
            MessageResult mes = new MessageResult();
            try
            {
                if (groupRoles.Count() > 0)
                {

                    for (int g = 0; g < groupRoles.Count; g++)
                    {
                        if (g < 1)
                        {
                            stagingGroupROle(groupRoles[g].GroupID);
                        }
                        AddGroupRole(groupRoles[g].GroupID, groupRoles[g].RoleID);

                    }
                    mes.Status = "success";
                    mes.Message = "Group Roles added successfully";
                }
                else
                {
                    mes.Status = "info";
                    mes.Message = "Check and select and try again";
                }


            }
            catch (Exception e)
            {
                mes.Status = "warning";
                mes.Message = "Failed reload and try again later";
            }
            return mes;
        }
        #endregion
        #region Assign user to group
        public static MessageResult AssignUserToGroup(string UserID, string GroupID)
        {
            MessageResult mes = new MessageResult();
            try
            {
                var groupRoles = GetGroupRoles(GroupID);
                for (int i = 0; i < groupRoles.Count(); i++)
                {
                    if (i < 1)
                    {
                        stageUserRole(UserID);
                    }
                    AssignUserRole(groupRoles[i].RoleID, UserID);
                }
                mes.Status = "success";
                mes.Message = "User assigned to group successfully";
            }
            catch (Exception e)
            {
                mes.Status = "warning";
                mes.Message = "Failed! Reload and try again later";
            }


            return mes;
        }
        #endregion
        #region Get group roles
        public static List<GroupRoles> GetGroupRoles(string GroupID)
        {
            List<GroupRoles> groupRoles = new List<GroupRoles>();
            using (SqlConnection conn = new SqlConnection(DBConn))
            {
                var querry = $@"Select * from tblAccessGroupRoles where GroupID = '{GroupID}'";
                using (SqlCommand cmd = new SqlCommand(querry, conn))
                {
                    conn.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        GroupRoles group = new GroupRoles
                        {
                            RoleID = rdr["RoleID"].ToString(),
                            GroupID = rdr["GroupID"].ToString()
                        };
                        groupRoles.Add(group);
                    }
                }
            }
            return groupRoles;
        }
        #endregion
        #region Assigning user to role
        public static void AssignUserRole(string RoleID, string UserID)
        {
            using (SqlConnection conn = new SqlConnection(DBConn))
            {
                var querry = $@" 
                            Insert into tblAccessUserRoles(UserID,RoleID)
                            Values('{UserID}','{RoleID}')";
                using (SqlCommand cmd = new SqlCommand(querry, conn))
                {
                    conn.Open();
                    cmd.ExecuteScalar();
                    conn.Close();
                }
            }
        }
        public static void stageUserRole(string UserID)
        {
            using (SqlConnection conn = new SqlConnection(DBConn))
            {
                var querry = $@" delete tblAccessUserRoles where UserID = '{UserID}'";
                using (SqlCommand cmd = new SqlCommand(querry, conn))
                {
                    conn.Open();
                    cmd.ExecuteScalar();
                    conn.Close();
                }
            }
        }
        #endregion
        #region Add group roles
        public static void AddGroupRole(string GroupID, string RoleID)
        {
            using (SqlConnection conn = new SqlConnection(DBConn))
            {
                var querry = $@" 
                            Insert into tblAccessGroupRoles(GroupID,RoleID)
                            Values(@GroupID,@RoleID)";
                using (SqlCommand cmd = new SqlCommand(querry, conn))
                {
                    conn.Open();
                    cmd.Parameters.AddWithValue("@GroupID", GroupID);
                    cmd.Parameters.AddWithValue("@RoleID", RoleID);
                    cmd.ExecuteScalar();
                    conn.Close();
                }
            }
        }

        public static void stagingGroupROle(string GroupID)
        {
            using (SqlConnection conn = new SqlConnection(DBConn))
            {
                var querry = $@" delete tblAccessGroupRoles where GroupID = @GroupID";
                using (SqlCommand cmd = new SqlCommand(querry, conn))
                {
                    conn.Open();
                    cmd.Parameters.AddWithValue("@GroupID", GroupID);
                    cmd.ExecuteScalar();
                    conn.Close();
                }
            }
        }
        #endregion
        #region Get Users
        public static List<Users> GetUsers()
        {
            List<Users> users = new List<Users>();
            using (SqlConnection conn = new SqlConnection(DBConn))
            {
                var querry = $@"select UserID ,UPPER( CONCAT(FirstName,' ',LastName))as UserName from tblUsers";
                using (SqlCommand cmd = new SqlCommand(querry, conn))
                {
                    conn.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        Users user = new Users()
                        {
                            UserID = rdr["UserID"].ToString(),
                            UserName = rdr["UserName"].ToString()
                        };
                        users.Add(user);

                    }
                }
            }
            return users;
        }
        #endregion

    }
    #region Modals
    public class Roles
    {
        public string RoleID { get; set; }
        public string RoleName { get; set; }
    }
    public class Groups
    {
        public string GroupID { get; set; }
        public string GroupName { get; set; }
    }
    public class GroupRoles
    {
        public string RoleID { get; set; }
        public string GroupID { get; set; }
    }
    public class GroupROlesModel
    {
        public List<GroupRoles> groupRoles { get; set; }
    }
    public class Users
    {
        public string UserID { get; set; }
        public string UserName { get; set; }
    }
    #endregion
}