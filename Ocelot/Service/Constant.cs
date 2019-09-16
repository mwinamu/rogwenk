using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Web;

namespace Ocelot.Service
{
    public static class Constant
    {
        public static readonly string DbConn = "Server=27.102.132.172; User ID=sa;Password=EsqueKing1;Database=rogwekbase";
        public static readonly string Conn = "Server=27.102.132.172;User ID=sa;Password=EsqueKing1;Database=rogwekbase";
        

        //public static readonly string NotConn = "Server=27.102.132.172;User ID=sa;Password=EsqueKing1;Database=TrippyGo";
        //public static readonly string DbConn = "Server=PEL047M;Integrated Security=true;Database=cokebase";
        // public static readonly string Conn = "Server=PEL047M;Integrated Security=true;Database=cokebase";


        public static string GetUserID()
        {
            string UserID = String.Empty;
            try
            {
                ClaimsPrincipal principal = HttpContext.Current.User as ClaimsPrincipal;
                UserID = principal.FindFirst(ClaimTypes.NameIdentifier).Value;
            }
            catch (Exception e)
            {

            }

            return UserID;

        }
    }
}
