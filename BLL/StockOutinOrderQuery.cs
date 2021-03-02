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
using SqlSugar;

namespace SleckEc2KdInterface.BLL
{
    class StockOutInOrderQuery
    {
        private static ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        static DbContext dbContext = new DbContext(false);
        static DbContext dbContextTB = new DbContext(true);
        List<T_Sale_StockOutInfo> stockInfos;
        List<T_Sale_StockOutItem> stockItems;
        List<T_Sale_ReturnStockInfo> returnInfos;
        List<T_Sale_ReturnStockItem> returnItems;
        private PropertyInfo[] ForeachClassProperties<T>()
        {
            Type t = typeof(T);
            PropertyInfo[] PropertyList = t.GetProperties();
            return PropertyList;

        }


        #region 旺店通接口

        //旺店通采购入库单
        public void GetPurData()
        {

            Logger.Info(DateTime.Now.ToLongTimeString() + "开启采购入库单同步任务");
            string starttime = ConfigurationManager.AppSettings["purstarttime"].ToString();
            string endtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            //var maxRec = dbContext.Db.Queryable<PurStockInfo>().OrderBy("check_time desc").First();
            //if (maxRec != null)
            //{
            //    starttime = DateTime.Parse(maxRec.Check_Time.ToString()).ToString("yyyy-MM-dd HH:mm:ss");
            //}
            StartDateTime maxRec = dbContext.Db.SqlQueryable<StartDateTime>("select starttime from vwe_purmaxtimefor_openapi").First();


            if (maxRec != null)
            {
                starttime = DateTime.Parse(maxRec.starttime.ToString()).ToString("yyyy-MM-dd HH:mm:ss");
            }


            endtime = DateTime.Parse(starttime).AddDays(30) > DateTime.Now ? DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") : DateTime.Parse(starttime).AddDays(1).ToString("yyyy-MM-dd HH:mm:ss");

            Logger.Info(DateTime.Now.ToLongTimeString() + "开始获取旺店通采购入库单数据时间范围:" + starttime + "--" + endtime);
            Dictionary<string, string> dt = new Dictionary<string, string>();
            dt.Add("start_time", starttime);
            dt.Add("end_time", endtime);
         //   dt.Add("warehouse_no", ConfigurationManager.AppSettings["warehouse_no"].ToString());
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

        //旺店通销售出库单
        public void GetSaleOutData()
        {

            Logger.Info(DateTime.Now.ToLongTimeString() + "开启销售出库单同步任务");
            string starttime = ConfigurationManager.AppSettings["saleoutstarttime"].ToString();
            string endtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            StartDateTime maxRec = dbContext.Db.SqlQueryable<StartDateTime>("select starttime from vwe_saleoutmaxtimefor_openapi").First();
            if (maxRec != null)
            {
                 starttime = DateTime.Parse(maxRec.starttime.ToString()).ToString("yyyy-MM-dd HH:mm:ss");
               // starttime = DateTime.Parse("2021-02-25 00:00:00").ToString("yyyy-MM-dd HH:mm:ss");
            }

           // endtime = DateTime.Parse("2021-02-25 23:54:00").ToString("yyyy-MM-dd HH:mm:ss");


              endtime = DateTime.Parse(starttime).AddDays(30) > DateTime.Now ? DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") : DateTime.Parse(starttime).AddDays(30).ToString("yyyy-MM-dd HH:mm:ss");

            Logger.Info(DateTime.Now.ToLongTimeString() + "开始获取旺店通销售出库单数据时间范围:" + starttime + "--" + endtime);
            Dictionary<string, string> dt = new Dictionary<string, string>();
            dt.Add("start_time", starttime);
            dt.Add("end_time", endtime);
        //    dt.Add("shop_no", ConfigurationManager.AppSettings["shop_no"].ToString());
          //  dt.Add("status","110");
            JObject resultobj = JObject.Parse(GetWdtDataBLL.GetDataFromApi(dt, "stockout_order_query_trade.php"));
          //  Logger.Info(resultobj.ToString());
            if (resultobj["code"].ToString() == "0")
            {

                if (resultobj["total_count"].ToString() != "0")
                {
                    stockInfos = new List<T_Sale_StockOutInfo>();
                    stockItems = new List<T_Sale_StockOutItem>();
                    int pagetotal = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(resultobj["total_count"].ToString()) / 40));
                    Logger.Info(DateTime.Now.ToLongTimeString() + "查询成功,获得记录数:" + resultobj["total_count"].ToString()+";页数为"+ pagetotal);
                    for(int i=0;i<pagetotal;i++)
                    {
                        Logger.Info(DateTime.Now.ToLongTimeString() + $"查询第{i+1}页销售出库单数据...");
                        dt = new Dictionary<string, string>();
                        dt.Add("start_time", starttime);
                        dt.Add("end_time", endtime);
                        dt.Add("page_size","40");
                        dt.Add("page_no",i.ToString());
                       // dt.Add("status", ConfigurationManager.AppSettings["saleoutbillstatus"].ToString());
                        resultobj = JObject.Parse(GetWdtDataBLL.GetDataFromApi(dt, "stockout_order_query_trade.php"));
                        //if (i>5 && i<9)
                        //{
                        //    Logger.Info(resultobj.ToString());
                        //}
                        ConvertSale(resultobj);
                        Logger.Info(DateTime.Now.ToLongTimeString() + $"保存转换第{i + 1}页销售出库单数据完成");
                    }
                   
                   
                    if (stockInfos.Count > 0)
                    {
                        try
                        {
                            Logger.Info(DateTime.Now.ToLongTimeString() + "开启事务，保存数据...");
                            dbContext.Db.BeginTran();//开启事务
                            bool flag1 = dbContext.SaleStockOutInfoDb.InsertRange(stockInfos.ToArray());
                            bool flag2 = dbContext.SaleStockOutItemDb.InsertRange(stockItems.ToArray());
                            if (flag1 & flag2)
                            {
                                dbContext.Db.CommitTran();
                                Logger.Info(DateTime.Now.ToLongTimeString() + "保存成功，提交事务，共同步单据数：" + stockInfos.Count.ToString());
                            }
                            else
                            {
                                dbContext.Db.RollbackTran();
                                Logger.Info(DateTime.Now.ToLongTimeString() + "保存数据失败,事务回滚");
                            }
                        }
                        catch(Exception er)
                        {
                            Logger.Info(DateTime.Now.ToLongTimeString() + "运行错误:" + er.Message);
                            dbContext.Db.RollbackTran();
                            Logger.Info(DateTime.Now.ToLongTimeString() + "事务回滚");
                        }
                        finally
                        {
                            Logger.Info(DateTime.Now.ToLongTimeString() + "获取旺店通销售出库单数据结束");
                        }
                    }
                    else
                    {
                        Logger.Info(DateTime.Now.ToLongTimeString() + "没有需要保存的数据!");
                    }
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

        //旺店通销售退货单
        public void GetSaleReturnData()
        {
            Logger.Info(DateTime.Now.ToLongTimeString() + "开启销售退货单同步任务");
            string starttime = ConfigurationManager.AppSettings["salereturnstarttime"].ToString();
            string endtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            //var maxRec = dbContext.Db.Queryable<T_Sale_ReturnStockInfo>().OrderBy("check_time desc").First();
            //if (maxRec != null)
            //{
            //    starttime = DateTime.Parse(maxRec.Check_Time.ToString()).ToString("yyyy-MM-dd HH:mm:ss");
            //}

            StartDateTime maxRec = dbContext.Db.SqlQueryable<StartDateTime>("select starttime  from vwe_salereturnmaxtimefor_openapi").First();
            if (maxRec != null)
            {
                  starttime = DateTime.Parse(maxRec.starttime.ToString()).ToString("yyyy-MM-dd HH:mm:ss");
              //  starttime = DateTime.Parse("2021-02-01").ToString("yyyy-MM-dd HH:mm:ss");
            }

            endtime = DateTime.Parse(starttime).AddDays(30) > DateTime.Now ? DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") : DateTime.Parse(starttime).AddDays(30).ToString("yyyy-MM-dd HH:mm:ss");

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
                    returnInfos = new List<T_Sale_ReturnStockInfo>();
                    returnItems = new List<T_Sale_ReturnStockItem>();
                    int pagetotal = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(resultobj["total_count"].ToString()) / 30));
                    Logger.Info(DateTime.Now.ToLongTimeString() + "查询成功,获得记录数:" + resultobj["total_count"].ToString() + ";页数为" + pagetotal);
                    for (int i=0;i<pagetotal;i++)
                    {
                        Logger.Info(DateTime.Now.ToLongTimeString() + $"查询第{i + 1}页销售退货单数据...");
                        dt = new Dictionary<string, string>();
                        dt.Add("start_time", starttime);
                        dt.Add("end_time", endtime);
                        dt.Add("page_no", i.ToString());
                        dt.Add("status", ConfigurationManager.AppSettings["salereturnbillstatus"].ToString());
                        resultobj = JObject.Parse(GetWdtDataBLL.GetDataFromApi(dt, "stockin_order_query_refund.php"));
                        ConvertSaleReturn(resultobj);
                        Logger.Info(DateTime.Now.ToLongTimeString() + $"转换保存第{i + 1}页销售退货单数据完成");
                    }
                    if (returnInfos.Count > 0)
                    {
                        try
                        {
                            Logger.Info(DateTime.Now.ToLongTimeString() + "开启事务，保存数据...");
                            dbContext.Db.BeginTran();//开启事务
                            bool flag1 = dbContext.SaleReturnStockInfoDb.InsertRange(returnInfos.ToArray());
                            bool flag2 = dbContext.SaleReturnStockItemDb.InsertRange(returnItems.ToArray());
                            if (flag1 & flag2)
                            {
                                dbContext.Db.CommitTran();
                                Logger.Info(DateTime.Now.ToLongTimeString() + "保存成功，提交事务，共同步单据数：" + returnInfos.Count.ToString());
                            }
                            else
                            {
                                dbContext.Db.RollbackTran();
                                Logger.Info(DateTime.Now.ToLongTimeString() + "保存数据失败,事务回滚");
                            }
                        }
                        catch (Exception er)
                        {
                            Logger.Info(DateTime.Now.ToLongTimeString() + "出现错误:" + er.Message);
                            dbContext.Db.RollbackTran();
                            Logger.Info(DateTime.Now.ToLongTimeString() + "事务回滚");
                        }
                        finally
                        {
                            Logger.Info(DateTime.Now.ToLongTimeString() + "销售退货单同步任务完成");
                        }
                    }
                    else
                    {
                        Logger.Info(DateTime.Now.ToLongTimeString() + "没有需要保存的数据!");
                    }
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
              
                int rec = int.Parse(obj["total_count"].ToString());
              //  Logger.Info(DateTime.Now.ToLongTimeString() + obj.ToString());
                if (rec >= 0)
                {
                  //  PropertyInfo[] propertyInfos = ForeachClassProperties<T_Sale_StockOutInfo>();
                    JArray arrRec = JArray.Parse(obj["stockout_list"].ToString());
                    Logger.Info(DateTime.Now.ToLongTimeString() + "该页记录数为:"+ arrRec.Count.ToString());
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

            

            }
            catch (Exception er)
            {

                Logger.Info(DateTime.Now.ToLongTimeString() + "运行错误:" + er.Message);
               

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
              
                int rec = int.Parse(obj["total_count"].ToString());
                if (rec >= 0)
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

               

            }
            catch (Exception er)
            {

                Logger.Info(DateTime.Now.ToLongTimeString() + "运行错误:" + er.Message);
             

            }
          

        }

        #endregion


        #region 奇门销售出库单
        public void GetTBSaleOutData()
        {
            Logger.Info(DateTime.Now.ToLongTimeString() + "开启奇门销售出库单同步任务");
            string starttime = ConfigurationManager.AppSettings["saleoutstarttimetb"].ToString();
            string tbshop_no = ConfigurationManager.AppSettings["tbshop_no"].ToString();
            string endtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            //select* from vwe_saleoutmaxtimefor_openapi
            //select* from vwe_saleoutmaxtimefor_qmapi
            //select* from vwe_salereturnmaxtimefor_openapi
            //select* from vwe_salereturnmaxtimefor_qmapi
            //vwe_purmaxtimefor_openapi
           StartDateTime maxRec = dbContext.Db.SqlQueryable<StartDateTime>("select starttime from vwe_saleoutmaxtimefor_qmapi").First();
           
            
            if (maxRec != null)
            {
                  starttime = DateTime.Parse(maxRec.starttime.ToString()).ToString("yyyy-MM-dd HH:mm:ss");
               // starttime = DateTime.Parse("2021-02-27 10:00:48").ToString("yyyy-MM-dd HH:mm:ss");
            }
          //  endtime = DateTime.Parse("2021-02-27 10:00:49").ToString("yyyy-MM-dd HH:mm:ss");
            endtime = DateTime.Parse(starttime).AddDays(30) > DateTime.Now ? DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") : DateTime.Parse(starttime).AddDays(30).ToString("yyyy-MM-dd HH:mm:ss");
            string[] shops = new string[] { "TM01", "002" };
            Logger.Info(DateTime.Now.ToLongTimeString() + "开始获取奇门销售出库单数据时间范围:" + starttime + "--" + endtime);
            List<wdtstockout> orderlst = dbContextTB.wdtstockoutTBDb.GetList(h => SqlFunc.Between(h.Consign_Time, starttime, endtime) && (h.Status == "95" || h.Status == "110" || h.Status == "105") && shops.Contains(h.Shop_No)) ;

            if (orderlst.Count>0)
            {
                 Logger.Info(DateTime.Now.ToLongTimeString() + "查询成功,获得奇门销售出库记录数:" + orderlst.Count.ToString());
                 ConvertSaleTB(orderlst);
            }
            else
            {
                 Logger.Info(DateTime.Now.ToLongTimeString() + "获取到奇门销售出库单记录数为0，无须转换与保存，任务结束");
            }
        }

        private void ConvertSaleTB(List<wdtstockout> orders)
        {
           
            int insertrec = 0;
            try
            {
                Logger.Info(DateTime.Now.ToLongTimeString() + "开始转换并保存获取到的奇门销售出库单数据...");
                T_Sale_StockOutInfo stockInfo;
                T_Sale_StockOutItem stockItem;
                List<T_Sale_StockOutInfo> stockInfos = new List<T_Sale_StockOutInfo>();
                List<T_Sale_StockOutItem> stockItems = new List<T_Sale_StockOutItem>();
                List<detailslist> details =new List<detailslist>();
                detailslist detail = new detailslist();
                wdtstockout wdtstockoutinfo = new wdtstockout();
                string tboutstr2num = ConfigurationManager.AppSettings["tboutstr"].ToString();
                int rec = orders.Count ;
                if (rec > 0)
                {
                                    
                    for (int r = 0; r < rec; r++)
                    {
                        wdtstockoutinfo = orders[r];
                                            
                        if (dbContext.SaleStockOutInfoDb.GetSingle(h => h.Order_No == wdtstockoutinfo.Order_No) is null)
                        {
                            stockInfo = new T_Sale_StockOutInfo();
                            PropertyInfo[] PropertyList = stockInfo.GetType().GetProperties();
                            foreach (PropertyInfo item in PropertyList)
                            {

                                string value = JObject.Parse(wdtstockoutinfo.ToJsonString())[item.Name].ToString();
                                if (tboutstr2num.Contains(item.Name) && value=="")
                                {
                                    value = "0";
                                }
                                if (item.PropertyType == typeof(System.DateTime))
                                {
                                    item.SetValue(stockInfo, DateTime.Parse(value.Replace("0000-00-00 00:00:00", "1970-01-01 00:00:00")));
                                }
                                else
                                {
                                    item.SetValue(stockInfo, value);
                                }
                            }
                            Logger.Info(DateTime.Now.ToLongTimeString() + $"将销售出库单{wdtstockoutinfo.Order_No}添加到待插入列表");
                        
                            stockInfos.Add(stockInfo);
                            //查询主表对应明细
                            details = dbContextTB.detailslistTBDb.GetList(d => d.Stockout_Id == wdtstockoutinfo.Stockout_Id);
                            for(int d=0;d<details.Count;d++)
                            {
                                stockItem = new T_Sale_StockOutItem();
                                //将对应明细数据赋值给stockItem对象
                                 detail = details[d];
                                PropertyInfo[] ProDetailList = stockItem.GetType().GetProperties();
                                foreach (PropertyInfo pdetail in ProDetailList)
                                {
                                    object  value;
                                    if (pdetail.Name != "Order_No")
                                    {
                                        
                                        if (pdetail.Name != "Expire_Date")
                                        {
                                                value = JObject.Parse(detail.ToJsonString())[pdetail.Name].ToString();
                                        }
                                        else
                                        {
                                                value = "1970-01-01 00:00:00";
                                        }
                                        if (pdetail.PropertyType == typeof(System.DateTime))
                                        {
                                            pdetail.SetValue(stockItem, DateTime.Parse(value.ToString().Replace("0000-00-00 00:00:00", "1970-01-01 00:00:00")));
                                        }
                                        else
                                        {
                                           
                                            pdetail.SetValue(stockItem, value);
                                          
                                        }
                                    }
                                }
                                stockItem.Order_No = stockInfo.Order_No;
                                stockItems.Add(stockItem);

                            }

                            insertrec++;
                        }
                        else
                        {
                            Logger.Info(DateTime.Now.ToLongTimeString() + $"奇门销售出库单{wdtstockoutinfo.Order_No}已存在");
                            continue;
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
                        Logger.Info(DateTime.Now.ToLongTimeString() + "保存成功，提交事务，共同步奇门销售出库单据数：" + insertrec.ToString());
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
                Logger.Info(DateTime.Now.ToLongTimeString() + "奇门销售出库单同步任务完成");
            }

        }
        #endregion


        #region  奇门销售退货单
        public void GetTBSaleReturnData()
        {
            Logger.Info(DateTime.Now.ToLongTimeString() + "开启奇门销售退货单同步任务");
            string starttime = ConfigurationManager.AppSettings["salereturnstarttimetb"].ToString();
            string tbshop_no = ConfigurationManager.AppSettings["tbshop_no"].ToString();
            string endtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            StartDateTime maxRec = dbContext.Db.SqlQueryable<StartDateTime>("select starttime from vwe_salereturnmaxtimefor_qmapi").First();
            if (maxRec != null)
            {
                starttime = maxRec.starttime.ToString("yyyy-MM-dd HH:mm:ss");
                //starttime = DateTime.Parse("2021-02-16 00:00:01").ToString("yyyy-MM-dd HH:mm:ss");
            }

          // endtime = "2021-02-28 23:59:59";
            endtime = DateTime.Parse(starttime).AddDays(30) > DateTime.Now ? DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") : DateTime.Parse(starttime).AddDays(30).ToString("yyyy-MM-dd HH:mm:ss");
            Logger.Info(DateTime.Now.ToLongTimeString() + "开始获取奇门销售退货单数据时间范围:" + starttime + "--" + endtime);
            // && h.Status == ConfigurationManager.AppSettings["saleoutbillstatustb"].ToString()
            List<wdtthOrder> orderlst = dbContextTB.wdtthOrderTBDb.GetList(h => SqlFunc.Between(h.Refund_Time, starttime, endtime) && h.Shop_No == "TM01");

            if (orderlst.Count > 0)
            {
                Logger.Info(DateTime.Now.ToLongTimeString() + "查询成功,获得奇门退货单记录数:" + orderlst.Count.ToString());
                ConvertSaleReturnTB(orderlst);
            }
            else
            {
                Logger.Info(DateTime.Now.ToLongTimeString() + "获取到奇门退货单单记录数为0，无须转换与保存，任务结束");
            }
        }

        private void ConvertSaleReturnTB(List<wdtthOrder> orders)
        {

            int insertrec = 0;
            try
            {
                Logger.Info(DateTime.Now.ToLongTimeString() + "开始转换并保存获取到的奇门销售退货单数据...");
                T_Sale_ReturnStockInfo stockInfo;
                T_Sale_ReturnStockItem stockItem;
                List<T_Sale_ReturnStockInfo> stockInfos = new List<T_Sale_ReturnStockInfo>();
                List<T_Sale_ReturnStockItem> stockItems = new List<T_Sale_ReturnStockItem>();
                List<wdtthOrder> wdtthorders = new List<wdtthOrder>();
                List<wdtthOrderinfo> wdtthorderinfos = new List<wdtthOrderinfo>();
                wdtthOrderinfo wdtthorderinfo = new wdtthOrderinfo();
                wdtthOrder wdtthorder = new wdtthOrder();
                string tboutstr2num = ConfigurationManager.AppSettings["tboutstr"].ToString();
                int rec = orders.Count;
                if (rec > 0)
                {

                    for (int r = 0; r < rec; r++)
                    {
                        wdtthorder = orders[r];

                        if (dbContext.SaleReturnStockInfoDb.GetSingle(h => h.Order_No == wdtthorder.Refund_No) is null)
                        {
                            stockInfo = new T_Sale_ReturnStockInfo();
                          

                            stockInfo.Order_No = wdtthorder.Refund_No; //单号
                            stockInfo.Warehouse_No = wdtthorder.Warehouse_No; //仓库编号
                            stockInfo.Warehouse_Id = wdtthorder.Warehouse_Id == null ? "0" : wdtthorder.Warehouse_Id;//仓库ID
                            stockInfo.Shop_No = wdtthorder.Shop_No;//店铺编号
                            stockInfo.Shop_Name = wdtthorder.Shop_Name; //店铺名称
                            stockInfo.Shop_Remark = "";
                            stockInfo.Src_Order_No= wdtthorder.Src_No; //上层单号
                            stockInfo.Process_Status = wdtthorder.Process_Status; //退换单状态
                            stockInfo.Trade_No = wdtthorder.Sales_Tid; //订单编号
                            stockInfo.Trade_Type = "网店销售"; //系统订单类别
                            stockInfo.Order_Type = wdtthorder.From_Type; ////单据类别
                            stockInfo.Order_Type_Name = "退换单"; //单据类别名称
                            stockInfo.Goods_Count = wdtthorder.Return_Goods_Count; //货品数量
                            stockInfo.Goods_Amount = wdtthorder.Goods_Amount; //货款金额
                            stockInfo.Actual_Refund_Amount = wdtthorder.Actual_Refund_Amount; //实际退款金额
                            stockInfo.Total_Price = "0"; //退回货品成本
                            stockInfo.Discount = "0"; //优惠金额
                            stockInfo.Tax_Amount = wdtthorder.Goods_Amount; //折后金额
                            stockInfo.Post_Fee = wdtthorder.Post_Amount; //邮费
                            stockInfo.Other_Fee = wdtthorder.Other_Amount;//其他费用
                            stockInfo.Adjust_Price= "0"; //纠错金额
                            stockInfo.Right_Price = wdtthorder.Goods_Amount; //调整后总金额
                            stockInfo.Logistics_Type = wdtthorder.Swap_Logistics_Type; //物流类型
                            stockInfo.Logistics_No = ""; //物流编号
                            stockInfo.Logistics_Code = wdtthorder.Logistics_No; //物流单号
                            stockInfo.Logistics_Name = wdtthorder.Logistics_Name; //物流名称
                            stockInfo.Refund_No = wdtthorder.Refund_No; //退换单号
                            stockInfo.Check_Time = wdtthorder.Refund_Time == null ? DateTime.Parse("1970-01-01 00:00:00") : Convert.ToDateTime(wdtthorder.Refund_Time);//审核时间
                            stockInfo.Customer_No = wdtthorder.Customer_No;//客户编码
                            stockInfo.Customer_Name = wdtthorder.Customer_Name; //客户名称
                            stockInfo.Nick_Name = ""; //客户网名
                            stockInfo.Stockin_Reason = wdtthorder.Refund_Reason; //原因
                            stockInfo.Tid = wdtthorder.Tid; //订单原始单号
                            stockInfo.Platform_Id = wdtthorder.Platform_Id; //平台ID
                            stockInfo.Check_Operator_Name = wdtthorder.Creator_Name; //审核人
                            stockInfo.Refund_Operator_Name = wdtthorder.Creator_Name; //退货人
                            stockInfo.Wms_Outer_No = stockInfo.Wms_Outer_No; //外部仓库单号
                            stockInfo.Wms_Result = wdtthorder.Wms_Result; //结果
                            stockInfo.Src_Order_Id = wdtthorder.Refund_Id; //业务单据id
                            stockInfo.Reason_Id = wdtthorder.Reason_Id;//原因id
                            stockInfo.Src_Order_Type = "0";//订单类型 
                            stockInfo.Fa_Status = "0"; //财务同步状态
                            stockInfo.Post_Share_Type = "0"; //邮费分摊类型
                            stockInfo.Adjust_Num = "0"; //调整数量
                            stockInfo.Note_Count = "0"; //便签数量
                            stockInfo.Flag_Id = "0"; //标记id
                            stockInfo.Remark = wdtthorder.Remark; //备注

                            Logger.Info(DateTime.Now.ToLongTimeString() + $"将销售退货单{wdtthorder.Outer_No}添加到待插入列表");

                            stockInfos.Add(stockInfo);
                            //查询主表对应明细
                            wdtthorderinfos = dbContextTB.wdtthOrderinfoTBDb.GetList(d => d.Refund_Id == wdtthorder.Refund_Id);
                            for (int d = 0; d < wdtthorderinfos.Count; d++)
                            {
                                stockItem = new T_Sale_ReturnStockItem();
                                //将对应明细数据赋值给stockItem对象
                                wdtthorderinfo = wdtthorderinfos[d];
                              
                                stockItem.Order_No = stockInfo.Order_No;
                                stockItem.Goods_No = wdtthorderinfo.Goods_No; //货品编号
                                stockItem.Src_Order_Detail_Id = wdtthorderinfo.Order_Id;
                                stockItem.Spec_No = wdtthorderinfo.Spec_No; //商家编码
                                stockItem.Spec_Id = wdtthorderinfo.Spec_Id==null?"0": wdtthorderinfo.Spec_Id; //erp商品主键
                                stockItem.Spec_Code = wdtthorderinfo.Spec_Code; ////商家单号
                                stockItem.Spec_Name = wdtthorderinfo.Spec_Name; //商家名称
                                stockItem.Goods_Count = wdtthorderinfo.Refund_Num; //货品数量
                                stockItem.Discount = wdtthorderinfo.Discount; //优惠金额
                                stockItem.Cost_Price = wdtthorderinfo.Cost_Price;//成本价
                                stockItem.Production_Date = DateTime.Parse("1970-01-01 00:00:00"); //生产日期
                                stockItem.Validity_Days ="0"; //有效期天数
                                stockItem.Expire_Date = DateTime.Parse("1970-01-01 00:00:00"); //有效期
                                stockItem.Src_Price = wdtthorderinfo.Price; //原价
                                stockItem.Tax_Price = wdtthorderinfo.Cost_Price;//税后单价
                                stockItem.Tax_Amount = wdtthorderinfo.Stockin_Amount; //税后金额
                                stockItem.Tax = "0"; //税率
                                stockItem.Total_Cost = wdtthorderinfo.Stockin_Amount; //总成本
                                stockItem.Adjust_Num = "0"; //纠正数量
                                stockItem.Adjust_Price = "0"; //纠正金额
                                stockItem.Right_Num = wdtthorderinfo.Refund_Num; //调整后数量
                                stockItem.Right_Price = (decimal.Parse(wdtthorderinfo.Cost_Price)/ decimal.Parse(wdtthorderinfo.Refund_Num)).ToString(); //调整后单价
                                stockItem.Num = wdtthorderinfo.Refund_Num;//入库数量
                                stockItem.Num2 = wdtthorderinfo.Refund_Num;//辅助数量
                                stockItem.Right_Cost=wdtthorderinfo.Cost_Price;//调整后总金额
                                stockItem.Position_Id = wdtthorderinfo.Position_Id == null ? "0" : wdtthorderinfo.Position_Id; //货位ID
                                stockItem.Position_No = wdtthorderinfo.Position_No; //货位编号
                                stockItem.Tid = wdtthorderinfo.Tid; //原订单原始单号
                                stockItem.Oid = wdtthorderinfo.Oid; //原订单原始子单号
                                stockItem.Src_Order_Type = "3";//订单类型
                                stockItem.Cost_Price2= wdtthorderinfo.Cost_Price;//辅助价格
                                stockItem.Org_Stockin_Detail_Id = "0"; //原始入库单明细
                                stockItem.Batch_Id = "0"; //批次
                                stockItem.Unit_Id = "0"; //单位id
                                stockItem.Base_Unit_Id = "0"; //基本单位id
                                stockItem.Unit_Ratio = "0"; //单位比率
                                stockItem.Expect_Num = "0"; //期望数量
                                stockItem.Share_Post_Cost = "0"; //邮资分摊成本
                                stockItem.Share_Post_Total = "0"; //邮资分摊总成本
                                stockItem.Remark = wdtthorderinfo.Remark; //备注
                                stockItems.Add(stockItem);

                            }

                            insertrec++;
                        }
                        else
                        {
                            Logger.Info(DateTime.Now.ToLongTimeString() + $"奇门销售退货单{wdtthorder.Refund_No}已存在");
                            continue;
                        }
                    }
                }

                if (stockInfos.Count > 0)
                {

                    Logger.Info(DateTime.Now.ToLongTimeString() + "开启事务，保存数据...");
                    dbContext.Db.BeginTran();//开启事务
                    bool flag1 = dbContext.SaleReturnStockInfoDb.InsertRange(stockInfos.ToArray());
                    bool flag2 = dbContext.SaleReturnStockItemDb.InsertRange(stockItems.ToArray());
                    if (flag1 & flag2)
                    {
                        dbContext.Db.CommitTran();
                        Logger.Info(DateTime.Now.ToLongTimeString() + "保存成功，提交事务，共同步奇门销售退货单据数：" + insertrec.ToString());
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
                Logger.Info(DateTime.Now.ToLongTimeString() + "奇门销售退货单同步任务完成");
            }

        }

        #endregion


    }
}
