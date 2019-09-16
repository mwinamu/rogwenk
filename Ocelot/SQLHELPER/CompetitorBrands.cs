using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ocelot.Models;
using System.Web.Http;
using Newtonsoft.Json;
using System.Data.SqlClient;
using Ocelot.Service;
using Ocelot.Logging;
using Ocelot.SQLHELPER;
using HttpGetAttribute = System.Web.Http.HttpGetAttribute;
using HttpPostAttribute = System.Web.Http.HttpPostAttribute;

namespace Ocelot.SQLHELPER
{
    public static class CompetitorBrands
    {
        private readonly static string DBConn = Constant.DbConn;
        private static string querry = String.Empty;
        private static MessageResult mes = new MessageResult();

        public static List<CompetitorBrandsModel> BrandsDropDown()
        {
            var competitors = new List<CompetitorBrandsModel>();
            try
            {
                using (SqlConnection conn = new SqlConnection(DBConn))
                {
                    querry = $@"Select BrandID,BrandName from CompetitorBrands";
                    using(SqlCommand cmd = new SqlCommand(querry, conn))
                    {
                        conn.Open();
                        SqlDataReader rdr = cmd.ExecuteReader();
                        while (rdr.Read())
                        {
                            var competitor = new CompetitorBrandsModel();
                            competitor.BrandID = Convert.ToInt32(rdr["BrandID"]);
                            competitor.BrandName = rdr["BrandName"].ToString();
                            competitors.Add(competitor);
                        }
                    }
                }

            }catch(Exception e)
            {

            }
            return competitors;
        }
    }
    public class CompetitorBrandsModel
    {
        public int BrandID { get; set; }
        public string BrandName { get; set; }
    }
}
