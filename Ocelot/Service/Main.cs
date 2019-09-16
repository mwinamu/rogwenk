using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ocelot.Service
{
    public static class Main
    {
        public static double ConvertToUnixTimestamp()
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan diff = DateTime.Now.ToUniversalTime() - origin;
            return Math.Floor(diff.TotalSeconds);
        }
    }
}