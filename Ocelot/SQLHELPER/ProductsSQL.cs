using Company.WebApplication1.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Ocelot.Service;
using Ocelot.Models;

namespace Ocelot.SQLHELPER
{
    public class ProductSQL
    {
        public static string DBConn = Constant.DbConn;
        public List<ProductsModel> GetProductsList()
        {
            List<ProductsModel> _returnBrands = new List<ProductsModel>();
            using (SqlConnection Conn = new SqlConnection(DBConn))
            {
                string sql = $@"Select p.ID ProductID,c.ID as PriceID, ProductName, Price ,isnull(d.Name,'NO ZONE') as ZoneName from Products p 
                        left outer  join Price c on c.ProductID = p.ID
                            left join Zones as d on d.ID = c.ZoneID 
                            where c.Status = 1";
                using (SqlCommand Cmd = new SqlCommand(sql, Conn))
                {
                    Conn.Open();
                    Cmd.CommandType = System.Data.CommandType.Text;
                    SqlDataReader reader = Cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        ProductsModel _data = new ProductsModel()
                        {
                            ProductName = $@"{reader["ProductName"].ToString()} || {reader["ZoneName"].ToString()}",
                            ProductID = reader["ProductID"].ToString().Trim(),
                            PriceID = reader["PriceID"].ToString().Trim(),
                            Price = reader["Price"].ToString().Trim(),
                        };
                        _returnBrands.Add(_data);
                    }
                }
            }
            return _returnBrands;
        }

        public List<ProductDropDownModel> GetProductDropDown()
        {
            var products = new List<ProductDropDownModel>();
            using (SqlConnection conn  = new SqlConnection(DBConn))
            {
                string querry = $@"select ID,ProductName as Name from Products";
                using(SqlCommand cmd = new SqlCommand(querry, conn))
                {
                    conn.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        var product = new ProductDropDownModel();
                        product.ID = rdr["ID"].ToString();
                        product.Name = rdr["Name"].ToString();
                        products.Add(product);
                    }
                    
                    
                }
            }
            return products;
        }
    }
}
