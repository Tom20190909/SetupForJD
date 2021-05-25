using Newtonsoft.Json.Linq;
using SleckEc2KdInterface.BLL;
using SleckEc2KdInterface.DAL;
using SleckEc2KdInterface.Entity;
using SleckEc2KdInterface.OPENAPI;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SleckEc2KdInterface
{
    public partial class Form2 : Form
    {
        static Thread FirstThread;
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
        public Form2()
        {
            InitializeComponent();
        }
        public string Csdate()
        {
            string sd = "";
            sd = std.Value.Year.ToString() + "-";
            if ((std.Value.Month.ToString()).Length == 1)
            {
                sd = sd + "0" + std.Value.Month.ToString() + "-";
            }
            else
            {
                sd = sd + std.Value.Month.ToString() + "-";
            }
            if ((std.Value.Day.ToString()).Length == 1)
            {
                sd = sd + "0" + std.Value.Day.ToString();
            }
            else
            {
                sd = sd + std.Value.Day.ToString();
            }
            sd = sd + " 00:00:00";
            return sd;

        }
        public string Cedate()
        {
            string ed = "";
            ed = etd.Value.Year.ToString() + "-";
            if ((etd.Value.Month.ToString()).Length == 1)
            {
                ed = ed + "0" + etd.Value.Month.ToString() + "-";
            }
            else
            {
                ed = ed + etd.Value.Month.ToString() + "-";
            }
            if ((etd.Value.Day.ToString()).Length == 1)
            {
                ed = ed + "0" + etd.Value.Day.ToString();
            }
            else
            {
                ed = ed + etd.Value.Day.ToString();
            }
            ed = ed + " 23:59:59";
            return ed;

        }
        private void Form2_Load(object sender, EventArgs e)
        {
            std.Value = DateTime.Today;
            etd.Value = DateTime.Today;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            switch (comboBox5.Text)
            {
                case "旺店通销售出库单":
                    FirstThread = new Thread(new ThreadStart(GetSaleOutData));

                    FirstThread.Start();
                  
                    break;
                case "旺店通销售退货单":
                    FirstThread = new Thread(new ThreadStart(GetSaleReturnData));

                    FirstThread.Start();
                   
                    break;
                case "奇门销售出库单":
                    FirstThread = new Thread(new ThreadStart(GetTBSaleOutData));

                    FirstThread.Start();
                   
                    break;
                case "奇门销售退货单":
                    FirstThread = new Thread(new ThreadStart(GetTBSaleReturnData));

                    FirstThread.Start();
                   
                    break;
               
            }
        }

        //旺店通采购入库单
        public void GetPurData()
        {

            this.Invoke(new Action(() =>
            {
                richTextBox1.AppendText(DateTime.Now.ToLongTimeString() + "开启采购入库单同步任务"+"\r\n");

            }));
           
           
         
              string  starttime =Csdate();
              string  endtime =Cedate();
            this.Invoke(new Action(() =>
            {
                richTextBox1.AppendText(DateTime.Now.ToLongTimeString() + "开始获取旺店通采购入库单数据时间范围:" + starttime + "--" + endtime + "\r\n");

            }));

           
          
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
                    this.Invoke(new Action(() =>
                    {
                        richTextBox1.AppendText(DateTime.Now.ToLongTimeString() + "查询成功,获得记录数:" + resultobj["total_count"].ToString() + "\r\n");

                    }));

                   
                  
                    ConvertPur(resultobj);
                }
                else
                {
                    this.Invoke(new Action(() =>
                    {
                        richTextBox1.AppendText(DateTime.Now.ToLongTimeString() + "获取到旺店通采购单记录数为0，无须转换与保存，任务结束" + "\r\n");

                    }));

                   
                  
                }
            }
            else
            {
                this.Invoke(new Action(() =>
                {
                    richTextBox1.AppendText(DateTime.Now.ToLongTimeString() + "错误码:" + resultobj["code"].ToString() + ";错误原因:" + resultobj["message"].ToString() + "\r\n");

                }));
               
               

            }
        }

        //旺店通销售出库单
        public void GetSaleOutData()
        {
            this.Invoke(new Action(() =>
            {
                richTextBox1.AppendText(DateTime.Now.ToLongTimeString() + "开启销售出库单同步任务" + "\r\n");

            }));
          
           
            string starttime =Csdate();
            string endtime = Cedate();

            this.Invoke(new Action(() =>
            {
                richTextBox1.AppendText(DateTime.Now.ToLongTimeString() + "开始获取旺店通销售出库单数据时间范围:" + starttime + "--" + endtime + "\r\n");

            }));

            Dictionary<string, string> dt = new Dictionary<string, string>();
            dt.Add("start_time", starttime);
            dt.Add("end_time", endtime);
            //  dt.Add("shop_no", "KS01");
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
                    this.Invoke(new Action(() =>
                    {
                        richTextBox1.AppendText(DateTime.Now.ToLongTimeString() + "查询成功,获得记录数:" + resultobj["total_count"].ToString() + ";页数为" + pagetotal + "\r\n");

                    }));
                   
                    for (int i = 0; i < pagetotal; i++)
                    {
                        this.Invoke(new Action(() =>
                        {
                            richTextBox1.AppendText(DateTime.Now.ToLongTimeString() + $"查询第{i + 1}页销售出库单数据..." + "\r\n");

                        }));
                     
                        dt = new Dictionary<string, string>();
                        dt.Add("start_time", starttime);
                        dt.Add("end_time", endtime);
                        //  dt.Add("shop_no", "KS01");
                        dt.Add("page_size", "40");
                        dt.Add("page_no", i.ToString());
                        // dt.Add("status", ConfigurationManager.AppSettings["saleoutbillstatus"].ToString());
                        resultobj = JObject.Parse(GetWdtDataBLL.GetDataFromApi(dt, "stockout_order_query_trade.php"));
                        //if (i>5 && i<9)
                        //{
                        //    Logger.Info(resultobj.ToString());
                        //}
                        ConvertSale(resultobj);
                        this.Invoke(new Action(() =>
                        {
                            richTextBox1.AppendText(DateTime.Now.ToLongTimeString() + $"保存转换第{i + 1}/{pagetotal}页销售出库单数据完成" + "\r\n");

                        }));
                       
                    }


                    if (stockInfos.Count > 0)
                    {
                        try
                        {
                            this.Invoke(new Action(() =>
                            {
                                richTextBox1.AppendText(DateTime.Now.ToLongTimeString() + "开启事务，保存数据..." + "\r\n");

                            }));
                          
                            dbContext.Db.BeginTran();//开启事务
                            bool flag1 = dbContext.SaleStockOutInfoDb.InsertRange(stockInfos.ToArray());
                            bool flag2 = dbContext.SaleStockOutItemDb.InsertRange(stockItems.ToArray());
                            if (flag1 & flag2)
                            {
                                dbContext.Db.CommitTran();
                                this.Invoke(new Action(() =>
                                {
                                    richTextBox1.AppendText(DateTime.Now.ToLongTimeString() + "保存成功，提交事务，共同步单据数：" + stockInfos.Count.ToString() + "\r\n");

                                }));
                               
                            }
                            else
                            {
                                dbContext.Db.RollbackTran();
                                this.Invoke(new Action(() =>
                                {
                                    richTextBox1.AppendText(DateTime.Now.ToLongTimeString() + "保存数据失败,事务回滚" + "\r\n");

                                }));
                             
                            }
                        }
                        catch (Exception er)
                        {
                            this.Invoke(new Action(() =>
                            {
                                richTextBox1.AppendText(DateTime.Now.ToLongTimeString() + "运行错误:" + er.Message + "\r\n");

                            }));
                         
                            dbContext.Db.RollbackTran();
                            this.Invoke(new Action(() =>
                            {
                                richTextBox1.AppendText(DateTime.Now.ToLongTimeString() + "事务回滚" + "\r\n");

                            }));
                           
                        }
                        finally
                        {
                            this.Invoke(new Action(() =>
                            {
                                richTextBox1.AppendText(DateTime.Now.ToLongTimeString() + "获取旺店通销售出库单数据结束" + "\r\n");

                            }));
                           
                        }
                    }
                    else
                    {
                        this.Invoke(new Action(() =>
                        {
                            richTextBox1.AppendText(DateTime.Now.ToLongTimeString() + "没有需要保存的数据!" + "\r\n");

                        }));
                       
                    }
                }
                else
                {
                    this.Invoke(new Action(() =>
                    {
                        richTextBox1.AppendText(DateTime.Now.ToLongTimeString() + "获取到旺店通销售出库单记录数为0，无须转换与保存，任务结束" + "\r\n");

                    }));
                  
                }
            }
            else
            {
                this.Invoke(new Action(() =>
                {
                    richTextBox1.AppendText(DateTime.Now.ToLongTimeString() + "错误码:" + resultobj["code"].ToString() + ";错误原因:" + resultobj["message"].ToString() + "\r\n");

                }));
              

            }
        }

        //旺店通销售退货单
        public void GetSaleReturnData()
        {
            this.Invoke(new Action(() =>
            {
                richTextBox1.AppendText(DateTime.Now.ToLongTimeString() + "开启销售退货单同步任务"+ "\r\n");

            }));
           
            string starttime = Csdate();
            string endtime = Cedate();

            this.Invoke(new Action(() =>
            {
                richTextBox1.AppendText(DateTime.Now.ToLongTimeString() + "开始获取旺店通销售退货单数据时间范围:" + starttime + "--" + endtime +"\r\n");

            }));

          
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
                    this.Invoke(new Action(() =>
                    {
                        richTextBox1.AppendText(DateTime.Now.ToLongTimeString() + "查询成功,获得记录数:" + resultobj["total_count"].ToString() + ";页数为" + pagetotal + "\r\n");

                    }));
                  
                    for (int i = 0; i < pagetotal; i++)
                    {
                        this.Invoke(new Action(() =>
                        {
                            richTextBox1.AppendText(DateTime.Now.ToLongTimeString() + $"查询第{i + 1}页销售退货单数据..." + "\r\n");

                        }));
                     
                        dt = new Dictionary<string, string>();
                        dt.Add("start_time", starttime);
                        dt.Add("end_time", endtime);
                        dt.Add("page_no", i.ToString());
                        dt.Add("status", ConfigurationManager.AppSettings["salereturnbillstatus"].ToString());
                        resultobj = JObject.Parse(GetWdtDataBLL.GetDataFromApi(dt, "stockin_order_query_refund.php"));
                        ConvertSaleReturn(resultobj);
                        this.Invoke(new Action(() =>
                        {
                            richTextBox1.AppendText(DateTime.Now.ToLongTimeString() + $"转换保存第{i + 1}页销售退货单数据完成" + "\r\n");

                        }));
                    
                    }
                    if (returnInfos.Count > 0)
                    {
                        try
                        {
                            this.Invoke(new Action(() =>
                            {
                                richTextBox1.AppendText(DateTime.Now.ToLongTimeString() + "开启事务，保存数据..."+ "\r\n");

                            }));
                           
                            dbContext.Db.BeginTran();//开启事务
                            bool flag1 = dbContext.SaleReturnStockInfoDb.InsertRange(returnInfos.ToArray());
                            bool flag2 = dbContext.SaleReturnStockItemDb.InsertRange(returnItems.ToArray());
                            if (flag1 & flag2)
                            {
                                dbContext.Db.CommitTran();
                                this.Invoke(new Action(() =>
                                {
                                    richTextBox1.AppendText(DateTime.Now.ToLongTimeString() + "保存成功，提交事务，共同步单据数：" + returnInfos.Count.ToString() + "\r\n");

                                }));
                              
                            }
                            else
                            {
                                dbContext.Db.RollbackTran();
                                this.Invoke(new Action(() =>
                                {
                                    richTextBox1.AppendText(DateTime.Now.ToLongTimeString()+"保存数据失败,事务回滚" + "\r\n");

                                }));
                              
                            }
                        }
                        catch (Exception er)
                        {
                            this.Invoke(new Action(() =>
                            {
                                richTextBox1.AppendText(DateTime.Now.ToLongTimeString() + "出现错误:" + er.Message + "\r\n");

                            }));
                           
                            dbContext.Db.RollbackTran();
                            this.Invoke(new Action(() =>
                            {
                                richTextBox1.AppendText(DateTime.Now.ToLongTimeString() + "事务回滚" + "\r\n");

                            }));
                           
                        }
                        finally
                        {
                            this.Invoke(new Action(() =>
                            {
                                richTextBox1.AppendText(DateTime.Now.ToLongTimeString() + "销售退货单同步任务完成" + "\r\n");

                            }));
                            
                        }
                    }
                    else
                    {
                        this.Invoke(new Action(() =>
                        {
                            richTextBox1.AppendText(DateTime.Now.ToLongTimeString() + "没有需要保存的数据!" + "\r\n");

                        }));
                       
                    }
                }
                else
                {
                    this.Invoke(new Action(() =>
                    {
                        richTextBox1.AppendText(DateTime.Now.ToLongTimeString() + "获取到旺店通销售退货单记录数为0，无须转换与保存，任务结束" + "\r\n");

                    }));
                  
                }
            }
            else
            {
                this.Invoke(new Action(() =>
                {
                    richTextBox1.AppendText(DateTime.Now.ToLongTimeString() + "错误码:" + resultobj["code"].ToString() + ";错误原因:" + resultobj["message"].ToString() + "\r\n");

                }));
              
            }
        }

        private void ConvertPur(JObject obj)
        {
            int insertrec = 0;
            try
            {
                this.Invoke(new Action(() =>
                {
                    richTextBox1.AppendText(DateTime.Now.ToLongTimeString() + "开始转换并保存获取到的采购入库单数据..." + "\r\n");

                }));
              

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
                            this.Invoke(new Action(() =>
                            {
                                richTextBox1.AppendText(DateTime.Now.ToLongTimeString()  +$"将采购单{pStockInfo.order_no}添加到待插入列表" + "\r\n");

                            }));
                           
                            pStockInfos.Add(pStockInfo);
                            insertrec++;
                        }
                        else
                        {
                            this.Invoke(new Action(() =>
                            {
                                richTextBox1.AppendText(DateTime.Now.ToLongTimeString() +$"采购单{pStockInfo.order_no}已存在" + "\r\n");

                            }));
                            
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
                    this.Invoke(new Action(() =>
                    {
                        richTextBox1.AppendText(DateTime.Now.ToLongTimeString() + "开启事务，保存数据..." + "\r\n");

                    }));
                                       
                    dbContext.Db.BeginTran();//开启事务
                    bool flag1 = dbContext.PurStockInfoDb.InsertRange(pStockInfos.ToArray());
                    bool flag2 = dbContext.PurStocItemDb.InsertRange(purStockItems.ToArray());
                    if (flag1 & flag2)
                    {
                        dbContext.Db.CommitTran();
                        this.Invoke(new Action(() =>
                        {
                            richTextBox1.AppendText(DateTime.Now.ToLongTimeString() + "保存成功，提交事务，共同步单据数：" + insertrec.ToString() + "\r\n");

                        }));
                       
                    }
                    else
                    {
                        dbContext.Db.RollbackTran();
                        this.Invoke(new Action(() =>
                        {
                            richTextBox1.AppendText(DateTime.Now.ToLongTimeString() + "保存数据失败,事务回滚" + "\r\n");

                        }));
                       
                    }
                }
                else
                {
                    this.Invoke(new Action(() =>
                    {
                        richTextBox1.AppendText(DateTime.Now.ToLongTimeString() + "没有需要保存的数据!" + "\r\n");

                    }));
                   
                }

            }
            catch (Exception er)
            {
                this.Invoke(new Action(() =>
                {
                    richTextBox1.AppendText(DateTime.Now.ToLongTimeString() + "运行错误:" + er.Message + "\r\n");

                }));
               
                dbContext.Db.RollbackTran();

                this.Invoke(new Action(() =>
                {
                    richTextBox1.AppendText(DateTime.Now.ToLongTimeString() + "事务回滚" + "\r\n");

                }));
               

            }
            finally
            {
                this.Invoke(new Action(() =>
                {
                    richTextBox1.AppendText(DateTime.Now.ToLongTimeString() + "采购入库单同步任务完成" + "\r\n");

                }));
                
            }

        }

        private void ConvertSale(JObject obj)
        {
            int insertrec = 0;
            try
            {
                this.Invoke(new Action(() =>
                {
                    richTextBox1.AppendText(DateTime.Now.ToLongTimeString() + "开始转换并保存获取到的销售出库单数据..." + "\r\n");

                }));
            
                T_Sale_StockOutInfo stockInfo;
                T_Sale_StockOutItem stockItem;

                int rec = int.Parse(obj["total_count"].ToString());
                //  Logger.Info(DateTime.Now.ToLongTimeString() + obj.ToString());
                if (rec >= 0)
                {
                    //  PropertyInfo[] propertyInfos = ForeachClassProperties<T_Sale_StockOutInfo>();
                    JArray arrRec = JArray.Parse(obj["stockout_list"].ToString());
                    this.Invoke(new Action(() =>
                    {
                        richTextBox1.AppendText(DateTime.Now.ToLongTimeString() + "该页记录数为:" + arrRec.Count.ToString() + "\r\n");

                    }));

                  
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
                            this.Invoke(new Action(() =>
                            {
                                richTextBox1.AppendText(DateTime.Now.ToLongTimeString() + $"将销售出库单{stockInfo.Order_No}添加到待插入列表" + "\r\n");

                            }));
                          
                            stockInfos.Add(stockInfo);
                            insertrec++;
                        }
                        else
                        {
                            this.Invoke(new Action(() =>
                            {
                                richTextBox1.AppendText(DateTime.Now.ToLongTimeString() + $"销售出库单{stockInfo.Order_No}已存在" + "\r\n");

                            }));
                          
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
                this.Invoke(new Action(() =>
                {
                    richTextBox1.AppendText(DateTime.Now.ToLongTimeString() + "运行错误:" + er.Message + "\r\n");

                }));
              


            }

        }

        private void ConvertSaleReturn(JObject obj)
        {
            int insertrec = 0;
            try
            {
                this.Invoke(new Action(() =>
                {
                    richTextBox1.AppendText(DateTime.Now.ToLongTimeString() + "开始转换并保存获取到的销售退货单数据..." + "\r\n");

                }));
             
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
                            this.Invoke(new Action(() =>
                            {
                                richTextBox1.AppendText(DateTime.Now.ToLongTimeString() + $"将销售退货单{returnInfo.Order_No}添加到待插入列表" + "\r\n");

                            }));
                          
                            returnInfos.Add(returnInfo);
                            insertrec++;
                        }
                        else
                        {
                            this.Invoke(new Action(() =>
                            {
                                richTextBox1.AppendText(DateTime.Now.ToLongTimeString() + $"销售退货单{returnInfo.Order_No}已存在" + "\r\n");

                            }));
                           
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
                this.Invoke(new Action(() =>
                {
                    richTextBox1.AppendText(DateTime.Now.ToLongTimeString() + "运行错误:" + er.Message + "\r\n");

                }));
              


            }


        }




       // 奇门销售出库单
        public void GetTBSaleOutData()
        {
            this.Invoke(new Action(() =>
            {
                richTextBox1.AppendText(DateTime.Now.ToLongTimeString() + "开启奇门销售出库单同步任务" + "\r\n");

            }));
           
            string starttime = Csdate();
            string tbshop_no = ConfigurationManager.AppSettings["tbshop_no"].ToString();
            string endtime =Cedate();

         
           
            endtime = DateTime.Parse(starttime).AddDays(30) > DateTime.Now ? DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") : DateTime.Parse(starttime).AddDays(30).ToString("yyyy-MM-dd HH:mm:ss");
            string[] shops = new string[] { "TM01", "002" };
            this.Invoke(new Action(() =>
            {
                richTextBox1.AppendText(DateTime.Now.ToLongTimeString() + "开始获取奇门销售出库单数据时间范围:" + starttime + "--" + endtime + "\r\n");

            }));
           
            List<wdtstockout> orderlst = dbContextTB.wdtstockoutTBDb.GetList(h => SqlFunc.Between(h.Consign_Time, starttime, endtime) && (h.Status == "95" || h.Status == "110" || h.Status == "105" || h.Status == "113") && shops.Contains(h.Shop_No));
            //  List<wdtstockout> orderlst = dbContextTB.wdtstockoutTBDb.GetList(h => SqlFunc.Between(h.Consign_Time, starttime, endtime) && (h.Status == "113") && shops.Contains(h.Shop_No));

            if (orderlst.Count > 0)
            {
                this.Invoke(new Action(() =>
                {
                    richTextBox1.AppendText(DateTime.Now.ToLongTimeString() + "查询成功,获得奇门销售出库记录数:" + orderlst.Count.ToString() + "\r\n");

                }));
             
                ConvertSaleTB(orderlst);
            }
            else
            {
                this.Invoke(new Action(() =>
                {
                    richTextBox1.AppendText(DateTime.Now.ToLongTimeString() + "获取到奇门销售出库单记录数为0，无须转换与保存，任务结束" + "\r\n");

                }));
               
            }
        }

        private void ConvertSaleTB(List<wdtstockout> orders)
        {

            int insertrec = 0;
            try
            {
                this.Invoke(new Action(() =>
                {
                    richTextBox1.AppendText(DateTime.Now.ToLongTimeString() + "开始转换并保存获取到的奇门销售出库单数据..." + "\r\n");

                }));
               
                T_Sale_StockOutInfo stockInfo;
                T_Sale_StockOutItem stockItem;
                List<T_Sale_StockOutInfo> stockInfos = new List<T_Sale_StockOutInfo>();
                List<T_Sale_StockOutItem> stockItems = new List<T_Sale_StockOutItem>();
                List<detailslist> details = new List<detailslist>();
                detailslist detail = new detailslist();
                wdtstockout wdtstockoutinfo = new wdtstockout();
                string tboutstr2num = ConfigurationManager.AppSettings["tboutstr"].ToString();
                int rec = orders.Count;
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
                                if (tboutstr2num.Contains(item.Name) && value == "")
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
                            this.Invoke(new Action(() =>
                            {
                                richTextBox1.AppendText(DateTime.Now.ToLongTimeString()  +$"将销售出库单{wdtstockoutinfo.Order_No}添加到待插入列表" + "\r\n");

                             }));
                         

                            stockInfos.Add(stockInfo);
                            //查询主表对应明细
                            details = dbContextTB.detailslistTBDb.GetList(d => d.Stockout_Id == wdtstockoutinfo.Stockout_Id);
                            for (int d = 0; d < details.Count; d++)
                            {
                                stockItem = new T_Sale_StockOutItem();
                                //将对应明细数据赋值给stockItem对象
                                detail = details[d];
                                PropertyInfo[] ProDetailList = stockItem.GetType().GetProperties();
                                foreach (PropertyInfo pdetail in ProDetailList)
                                {
                                    object value;
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
                            this.Invoke(new Action(() =>
                            {
                                richTextBox1.AppendText(DateTime.Now.ToLongTimeString() + $"奇门销售出库单{wdtstockoutinfo.Order_No}已存在" + "\r\n");

                            }));
                           
                            continue;
                        }
                    }
                }

                if (stockInfos.Count > 0)
                {
                    this.Invoke(new Action(() =>
                    {
                        richTextBox1.AppendText(DateTime.Now.ToLongTimeString() + "开启事务，保存数据..." + "\r\n");

                    }));
                   
                    dbContext.Db.BeginTran();//开启事务
                    bool flag1 = dbContext.SaleStockOutInfoDb.InsertRange(stockInfos.ToArray());
                    bool flag2 = dbContext.SaleStockOutItemDb.InsertRange(stockItems.ToArray());
                    if (flag1 & flag2)
                    {
                        dbContext.Db.CommitTran();
                        this.Invoke(new Action(() =>
                        {
                            richTextBox1.AppendText(DateTime.Now.ToLongTimeString() + "保存成功，提交事务，共同步奇门销售出库单据数：" + insertrec.ToString() + "\r\n");

                        }));
                       
                    }
                    else
                    {
                        dbContext.Db.RollbackTran();
                        this.Invoke(new Action(() =>
                        {
                            richTextBox1.AppendText(DateTime.Now.ToLongTimeString() + "保存数据失败,事务回滚" + "\r\n");

                        }));
                       
                    }
                }
                else
                {
                    this.Invoke(new Action(() =>
                    {
                        richTextBox1.AppendText(DateTime.Now.ToLongTimeString() + "没有需要保存的数据!" + "\r\n");

                    }));
                  
                }

            }
            catch (Exception er)
            {
                this.Invoke(new Action(() =>
                {
                    richTextBox1.AppendText(DateTime.Now.ToLongTimeString() + "运行错误:" + er.Message + "\r\n");

                }));
              

                dbContext.Db.RollbackTran();
                this.Invoke(new Action(() =>
                {
                    richTextBox1.AppendText(DateTime.Now.ToLongTimeString() + "事务回滚" + "\r\n");

                }));
               

            }
            finally
            {
                this.Invoke(new Action(() =>
                {
                    richTextBox1.AppendText(DateTime.Now.ToLongTimeString() + "奇门销售出库单同步任务完成" + "\r\n");

                }));
               
            }

        }
       


        #region  奇门销售退货单
        public void GetTBSaleReturnData()
        {
            this.Invoke(new Action(() =>
            {
                richTextBox1.AppendText(DateTime.Now.ToLongTimeString() + "开启奇门销售退货单同步任务" + "\r\n");

            }));
           
            string starttime =Csdate();
            string tbshop_no = ConfigurationManager.AppSettings["tbshop_no"].ToString();
            string endtime =Cedate();

            this.Invoke(new Action(() =>
            {
                richTextBox1.AppendText(DateTime.Now.ToLongTimeString() + "开始获取奇门销售退货单数据时间范围:" + starttime + "--" + endtime + "\r\n");

            }));
         
            List<wdtthstockin> orderlst = dbContextTB.wdtthOrderTBDb.GetList(h => SqlFunc.Between(h.check_time, starttime, endtime) && (h.warehouse_no == "TMCK01" || h.warehouse_no == "TMCK02"));

            if (orderlst.Count > 0)
            {
                this.Invoke(new Action(() =>
                {
                    richTextBox1.AppendText(DateTime.Now.ToLongTimeString() + "查询成功,获得奇门退货单记录数:" + orderlst.Count.ToString() + "\r\n");

                }));
               
                ConvertSaleReturnTB(orderlst);
            }
            else
            {
                this.Invoke(new Action(() =>
                {
                    richTextBox1.AppendText(DateTime.Now.ToLongTimeString() + "获取到奇门退货单单记录数为0，无须转换与保存，任务结束" + "\r\n");

                }));
                
            }
        }

        private void ConvertSaleReturnTB(List<wdtthstockin> orders)
        {

            int insertrec = 0;
            try
            {
                this.Invoke(new Action(() =>
                {
                    richTextBox1.AppendText(DateTime.Now.ToLongTimeString() + "开始转换并保存获取到的奇门销售退货单数据..." + "\r\n");

                }));
               
                T_Sale_ReturnStockInfo stockInfo;
                T_Sale_ReturnStockItem stockItem;
                List<T_Sale_ReturnStockInfo> stockInfos = new List<T_Sale_ReturnStockInfo>();
                List<T_Sale_ReturnStockItem> stockItems = new List<T_Sale_ReturnStockItem>();
                List<wdtthstockin> wdtthorders = new List<wdtthstockin>();
                List<wdtthdetailslist> wdtthorderinfos = new List<wdtthdetailslist>();
                wdtthdetailslist wdtthorderinfo = new wdtthdetailslist();
                wdtthstockin wdtthorder = new wdtthstockin();
                string tboutstr2num = ConfigurationManager.AppSettings["tboutstr"].ToString();
                int rec = orders.Count;
                if (rec > 0)
                {

                    for (int r = 0; r < rec; r++)
                    {
                        wdtthorder = orders[r];

                        if (dbContext.SaleReturnStockInfoDb.GetSingle(h => h.Order_No == wdtthorder.order_no) is null)
                        {
                            stockInfo = new T_Sale_ReturnStockInfo();
                            PropertyInfo[] PropertyList = stockInfo.GetType().GetProperties();
                            foreach (PropertyInfo item in PropertyList)
                            {

                                string value = JObject.Parse(wdtthorder.ToJsonString())[item.Name.ToLower()].ToString();
                                //if (tboutstr2num.Contains(item.Name) && value == "")
                                //{
                                //    value = "0";
                                //}
                                if (item.PropertyType == typeof(System.DateTime))
                                {
                                    item.SetValue(stockInfo, DateTime.Parse(value.Replace("0000-00-00 00:00:00", "1970-01-01 00:00:00")));
                                }
                                else
                                {
                                    item.SetValue(stockInfo, value);
                                }
                            }
                            this.Invoke(new Action(() =>
                            {
                                richTextBox1.AppendText(DateTime.Now.ToLongTimeString() + $"将奇门销售退货单{wdtthorder.order_no}添加到待插入列表" + "\r\n");

                            }));
                           
                            stockInfos.Add(stockInfo);
                            //查询主表对应明细
                            wdtthorderinfos = dbContextTB.wdtthOrderinfoTBDb.GetList(d => d.stockin_id == wdtthorder.stockin_id);
                            for (int d = 0; d < wdtthorderinfos.Count; d++)
                            {
                                stockItem = new T_Sale_ReturnStockItem();
                                wdtthorderinfo = wdtthorderinfos[d];
                                PropertyInfo[] ProDetailList = stockItem.GetType().GetProperties();
                                foreach (PropertyInfo detail in ProDetailList)
                                {

                                    object value;
                                    if (detail.Name != "Order_No")
                                    {

                                        if (detail.Name != "Expire_Date")
                                        {
                                            value = JObject.Parse(wdtthorderinfo.ToJsonString())[detail.Name.ToLower()].ToString();
                                        }
                                        else
                                        {
                                            value = "1970-01-01 00:00:00";
                                        }
                                        if (detail.PropertyType == typeof(System.DateTime))
                                        {
                                            detail.SetValue(stockItem, DateTime.Parse(value.ToString().Replace("0000-00-00 00:00:00", "1970-01-01 00:00:00")));
                                        }
                                        else
                                        {

                                            detail.SetValue(stockItem, value);

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
                            this.Invoke(new Action(() =>
                            {
                                richTextBox1.AppendText(DateTime.Now.ToLongTimeString() + $"奇门销售退货单{wdtthorder.order_no}已存在" + "\r\n");

                            }));
                          
                            continue;
                        }
                    }
                }

                if (stockInfos.Count > 0)
                {
                    this.Invoke(new Action(() =>
                    {
                        richTextBox1.AppendText(DateTime.Now.ToLongTimeString() + "开启事务，保存数据..." + "\r\n");

                    }));
                   
                    dbContext.Db.BeginTran();//开启事务
                    bool flag1 = dbContext.SaleReturnStockInfoDb.InsertRange(stockInfos.ToArray());
                    bool flag2 = dbContext.SaleReturnStockItemDb.InsertRange(stockItems.ToArray());
                    if (flag1 & flag2)
                    {
                        dbContext.Db.CommitTran();
                        this.Invoke(new Action(() =>
                        {
                            richTextBox1.AppendText(DateTime.Now.ToLongTimeString() + "保存成功，提交事务，共同步奇门销售退货单据数：" + insertrec.ToString() + "\r\n");

                        }));
                       
                    }
                    else
                    {
                        dbContext.Db.RollbackTran();
                        this.Invoke(new Action(() =>
                        {
                            richTextBox1.AppendText(DateTime.Now.ToLongTimeString() + "保存数据失败,事务回滚" + "\r\n");

                        }));
                        
                    }
                }
                else
                {
                    this.Invoke(new Action(() =>
                    {
                        richTextBox1.AppendText(DateTime.Now.ToLongTimeString() + "没有需要保存的数据!" + "\r\n");

                    }));
                   
                }

            }
            catch (Exception er)
            {
                this.Invoke(new Action(() =>
                {
                    richTextBox1.AppendText(DateTime.Now.ToLongTimeString() + "运行错误:" + er.Message + "\r\n");

                }));
               

                dbContext.Db.RollbackTran();
                this.Invoke(new Action(() =>
                {
                    richTextBox1.AppendText(DateTime.Now.ToLongTimeString() + "事务回滚" + "\r\n");

                }));
               


            }
            finally
            {
                this.Invoke(new Action(() =>
                {
                    richTextBox1.AppendText(DateTime.Now.ToLongTimeString() + "奇门销售退货单同步任务完成" + "\r\n");

                }));
               
            }

        }

        #endregion

    }
}
