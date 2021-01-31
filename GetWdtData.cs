using SleckEc2KdInterface.BLL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using SleckEc2KdInterface.Entity;
using System.Reflection;
using SleckEc2KdInterface.DAL;
using System.Configuration;

namespace SleckEc2KdInterface
{
    public partial class GetWdtData : Form
    {
        private PurOrder purOrder;
        private SaleOutOrder saleOutOrder;
        private SaleReturnOrder saleReturnOrder;
        DbContext dbContext = new DbContext(false);
        public GetWdtData()
        {
            InitializeComponent();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            Dictionary<string, string> dt = new Dictionary<string, string>();
            dt.Add("start_time", "2021-01-19 00:00:00");
            dt.Add("end_time", "2021-01-20 14:42:00");
            dt.Add("warehouse_no", ConfigurationManager.AppSettings["warehouse_no"].ToString());
            dt.Add("status", ConfigurationManager.AppSettings["purbillstatus"].ToString());    
            JObject resultobj =JObject.Parse(GetWdtDataBLL.GetDataFromApi(dt, "stockin_order_query_purchase.php"));
            richTextBox1.AppendText(resultobj.ToString());
            if(resultobj["code"].ToString()=="0")
            {
                label1.Text = "查询成功,获得记录数:"+ resultobj["total_count"].ToString();
                ConvertPur(resultobj);
            }
            else
            {
                label1.Text = "错误码:" + resultobj["code"].ToString()+ ";错误原因:" + resultobj["message"].ToString();
            }
        }
        private void  ConvertPur (JObject obj)
        {
            try
            {
                purOrder = new PurOrder();
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
                        pStockInfos.Add(pStockInfo);
                        //明细数据
                        JArray detailRec = JArray.Parse(tempo["details_list"].ToString());
                        for (int j = 0; j < detailRec.Count; j++)
                        {
                            tempo = detailRec[j] as JObject;
                            purStockItem = new PurStockItem();
                           
                            PropertyInfo[] ProDetailList = purStockItem.GetType().GetProperties();
                            foreach (PropertyInfo detail in ProDetailList)
                            {
                                if (detail.Name!= "Order_No")
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
              
                dbContext.Db.BeginTran();//开启事务
                bool flag1= dbContext.PurStockInfoDb.InsertRange(pStockInfos.ToArray());
                bool flag2 = dbContext.PurStocItemDb.InsertRange(purStockItems.ToArray());
                if (flag1 & flag2)
                {
                    dbContext.Db.CommitTran();
                }
                else
                {
                    dbContext.Db.RollbackTran();
                    MessageBox.Show("插入数据失败");
                }
                purOrder.PurOrderHead = pStockInfos;
                purOrder.PurOrderDetail = purStockItems;
                dataGridView6.DataSource = pStockInfos;
            }
            catch (Exception er)
            {
                MessageBox.Show(er.Message);
            }

        }

        private void ConvertSale(JObject obj)
        {
            try
           {
                saleOutOrder = new SaleOutOrder();
                T_Sale_StockOutInfo stockInfo;
                T_Sale_StockOutItem stockItem;
                List<T_Sale_StockOutInfo> stockInfos = new List<T_Sale_StockOutInfo>();
                List<T_Sale_StockOutItem> stockItems = new List<T_Sale_StockOutItem>();
                int rec = int.Parse(obj["total_count"].ToString());
                if (rec > 0)
                {
                    PropertyInfo[] propertyInfos = ForeachClassProperties<T_Sale_StockOutInfo>();
                    JArray arrRec = JArray.Parse(obj["stockin_list"].ToString());
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
                            item.SetValue(stockInfo,value);
                        }
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

                dbContext.Db.BeginTran();//开启事务
                bool flag1 = dbContext.SaleStockOutInfoDb.InsertRange(stockInfos.ToArray());
                bool flag2 = dbContext.SaleStockOutItemDb.InsertRange(stockItems.ToArray());
                if (flag1 & flag2)
                {
                    dbContext.Db.CommitTran();
                }
                else
                {
                    dbContext.Db.RollbackTran();
                    MessageBox.Show("插入数据失败");
                }
                saleOutOrder.SaleStockOutInfoHead=stockInfos;
                saleOutOrder.SaleStockOutInfoDetail =stockItems;
                dataGridView6.DataSource = stockInfos;
        }
            catch (Exception er)
            {
                MessageBox.Show(er.Message);
            }

}

        private void ConvertSaleReturn(JObject obj)
        {
            try
            {
                saleReturnOrder = new SaleReturnOrder();
                T_Sale_ReturnStockInfo returnInfo;
                T_Sale_ReturnStockItem returnItem;
                List<T_Sale_ReturnStockInfo > returnInfos = new List<T_Sale_ReturnStockInfo >();
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
                        returnInfos.Add(returnInfo);
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

                dbContext.Db.BeginTran();//开启事务
                bool flag1 = dbContext.SaleReturnStockInfoDb.InsertRange(returnInfos.ToArray());
                bool flag2 = dbContext.SaleReturnStockItemDb.InsertRange(returnItems.ToArray());
                if (flag1 & flag2)
                {
                    dbContext.Db.CommitTran();
                }
                else
                {
                    dbContext.Db.RollbackTran();
                    MessageBox.Show("插入数据失败");
                }
                saleReturnOrder.SaleReturnHead = returnInfos;
                saleReturnOrder.SaleReturnDetail = returnItems;
                dataGridView6.DataSource = returnInfos;
            }
            catch (Exception er)
            {
                MessageBox.Show(er.Message);
            }

        }

        private PropertyInfo[] ForeachClassProperties<T>()
        {
            Type t = typeof(T);
            PropertyInfo[] PropertyList = t.GetProperties();
            return PropertyList;
          
        }
        private void GetWdtData_Load(object sender, EventArgs e)
        {

        }

        private void dataGridView6_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
             if (e.RowIndex != -1)
                {

                    string order_no = dataGridView6.Rows[e.RowIndex].Cells[0].Value.ToString();
                //   var itemlst =saleOutOrder.SaleStockOutInfoDetail.Where(h=>h.Order_No== order_no).ToList();
                var itemlst = saleReturnOrder.SaleReturnDetail.Where(h => h.Order_No == order_no).ToList();
                dataGridView1.DataSource = itemlst;

                }
           
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Dictionary<string, string> dt = new Dictionary<string, string>();
            dt.Add("start_time", "2021-01-22 13:00:00");
            dt.Add("end_time", "2021-01-22 13:38:00");
            dt.Add("shop_no", "shulei2-test");
            dt.Add("status", ConfigurationManager.AppSettings["saleoutbillstatus"].ToString());
            JObject resultobj = JObject.Parse(GetWdtDataBLL.GetDataFromApi(dt, "stockout_order_query_trade.php"));
            richTextBox1.AppendText(resultobj.ToString());
            if (resultobj["code"].ToString() == "0")
            {
                label1.Text = "查询成功,获得记录数:" + resultobj["total_count"].ToString();
                ConvertSale(resultobj);
            }
            else
            {
                label1.Text = "错误码:" + resultobj["code"].ToString() + ";错误原因:" + resultobj["message"].ToString();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Dictionary<string, string> dt = new Dictionary<string, string>();
            dt.Add("start_time", "2021-01-19 17:00:00");
            dt.Add("end_time", "2021-01-22 13:43:00");
            dt.Add("shop_no", "shulei2-test");
            dt.Add("status", ConfigurationManager.AppSettings["salereturnbillstatus"].ToString());
            JObject resultobj = JObject.Parse(GetWdtDataBLL.GetDataFromApi(dt, "stockin_order_query_refund.php"));
            richTextBox1.AppendText(resultobj.ToString());
            if (resultobj["code"].ToString() == "0")
            {
                label1.Text = "查询成功,获得记录数:" + resultobj["total_count"].ToString();
                ConvertSaleReturn(resultobj);
            }
            else
            {
                label1.Text = "错误码:" + resultobj["code"].ToString() + ";错误原因:" + resultobj["message"].ToString();
            }
        }
    }
   
}
