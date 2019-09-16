using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ocelot.Service;
using Ocelot.Models;
using System.Data.SqlClient;
using System.Data;

namespace Ocelot.SQLHELPER
{
    public class VendorSQL
    {
        string VendorConn = Constant.DbConn;
        public List<VendorSalesModel> GetAllSales(DateTime? StartDate, DateTime? EndDate)
        {
            List<VendorSalesModel> _Sales = new List<VendorSalesModel>();

            using (SqlConnection conn = new SqlConnection(VendorConn))
            {
                using (SqlCommand cmd = new SqlCommand("GetVendorSales", conn))//call Stored Procedure
                {
                    if (StartDate == null) { StartDate = DateTime.Now.Date; }
                    if (EndDate == null) { EndDate = DateTime.Now.Date; }

                    cmd.Parameters.AddWithValue("@StartDate", StartDate);
                    cmd.Parameters.AddWithValue("@EndDate", EndDate);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        VendorSalesModel _data = new VendorSalesModel()
                        {
                            VendorAttendant = reader["FirstName"].ToString().Trim() + " " + reader["LastName"].ToString().Trim(),
                            UserID = int.Parse(reader["UserID"].ToString()),
                            BusinessID = reader["BusinessID"].ToString(),
                            SaleID = reader["sale_id"].ToString().Trim(),
                            BusinessName = reader["BusinessName"].ToString(),
                            Refill_6KG = Convert.ToInt32(reader["Refill6KG"].ToString()),
                            Refill_13KG = Convert.ToInt32(reader["Refill13KG"].ToString()),
                            Refill_50KG = Convert.ToInt32(reader["Refill50KG"].ToString()),
                            Outright_6KG = Convert.ToInt32(reader["Outright6KG"].ToString()),
                            Outright_13KG = Convert.ToInt32(reader["Outright13KG"].ToString()),
                            Outright_50KG = Convert.ToInt32(reader["Outright50KG"].ToString()),
                            Complete13KG = Convert.ToInt32(reader["Complete13KG"].ToString()),
                            Complete6KG = Convert.ToInt32(reader["Complete6KG"].ToString()),
                            TranAmount = Convert.ToInt32(reader["trans_amount"].ToString()),
                            Variance = reader["Variance"].ToString(),
                            Commission = reader["Commission"].ToString(),
                            DateSold = Convert.ToDateTime(reader["TransTime"].ToString()),
                            comment = reader["comment"].ToString().Trim(),
                            TransID = reader["TransID"].ToString().Trim(),
                            Status = Convert.ToInt32(reader["Status"].ToString()),
                        };

                        _Sales.Add(_data);
                    }
                }
            }

            return _Sales;

        }



        public List<VendorStockUpModel> StockTransfer(DateTime? StartDate, DateTime? EndDate)
        {
            List<VendorStockUpModel> _Stock = new List<VendorStockUpModel>();

            using (SqlConnection conn = new SqlConnection(VendorConn))
            {
                using (SqlCommand cmd = new SqlCommand("VendorStockTransfer", conn))//call Stored Procedure
                {
                    if (StartDate == null) { StartDate = DateTime.Now.Date; }
                    if (EndDate == null) { EndDate = DateTime.Now.Date; }

                    cmd.Parameters.AddWithValue("@StartDate", StartDate.Value.Date);
                    cmd.Parameters.AddWithValue("@EndDate", EndDate.Value.Date);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        VendorStockUpModel _data = new VendorStockUpModel()
                        {

                            DriverName = reader["FirstName"].ToString().Trim() + " " + reader["LastName"].ToString().Trim(),
                            TripID = reader["TripID"].ToString().Trim(),
                            VRegNo = reader["VRegNo"].ToString().Trim(),
                            DriverID = reader["DriverID"].ToString().Trim(),
                            VendorID = reader["BusinessID"].ToString().Trim(),
                            BusinessName = reader["Outletname"].ToString().Trim(),
                            Transferred6KG = Convert.ToInt32(reader["Transferred6KG"].ToString()),
                            Transferred13KG = Convert.ToInt32(reader["Transferred13KG"].ToString()),
                            Transferred50KG = Convert.ToInt32(reader["Transferred50KG"].ToString()),
                            Burners = Convert.ToInt32(reader["Burner"].ToString()),
                            Grills = Convert.ToInt32(reader["Grill"].ToString()),
                            Status = Convert.ToInt32(reader["Status"].ToString()),
                            DateTransferred = Convert.ToDateTime(reader["DateTransferred"].ToString()),

                        };

                        _Stock.Add(_data);
                    }
                }
            }
            return _Stock;
        }


        public List<VendorCashWithdrawals> GetVendorWithdrawals(DateTime? StartDate, DateTime? EndDate)
        {
            List<VendorCashWithdrawals> Money = new List<VendorCashWithdrawals>();

            using (SqlConnection conn = new SqlConnection(VendorConn))
            {
                using (SqlCommand cmd = new SqlCommand("[VendorWithdrawals]", conn))//call Stored Procedure
                {
                    if (StartDate == null) { StartDate = DateTime.Now.Date; }
                    if (EndDate == null) { EndDate = DateTime.Now.Date; }

                    cmd.Parameters.AddWithValue("@StartDate", StartDate.Value.Date);
                    cmd.Parameters.AddWithValue("@EndDate", EndDate.Value.Date);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        VendorCashWithdrawals _data = new VendorCashWithdrawals()
                        {
                            BusinessID = reader["BusinessID"].ToString().Trim(),
                            BusinessName = reader["BusinessName"].ToString().Trim(),
                            TransactionAmount = Convert.ToInt32(reader["TransactionAmount"].ToString()),
                            ReceiverPartyPublicName = reader["ReceiverPartyPublicName"].ToString(),
                            TransID = reader["TransID"].ToString(),
                            TransactionCompletedDateTime = Convert.ToDateTime(reader["TransactionCompletedDateTime"].ToString()),
                        };

                        Money.Add(_data);
                    }
                }
            }
            return Money;
        }


        public int ProcessCommission(string[] data)
        {
            int i = 0;
            foreach (string SaleID in data)
            {
                int TotalCommission = GetCommission(SaleID);
                string BusinessID = GetBusinessID(SaleID);
                DateTime LastUpdated = DateTime.Now;
                var Validation = GetCurrentCommision(BusinessID);
                int NewBalance;
                if (Validation.ValidChecker.Length > 1)
                {
                    string[] validChecker = SystemTools.Decrypt(Validation.ValidChecker.ToString().Trim()).Split(',');
                    DateTime validDate = Convert.ToDateTime(validChecker[0]);
                    int validAmount = int.Parse(validChecker[1]);

                    if (validAmount != Convert.ToInt32(Validation.AvailableBalance) && validDate != Validation.DateUpdated)
                    {
                        return 99;
                    }
                    NewBalance = validAmount + TotalCommission;
                }
                else
                {
                    NewBalance = 0 + TotalCommission;

                }

                string NewValidChecker = SystemTools.EncryptPass(LastUpdated.ToString("yyyy-MM-dd HH:mm:ss") + "," + NewBalance);
                try
                {
                    using (SqlConnection con = new SqlConnection(VendorConn))
                    {

                        con.Open();
                        using (SqlCommand com = new SqlCommand("UPDATE tbl_comm set actual_bal=actual_bal-@TotalCommission,available_bal=@NewBalance,ValidChecker=@NewValidChecker,DateUpdated=@DateUpdated WHERE business_id=@BusinessID", con))
                        {
                            com.Parameters.AddWithValue("@TotalCommission", TotalCommission);
                            com.Parameters.AddWithValue("@NewBalance", NewBalance);
                            com.Parameters.AddWithValue("@NewValidChecker", NewValidChecker);
                            com.Parameters.AddWithValue("@DateUpdated", LastUpdated);
                            com.Parameters.AddWithValue("@BusinessID", BusinessID);
                            i = com.ExecuteNonQuery();
                        }
                    }
                    SQLExecutor("Update tbl_sales_details set status=1 where sale_id='" + SaleID + "'");
                }
                catch (Exception ex)
                {

                }
            }
            return i;
        }

        public class ValidityCheckerModel
        {
            public int AvailableBalance { get; set; }
            public string ValidChecker { get; set; }
            public DateTime DateUpdated { get; set; }
        }

        public int SQLExecutor(string sql)
        {
            int i = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(VendorConn))
                {
                    con.Open();
                    SqlCommand com = new SqlCommand(sql, con);
                    com.CommandType = CommandType.Text;
                    //com.Parameters.AddWithValue("@SaleID",SaleID);
                    i = com.ExecuteNonQuery();
                    con.Close();
                    con.Dispose();
                }

            }
            catch (Exception ex)
            {

                i = 0;
            }
            return i;
        }
        public ValidityCheckerModel GetCurrentCommision(string BusinessID)
        {
            var ValidChecker = new ValidityCheckerModel();
            try
            {
                using (SqlConnection con = new SqlConnection(VendorConn))
                {

                    con.Open();
                    using (SqlCommand com = new SqlCommand("SELECT available_bal,ValidChecker,DateUpdated from tbl_comm where business_id=@BusinessID", con))
                    {
                        com.Parameters.AddWithValue("@BusinessID", BusinessID);
                        SqlDataReader reader = com.ExecuteReader();
                        if (reader.Read())
                        {
                            ValidChecker.AvailableBalance = int.Parse(reader["available_bal"].ToString());
                            ValidChecker.ValidChecker = reader["ValidChecker"].ToString().Trim();
                            ValidChecker.DateUpdated = Convert.ToDateTime(reader["DateUpdated"].ToString().Trim());
                        }

                    }
                }
                //return ValidChecker;
            }
            catch (Exception ex)
            {
                ValidChecker.AvailableBalance = 0;
                ValidChecker.ValidChecker = "Error";
            }
            return ValidChecker;
        }

        public string GetBusinessID(string SaleID)
        {
            string BusinessID = string.Empty;
            try
            {
                using (SqlConnection con = new SqlConnection(VendorConn))
                {

                    con.Open();
                    using (SqlCommand com = new SqlCommand("SELECT top 1 business_id from tbl_sales_details where sale_id=@SaleID", con))
                    {
                        com.Parameters.AddWithValue("@SaleID", SaleID);
                        SqlDataReader reader = com.ExecuteReader();
                        if (reader.Read())
                        {
                            BusinessID = reader["business_id"].ToString().Trim();
                        }

                    }
                }
                //return ValidChecker;
            }
            catch (Exception ex)
            {
            }
            return BusinessID;
        }

        public int GetCommission(string SaleID)
        {
            int TotalCommission = 0;
            using (SqlConnection conn = new SqlConnection(VendorConn))
            {
                string sql = "SELECT ProductDesc,sum(Quantity)Quantity from tbl_sold_products where SaleID='" + SaleID + "' GROUP BY ProductDesc";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.CommandType = System.Data.CommandType.Text;
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        if (reader["ProductDesc"].ToString() == "Refill6KG")
                        {
                            TotalCommission = TotalCommission + (210 * Convert.ToInt32(reader["Quantity"].ToString()));

                        }
                        else if (reader["ProductDesc"].ToString() == "Refill13KG")
                        {
                            TotalCommission = TotalCommission + (450 * Convert.ToInt32(reader["Quantity"].ToString()));

                        }
                        else if (reader["ProductDesc"].ToString() == "Outright6KG")
                        {
                            TotalCommission = TotalCommission + (325 * Convert.ToInt32(reader["Quantity"].ToString()));

                        }
                        else if (reader["ProductDesc"].ToString() == "Outright13KG")
                        {
                            TotalCommission = TotalCommission + (800 * Convert.ToInt32(reader["Quantity"].ToString()));

                        }
                        else if (reader["ProductDesc"].ToString() == "Completet6KG")
                        {
                            TotalCommission = TotalCommission + 0;

                        }
                        else if (reader["ProductDesc"].ToString() == "Completet13KG")
                        {
                            TotalCommission = TotalCommission + 0;
                        }

                    }
                }
            }
            return TotalCommission;
        }
        
    }
}