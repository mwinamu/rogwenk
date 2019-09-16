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
    public static  class PriceSQL
    {

        private static string DBConn = Constant.DbConn;
        private static MessageResult mes = new MessageResult();
        private static string querry = String.Empty;
 
        public static List<PriceModel> GetPrices()
        {
            var prices = new List<PriceModel>();
            try
            {
                using (SqlConnection conn = new SqlConnection(DBConn))
                {
                    querry = $@"select a.ID,a.ProductID,a.Price,c.Email as AddedBy,a.DateAdded,
                            b.ProductName from Price as a 
                            inner join Products as b on b.ID =  a.ProductID
                            inner join Users as c on Convert(nvarchar(50),c.UserID) = a.AddedBy 
                            where a.Status = 1
                            order by a.DateAdded desc";

                    using (SqlCommand cmd = new SqlCommand(querry, conn))
                    {
                        conn.Open();
                        SqlDataReader rdr = cmd.ExecuteReader();
                        while (rdr.Read())
                        {
                            var price = new PriceModel();
                            price.ID = rdr["ID"].ToString();
                            price.ProductID = rdr["ProductID"].ToString();
                            price.Price = rdr["Price"].ToString();
                            price.AddedBy = rdr["AddedBy"].ToString();
                            price.DateAdded = rdr["DateAdded"].ToString();
                            price.ProductName = rdr["ProductName"].ToString();
                            //price.ZoneName = rdr["ZoneName"].ToString();
                            prices.Add(price);
                        }

                    }
                }
            }catch(Exception e)
            {
                prices = null;
            }
            
            return prices;
        }
        public static MessageResult AddPrice(PriceModel price,string AddedBy)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(DBConn))
                {
                    querry = $@"Update Price set Status = @Status where ProductID = @ProductID 
                            Insert into price(ID,ProductID,Price,AddedBy,DateAdded)
                            values(@ID,@ProductID,@Price,@AddedBy,@DateAdded)";
                    using(SqlCommand cmd = new SqlCommand(querry, conn))
                    {
                        conn.Open();
                        cmd.Parameters.AddWithValue("@ID", Guid.NewGuid());
                        cmd.Parameters.AddWithValue("@ProductID",price.ProductID);
                        //cmd.Parameters.AddWithValue("@ZoneID", price.ZoneID);
                        cmd.Parameters.AddWithValue("@Price", price.Price);
                        cmd.Parameters.AddWithValue("@AddedBy",AddedBy);
                        cmd.Parameters.AddWithValue("@Status", 0);
                        cmd.Parameters.AddWithValue("@DateAdded", DateTime.Now);

                        cmd.ExecuteScalar();
                        mes.Status = "success";
                        mes.Message = "Price added successfully";
                        
                    }
                }
            }catch(Exception e)
            {
                mes.Status = "info";
                mes.Status = "Failed! reload and try again later";
            }
         
            return mes;
        }

    }
}
