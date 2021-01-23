using Newtonsoft.Json.Linq;
using SleckEc2KdInterface.DAL;
using SleckEc2KdInterface.Entity;
using SleckEc2KdInterface.OPENAPI;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Quartz;
using log4net;

namespace SleckEc2KdInterface.BLL
{
    class StockinOrderQueryPurchase
    {
        private static ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        static DbContext dbContext = new DbContext();
        private PropertyInfo[] ForeachClassProperties<T>()
        {
            Type t = typeof(T);
            PropertyInfo[] PropertyList = t.GetProperties();
            return PropertyList;

        }
        public void GetPurData()
        {

            Logger.Info(DateTime.Now.ToLongTimeString() + "开启采购入库单同步任务");
            string starttime = ConfigurationManager.AppSettings["purstarttime"].ToString();
            string endtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            var maxRec = dbContext.Db.Queryable<PurStockInfo>().OrderBy("check_time desc").First();
            if (maxRec != null)
            {
                starttime = DateTime.Parse(maxRec.Check_Time.ToString()).ToString("yyyy-MM-dd HH:mm:ss");
            }

            endtime = DateTime.Parse(starttime).AddDays(1) > DateTime.Now ? DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") : DateTime.Parse(starttime).AddDays(1).ToString("yyyy-MM-dd HH:mm:ss");

            Logger.Info(DateTime.Now.ToLongTimeString() + "开始获取旺店通采购入库单数据时间范围:" + starttime + "--" + endtime);
            Dictionary<string, string> dt = new Dictionary<string, string>();
            dt.Add("start_time", starttime);
            dt.Add("end_time", endtime);
            dt.Add("warehouse_no", ConfigurationManager.AppSettings["warehouse_no"].ToString());
            dt.Add("status", ConfigurationManager.AppSettings["purbillstatus"].ToString());
            JObject resultobj = JObject.Parse(GetWdtDataBLL.GetDataFromApi(dt, "stockin_order_query_purchase.php"));

            if (resultobj["code"].ToString() == "0")
            {

                if (resultobj["total_count"].ToString() != "0")
                {
                    Logger.Info(DateTime.Now.ToLongTimeString() + "查询成功,获得记录数:" + resultobj["total_count"].ToString());
                    ConvertPur(resultobj);
                }
                else
                {
                    Logger.Info(DateTime.Now.ToLongTimeString() + "获取到旺店通采购单记录数为0，无须转换与保存，任务结束");
                }
            }
            else
            {
                Logger.Info(DateTime.Now.ToLongTimeString() + "错误码:" + resultobj["code"].ToString() + ";错误原因:" + resultobj["message"].ToString() + "  任务结束");

            }
        }

        public void GetSaleOutData()
        {

            Logger.Info(DateTime.Now.ToLongTimeString() + "开启销售出库单同步任务");
            string starttime = ConfigurationManager.AppSettings["saleoutstarttime"].ToString();
            string endtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            var maxRec = dbContext.Db.Queryable<T_Sale_StockOutInfo>().OrderBy("consign_time desc").First();
            if (maxRec != null)
            {
                starttime = DateTime.Parse(maxRec.Consign_Time.ToString()).ToString("yyyy-MM-dd HH:mm:ss");
            }

            endtime = DateTime.Parse(starttime).AddHours(1) > DateTime.Now ? DateTime.Parse(starttime).AddHours(1).ToString("yyyy-MM-dd HH:mm:ss") : DateTime.Parse(starttime).AddHours(1).ToString("yyyy-MM-dd HH:mm:ss");

            Logger.Info(DateTime.Now.ToLongTimeString() + "开始获取旺店通销售出库单数据时间范围:" + starttime + "--" + endtime);
            Dictionary<string, string> dt = new Dictionary<string, string>();
            dt.Add("start_time", starttime);
            dt.Add("end_time", endtime);
            dt.Add("shop_no", ConfigurationManager.AppSettings["shop_no"].ToString());
            dt.Add("status", ConfigurationManager.AppSettings["saleoutbillstatus"].ToString());
            JObject resultobj = JObject.Parse(GetWdtDataBLL.GetDataFromApi(dt, "stockout_order_query_trade.php"));

            if (resultobj["code"].ToString() == "0")
            {

                if (resultobj["total_count"].ToString() != "0")
                {
                    Logger.Info(DateTime.Now.ToLongTimeString() + "查询成功,获得记录数:" + resultobj["total_count"].ToString());
                    ConvertSale(resultobj);
                }
                else
                {
                    Logger.Info(DateTime.Now.ToLongTimeString() + "获取到旺店通销售出库单记录数为0，无须转换与保存，任务结束");
                }
            }
            else
            {
                Logger.Info(DateTime.Now.ToLongTimeString() + "错误码:" + resultobj["code"].ToString() + ";错误原因:" + resultobj["message"].ToString() + "  任务结束");

            }
        }

        public void GetSaleReturnData()
        {
            Logger.Info(DateTime.Now.ToLongTimeString() + "开启销售退货单同步任务");
            string starttime = ConfigurationManager.AppSettings["salereturnstarttime"].ToString();
            string endtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            var maxRec = dbContext.Db.Queryable<T_Sale_ReturnStockInfo>().OrderBy("check_time desc").First();
            if (maxRec != null)
            {
                starttime = DateTime.Parse(maxRec.Check_Time.ToString()).ToString("yyyy-MM-dd HH:mm:ss");
            }

            endtime = DateTime.Parse(starttime).AddDays(1) > DateTime.Now ? DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") : DateTime.Parse(starttime).AddDays(1).ToString("yyyy-MM-dd HH:mm:ss");

            Logger.Info(DateTime.Now.ToLongTimeString() + "开始获取旺店通销售退货单数据时间范围:" + starttime + "--" + endtime);
            Dictionary<string, string> dt = new Dictionary<string, string>();
            dt.Add("start_time", starttime);
            dt.Add("end_time", endtime);

            dt.Add("status", ConfigurationManager.AppSettings["salereturnbillstatus"].ToString());
            JObject resultobj = JObject.Parse(GetWdtDataBLL.GetDataFromApi(dt, "stockin_order_query_refund.php"));

            if (resultobj["code"].ToString() == "0")
            {

                if (resultobj["total_count"].ToString() != "0")
                {
                    Logger.Info(DateTime.Now.ToLongTimeString() + "查询成功,获得记录数:" + resultobj["total_count"].ToString());
                    ConvertSaleReturn(resultobj);
                }
                else
                {
                    Logger.Info(DateTime.Now.ToLongTimeString() + "获取到旺店通销售退货单记录数为0，无须转换与保存，任务结束");
                }
            }
            else
            {
                Logger.Info(DateTime.Now.ToLongTimeString() + "错误码:" + resultobj["code"].ToString() + ";错误原因:" + resultobj["message"].ToString() + "  任务结束");

            }
        }

        private void ConvertPur(JObject obj)
        {
            int insertrec = 0;
            try
            {
                Logger.Info(DateTime.Now.ToLongTimeString() + "开始转换并保存获取到的采购入库单数据...");

                PurStockInfo pStockInfo;
                PurStockItem purStockItem;
                List<PurStockInfo> pStockInfos = new List<PurStockInfo>();
                List<PurStockItem> purStockItems = new List<PurStockItem>();
                int rec = int.Parse(obj["total_count"].ToString());
                if (rec > 0)
                {
                    PropertyInfo[] propertyInfos = ForeachClassProperties<PurStockInfo>();
                    JArray arrRec = JArray.Parse(obj["stockin_list"].ToString());
                    for (int r = 0; r < arrRec.Count; r++)
                    {
                        JObject tempo = arrRec[r] as JObject;
                        pStockInfo = new PurStockInfo();

                        PropertyInfo[] PropertyList = pStockInfo.GetType().GetProperties();

                        foreach (PropertyInfo item in PropertyList)
                        {
                            string value = tempo[item.Name.ToLower()].ToString();
                            item.SetValue(pStockInfo, value);
                        }
                        if (dbContext.PurStockInfoDb.GetSingle(h => h.order_no == pStockInfo.order_no) is null)
                        {
                            Logger.Info(DateTime.Now.ToLongTimeString() + $"将采购单{pStockInfo.order_no}添加到待插入列表");
                            pStockInfos.Add(pStockInfo);
                            insertrec++;
                        }
                        else
                        {
                            Logger.Info(DateTime.Now.ToLongTimeString() + $"采购单{pStockInfo.order_no}已存在");
                            continue;
                        }


                        //明细数据
                        JArray detailRec = JArray.Parse(tempo["details_list"].ToString());
                        for (int j = 0; j < detailRec.Count; j++)
                        {
                            tempo = detailRec[j] as JObject;
                            purStockItem = new PurStockItem();

                            PropertyInfo[] ProDetailList = purStockItem.GetType().GetProperties();
                            foreach (PropertyInfo detail in ProDetailList)
                            {
                                if (detail.Name != "Order_No")
                                {
                                    if (detail.PropertyType == typeof(System.DateTime))
                                    {
                                        detail.SetValue(purStockItem, DateTime.Parse(tempo[detail.Name.ToLower()].ToString().Replace("0000-00-00 00:00:00", "1970-01-01 00:00:00")));
                                    }
                                    else
                                    {
                                        detail.SetValue(purStockItem, tempo[detail.Name.ToLower()].ToString());
                                    }
                                }

                            }
                            purStockItem.Order_No = pStockInfo.order_no;
                            purStockItems.Add(purStockItem);
                        }
                    }
                }
                if (pStockInfos.Count > 0)
                {
                    Logger.Info(DateTime.Now.ToLongTimeString() + "开启事务，保存数据...");
                    dbContext.Db.BeginTran();//开启事务
                    bool flag1 = dbContext.PurStockInfoDb.InsertRange(pStockInfos.ToArray());
                    bool flag2 = dbContext.PurStocItemDb.InsertRange(purStockItems.ToArray());
                    if (flag1 & flag2)
                    {
                        dbContext.Db.CommitTran();
                        Logger.Info(DateTime.Now.ToLongTimeString() + "保存成功，提交事务，共同步单据数：" + insertrec.ToString());
                    }
                    else
                    {
                        dbContext.Db.RollbackTran();
                        Logger.Info(DateTime.Now.ToLongTimeString() + "保存数据失败,事务回滚");
                    }
                }
                else
                {
                    Logger.Info(DateTime.Now.ToLongTimeString() + "没有需要保存的数据!");
                }

            }
            catch (Exception er)
            {

                Logger.Info(DateTime.Now.ToLongTimeString() + "运行错误:" + er.Message);
                dbContext.Db.RollbackTran();
                Logger.Info(DateTime.Now.ToLongTimeString() + "事务回滚");

            }
            finally
            {
                Logger.Info(DateTime.Now.ToLongTimeString() + "采购入库单同步任务完成");
            }

        }


        private void ConvertSale(JObject obj)
        {
            int insertrec = 0;
            try
            {
                Logger.Info(DateTime.Now.ToLongTimeString() + "开始转换并保存获取到的销售出库单数据...");
                T_Sale_StockOutInfo stockInfo;
                T_Sale_StockOutItem stockItem;
                List<T_Sale_StockOutInfo> stockInfos = new List<T_Sale_StockOutInfo>();
                List<T_Sale_StockOutItem> stockItems = new List<T_Sale_StockOutItem>();
                int rec = int.Parse(obj["total_count"].ToString());
                if (rec > 0)
                {
                    PropertyInfo[] propertyInfos = ForeachClassProperties<T_Sale_StockOutInfo>();
                    JArray arrRec = JArray.Parse(obj["stockout_list"].ToString());
                    for (int r = 0; r < arrRec.Count; r++)
                    {
                        JObject tempo = arrRec[r] as JObject;
                        stockInfo = new T_Sale_StockOutInfo();

                        PropertyInfo[] PropertyList = stockInfo.GetType().GetProperties();

                        foreach (PropertyInfo item in PropertyList)
                        {

                            string value = tempo[item.Name.ToLower()].ToString();
                            if (item.PropertyType == typeof(System.DateTime))
                            {
                                item.SetValue(stockInfo, DateTime.Parse(value.Replace("0000-00-00 00:00:00", "1970-01-01 00:00:00")));
                            }
                            else
                            {
                                item.SetValue(stockInfo, value);
                            }
                        }
                        if (dbContext.SaleStockOutInfoDb.GetSingle(h => h.Order_No == stockInfo.Order_No) is null)
                        {
                            Logger.Info(DateTime.Now.ToLongTimeString() + $"将销售出库单{stockInfo.Order_No}添加到待插入列表");
                            stockInfos.Add(stockInfo);
                            insertrec++;
                        }
                        else
                        {
                            Logger.Info(DateTime.Now.ToLongTimeString() + $"销售出库单{stockInfo.Order_No}已存在");
                            continue;
                        }

                        stockInfos.Add(stockInfo);
                        //明细数据
                        JArray detailRec = JArray.Parse(tempo["details_list"].ToString());
                        for (int j = 0; j < detailRec.Count; j++)
                        {
                            tempo = detailRec[j] as JObject;
                            stockItem = new T_Sale_StockOutItem();

                            PropertyInfo[] ProDetailList = stockItem.GetType().GetProperties();
                            foreach (PropertyInfo detail in ProDetailList)
                            {
                                if (detail.Name != "Order_No")
                                {
                                    if (detail.PropertyType == typeof(System.DateTime))
                                    {
                                        detail.SetValue(stockItem, DateTime.Parse(tempo[detail.Name.ToLower()].ToString().Replace("0000-00-00 00:00:00", "1970-01-01 00:00:00")));
                                    }
                                    else
                                    {
                                        detail.SetValue(stockItem, tempo[detail.Name.ToLower()].ToString());
                                    }
                                }

                            }
                            stockItem.Order_No = stockInfo.Order_No;
                            stockItems.Add(stockItem);
                        }
                    }
                }

                if (stockInfos.Count > 0)
                {
                    Logger.Info(DateTime.Now.ToLongTimeString() + "开启事务，保存数据...");
                    dbContext.Db.BeginTran();//开启事务
                    bool flag1 = dbContext.SaleStockOutInfoDb.InsertRange(stockInfos.ToArray());
                    bool flag2 = dbContext.SaleStockOutItemDb.InsertRange(stockItems.ToArray());
                    if (flag1 & flag2)
                    {
                        dbContext.Db.CommitTran();
                        Logger.Info(DateTime.Now.ToLongTimeString() + "保存成功，提交事务，共同步单据数：" + insertrec.ToString());
                    }
                    else
                    {
                        dbContext.Db.RollbackTran();
                        Logger.Info(DateTime.Now.ToLongTimeString() + "保存数据失败,事务回滚");
                    }
                }
                else
                {
                    Logger.Info(DateTime.Now.ToLongTimeString() + "没有需要保存的数据!");
                }

            }
            catch (Exception er)
            {

                Logger.Info(DateTime.Now.ToLongTimeString() + "运行错误:" + er.Message);
                dbContext.Db.RollbackTran();
                Logger.Info(DateTime.Now.ToLongTimeString() + "事务回滚");

            }
            finally
            {
                Logger.Info(DateTime.Now.ToLongTimeString() + "销售出库单同步任务完成");
            }

        }

        private void ConvertSaleReturn(JObject obj)
        {
            int insertrec = 0;
            try
            {
                Logger.Info(DateTime.Now.ToLongTimeString() + "开始转换并保存获取到的销售退货单数据...");

                T_Sale_ReturnStockInfo returnInfo;
                T_Sale_ReturnStockItem returnItem;
                List<T_Sale_ReturnStockInfo> returnInfos = new List<T_Sale_ReturnStockInfo>();
                List<T_Sale_ReturnStockItem> returnItems = new List<T_Sale_ReturnStockItem>();
                int rec = int.Parse(obj["total_count"].ToString());
                if (rec > 0)
                {
                    PropertyInfo[] propertyInfos = ForeachClassProperties<T_Sale_StockOutInfo>();
                    JArray arrRec = JArray.Parse(obj["stockin_list"].ToString());
                    for (int r = 0; r < arrRec.Count; r++)
                    {
                        JObject tempo = arrRec[r] as JObject;
                        returnInfo = new T_Sale_ReturnStockInfo();

                        PropertyInfo[] PropertyList = returnInfo.GetType().GetProperties();

                        foreach (PropertyInfo item in PropertyList)
                        {

                            string value = tempo[item.Name.ToLower()].ToString();
                            if (item.PropertyType == typeof(System.DateTime))
                            {
                                item.SetValue(returnInfo, DateTime.Parse(value.Replace("0000-00-00 00:00:00", "1970-01-01 00:00:00")));
                            }
                            else
                            {
                                item.SetValue(returnInfo, value);
                            }
                        }
                        if (dbContext.SaleReturnStockInfoDb.GetSingle(h => h.Order_No == returnInfo.Order_No) is null)
                        {
                            Logger.Info(DateTime.Now.ToLongTimeString() + $"将销售退货单{returnInfo.Order_No}添加到待插入列表");
                            returnInfos.Add(returnInfo);
                            insertrec++;
                        }
                        else
                        {
                            Logger.Info(DateTime.Now.ToLongTimeString() + $"销售退货单{returnInfo.Order_No}已存在");
                            continue;
                        }

                        //明细数据
                        JArray detailRec = JArray.Parse(tempo["details_list"].ToString());
                        for (int j = 0; j < detailRec.Count; j++)
                        {
                            tempo = detailRec[j] as JObject;
                            returnItem = new T_Sale_ReturnStockItem();

                            PropertyInfo[] ProDetailList = returnItem.GetType().GetProperties();
                            foreach (PropertyInfo detail in ProDetailList)
                            {
                                if (detail.Name != "Order_No")
                                {
                                    if (detail.PropertyType == typeof(System.DateTime))
                                    {
                                        detail.SetValue(returnItem, DateTime.Parse(tempo[detail.Name.ToLower()].ToString().Replace("0000-00-00 00:00:00", "1970-01-01 00:00:00")));
                                    }
                                    else
                                    {
                                        detail.SetValue(returnItem, tempo[detail.Name.ToLower()].ToString());
                                    }
                                }

                            }
                            returnItem.Order_No = returnInfo.Order_No;
                            returnItems.Add(returnItem);
                        }
                    }
                }

                if (returnInfos.Count > 0)
                {
                    Logger.Info(DateTime.Now.ToLongTimeString() + "开启事务，保存数据...");
                    dbContext.Db.BeginTran();//开启事务
                    bool flag1 = dbContext.SaleReturnStockInfoDb.InsertRange(returnInfos.ToArray());
                    bool flag2 = dbContext.SaleReturnStockItemDb.InsertRange(returnItems.ToArray());
                    if (flag1 & flag2)
                    {
                        dbContext.Db.CommitTran();
                        Logger.Info(DateTime.Now.ToLongTimeString() + "保存成功，提交事务，共同步单据数：" + insertrec.ToString());
                    }
                    else
                    {
                        dbContext.Db.RollbackTran();
                        Logger.Info(DateTime.Now.ToLongTimeString() + "保存数据失败,事务回滚");
                    }
                }
                else
                {
                    Logger.Info(DateTime.Now.ToLongTimeString() + "没有需要保存的数据!");
                }

            }
            catch (Exception er)
            {

                Logger.Info(DateTime.Now.ToLongTimeString() + "运行错误:" + er.Message);
                dbContext.Db.RollbackTran();
                Logger.Info(DateTime.Now.ToLongTimeString() + "事务回滚");

            }
            finally
            {
                Logger.Info(DateTime.Now.ToLongTimeString() + "销售退货单同步任务完成");
            }

        }

    }
}
