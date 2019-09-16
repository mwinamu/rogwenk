using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//using System.Web.Http.Filters;
using System.Web.Http;
using System.Security.Claims;
using System.Web.Security;
using Ocelot.Service;

namespace Ocelot.Service
{
    public class AppRoleProvider : RoleProvider
    {

        public override string[] GetAllRoles()
        {
            List<Roles> userContext = ClaimsManager.GetAllRoles();

            return userContext.Select(r => r.RoleName).ToArray();

        }

        public override string[] GetRolesForUser(string username)
        {
            ClaimsPrincipal principal = HttpContext.Current.User as ClaimsPrincipal;
            // username in this case we use user id
            List<Roles> user = ClaimsManager.RolesPerUserId(principal.FindFirst(ClaimTypes.Name).Value);
            var userRoles = user.Select(r => r.RoleName);

            if (user == null)
                return new string[] { };
            return userRoles == null ? new string[] { } :
                    userRoles.ToArray();

        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            ClaimsPrincipal principal = HttpContext.Current.User as ClaimsPrincipal;
            List<Roles> user = ClaimsManager.RolesPerUserId(principal.FindFirst(ClaimTypes.Name).Value);
            var userRoles = user.Select(r => r.RoleName);

            if (user == null)
                return false;
            return userRoles != null &&
                userRoles.Any(r => r == roleName);

        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            throw new NotImplementedException();
        }
        public override string ApplicationName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

    }
}