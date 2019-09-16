using Company.WebApplication1.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Transactions;
using Ocelot.Models;
using Ocelot.Service;

namespace Ocelot.SQLHELPER
{
    public static  class StockTransferredOutSQL
    {
        public static string DBConn = Constant.DbConn;
        public static MessageResult mes = new MessageResult();

        public static IEnumerable<StockTransferredOut> StockTransferredOut(string type,string UserID, DateTime FromDate, DateTime ToDate)
        {
            var stocks = new List<StockTransferredOut>();
            
            try
            {
                var ShopID = GetShopID(UserID);
                string querry = String.Empty;
                if (ShopID.Equals(""))
                {
                    querry = $@"select top 50 a.ID,a.Transferred6KG,a.Transferred13KG, a.Transferred50KG,a.TransferredAt, ISNULL( b.VehicleRegistration,a.VehicleID)  as VehicleRegistration,
                    c.NormalizedEmail as TransferredBy,a.TransferredStockGrill,a.TransferredStockBurner from ContainerStockLogs as a 
                    left join Vehicles as b on Convert(nvarchar(50),b.ID) = Convert(nvarchar(50),a.VehicleID)
                    left join AspNetUsers as c on Convert(nvarchar(50),c.Id) = COnvert(nvarchar(50),a.TransferredBy)
                    where Action = 'IN' and cast(a.TransferredAt as date)>=cast(@FromDate as date)  and cast(a.TransferredAt as date)<=
cast(@ToDate as date)
                    order by  a.TransferredAt desc
                        ";
                }else if (type.Equals("OFFLOAD"))
                {
                    if (ShopID.Equals(""))
                    {
                        querry = $@"select distinct top 50 a.ID,a.Transferred6KG,a.Transferred13KG, a.Transferred50KG,a.TransferredAt, ISNULL( b.VehicleRegistration,a.VehicleID)  as VehicleRegistration,
                    c.NormalizedEmail as TransferredBy,a.TransferredStockGrill,a.TransferredStockBurner from ContainerStockLogs as a 
                    left join Vehicles as b on Convert(nvarchar(50),b.ID) = Convert(nvarchar(50),a.VehicleID)
					left join VehicleStockLogs as d on d.VehicleID = Convert(nvarchar(50),a.VehicleID)
                    left join AspNetUsers as c on Convert(nvarchar(50),c.Id) = COnvert(nvarchar(50),a.TransferredBy)
                    where a.Action = 'IN' and d.Status = 99 and cast(a.TransferredAt as date)>=cast(@FromDate as date)  and cast(a.TransferredAt as date)<=
cast(@ToDate as date)
                    order by  a.TransferredAt desc
                        ";
                    }
                    else
                    {
                        querry = $@"select distinct top 50  a.ID,a.Transferred6KG,a.Transferred13KG, a.Transferred50KG,a.TransferredAt, ISNULL( b.VehicleRegistration,a.VehicleID)  as VehicleRegistration,
                    c.NormalizedEmail as TransferredBy,a.TransferredStockGrill,a.TransferredStockBurner from ContainerStockLogs as a 
                    left join Vehicles as b on Convert(nvarchar(50),b.ID) = Convert(nvarchar(50),a.VehicleID)
					left join VehicleStockLogs as d on d.VehicleID = Convert(nvarchar(50),a.VehicleID)
                    left join AspNetUsers as c on Convert(nvarchar(50),c.Id) = COnvert(nvarchar(50),a.TransferredBy)
                    where a.Action = 'IN' and d.Status = 99 and ShopID = '{ShopID}'
                    and cast(a.TransferredAt as date)>=cast(@FromDate as date)  and cast(a.TransferredAt as date)<=
cast(@ToDate as date)
                    order by  a.TransferredAt desc
                        ";
                    }
                        
                }
                else
                {
                    querry = $@"select top 50 a.ID,a.Transferred6KG,a.Transferred13KG, a.Transferred50KG,a.TransferredAt, ISNULL( b.VehicleRegistration,a.VehicleID)  as VehicleRegistration,
                    c.NormalizedEmail as TransferredBy,a.TransferredStockGrill,a.TransferredStockBurner from ContainerStockLogs as a 
                    left join Vehicles as b on Convert(nvarchar(50),b.ID) = Convert(nvarchar(50),a.VehicleID)
                    left join AspNetUsers as c on Convert(nvarchar(50),c.Id) = COnvert(nvarchar(50),a.TransferredBy)
                    where Action = 'OUT' and ShopID = '{ShopID}' and cast(a.TransferredAt as date)>=cast(@FromDate as date)  and cast(a.TransferredAt as date)<=
cast(@ToDate as date)
                    order by  a.TransferredAt desc";
                }
                using (SqlConnection conn = new SqlConnection(DBConn))
                { 
                    using (SqlCommand cmd = new SqlCommand(querry, conn))
                    {
                        conn.Open();
                        //cmd.Parameters.AddWithValue("@Action", type);
                        cmd.Parameters.AddWithValue("@FromDate", FromDate);
                        cmd.Parameters.AddWithValue("@ToDate", ToDate);
                        SqlDataReader rdr = cmd.ExecuteReader();
                        while (rdr.Read())
                        {
                            var stock = new StockTransferredOut();
                            stock.ID = rdr["ID"].ToString();
                            stock.VehicleReg = rdr["VehicleRegistration"].ToString();
                            stock.Transferred13KG = Convert.ToInt32(rdr["Transferred13KG"]);
                            stock.Transferred50KG = Convert.ToInt32(rdr["Transferred50KG"]);
                            stock.Transferred6KG = Convert.ToInt32(rdr["Transferred6KG"]);
                            stock.TransferredAt = rdr["TransferredAt"].ToString();
                            stock.TransferredBy = rdr["TransferredBy"].ToString();
                            stock.TransferredStockGrill = Convert.ToInt32(rdr["TransferredStockGrill"]);
                            stock.TransferredStockBurner = Convert.ToInt32(rdr["TransferredStockBurner"]);

                            stocks.Add(stock);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                stocks = null;
            }
            return stocks;
        }
        public static IEnumerable<StockTransferredOut> ShopStockTransferredOut(string UserID, DateTime FromDate, DateTime ToDate)
        {
            var stocks = new List<StockTransferredOut>();
            try
            {
                var shopID = GetShopID(UserID);
                string querry = String.Empty;
                using (SqlConnection conn = new SqlConnection(DBConn))
                {
                    if (shopID.Equals(""))
                    {
                        querry = $@"select distinct top 100 a.ID,a.Transferred6KG,a.Transferred13KG, a.Transferred50KG,a.TransferredAt, ISNULL( b.VehicleRegistration,a.VehicleID)  as VehicleRegistration,
concat(c.Firstname,' ',c.LastName) TransferredBy,a.TransferredStockGrill,a.TransferredStockBurner,Upper(h.ShopName) as ShopName from ContainerStockLogs as a 
inner join Vehicles as b on Convert(nvarchar(50),b.ID) = Convert(nvarchar(50),a.VehicleID)
inner join users as c on Convert(nvarchar(50),c.userID) = COnvert(nvarchar(50),a.TransferredBy)
inner join VehicleAssignmentHistory as f on f.VehicleID = a.VehicleID
inner join Drivers as e on e.ID = f.DriverID
inner join AssignShop as g on Convert(nvarchar(50),g.UserID) = convert(nvarchar(50), e.ID)
inner join Shops as h on h.ShopID = g.ShopsID
where Action = 'OUT'  and Type is Null and f.Status = 1 and
cast(a.TransferredAt as date)>=cast(@FromDate as date)  and cast(a.TransferredAt as date)<=
cast(@ToDate as date) AND G.StatuS = 1
order by  a.TransferredAt desc";
                    }
                    else
                    {
                       querry = $@"select distinct top 100 a.ID,a.Transferred6KG,a.Transferred13KG, a.Transferred50KG,a.TransferredAt, ISNULL( b.VehicleRegistration,a.VehicleID)  as VehicleRegistration,
concat(c.Firstname,' ',c.LastName) as TransferredBy,a.TransferredStockGrill,a.TransferredStockBurner,Upper(h.ShopName) as ShopName from ContainerStockLogs as a 
inner join Vehicles as b on Convert(nvarchar(50),b.ID) = Convert(nvarchar(50),a.VehicleID)
inner join Users as c on Convert(nvarchar(50),c.UserID) = COnvert(nvarchar(50),a.TransferredBy)
inner join VehicleAssignmentHistory as f on f.VehicleID = a.VehicleID
inner join Drivers as e on e.ID = f.DriverID
inner join AssignShop as g on convert(nvarchar(50),g.UserID) = Convert(nvarchar(50),e.ID)
inner join Shops as h on h.ShopID = a.ShopID
where Action = 'OUT'  and Type is Null and f.Status = 1 and a.ShopID = '{shopID}' and 
cast(a.TransferredAt as date)>=cast(@FromDate as date)  and cast(a.TransferredAt as date)<=
cast(@ToDate as date)
order by  a.TransferredAt desc ";

                    }
                    
                    using (SqlCommand cmd = new SqlCommand(querry, conn))
                    {
                        conn.Open();
                        cmd.Parameters.AddWithValue("@FromDate", FromDate);
                        cmd.Parameters.AddWithValue("@ToDate", ToDate);
                        SqlDataReader rdr = cmd.ExecuteReader();
                        while (rdr.Read())
                        {
                            var stock = new StockTransferredOut();
                            stock.ID = rdr["ID"].ToString();
                            stock.VehicleReg = rdr["VehicleRegistration"].ToString();
                            stock.ShopName = rdr["ShopName"].ToString();
                            stock.Transferred13KG = Convert.ToInt32(rdr["Transferred13KG"]);
                            stock.Transferred50KG = Convert.ToInt32(rdr["Transferred50KG"]);
                            stock.Transferred6KG = Convert.ToInt32(rdr["Transferred6KG"]);
                            stock.TransferredAt = rdr["TransferredAt"].ToString();
                            stock.TransferredBy = rdr["TransferredBy"].ToString();
                            stock.TransferredStockGrill = Convert.ToInt32(rdr["TransferredStockGrill"]);
                            stock.TransferredStockBurner = Convert.ToInt32(rdr["TransferredStockBurner"]);

                            stocks.Add(stock);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                stocks = null;
            }
            return stocks;
        }
        public static IEnumerable<StockTransferredOut> ShopStockFromDistributor(string UserID, DateTime FromDate, DateTime ToDate)
        {
            var stocks = new List<StockTransferredOut>();
            try
            {
                var shopID = GetShopID(UserID);
                string querry = String.Empty;
                if (shopID.Equals(""))
                {
                    querry = $@"select top 100 a.ID,a.Transferred6KG,a.Transferred13KG, a.Transferred50KG,a.TransferredAt,
                     ISNULL( b.VehicleRegistration,a.VehicleID)  as VehicleRegistration,
                    c.NormalizedEmail as TransferredBy,a.TransferredStockGrill,a.TransferredStockBurner,e.ShopName  from ContainerStockLogs as a 
                    left join Vehicles as b on Convert(nvarchar(50),b.ID) = Convert(nvarchar(50),a.VehicleID)
                    left join Users as c on Convert(nvarchar(50),c.UserID) = COnvert(nvarchar(50),a.TransferredBy)
                    left join shops as e on e.ShopId = a.ShopID
                    where Action = 'IN'  and Type = 'EXTERNAL'
                    and cast(a.TransferredAt as date)>=cast(@FromDate as date)  and cast(a.TransferredAt as date)<=
cast(@ToDate as date)
                            order by  a.TransferredAt desc";
                }
                else
                {
                    querry = $@"select top 100 a.ID,a.Transferred6KG,a.Transferred13KG, a.Transferred50KG,a.TransferredAt,
                     ISNULL( b.VehicleRegistration,a.VehicleID)  as VehicleRegistration,
                    c.Email as TransferredBy,a.TransferredStockGrill,a.TransferredStockBurner,e.ShopName from ContainerStockLogs as a 
                    left join Vehicles as b on Convert(nvarchar(50),b.ID) = Convert(nvarchar(50),a.VehicleID)
                    left join Users as c on Convert(nvarchar(50),c.UserID) = COnvert(nvarchar(50),a.TransferredBy)
                        left join shops as e on e.ShopId = a.ShopID
                    where Action = 'IN' and a.ShopID = '{shopID}' and Type = 'EXTERNAL'
                    and cast(a.TransferredAt as date)>=cast(@FromDate as date)  and cast(a.TransferredAt as date)<=
                    cast(@ToDate as date)
                            order by  a.TransferredAt desc";
                }
                using(TransactionScope scope = new TransactionScope())
                {
                    using (SqlConnection conn = new SqlConnection(DBConn))
                    {
                        using (SqlCommand cmd = new SqlCommand(querry, conn))
                        {
                            conn.Open();

                            cmd.Parameters.AddWithValue("@FromDate", FromDate);
                            cmd.Parameters.AddWithValue("@ToDate", ToDate);
                            SqlDataReader rdr = cmd.ExecuteReader();
                            while (rdr.Read())
                            {
                                var stock = new StockTransferredOut();
                                stock.ID = rdr["ID"].ToString();
                                stock.VehicleReg = rdr["VehicleRegistration"].ToString();
                                stock.ShopName = rdr["ShopName"].ToString();
                                stock.Transferred13KG = Convert.ToInt32(rdr["Transferred13KG"]);
                                stock.Transferred50KG = Convert.ToInt32(rdr["Transferred50KG"]);
                                stock.Transferred6KG = Convert.ToInt32(rdr["Transferred6KG"]);
                                stock.TransferredAt = rdr["TransferredAt"].ToString();
                                stock.TransferredBy = rdr["TransferredBy"].ToString();
                                stock.TransferredStockGrill = Convert.ToInt32(rdr["TransferredStockGrill"]);
                                stock.TransferredStockBurner = Convert.ToInt32(rdr["TransferredStockBurner"]);

                                stocks.Add(stock);
                            }
                        }
                    }
                    scope.Complete();
                }
                
            }
            catch (Exception e)
            {
                stocks = null;
            }
            return stocks;
        }
        public static MessageResult TransferStockOut(StockTransferredOut stock,string addedBy)
        {
           
            try
            {
                var currentDriver = CurrentVehicleCheck(stock.VehicleID);
                String ShopID = GetShopID(addedBy);
                if (ShopID.Equals(""))
                {
                    mes.Status = "info";
                    mes.Message = "assign user to shop before you proceed";
                    return mes;
                }
               // using(TransactionScope scope = new TransactionScope())
               // {
                    using (SqlConnection conn = new SqlConnection(DBConn))
                    {
                        string querry = $@"Insert into ContainerStockLogs(ID,VehicleID,Transferred6KG,
                        Transferred13KG,Transferred50KG,TransferredAt,TransferredBy,Action,ShopID,
                            TransferredStockGrill,TransferredStockBurner)
                         values(@ID,@VehicleID,@Transferred6KG,@Transferred13KG,@Transferred50KG,
                           @TransferredAt,@TransferredBy,@Action,@ShopID,
                            @TransferredStockGrill,@TransferredStockBurner)";
                        using (SqlCommand cmd = new SqlCommand(querry, conn))
                        {
                            conn.Open();
                            cmd.Parameters.AddWithValue("@ID", Guid.NewGuid());
                            cmd.Parameters.AddWithValue("@VehicleID", stock.VehicleID);
                            cmd.Parameters.AddWithValue("@Transferred6KG", stock.Transferred6KG);
                            cmd.Parameters.AddWithValue("@Transferred13KG", stock.Transferred13KG);
                            cmd.Parameters.AddWithValue("@Transferred50KG", stock.Transferred50KG);
                            cmd.Parameters.AddWithValue("@TransferredStockGrill", stock.TransferredStockGrill);
                            cmd.Parameters.AddWithValue("@TransferredStockBurner", stock.TransferredStockBurner);
                            cmd.Parameters.AddWithValue("@TransferredAt", DateTime.Now);
                            cmd.Parameters.AddWithValue("@TransferredBy", addedBy);
                            cmd.Parameters.AddWithValue("@ShopID", ShopID);
                            cmd.Parameters.AddWithValue("@Action", "OUT");
                           
                            var current = ContainerStock(ShopID);
                            if (current.ID == null)
                            {
                                mes.Status = "info";
                                mes.Message = "Shop does not contain any stock to transfer out";
                                return mes;
                            }
                          
                           
                                // EffectVehicleStockLevel(stock);

                                if (currentDriver.Status == 1)
                                {
                                    mes.Message = "Driver has an active trip, let them end the proceed";
                                    mes.Status = "info";
                                }
                                else if (currentDriver.Status == 0)
                                {
                                    mes.Message = "You have already loaded this truck but triver has not yet accepeted stock";
                                    mes.Status = "info";
                                }
                                else if (currentDriver.Status == 2)
                                {
                                    mes.Message = "Driver has ended trip but you have not unloaded";
                                    mes.Status = "info";
                                }
                                else
                                {
                                    AddVehicleStockLog(stock, "IN", ShopID, "0");
                                    mes = ReduceStockLevel(stock, ShopID, current);
                            if (mes.Status.Equals("success"))
                            {
                                  cmd.ExecuteScalar();
                                    mes.Status = "success";
                                    mes.Message = "Stock transfered succesfully";


                                }


                            }

                        }
                    }
                   // scope.Complete();
               // }
                
            }catch(Exception e)
            {
                mes.Status = "warning";
                mes.Message = "Failed! reload and try again later";
            }
            return mes;
        }
        public static MessageResult TransferStockIn(StockModels stock,string addedBy)
        {
            try
            {
                using(TransactionScope tr = new TransactionScope())
                {
                    var current = CurrentVehicle(stock.Unsold.VehicleID);
                    var CheckTriEnd = VehicleOffloadingCheck(stock.Unsold.VehicleID);
                    if (current.TripID == null)
                    {
                        mes.Status = "info";
                        mes.Message = "Can not unload this vehicle because it was not loaded";
                        return mes;
                    }
                    if (CheckTriEnd.Status != 2)
                    {
                        mes.Status = "info";
                        mes.Message = "Can not unload this vehicle because trip has not been ended";
                        return mes;
                    }
                    String ShopID = GetShopID(addedBy);
                    if (ShopID.Equals(""))
                    {
                        mes.Status = "info";
                        mes.Message = "assign user to shop before you proceed";
                        return mes;
                    }
                    //getVariance
                    if(current.Stock6KG < stock.Unsold.Transferred6KG)
                    {
                        mes.Status = "info";
                        mes.Message = $@"Vehicle loaded {current.Stock6KG}6KG 
                            but returns {stock.Unsold.Transferred6KG}6KG, doesnot make sense ";
                        return mes;
                    }
                    if (current.Stock13KG < stock.Unsold.Transferred13KG)
                    {
                        mes.Status = "info";
                        mes.Message = $@"Vehicle loaded {current.Stock13KG} 
                            but returns {stock.Unsold.Transferred13KG}13KG, doesnot make sense ";
                        return mes;
                    }if(current.Stock50KG < stock.Unsold.Transferred50KG)
                    {
                        mes.Status = "info";
                        mes.Message = $@"Vehicle loaded {current.Stock50KG} KG
                            but returns {stock.Unsold.Transferred50KG}KG, doesnot make sense ";
                        return mes;
                    }
                    int Variance6KG = current.Stock6KG - stock.Unsold.Transferred6KG;
                    int Variance13KG = current.Stock13KG - stock.Unsold.Transferred13KG;
                    int Variance50KG = current.Stock50KG - stock.Unsold.Transferred50KG;
                    int VarianceBurner = current.StockBurners - stock.Unsold.TransferredStockBurner;
                    int VarianceGrill = current.StockGrills - stock.Unsold.TransferredStockGrill;
                    SaveTripVariance(current.TripID, Variance6KG, Variance13KG, Variance50KG, VarianceBurner, VarianceGrill, addedBy);
                    AddVehicleStockLog(stock.Unsold, "OUT", stock.Unsold.VehicleID, "99");
                    //Update Stock level to 1 and logs to 3
                    UpdateStatus($@"update VehicleStockLevels set status = 1 where VehicleID = '{stock.Unsold.VehicleID}' and Status = 0");
                    UpdateStatus($@"update VehicleStockLogs set status = 3 where VehicleID = '{stock.Unsold.VehicleID}' and Status = 2");
                    // update container stock level and logs 
                    EffectContainerStockLog(stock.Unsold, addedBy, 0);
                    AddStockLevel(stock.Unsold, ShopID);

                    // unsold
                    if (stock.Empties.Count > 1)
                    {
                        for (int i = 0; i < stock.Empties.Count(); i++)
                        {
                            var empt = stock.Empties[i];
                            var unload = new VehicleUnloadingEmptiesModels();
                            unload.TripID = current.TripID;
                            unload.BrandID = empt.BrandID;
                            unload.Size = empt.EmptyCylindersSKU;
                            unload.Quantity = Convert.ToInt32(empt.EmptySkuQunatity);
                            unload.AddedBy = addedBy;
                            var st = new StockTransferredOut();
                            EffectVehicleUnloadingEmpties(unload);
                        }
                    }

                    mes.Status = "success";
                    mes.Message = "Unloaded successfully";
                    tr.Complete();

                }

            }
            catch (Exception ex)
            {
                mes.Status = "warning";
                mes.Message = "Failed! reload and try again later";
            }
            return mes;
        }
        private static void EffectVehicleUnloadingEmpties(VehicleUnloadingEmptiesModels unload)
        {
            using(SqlConnection conn = new SqlConnection(DBConn))
            {
                string querry = $@"insert into [VehicleUnloadingEmpties](Id,TripID,BrandID,
                Size,Quantity,AddedAt,AddedBy) values(@Id,@TripID,@BrandID,
                @Size,@Quantity,@AddedAt,@AddedBy)";
                using (SqlCommand cmd = new SqlCommand(querry, conn))
                {
                    conn.Open();
                    cmd.Parameters.AddWithValue("@Id", Guid.NewGuid());
                    cmd.Parameters.AddWithValue("@TripID", unload.TripID);
                    cmd.Parameters.AddWithValue("@BrandID", unload.BrandID);
                    cmd.Parameters.AddWithValue("@Size", unload.Size);
                    cmd.Parameters.AddWithValue("@Quantity", unload.Quantity);
                    cmd.Parameters.AddWithValue("@AddedAt", DateTime.Now);
                    cmd.Parameters.AddWithValue("@AddedBy", unload.AddedBy);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        private static void SaveTripVariance(string TripID, int Variance6KG, int Variance13KG, int Variance50KG,  int VarianceBurner,
            int VarianceGrill,string AddedBy)
        {
            using (SqlConnection conn = new SqlConnection(DBConn))
            {
                string querry = $@"INSERT INTO TripVariance(ID,TripID,Variance6KG,Variance13KG,Variance50KG,VarianceBurner,VarianceGrill,
                DateAdded,AddedBy)values(
                @ID,@TripID,@Variance6KG,@Variance13KG,@Variance50KG,@VarianceBurner,@VarianceGrill,@DateAdded,@AddedBy)";
                using(SqlCommand cmd = new SqlCommand(querry, conn))
                {
                    conn.Open();
                    cmd.Parameters.AddWithValue("@ID", Guid.NewGuid());
                    cmd.Parameters.AddWithValue("@TripID", TripID);
                    cmd.Parameters.AddWithValue("@Variance6KG", Variance6KG);
                    cmd.Parameters.AddWithValue("@Variance13KG", Variance13KG);
                    cmd.Parameters.AddWithValue("@Variance50KG", Variance50KG);
                    cmd.Parameters.AddWithValue("@VarianceBurner", VarianceBurner);
                    cmd.Parameters.AddWithValue("@VarianceGrill", VarianceGrill);
                    cmd.Parameters.AddWithValue("@DateAdded", DateTime.Now);
                    cmd.Parameters.AddWithValue("@AddedBy", AddedBy);
                    cmd.ExecuteScalar();
                }
            }
        }
        public static string GetVehicleLogsId(string vehicleId)
        {
            string id = String.Empty;
            using(SqlConnection conn = new SqlConnection(DBConn))
            {
                string querry = $@"select top 1 ID from VehicleStockLogs where VehicleID = '{vehicleId}' where status = 1";
                using(SqlCommand cmd = new SqlCommand(querry,conn))
                {
                    conn.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        id = rdr["ID"].ToString();
                    }
                    
                }
            }
            return id;
        }
        public static void EffectContainerStockLog(StockTransferredOut stock,string AddedBy,int IsEmpty)
        {
            using (SqlConnection conn = new SqlConnection(DBConn))
            {
                var ShopID = GetShopID(AddedBy);
                string querry = $@"Insert into ContainerStockLogs(ID,VehicleID,Transferred6KG,
                        Transferred13KG,Transferred50KG,TransferredAt,TransferredBy,Action,
                        TransferredStockBurner,TransferredStockGrill,Type,IsEmpty,ShopID)
                         values(@ID,@VehicleID,@Transferred6KG,@Transferred13KG,@Transferred50KG,
                           @TransferredAt,@TransferredBy,@Action,@TransferredStockBurner,
                            @TransferredStockGrill,@Type,@IsEmpty,@ShopID)";
                using (SqlCommand cmd = new SqlCommand(querry, conn))
                {
                    conn.Open();
                    cmd.Parameters.AddWithValue("@ID", Guid.NewGuid());
                    cmd.Parameters.AddWithValue("@VehicleID", stock.VehicleID);
                    cmd.Parameters.AddWithValue("@Transferred6KG", stock.Transferred6KG);
                    cmd.Parameters.AddWithValue("@Transferred13KG", stock.Transferred13KG);
                    cmd.Parameters.AddWithValue("@Transferred50KG", stock.Transferred50KG);
                    cmd.Parameters.AddWithValue("@TransferredAt", DateTime.Now);
                    cmd.Parameters.AddWithValue("@TransferredBy", AddedBy);
                    cmd.Parameters.AddWithValue("@Action", "IN");
                    cmd.Parameters.AddWithValue("@TransferredStockBurner", stock.TransferredStockBurner);
                    cmd.Parameters.AddWithValue("@TransferredStockGrill", stock.TransferredStockGrill);
                    cmd.Parameters.AddWithValue("@Type", "INTERNAL");
                    cmd.Parameters.AddWithValue("@IsEmpty", IsEmpty);
                    cmd.Parameters.AddWithValue("@ShopID", ShopID);

                    cmd.ExecuteScalar();

                    mes.Status = "success";
                    mes.Message = "Stock transfered succesfully";
                }
            }
        }
        public static void EffectVehicleStockUnsold(VehicleStockUnsold vStock)
        {
            using(SqlConnection conn = new SqlConnection(DBConn))
            {
                string querry = $@"Insert into VehicleStockUnsold(ID,VehicleLogsId,Cylinder6KG,
                   Cylinder13KG,Cylinder50KG,AddedAt,AddedBy )
                    values(@ID,@VehicleLogsId,@Cylinder6KG,@Cylinder13KG,@Cylinder50KG,@AddedAt,@AddedBy )";
                using(SqlCommand cmd = new SqlCommand(querry,conn))
                {
                    conn.Open();
                    cmd.Parameters.AddWithValue("@ID", Guid.NewGuid());
                    cmd.Parameters.AddWithValue("@VehicleLogsId",vStock.VehicleLogsId);
                    cmd.Parameters.AddWithValue("@Cylinder6KG",vStock.Cylinder6KG);
                    cmd.Parameters.AddWithValue("@Cylinder13KG", vStock.Cylinder13KG);
                    cmd.Parameters.AddWithValue("@Cylinder50KG", vStock.Cylinder50KG);
                    cmd.Parameters.AddWithValue("@AddedAt", DateTime.Now);
                    cmd.Parameters.AddWithValue("@AddedBy", vStock.AddedBy);
                    cmd.ExecuteScalar();
                }
            }
        }
        public static MessageResult TransferStockFromDistributor(StockTransferredOut stock,string addedBy)
        {
            var mes = new MessageResult();
            try
            {
                String ShopID = GetShopID(addedBy);
                if (ShopID.Equals(""))
                {
                    mes.Status = "info";
                    mes.Message = "assign user to shop before you proceed";
                    return mes;
                }
                using (SqlConnection conn = new SqlConnection(DBConn))
                {
                    string querry = $@"Insert into ContainerStockLogs(ID,VehicleID,Transferred6KG,
                        Transferred13KG,Transferred50KG,TransferredAt,TransferredBy,Action,TransferredStockBurner,TransferredStockGrill,
                        Type,ShopID)
                         values(@ID,@VehicleID,@Transferred6KG,@Transferred13KG,@Transferred50KG,
                           @TransferredAt,@TransferredBy,@Action,@TransferredStockBurner,@TransferredStockGrill,@Type,
                            @ShopID)";
                    using(SqlCommand cmd = new SqlCommand(querry, conn))
                    {
                        conn.Open();
                        cmd.Parameters.AddWithValue("@ID",Guid.NewGuid());
                        cmd.Parameters.AddWithValue("@ShopID", ShopID);
                        cmd.Parameters.AddWithValue("@VehicleID", stock.VehicleID);
                        cmd.Parameters.AddWithValue("@Transferred6KG", stock.Transferred6KG);
                        cmd.Parameters.AddWithValue("@Transferred13KG", stock.Transferred13KG);
                        cmd.Parameters.AddWithValue("@Transferred50KG", stock.Transferred50KG);
                        cmd.Parameters.AddWithValue("@TransferredStockBurner", stock.TransferredStockBurner);
                        cmd.Parameters.AddWithValue("@TransferredStockGrill", stock.TransferredStockGrill);
                        cmd.Parameters.AddWithValue("@Type", "EXTERNAL");
                        cmd.Parameters.AddWithValue("@TransferredAt", DateTime.Now);
                        cmd.Parameters.AddWithValue("@TransferredBy", addedBy);
                        cmd.Parameters.AddWithValue("@Action", "IN");
                        AddStockLevel(stock, ShopID);
                       
                        cmd.ExecuteScalar();
                        
                        mes.Status = "success";
                        mes.Message = "Stock transfered succesfully";
                    }
                }
            }catch(Exception e)
            {
                mes.Status = "warning";
                mes.Message = "Failed! reload and try again later";
            }
            return mes;
        }
        public static void EffectVehicleStockLevel(StockTransferredOut stock)
        {
            try
            {
                var current = CurrentVehicle(stock.VehicleID);
                if(current.ID == 0)
                {
                    using (SqlConnection conn = new SqlConnection(DBConn))
                    {
                        var querry = $@"Insert into VehicleStockLevels(VehicleID,Stock6KG,Stock13KG,Stock50KG,
                        StockBurners,StockGrills,LastUpdated)
                        values(@VehicleID,@Stock6KG,@Stock13KG,@Stock50KG,@StockBurners,@StockGrills,
                        @LastUpdated)";
                        using (SqlCommand cmd = new SqlCommand(querry, conn))
                        {
                            conn.Open();
                            cmd.Parameters.AddWithValue("@VehicleID", stock.VehicleID);
                            cmd.Parameters.AddWithValue("@Stock6KG", stock.Transferred6KG);
                            cmd.Parameters.AddWithValue("@Stock13KG", stock.Transferred13KG);
                            cmd.Parameters.AddWithValue("@Stock50KG", stock.Transferred50KG);
                            cmd.Parameters.AddWithValue("@StockBurners", "0");
                            cmd.Parameters.AddWithValue("@StockGrills", "0");
                            cmd.Parameters.AddWithValue("@LastUpdated", DateTime.Now);
                            cmd.ExecuteScalar();
                        }
                    }
                }
                else
                {
                    using (SqlConnection conn = new SqlConnection(DBConn))
                    {
                        var querry = $@"Update VehicleStockLevels set VehicleID = @VehicleID,
                            Stock6KG = @Stock6KG,Stock13KG = @Stock13KG,Stock50KG =@Stock50KG,
                        StockBurners = @StockBurners,StockGrills = @StockGrills,LastUpdated = @LastUpdated";
                        using (SqlCommand cmd = new SqlCommand(querry, conn))
                        {
                            // change addition well
                            conn.Open();
                            cmd.Parameters.AddWithValue("@VehicleID", stock.VehicleID);
                            cmd.Parameters.AddWithValue("@Stock6KG", current.Stock6KG  += Convert.ToInt32(stock.Transferred6KG));
                            cmd.Parameters.AddWithValue("@Stock13KG",  current.Stock13KG +=  Convert.ToInt32(stock.Transferred13KG));
                            cmd.Parameters.AddWithValue("@Stock50KG",  current.Stock50KG += Convert.ToInt32(stock.Transferred50KG));
                            cmd.Parameters.AddWithValue("@StockBurners", "0");
                            cmd.Parameters.AddWithValue("@StockGrills", "0");
                            cmd.Parameters.AddWithValue("@LastUpdated", DateTime.Now);
                            cmd.ExecuteScalar();
                        }
                    }
                }
               

            }catch(Exception e)
            {
                var mes = e.Message;
            }
        }
        public static MessageResult ReduceVehicleStockLevel(StockTransferredOut stock)
        {
            var mes = new MessageResult();
            try
            {
                var current = CurrentVehicle(stock.VehicleID);
                if(current.ID == 0)
                {
                    using (SqlConnection conn = new SqlConnection(DBConn))
                    {
                        var querry = $@"Insert into VehicleStockLevels(VehicleID,Stock6KG,Stock13KG,Stock50KG,
                        StockBurners,StockGrills,LastUpdated)
                        values(@VehicleID,@Stock6KG,@Stock13KG,@Stock50KG,@StockBurners,@StockGrills,
                        @LastUpdated)";
                        using (SqlCommand cmd = new SqlCommand(querry, conn))
                        {
                            conn.Open();
                            cmd.Parameters.AddWithValue("@VehicleID", stock.VehicleID);
                            cmd.Parameters.AddWithValue("@Stock6KG", stock.Transferred6KG);
                            cmd.Parameters.AddWithValue("@Stock13KG", stock.Transferred13KG);
                            cmd.Parameters.AddWithValue("@Stock50KG", stock.Transferred50KG);
                            cmd.Parameters.AddWithValue("@StockBurners", stock.TransferredStockBurner);
                            cmd.Parameters.AddWithValue("@StockGrills", stock.TransferredStockGrill);
                            cmd.Parameters.AddWithValue("@LastUpdated", DateTime.Now);
                            cmd.ExecuteScalar();
                        }
                    }
                }
                else
                {
                    using (SqlConnection conn = new SqlConnection(DBConn))
                    {
                        var querry = $@"Update VehicleStockLevels set VehicleID = @VehicleID,
                            Stock6KG = @Stock6KG,Stock13KG = @Stock13KG,Stock50KG =@Stock50KG,
                        StockBurners = @StockBurners,StockGrills = @StockGrills,LastUpdated = @LastUpdated";
                        using (SqlCommand cmd = new SqlCommand(querry, conn))
                        {
                            // change addition well
                            conn.Open();
                            cmd.Parameters.AddWithValue("@VehicleID", stock.VehicleID);
                            if (current.Stock6KG < Convert.ToInt32(stock.Transferred6KG))
                            {
                                mes.Status = "info";
                                mes.Message = $@"Vehicle has {current.Stock6KG } 6KG cylinders , un able to then transfer 
                                in { Convert.ToInt32(stock.Transferred6KG)} 6KG";
                            }
                            cmd.Parameters.AddWithValue("@Stock6KG", current.Stock6KG  -= Convert.ToInt32(stock.Transferred6KG));
                            if (current.Stock13KG < Convert.ToInt32(stock.Transferred13KG))
                            {
                                mes.Status = "info";
                                mes.Message = $@"Vehicle has {current.Stock13KG } 13KG cylinders , un able to then transfer 
                                in { Convert.ToInt32(stock.Transferred13KG)} 13KG";
                            }
                            cmd.Parameters.AddWithValue("@Stock13KG",  current.Stock13KG -=  Convert.ToInt32(stock.Transferred13KG));
                            if (current.StockBurners < Convert.ToInt32(stock.TransferredStockBurner))
                            {
                                mes.Status = "info";
                                mes.Message = $@"Vehicle has {current.StockBurners }  burners , un able to then transfer 
                                in { Convert.ToInt32(stock.TransferredStockBurner)} burners";
                            }
                            if (current.Stock50KG < Convert.ToInt32(stock.Transferred50KG))
                            {
                                mes.Status = "info";
                                mes.Message = $@"Vehicle has {current.Stock50KG } 50KG cylinders , un able to then transfer 
                                in { Convert.ToInt32(stock.Transferred50KG)} 50KG";
                            }
                            if (current.StockGrills < Convert.ToInt32(stock.TransferredStockGrill))
                            {
                                mes.Status = "info";
                                mes.Message = $@"Vehicle has {current.StockGrills } grills, un able to then transfer 
                                in { Convert.ToInt32(stock.TransferredStockGrill)} grills.";
                            }
                            
                            cmd.Parameters.AddWithValue("@Stock50KG",  current.Stock50KG -= Convert.ToInt32(stock.Transferred50KG));
                            cmd.Parameters.AddWithValue("@StockBurners", current.StockBurners -= Convert.ToInt32(stock.TransferredStockBurner));
                            cmd.Parameters.AddWithValue("@StockGrills", current.StockGrills -= Convert.ToInt32(stock.TransferredStockGrill));
                            cmd.Parameters.AddWithValue("@LastUpdated", DateTime.Now);
                            cmd.ExecuteScalar();
                            mes.Status = "success";
                        }
                    }
                }
               

            }catch(Exception e)
            {
                mes.Message = "Failed ! reload and try again later";
                mes.Status = "warning";
            }
            return mes;
        }
        public static VehicleStockLevels CurrentVehicle(string vehicleid)
        {
            var stock = new VehicleStockLevels();
            using(SqlConnection conn = new SqlConnection(DBConn))
            {
                string querry = $"select v.ID as TripID ,l.* from VehicleStockLevels l inner join VehicleStockLogs v on l.VehicleID = v.VehicleID where v.Status = 2 AND l.Status = 0 and v.VehicleID = @VehicleID";
                using(SqlCommand cmd = new SqlCommand(querry,conn))
                {
                    conn.Open();
                    cmd.Parameters.AddWithValue("@VehicleID", vehicleid);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.Read())
                    {
                        stock.ID = Convert.ToInt32( rdr["ID"]);
                        stock.TripID = rdr["TripID"].ToString().Trim();
                        stock.Stock6KG = Convert.ToInt32( rdr["Stock6KG"]);
                        stock.Stock13KG = Convert.ToInt32( rdr["Stock13KG"]);
                        stock.Stock50KG = Convert.ToInt32( rdr["Stock50KG"]);
                        stock.StockBurners = Convert.ToInt32( rdr["StockBurners"]);
                        stock.StockGrills = Convert.ToInt32( rdr["StockGrills"]);
                        stock.StockGrills = Convert.ToInt32( rdr["Status"]);
                    }
                }
            }
            return stock;
        }
        public static VehicleStockLevels CurrentVehicleCheck(string vehicleid)
        {
            var stock = new VehicleStockLevels();
            using (SqlConnection conn = new SqlConnection(DBConn))
            {
                string querry = $@"select top 1 * from VehicleStockLogs
where (VehicleID = '{vehicleid}' and( Status = 0 or Status =1)) order by DateAdded  desc ";
                using (SqlCommand cmd = new SqlCommand(querry, conn))
                {
                    conn.Open();
                    cmd.Parameters.AddWithValue("@VehicleID", vehicleid);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.Read())
                    {
                        //stock.ID = Convert.ToInt32(rdr["ID"]);
                        stock.TripID = rdr["ID"].ToString().Trim();
                        //stock.Stock6KG = Convert.ToInt32(rdr["Stock6KG"]);
                        //stock.Stock13KG = Convert.ToInt32(rdr["Stock13KG"]);
                        //stock.Stock50KG = Convert.ToInt32(rdr["Stock50KG"]);
                        //stock.StockBurners = Convert.ToInt32(rdr["StockBurners"]);
                        //stock.StockGrills = Convert.ToInt32(rdr["StockGrills"]);
                        stock.Status = Convert.ToInt32(rdr["Status"]);
                    }
                    else
                    {
                        stock.TripID = null;
                        stock.Status = 10;
                    }
                }
            }
            return stock;
        }public static VehicleStockLevels VehicleOffloadingCheck(string vehicleid)
        {
            var stock = new VehicleStockLevels();
            using (SqlConnection conn = new SqlConnection(DBConn))
            {
                string querry = $@"select top 1 * from VehicleStockLogs
where (VehicleID = '{vehicleid}' and( Status=2)) order by DateAdded  desc ";
                using (SqlCommand cmd = new SqlCommand(querry, conn))
                {
                    conn.Open();
                    cmd.Parameters.AddWithValue("@VehicleID", vehicleid);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.Read())
                    {
                        //stock.ID = Convert.ToInt32(rdr["ID"]);
                        stock.TripID = rdr["ID"].ToString().Trim();
                        //stock.Stock6KG = Convert.ToInt32(rdr["Stock6KG"]);
                        //stock.Stock13KG = Convert.ToInt32(rdr["Stock13KG"]);
                        //stock.Stock50KG = Convert.ToInt32(rdr["Stock50KG"]);
                        //stock.StockBurners = Convert.ToInt32(rdr["StockBurners"]);
                        //stock.StockGrills = Convert.ToInt32(rdr["StockGrills"]);
                        stock.Status = Convert.ToInt32(rdr["Status"]);
                    }
                    else
                    {
                        stock.TripID = null;
                        stock.Status = 10;
                    }
                }
            }
            return stock;
        }

        public static void UpdateStatus(string SQL)
        {
            using (SqlConnection conn = new SqlConnection(DBConn))
            {
               using (SqlCommand cmd = new SqlCommand(SQL, conn))
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
           
        }

        public static MessageResult ReduceStockLevel(StockTransferredOut stock,string shopID,ContainerStockSummary current)
        {
            
            using(SqlConnection conn = new SqlConnection(DBConn))
            {
                string querry = $@"Update ContainerStockSummary set Stock6KG = @Stock6KG,
                    Stock13KG = @Stock13KG,Stock50KG = @Stock50KG,StockBurner = @TransferredStockBurner,
                    StockGrill = @TransferredStockGrill
                    where ShopID = @ShopID";
                using(SqlCommand cmd = new SqlCommand(querry,conn))
                {
                    conn.Open();
                    cmd.Parameters.AddWithValue("@ShopID", shopID);
                    if(current.Stock6KG < Convert.ToInt32(stock.Transferred6KG))
                    {
                        mes.Status = "info";
                        mes.Message = $@"only {current.Stock6KG} 6KG remaining , makes it hard to transfer
                                out {Convert.ToInt32(stock.Transferred6KG)}";
                        return mes;
                    }
                    cmd.Parameters.AddWithValue("@Stock6KG", Math.Abs(current.Stock6KG - Convert.ToInt32(stock.Transferred6KG)));
                    if (current.Stock13KG < Convert.ToInt32(stock.Transferred13KG))
                    {
                        mes.Status = "info";
                        mes.Message = $@"only {current.Stock13KG} 13KG remaining , makes it hard to transfer
                                out {Convert.ToInt32(stock.Transferred13KG)}";
                        return mes;
                    }
                    cmd.Parameters.AddWithValue("@Stock13KG",Math.Abs( current.Stock13KG - Convert.ToInt32(stock.Transferred13KG)));
                    if (current.Stock50KG < Convert.ToInt32(stock.Transferred50KG))
                    {
                        mes.Status = "info";
                        mes.Message = $@"only {current.Stock50KG} 50KG remaining , makes it hard to transfer
                                out {Convert.ToInt32(stock.Transferred50KG)}";
                        return mes;
                    }
                    cmd.Parameters.AddWithValue("@Stock50KG",Math.Abs( current.Stock50KG - Convert.ToInt32(stock.Transferred50KG)));
                    if (current.StockGrill < Convert.ToInt32(stock.TransferredStockGrill))
                    {
                        mes.Status = "info";
                        mes.Message = $@"only {current.StockGrill} stock grill remaining , makes it hard to transfer
                                out {Convert.ToInt32(stock.TransferredStockGrill)}";
                        return mes;
                    }
                     
                    cmd.Parameters.AddWithValue("@TransferredStockGrill", Math.Abs(current.StockGrill - Convert.ToInt32(stock.TransferredStockGrill)));
                    if (current.StockBurner < Convert.ToInt32(stock.TransferredStockBurner))
                    {
                        mes.Status = "info";
                        mes.Message = $@"only {current.StockBurner} stock grill remaining , makes it hard to transfer
                                out {Convert.ToInt32(stock.TransferredStockBurner)}";
                        return mes;
                    }
                    cmd.Parameters.AddWithValue("@TransferredStockBurner", Math.Abs(current.StockBurner - Convert.ToInt32(stock.TransferredStockBurner)));
                    cmd.ExecuteScalar();
                    mes.Status = "success";
                    mes.Message = "Proceed..";
                }
            }
            return mes;
        }
        public static void AddStockLevel(StockTransferredOut stock,string shopID)
        {
            var current = ContainerStock(shopID);
            string querry = String.Empty;
            if (current.ID == null)
            {
                querry = $@"Insert into ContainerStockSummary(Id,Stock6KG,Stock13KG,Stock50KG,ShopID,StockBurner,StockGrill,LastUpdated) 
                    values(@Id,@Stock6KG,@Stock13KG,@Stock50KG,@ShopID,@StockBurner,@StockGrill,@LastUpdated)";
                using (SqlConnection conn = new SqlConnection(DBConn))
                {


                    using (SqlCommand cmd = new SqlCommand(querry, conn))
                    {
                        conn.Open();
                        cmd.Parameters.AddWithValue("@ShopID", shopID);
                        cmd.Parameters.AddWithValue("@ID", Guid.NewGuid());
                        cmd.Parameters.AddWithValue("@Stock6KG",Convert.ToInt32(stock.Transferred6KG));
                        cmd.Parameters.AddWithValue("@Stock13KG", Convert.ToInt32(stock.Transferred13KG));
                        cmd.Parameters.AddWithValue("@Stock50KG", Convert.ToInt32(stock.Transferred50KG));
                        cmd.Parameters.AddWithValue("@StockBurner",  Convert.ToInt32(stock.TransferredStockBurner));
                        cmd.Parameters.AddWithValue("@StockGrill",  Convert.ToInt32(stock.TransferredStockGrill));
                        cmd.Parameters.AddWithValue("@LastUpdated",  DateTime.Now);
                        cmd.ExecuteScalar();
                    }
                }
            }
            else
            {
                querry = $@"Update ContainerStockSummary set Stock6KG = @Stock6KG,StockBurner = @StockBurner,StockGrill = @StockGrill,
                    Stock13KG = @Stock13KG,Stock50KG = @Stock50KG,LastUpdated = @LastUpdated where ShopID = @ShopID";
                using (SqlConnection conn = new SqlConnection(DBConn))
                {


                    using (SqlCommand cmd = new SqlCommand(querry, conn))
                    {
                        conn.Open();
                        cmd.Parameters.AddWithValue("@ShopID", shopID);
                        cmd.Parameters.AddWithValue("@Stock6KG", Math.Abs(current.Stock6KG + Convert.ToInt32(stock.Transferred6KG)));
                        cmd.Parameters.AddWithValue("@Stock13KG", Math.Abs( current.Stock13KG + Convert.ToInt32(stock.Transferred13KG)));
                        cmd.Parameters.AddWithValue("@Stock50KG", Math.Abs(current.Stock50KG + Convert.ToInt32(stock.Transferred50KG)));
                        cmd.Parameters.AddWithValue("@StockBurner",Math.Abs( current.StockBurner + Convert.ToInt32(stock.TransferredStockBurner)));
                        cmd.Parameters.AddWithValue("@StockGrill", Math.Abs(current.StockGrill + Convert.ToInt32(stock.TransferredStockGrill)));
                        cmd.Parameters.AddWithValue("@LastUpdated", DateTime.Now);
                        cmd.ExecuteScalar();
                    }
                }
            }
            
        }
        public static List<ContainerStockSummary> ContainerStocks(string ShopID)
        {
            var stocks = new List<ContainerStockSummary>();
            string querry = String.Empty;
            using (SqlConnection conn = new SqlConnection(DBConn))
            {
                if (ShopID.Equals(""))
                {
                    querry = $@"Select a.*,b.ShopName from ContainerStockSummary as a
inner join Shops as b on a.ShopID = b.ShopID";
                }
                else
                {
                    querry = $@"Select a.*,b.ShopName from ContainerStockSummary as a
inner join Shops as b on a.ShopID = b.ShopID
where a.ShopID = @ShopID";

                }

                using (SqlCommand cmd = new SqlCommand(querry, conn))
                {
                    conn.Open();
                    cmd.Parameters.AddWithValue("@ShopID", ShopID);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        var stock = new ContainerStockSummary();
                        stock.ID = rdr["ID"].ToString();
                        stock.ShopID = rdr["ShopID"].ToString();
                        stock.ShopName = rdr["ShopName"].ToString();
                        stock.Stock6KG = Convert.ToInt32(rdr["Stock6KG"]);
                        stock.Stock13KG = Convert.ToInt32(rdr["Stock13KG"]);
                        stock.Stock50KG = Convert.ToInt32(rdr["Stock50KG"]);
                        stock.StockBurner = Convert.ToInt32(rdr["StockBurner"]);
                        stock.StockGrill = Convert.ToInt32(rdr["StockGrill"]);
                        stocks.Add(stock);
                    }

                }
            }
            return stocks;

        }
        public static ContainerStockSummary ContainerStock(string ShopID)
        {
            var stock = new ContainerStockSummary();
            using (SqlConnection conn = new SqlConnection(DBConn))
            {
                string querry = $@"Select a.*,b.ShopName from ContainerStockSummary as a
inner join Shops as b on a.ShopID = b.ShopID
where a.ShopID = @ShopID";
                using (SqlCommand cmd = new SqlCommand(querry, conn))
                {
                    conn.Open();
                    cmd.Parameters.AddWithValue("@ShopID", ShopID);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.Read())
                    {
                        stock.ID = rdr["ID"].ToString();
                        stock.ShopID = rdr["ShopID"].ToString();
                        stock.ShopName = rdr["ShopName"].ToString();
                        stock.Stock6KG = Convert.ToInt32( rdr["Stock6KG"]);
                        stock.Stock13KG = Convert.ToInt32(rdr["Stock13KG"]);
                        stock.Stock50KG = Convert.ToInt32(rdr["Stock50KG"]);
                        stock.StockBurner = Convert.ToInt32(rdr["StockBurner"]);
                        stock.StockGrill = Convert.ToInt32(rdr["StockGrill"]);
                    }
                    
                }
            }
            return stock;

        }
        public static void AddVehicleStockLog(StockTransferredOut stock,string Action,string From,string Status)
        {
            
            using (SqlConnection conn = new SqlConnection(DBConn))
            {
                string querry = $@"Insert into VehicleStockLogs(ID,VehicleID,StockFrom,Stock6KG,Stock13KG,Stock50KG,StockBurner,
                StockGrill,Action,Status,DateAdded)
                    values(@ID,@VehicleID,@StockFrom,@Stock6KG,@Stock13KG,@Stock50KG,@StockBurner,@StockGrill,@Action,@Status,@DateAdded)";
                using(SqlCommand cmd = new SqlCommand(querry,conn))
                {
                    conn.Open();
                    cmd.Parameters.AddWithValue("@ID",Guid.NewGuid());
                    cmd.Parameters.AddWithValue("@VehicleID", stock.VehicleID);
                    cmd.Parameters.AddWithValue("@StockFrom", From);
                    cmd.Parameters.AddWithValue("@Stock6KG", stock.Transferred6KG);
                    cmd.Parameters.AddWithValue("@Stock13KG", stock.Transferred13KG);
                    cmd.Parameters.AddWithValue("@Stock50KG", stock.Transferred50KG);
                    cmd.Parameters.AddWithValue("@StockBurner", stock.TransferredStockBurner);
                    cmd.Parameters.AddWithValue("@StockGrill", stock.TransferredStockGrill);
                    cmd.Parameters.AddWithValue("@Status", Status);
                    cmd.Parameters.AddWithValue("@DateAdded", DateTime.Now);
                    cmd.Parameters.AddWithValue("@Action", Action);
                    cmd.ExecuteScalar();
                }
            }
        }
        public static void AddStockTransfer(StockTransferredOut stock,string Action,string From,string Status)
        {
            
            using (SqlConnection conn = new SqlConnection(DBConn))
            {
                string querry = $@"Insert into VehicleStockLogs(ID,VehicleID,StockFrom,Stock6KG,Stock13KG,Stock50KG,StockBurner,
                StockGrill,Action,Status,DateAdded)
                    values(@ID,@VehicleID,@StockFrom,@Stock6KG,@Stock13KG,@Stock50KG,@StockBurner,@StockGrill,@Action,@Status,@DateAdded)";
                using(SqlCommand cmd = new SqlCommand(querry,conn))
                {
                    conn.Open();
                    cmd.Parameters.AddWithValue("@ID",Guid.NewGuid());
                    cmd.Parameters.AddWithValue("@VehicleID", stock.VehicleID);
                    cmd.Parameters.AddWithValue("@StockFrom", From);
                    cmd.Parameters.AddWithValue("@Stock6KG", stock.Transferred6KG);
                    cmd.Parameters.AddWithValue("@Stock13KG", stock.Transferred13KG);
                    cmd.Parameters.AddWithValue("@Stock50KG", stock.Transferred50KG);
                    cmd.Parameters.AddWithValue("@StockBurner", stock.TransferredStockBurner);
                    cmd.Parameters.AddWithValue("@StockGrill", stock.TransferredStockGrill);
                    cmd.Parameters.AddWithValue("@Status", Status);
                    cmd.Parameters.AddWithValue("@DateAdded", DateTime.Now);
                    cmd.Parameters.AddWithValue("@Action", Action);
                    cmd.ExecuteScalar();
                }
            }
        }
        public static string GetShopID(string UserID)
        {
            string ShopID = String.Empty;
            using (SqlConnection conn = new SqlConnection(DBConn))
            {
                string querry = $@"select ShopsID from AssignShop where UserID = @UserID and Status = 1";
                using(SqlCommand cmd = new SqlCommand(querry,conn))
                {
                    conn.Open();
                    cmd.Parameters.AddWithValue("@UserID", UserID);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.Read())
                    {
                        ShopID = rdr["ShopsID"].ToString();
                    }
                }
            }
            return ShopID;
        }
        public static List<ContainerStockSummary> GetContainerStockSummary(string UserID)
        {
            var containers = new List<ContainerStockSummary>();
            try
            {
                var shopID = GetShopID(UserID);
                if (!shopID.Equals(""))
                {
                    containers = ContainerStocks(shopID);
                }
                else
                {
                    containers = ContainerStocks("");
                }

            }catch(Exception e)
            {
               
            }
            return containers;
        }
        public static List<TripVariance> GetVariances()
        {
            var variances = new List<TripVariance>();
            using(SqlConnection conn = new SqlConnection(DBConn))
            {
                var querry = $@"select a.ID,a.TripID,Variance13KG,Variance50KG,Variance6KG,
VarianceBurner,VarianceGrill,c.VehicleRegistration,
concat(e.FirstName,' ',e.MiddleName,' ',e.LastName) as DriverName from TripVariance as a
inner join VehicleStockLogs as b on b.ID = a.TripID
inner join Vehicles as c on c.ID = b.VehicleID
inner join VehicleAssignmentHistory as d on d.VehicleID = b.VehicleID
inner join Drivers as e on e.ID = d.DriverID
where d.Status = 1";
                using(SqlCommand cmd = new SqlCommand(querry, conn))
                {
                    conn.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        var variance = new TripVariance();
                        variance.ID = rdr["ID"].ToString().Trim();
                        variance.TripID = rdr["TripID"].ToString().Trim();
                        variance.VehicleRegistration = rdr["VehicleRegistration"].ToString().Trim();
                        variance.DriverName = rdr["DriverName"].ToString().Trim();
                        variance.Variance13KG = Convert.ToInt32(rdr["Variance13KG"]);
                        variance.Variance50KG = Convert.ToInt32(rdr["Variance50KG"]);
                        variance.Variance6KG = Convert.ToInt32(rdr["Variance6KG"]);
                        variance.VarianceBurner = Convert.ToInt32(rdr["VarianceBurner"]);
                        variance.VarianceGrill = Convert.ToInt32(rdr["VarianceGrill"]);
                        variances.Add(variance);
                    }

                }
            }
            return variances;
        }
        public static MessageResult ReconcileContainerStock(ContainerStockSummary stock,string User)
        {
            var mes = new MessageResult();
            try
            {

                string querry = $@"update ContainerStockSummary 
                        set Stock6KG =@Stock6KG,Stock13KG = @Stock13KG,
                        Stock50KG = @Stock50KG,StockBurner = @StockBurner,
                        StockGrill = @StockGrill,LastUpdated = @LastUpdated
                        where ShopID = @ShopID";
                using (SqlConnection conn = new SqlConnection(DBConn))
                {


                    using (SqlCommand cmd = new SqlCommand(querry, conn))
                    {
                        conn.Open();
                        cmd.Parameters.AddWithValue("@ShopID", stock.ShopID);
                        cmd.Parameters.AddWithValue("@Stock6KG", Convert.ToInt32(stock.Stock6KG));
                        cmd.Parameters.AddWithValue("@Stock13KG", Convert.ToInt32(stock.Stock13KG));
                        cmd.Parameters.AddWithValue("@Stock50KG", Convert.ToInt32(stock.Stock50KG));
                        cmd.Parameters.AddWithValue("@StockBurner", Convert.ToInt32(stock.StockBurner));
                        cmd.Parameters.AddWithValue("@StockGrill", Convert.ToInt32(stock.StockGrill));
                        cmd.Parameters.AddWithValue("@LastUpdated", DateTime.Now);
                        cmd.ExecuteScalar();

                        mes.Status = "success";
                        mes.Message = "Stock reconcilled";
                        var current = StockTransferredOutSQL.ContainerStock(stock.ShopID);
                        //ErrorLog.AuditLogs($@"User:{User} reconciled container stock of {stock.ShopID}
                        //    to Stock6KG: {stock.Stock6KG} from :{current.Stock6KG}, 
                        //    Stock13KG: {stock.Stock13KG} from :{current.Stock13KG},
                        //    Stock50KG: {stock.Stock50KG} from :{current.Stock50KG},
                        //    StockBurner: {stock.StockBurner} from :{current.StockBurner},
                        //    StockGrill: {stock.StockGrill} from:{current.StockGrill}",
                        //    "StockTransferOutController", "ReconcileVehicleVariance");
                    }
                }
            }catch(Exception e)
            {
                mes.Status = "warning";
                mes.Message = "Failed! reload and try again later";
               // ErrorLog.WriteLogs(e.Message, "StockTransferOutController", "ReconcileVehicleVariance");
            }
            return mes;
        }
    }
    
}
