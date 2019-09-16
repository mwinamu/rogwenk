using Ocelot.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ocelot.Logging
{
    public static class OcelotLog
    {
        public static void ErrorLogs(string error,string controller,string action)
        {
            //using (System.IO.StreamWriter write = new System.IO.StreamWriter($"C://Rogwek//Logs//Error//errs{DateTime.Now.ToString("yyyyMMdd")}.txt", true))
            //{
            //    write.WriteLine(Environment.NewLine);
            //    write.WriteLine(DateTime.Now.ToString() + ":  " + error +" in :" +controller+"/"+action);
            //}
        }
        public static void AuditLogs(string error, string controller, string action)
        {
            //using (System.IO.StreamWriter write = new System.IO.StreamWriter($"C://Rogwek//Logs//Audit//{Constant.GetUserID()}logs{DateTime.Now.ToString("yyyyMMdd")}.txt", true))
            //{
            //    write.WriteLine(Environment.NewLine);
            //    write.WriteLine(DateTime.Now.ToString() + ":  " + error + " in :" + controller + "/" + action);
            //}
        }
    }
}
