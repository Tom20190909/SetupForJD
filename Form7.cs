using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NPOI.HSSF.UserModel;
using NPOI.SS.Formula;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using RestSharp;
using SleckEc2KdInterface.BLL;
using SleckEc2KdInterface.DAL;
using SleckEc2KdInterface.Entity;
using SlekGift.Models;
using SqlSugar;
using Sunny.UI;
using Sunny.UI.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.Util;
using System.Windows.Forms;
using System.Xml.Linq;
using static NPOI.HSSF.Util.HSSFColor;
using static System.Windows.Forms.AxHost;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using UIMessageBox = SleckEc2KdInterface.UI.NexusMessageBox;
using MessageBox = SleckEc2KdInterface.UI.NexusMessageBox;


namespace SleckEc2KdInterface
{
    public partial class Form7 : UI.NexusWindow
    {
        static DbContext dbContext = new DbContext(false);
        static DbContext dbContextEc = new DbContext(true);
        List<T_Sale_StockOutInfo> t_Sale_StockOutInfolst;
        List<T_Sale_StockOutItem> t_Sale_StockOutItemlst;
        List<T_Sale_ReturnStockInfo> t_Sale_ReturnStockInfolst;
        List<T_Sale_ReturnStockItem> t_Sale_ReturnStockItemlst;
        List<U_Plans_WDTOrder> u_Plans_WDTOrderlst;
        List<U_Plans_WDTOrderItems> u_Plans_WDTOrderItemslst;
        static bool checkPass;
        static int errorcount;
        static int noerrorcount;
        /// <summary>
        /// 排序规则
        /// </summary>
        private bool dataGridView2ASC = false;
        public Form7()
        {
            InitializeComponent();
            UI.NexusShell.ApplyMain(this, uiTabControl1, true);
            UI.NexusPageLayouts.Apply(this, true);
        }

        private void Form7_Load(object sender, EventArgs e)
        {
            sdt.Value = DateTime.Today;
            edt.Value = DateTime.Today;
            dzsdt.Value = DateTime.Today;
            dzedt.Value = DateTime.Today;
            ssdt.Value = DateTime.Today;
            sedt.Value = DateTime.Today;
            rsdt.Value = DateTime.Today;
            redt.Value = DateTime.Today;
            if (wflogin.FUserType.Trim() != "5")
            {
                uiTextBox4.Enabled = true;
                uiTextBox8.Enabled = true;
                uitxtdzCustName.Enabled = true;
                uiTextBox11.Enabled = true;
            }
            else
            {
                uiTextBox4.Enabled = false;
                uiTextBox8.Enabled = false;
                uitxtdzCustName.Enabled = false;
                uiTextBox11.Enabled = false;
            }
            uiTextBox4.Text = wflogin.fcustName;
            uiTextBox8.Text = wflogin.fcustName;
            uitxtdzCustName.Text = wflogin.fcustName;
            uiTextBox11.Text = wflogin.fcustName;
        }

        private void Form7_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void uiButton2_Click(object sender, EventArgs e)
        {
            try
            {
                openFileDialog1.Filter = "EXCEL文件|*.xls";
                openFileDialog1.ShowDialog();

            }
            catch (Exception er)
            {
                UIMessageBox.ShowError(er.Message);
            }
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            uitxtfilepath.Text = openFileDialog1.FileName;
            Task.Run(exceltodata);
        }
        private void exceltodata()
        {
            this.Invoke(new Action(() =>
            {
                uiRichTextBox1.Clear();
                uiRichTextBox1.AppendText(DateTime.Now.ToShortTimeString() + ":开始导入...\r\n");
                uiButton2.Text = "导入中...";
                uiButton2.Enabled = false;


            }));
            DataTable dt = ExcelToDataTable(uitxtfilepath.Text, true);
            this.Invoke(new Action(() =>
            {
                uiDataGridView1.DataSource = dt;
                uiButton2.Text = "导入";
                uiButton2.Enabled = true;
                uiRichTextBox1.AppendText(DateTime.Now.ToShortTimeString() + ":导入结束\r\n");

            }));
            checkPass = false;
            errorcount = 0;

        }

        public static DataTable ExcelToDataTable(string filePath, bool isColumnName)
        {
            DataTable dataTable = null;
            FileStream fs = null;
            DataColumn column = null;
            DataRow dataRow = null;
            IWorkbook workbook = null;
            ISheet sheet = null;
            IRow row = null;
            ICell cell = null;
            int startRow = 0;
            int nullRow = 0;
            try
            {
                using (fs = File.OpenRead(filePath))
                {
                    // 2007版本  
                    if (filePath.IndexOf(".xlsx") > 0)
                        workbook = new XSSFWorkbook(fs);
                    // 2003版本  
                    else if (filePath.IndexOf(".xls") > 0)
                        workbook = new HSSFWorkbook(fs);
                    int Addresscell = 0;

                    if (workbook != null)
                    {
                        sheet = workbook.GetSheetAt(0);//读取第一个sheet，当然也可以循环读取每个sheet  
                        dataTable = new DataTable();
                        if (sheet != null)
                        {
                            int rowCount = sheet.LastRowNum;//总行数  
                            if (rowCount > 0)
                            {
                                IRow firstRow = sheet.GetRow(0);//第一行  
                                int cellCount = firstRow.LastCellNum + 1;//列数  

                                //构建datatable的列  
                                if (isColumnName)
                                {
                                    startRow = 1;//如果第一行是列名，则从第二行开始读取  
                                    for (int i = firstRow.FirstCellNum; i < cellCount; ++i)
                                    {
                                        cell = firstRow.GetCell(i);
                                        if (cell != null)
                                        {
                                            if (cell.StringCellValue != null)
                                            {
                                                column = new DataColumn(cell.StringCellValue);
                                                dataTable.Columns.Add(column);

                                                if (cell.StringCellValue.Contains("地址"))
                                                {
                                                    if (Addresscell == 0)
                                                    {
                                                        Addresscell = i;
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {

                                            if (nullRow == 0)
                                            {
                                                column = new DataColumn("原地址");
                                                dataTable.Columns.Add(column);
                                                nullRow = dataTable.Columns.Count - 1;
                                                nullRow++;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    for (int i = firstRow.FirstCellNum; i < cellCount; ++i)
                                    {
                                        column = new DataColumn("column" + (i + 1));
                                        dataTable.Columns.Add(column);
                                    }
                                }

                                //填充行  
                                for (int i = startRow; i <= rowCount; ++i)
                                {
                                    row = sheet.GetRow(i);
                                    if (row == null) continue;

                                    dataRow = dataTable.NewRow();
                                    for (int j = row.FirstCellNum; j < cellCount; ++j)
                                    {
                                        cell = row.GetCell(j);
                                        if (cell == null)
                                        {
                                            dataRow[j] = "";
                                        }
                                        else
                                        {
                                            //CellType(Unknown = -1,Numeric = 0,String = 1,Formula = 2,Blank = 3,Boolean = 4,Error = 5,)  
                                            switch (cell.CellType)
                                            {
                                                case CellType.Blank:
                                                    dataRow[j] = "";
                                                    break;
                                                case CellType.Numeric:
                                                    short format = cell.CellStyle.DataFormat;
                                                    //对时间格式（2015.12.5、2015/12/5、2015-12-5等）的处理  
                                                    if (format == 14 || format == 31 || format == 57 || format == 58)
                                                        dataRow[j] = cell.DateCellValue;
                                                    else
                                                        dataRow[j] = cell.NumericCellValue;
                                                    break;
                                                case CellType.String:
                                                    dataRow[j] = cell.StringCellValue.Trim().Replace("'", "");
                                                    break;
                                            }
                                        }

                                        if (j == (nullRow - 1))
                                        {
                                            dataRow[j] = row.GetCell(Addresscell);
                                        }
                                    }
                                    dataTable.Rows.Add(dataRow);
                                }
                            }
                        }
                    }
                }
                return dataTable;
            }
            catch (Exception ex)
            {
                if (fs != null)
                {
                    fs.Close();
                }
                return null;
            }
        }

        private void uiButton7_Click(object sender, EventArgs e)
        {
            if (uiDataGridView1.Rows.Count < 1)
            {
                UIMessageBox.Show("没有可以校验的数据!");
                return;
            }
            Task.Run(CheckData);
        }

        private void CheckData()
        {
            try
            {
                errorcount = 0;
                noerrorcount = 0;
                this.Invoke(new Action(() =>
                {
                    uiRichTextBox1.Clear();
                    uiRichTextBox1.AppendText(DateTime.Now.ToLongTimeString() + ":开始校验...\r\n");
                    uiButton7.Text = "校验中...";
                    uiButton7.Enabled = false;


                }));
                int r = 1;
                List<OrderImpInfo> lstorinfo = new List<OrderImpInfo>();
                var datas = JArray.FromObject(uiDataGridView1.DataSource);
                var addresslst = datas.Where(a => a["地址"].ToString().Split(" ").Length < 4).Select(a => a["地址"].ToString()).Distinct().ToList();
                this.Invoke(new Action(() =>
                {

                    uiRichTextBox1.AppendText(DateTime.Now.ToLongTimeString() + ":开始校验收货地址...." + "\r\n");

                }));
                using (HttpClient hclient = new HttpClient())
                {
                    string url = "https://aip.baidubce.com/oauth/2.0/token?grant_type=client_credentials&client_id=2zXXDULe8nqpLsUHQAIeks23&client_secret=pZyzzbZsRv8Gl2i3BMwy7BjdtdQ6VNmG";
                    var hresponse = hclient.GetAsync(url);
                    var str = hresponse.Result.Content.ReadAsStringAsync();
                    string token;
                    string newAddress;
                    if (str.Result.Contains("access_token"))
                    {
                        JObject Obj = JObject.Parse(str.Result);
                        token = Obj["access_token"].ToString();
                        int editRow = 0;
                        addresslst.ForEach(a =>
                        {
                            if (a != "" && a != null)
                            {
                                url = "https://aip.baidubce.com/rpc/2.0/nlp/v1/address?access_token=" + token + "&text=" + a;
                                hresponse = hclient.GetAsync(url);
                                Obj = JObject.Parse(hresponse.Result.Content.ReadAsStringAsync().Result);
                                newAddress = Obj["province"].ToString() + " " + Obj["city"].ToString() + " " + Obj["county"].ToString() + " " + Obj["town"].ToString() + " " + Obj["detail"].ToString();
                                for (int rw = 0; rw < datas.Count; rw++)
                                {
                                    if (datas[rw]["地址"].ToString() == a.ToString())
                                    {
                                        datas[rw]["地址"] = newAddress;
                                        editRow++;
                                    }
                                }
                            }
                        });
                        if (editRow > 0)
                        {
                            this.Invoke(new Action(() =>
                            {
                                uiRichTextBox1.AppendText("合计" + editRow.ToString() + "行地址被修改" + "\r\n");
                            }));
                        }

                    };

                }

                this.Invoke(new Action(() =>
                {

                    uiDataGridView1.DataSource = datas;
                    uiRichTextBox1.AppendText(DateTime.Now.ToLongTimeString() + ":收货地址校验完成" + "\r\n");

                }));
                this.Invoke(new Action(() =>
                {
                    uiRichTextBox1.AppendText(DateTime.Now.ToLongTimeString() + ":开始校验店铺、仓库及手机号、收件人、商家编码、物流类型...." + "\r\n");
                }));
                datas.ForEach(a =>
                {

                    if (a["店铺名称"] == null || a["店铺名称"].ToString() == "")
                    {
                        errorcount++;
                        this.Invoke(new Action(() =>
                        {
                            uiRichTextBox1.AppendText("第" + r.ToString() + "行店铺名称为空" + "\r\n");
                            uiDataGridView1.Rows[r - 1].Cells[0].Style.BackColor = Color.Yellow;

                        }));
                    }
                    if (a["发货仓库"] == null || a["发货仓库"].ToString() == "")
                    {
                        errorcount++;
                        this.Invoke(new Action(() =>
                        {
                            uiRichTextBox1.AppendText("第" + r.ToString() + "行发货仓库为空" + "\r\n");
                            uiDataGridView1.Rows[r - 1].Cells[1].Style.BackColor = Color.Yellow;

                        }));
                    }
                    if (a["收件人"] == null || a["收件人"].ToString() == "")
                    {
                        errorcount++;
                        this.Invoke(new Action(() =>
                        {
                            uiRichTextBox1.AppendText("第" + r.ToString() + "行收件人为空" + "\r\n");
                            uiDataGridView1.Rows[r - 1].Cells[2].Style.BackColor = Color.Yellow;

                        }));
                    }
                    if (!System.Text.RegularExpressions.Regex.IsMatch(a["手机"].ToString(), @"^(?:(?:\+|00)86)?1[3-9]\d{9}$"))
                    {
                        errorcount++;
                        this.Invoke(new Action(() =>
                        {
                            uiRichTextBox1.AppendText("第" + r.ToString() + "行手机号有误" + "\r\n");
                            uiDataGridView1.Rows[r - 1].Cells[3].Style.BackColor = Color.Yellow;

                        }));
                    }

                    if (a["商家编码"] == null || a["商家编码"].ToString() == "")
                    {
                        errorcount++;
                        this.Invoke(new Action(() =>
                        {
                            uiRichTextBox1.AppendText("第" + r.ToString() + "行商家编码为空" + "\r\n");
                            uiDataGridView1.Rows[r - 1].Cells[5].Style.BackColor = Color.Yellow;

                        }));
                    }
                    if (!System.Text.RegularExpressions.Regex.IsMatch(a["货品数量"].ToString(), @"^[1-9]\d*$"))
                    {
                        errorcount++;
                        this.Invoke(new Action(() =>
                        {
                            uiRichTextBox1.AppendText("第" + r.ToString() + "行货品数量有误" + "\r\n");
                            uiDataGridView1.Rows[r - 1].Cells[6].Style.BackColor = Color.Yellow;

                        }));
                    }

                    r++;

                });
                #region 校验店铺名
                var shopName = datas.Where(a => a["店铺名称"].ToString() != null && a["店铺名称"].ToString() != "").Select(a => a["店铺名称"].ToString()).Distinct().ToList();
                Dictionary<string, string> dt = new Dictionary<string, string>();
                dt = new Dictionary<string, string>();
                dt.Add("page_size", 100.ToString());
                JObject resultobj = JObject.Parse(GetWdtDataBLL.GetDataFromApi(dt, "shop.php"));
                if (resultobj["code"].ToString() == "0")
                {


                    JArray shopS = JArray.Parse(resultobj["shoplist"].ToString());


                    shopName.ForEach(s =>
                    {
                        if (shopS.Where(a => a["shop_name"].ToString().Equals(s.ToString())).ToList().Count < 1)
                        {
                            errorcount++;
                            r = 1;
                            datas.ForEach(b =>
                            {
                                if (b["店铺名称"].ToString() == s.ToString())
                                {
                                    this.Invoke(new Action(() =>
                                    {
                                        uiRichTextBox1.AppendText("第" + r.ToString() + "行店铺名称验证不通过" + "\r\n");
                                        uiDataGridView1.Rows[r - 1].Cells[0].Style.BackColor = Color.Yellow;

                                    }));
                                }
                                r++;
                            });

                        }
                    });
                    this.Invoke(new Action(() =>
                    {
                        uiRichTextBox1.AppendText("店铺数据校验完成\r\n");


                    }));


                }
                else
                {
                    this.Invoke(new Action(() =>
                    {
                        uiRichTextBox1.AppendText(DateTime.Now.ToLongTimeString() + "校验店铺失败" + "\r\n");


                    }));

                }
                #endregion 校验店铺名结束
                #region 校验仓库
                var stockName = datas.Where(a => a["发货仓库"].ToString() != null && a["发货仓库"].ToString() != "").Select(a => a["发货仓库"].ToString()).Distinct().ToList();
                dt = new Dictionary<string, string>();
                dt.Add("page_size", 100.ToString());
                resultobj = JObject.Parse(GetWdtDataBLL.GetDataFromApi(dt, "warehouse_query.php"));
                if (resultobj["code"].ToString() == "0")
                {

                    bool checkSop = false;
                    JArray warehouses = JArray.Parse(resultobj["warehouses"].ToString());
                    stockName.ForEach(s =>
                    {

                        if (warehouses.Where(a => a["name"].ToString() == s.ToString()).ToList().Count < 1)
                        {
                            errorcount++;
                            r = 1;
                            datas.ForEach(b =>
                            {
                                if (b["发货仓库"].ToString() == s.ToString())
                                {
                                    this.Invoke(new Action(() =>
                                    {
                                        uiRichTextBox1.AppendText("第" + r.ToString() + "行发货仓库验证不通过" + "\r\n");
                                        uiDataGridView1.Rows[r - 1].Cells[1].Style.BackColor = Color.Yellow;

                                    }));
                                }
                                r++;
                            });
                        }
                    });


                }
                else
                {
                    this.Invoke(new Action(() =>
                    {
                        uiRichTextBox1.AppendText(DateTime.Now.ToLongTimeString() + "校验仓库失败" + resultobj["total_count"].ToString() + "\r\n");


                    }));

                }
                #endregion 校验仓库结束
                #region 校验商家编码
                var matNames = datas.Where(a => a["商家编码"].ToString() != null && a["商家编码"].ToString() != "").Select(a => a["商家编码"].ToString()).Distinct().ToList();


                this.Invoke(new Action(() =>
                {
                    uiRichTextBox1.AppendText("开始验证货品数据...\r\n");

                }));
                int matNamesCount = matNames.Count;
                RestClient client;
                RestRequest request;

                RestResponse response;
                matNames.ForEach(m =>
                {


                    try
                    {
                        client = new RestClient("http://120.78.210.159:3304/api/Wdt/GetCustWDTPriceNextSellMatList");
                        request = new RestRequest();
                        request.AddHeader("Content-Type", "application/json; charset=utf-8");
                        request.Method = Method.Post;

                        var matInfo = new { userid = wflogin.fuserId.Trim(), goodno = m };
                        var str = JsonConvert.SerializeObject(matInfo);

                        //request.AddJsonBody(str);
                        request.AddBody(str);
                        response = client.Execute(request);
                        var resultobj1 = JObject.Parse(response.Content);


                        if (resultobj1["code"].ToString() == "1")
                        {
                            errorcount++;
                            noerrorcount++;
                            r = 1;
                            datas.ForEach(b =>
                            {
                                if (b["商家编码"].ToString() == m.ToString())
                                {
                                    this.Invoke(new Action(() =>
                                    {
                                        uiRichTextBox1.AppendText("第" + r.ToString() + "行商家编码不存在于价目表中" + "\r\n");
                                        uiDataGridView1.Rows[r - 1].Cells[5].Style.BackColor = Color.Yellow;

                                    }));
                                }
                                r++;
                            });

                        }

                        if (resultobj1["code"].ToString() == "2")
                        {
                            errorcount++;
                            noerrorcount++;
                            r = 1;
                            datas.ForEach(b =>
                            {
                                if (b["商家编码"].ToString() == m.ToString())
                                {
                                    this.Invoke(new Action(() =>
                                    {
                                        uiRichTextBox1.AppendText("第" + r.ToString() + "行商家编码价目表中价格为0" + "\r\n");
                                        uiDataGridView1.Rows[r - 1].Cells[5].Style.BackColor = Color.Yellow;

                                    }));
                                }
                                r++;
                            });

                        }

                    }
                    catch (Exception er)
                    {
                        this.Invoke(new Action(() =>
                        {
                            uiRichTextBox1.AppendText("出现错误," + er.Message + "\r\n");

                        }));
                    }
                    #region 验证货品是否存在于旺店通
                    if (matNamesCount > 30)
                    {
                        Thread.Sleep(1000);
                    }
                    dt = new Dictionary<string, string>();
                    dt.Add("goods_no", m.ToString());
                    resultobj = JObject.Parse(GetWdtDataBLL.GetDataFromApi(dt, "goods_query.php"));

                    if (resultobj["total_count"].ToString() != "1")
                    {
                        dt = new Dictionary<string, string>();
                        dt.Add("suite_no", m.ToString());
                        resultobj = JObject.Parse(GetWdtDataBLL.GetDataFromApi(dt, "suites_query.php"));
                        if (resultobj["total_count"].ToString() != "1")
                        {
                            errorcount++;
                            r = 1;
                            datas.ForEach(b =>
                            {
                                if (b["商家编码"].ToString() == m.ToString())
                                {
                                    this.Invoke(new Action(() =>
                                    {
                                        uiRichTextBox1.AppendText("第" + r.ToString() + "行商家编码不存在于旺店通" + "\r\n");
                                        uiDataGridView1.Rows[r - 1].Cells[5].Style.BackColor = Color.Yellow;

                                    }));
                                }
                                r++;
                            });
                        }
                    }
                    #endregion
                });
                this.Invoke(new Action(() =>
                {

                    uiRichTextBox1.AppendText("验证货品数据结束\r\n");
                }));
                #endregion 校验商家编码结束

                #region 校验物流类型
                try
                {
                    JObject da = (JObject)datas.First();
                    bool useLogistics = false;
                    foreach (JProperty prop in da.Properties())
                    {
                        if (prop.Name == "物流类型")
                        {
                            useLogistics = true;
                            break;
                        }

                    }
                    if (useLogistics)
                    {
                        //    client = new RestClient("http://120.78.210.159:3304/api/Wdt/getLogistics");
                        //request = new RestRequest();
                        //request.AddHeader("Content-Type", "application/json; charset=utf-8");
                        //request.Method = Method.Post;
                        //response = client.Execute(request);
                        //var result = JObject.Parse(response.Content);
                        //this.Invoke(new Action(() =>
                        //{
                        //    uiRichTextBox1.AppendText(response.Content + "\r\n");


                        //}));
                        //if (result["code"].ToString() == "1")
                        //{
                        // var lst = JArray.Parse(result["logistics"].ToString());
                        JArray lst;
                        resultobj = JObject.Parse(GetWdtDataBLL.GetDataFromApi(dt, "logistics.php"));
                        if (resultobj["code"].ToString() == "0")
                        {
                            var logislst = JArray.Parse(JsonConvert.SerializeObject(resultobj["logistics_list"])).Where(a => a["is_disabled"].ToString() == "0" && a["logistics_type"].ToString() != "0").Select(a =>
                                  new
                                  {
                                      //logistics_typeName = a["logistics_name"].ToString(),
                                      logistics_typeName = getLogisticsName(Convert.ToInt32(a["logistics_type"])),
                                      logistics_type = Convert.ToInt32(a["logistics_type"])
                                  }).ToList();

                            lst = JArray.Parse(JsonConvert.SerializeObject(logislst));
                            r = 1;
                            datas.ForEach(a =>
                            {
                                if (lst.Where(l => l["logistics_typeName"].ToString() == a["物流类型"].ToString()).ToList().Count < 1)
                                {
                                    this.Invoke(new Action(() =>
                                    {
                                        this.Invoke(new Action(() =>
                                        {
                                            uiRichTextBox1.AppendText("第" + r.ToString() + "行物流类型不存在" + "\r\n");
                                            uiDataGridView1.Rows[r - 1].Cells[9].Style.BackColor = Color.Yellow;

                                        }));

                                    }));
                                    errorcount++;
                                }
                                r++;
                            });
                        }
                        else
                        {
                            this.Invoke(new Action(() =>
                            {
                                uiRichTextBox1.AppendText("校验物流出现错误\r\n");

                            }));
                        }
                    }
                    else
                    {
                        this.Invoke(new Action(() =>
                        {
                            uiRichTextBox1.AppendText("校验物流出现错误，找不到物流类型列\r\n");

                        }));

                    }
                }
                catch (Exception er)
                {
                    this.Invoke(new Action(() =>
                    {
                        uiRichTextBox1.AppendText("校验物流出现错误," + er.Message + "\r\n");

                    }));
                }

                #endregion 结束校验
                this.Invoke(new Action(() =>
                {

                    uiRichTextBox1.AppendText(DateTime.Now.ToLongTimeString() + ":校验店铺、仓库及手机号、收件人、商家编码、物流类型完成" + "\r\n");
                    uiRichTextBox1.AppendText(DateTime.Now.ToLongTimeString() + ":校验结束\r\n");
                    uiRichTextBox1.AppendText("存在错误数:" + errorcount.ToString() + "\r\n");
                    uiButton7.Text = "校验";
                    uiButton7.Enabled = true;
                }));
                checkPass = true;
            }
            catch (Exception ex)
            {

                this.Invoke(new Action(() =>
                {
                    uiRichTextBox1.AppendText("验证失败:" + ex.ToString() + "\r\n");
                    uiButton7.Text = "校验";
                    uiButton7.Enabled = true;
                }));
            }

        }

        private string getLogisticsName(int logisticsid)
        {
            //Thread.Sleep(500);

            RestClient client = new RestClient("http://120.78.210.159:3304/api/Wdt/getLogisticsName");
            RestRequest request = new RestRequest();
            request.AddHeader("Content-Type", "application/json");
            request.Method = Method.Post;
            RestResponse response;
            //   string shopname, string wdtsrcbillno, string statusid
            var obj = new { logisticsid = logisticsid };
            var str = JsonConvert.SerializeObject(obj);

            request.AddBody(str);
            response = client.Execute(request);
            var result = JObject.Parse(response.Content);

            return result["name"].ToString();

        }

        private void uiButton27_Click(object sender, EventArgs e)
        {
            if (uiDataGridView1.Rows.Count < 1)
            {
                UIMessageBox.Show("没有可以校验的数据!");
                return;
            }
            Task.Run(AddressCheckData);
        }


        private void AddressCheckData()
        {
            this.Invoke(new Action(() =>
            {
                uiRichTextBox1.Clear();
                uiRichTextBox1.AppendText(DateTime.Now.ToLongTimeString() + ":开始核对地址...\r\n");
                uiButton27.Text = "核对地址中...";
                uiButton27.Enabled = false;


            }));
            try
            {
                List<OrderImpInfo> lstorinfo = new List<OrderImpInfo>();
                var datas = JArray.FromObject(uiDataGridView1.DataSource);
                var addresslst = datas.Select(a => new { 新地址 = a["地址"].ToString().Trim(), 旧地址 = a["原地址"].ToString().Trim() }).Distinct().ToList();
                int messrow = 0;
                addresslst.ForEach(tempo =>
                {

                    //收货地址
                    string NewAddress = tempo.新地址 == null ? "" : tempo.新地址.ToString().Trim();
                    string OldAddress = tempo.旧地址 == null ? "" : tempo.旧地址.ToString().Trim();
                    int IsIdentical = 0;

                    if (NewAddress != "")
                    {

                        string[] readdresslist = NewAddress.Split(' ');
                        for (int i = 0; i < readdresslist.Length; i++)
                        {

                            string earaddress = readdresslist[i].Trim();
                            if (i <= 2)
                            {
                                string endaddress = readdresslist[i].Trim().Substring(0, 2);
                                if (!OldAddress.Contains(endaddress))
                                {
                                    IsIdentical = 2;
                                    messrow++;
                                    break;
                                }
                            }
                            //if (i == readdresslist.Length - 1)
                            //{

                            //    string endaddress = earaddress.Length >= 10 ? earaddress.Substring(0, 10) : earaddress.Substring(0, 5);
                            //    if (!OldAddress.Contains(endaddress))
                            //    {

                            //        IsIdentical = 2;
                            //        messrow++;
                            //        break;
                            //    }
                            //}


                        }

                        if (IsIdentical == 2)
                        {
                            int editRow = 0;
                            for (int rw = 0; rw < datas.Count; rw++)
                            {
                                if (datas[rw]["地址"].ToString().Trim() == NewAddress)
                                {
                                    editRow++;
                                    this.Invoke(new Action(() =>
                                    {
                                        uiDataGridView1.Rows[rw].Cells[4].Style.BackColor = Color.Red;
                                    }));
                                }
                            }

                            this.Invoke(new Action(() =>
                            {
                                uiRichTextBox1.AppendText("地址：【" + NewAddress + "】异常，与下单地址【" + OldAddress + "】不一致; " + "\r\n");
                            }));
                        }
                    }



                });


                this.Invoke(new Action(() =>
                {


                    if (messrow > 0)
                    {
                        uiRichTextBox1.AppendText(DateTime.Now.ToLongTimeString() + "存在地址异常，可直接修改【地址】 列,点保存即可,省市区街道分别用空格隔开" + "\r\n");
                        uiRichTextBox1.AppendText("地址格式如：xx省 xx市 xx区 街道地址" + "\r\n");
                    }
                    else
                    {
                        uiRichTextBox1.AppendText(DateTime.Now.ToLongTimeString() + "地址无异常" + "\r\n");
                    }
                    uiRichTextBox1.AppendText(DateTime.Now.ToLongTimeString() + ":核对地址完成" + "\r\n");
                    uiButton27.Text = "校验后地址核对";
                    uiButton27.Enabled = true;

                }));
            }
            catch (Exception ex)
            {
                this.Invoke(new Action(() =>
                {
                    uiRichTextBox1.AppendText(DateTime.Now.ToLongTimeString() + ":核对异常" + "\r\n");
                    uiRichTextBox1.AppendText(DateTime.Now.ToLongTimeString() + ":核对地址完成" + "\r\n");
                    uiButton27.Text = "校验后地址核对";
                    uiButton27.Enabled = true;

                }));
            }

        }

        private void uiButton17_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "Excel 文件(*.xls)| *.xls";
            saveFileDialog1.ShowDialog();

        }

        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            try
            {
                if (saveFileDialog1.FileName != "")
                {
                    string tfilepath = saveFileDialog1.FileName;
                    string sfilepath = Application.StartupPath + "\\ExcelTemp\\订单导入模板.xls";
                    File.Copy(sfilepath, tfilepath);
                    UIMessageBox.Show("模板复制成功！存放路径为:" + tfilepath);
                }
            }
            catch (Exception er)
            {
                UIMessageBox.Show("出现错误:" + er.Message);
            }

        }

        private void uiButton8_Click(object sender, EventArgs e)
        {
            if (uiDataGridView1.Rows.Count < 1)
            {
                UIMessageBox.Show("没有可以保存的数据!");
                return;
            }
            if (!checkPass)
            {
                UIMessageBox.Show("请先校验导入的数据!");
                return;

            }
            else
            {

                if (errorcount > 0)
                {
                    if (noerrorcount > 0)
                    {
                        UIMessageBox.Show("保存失败，校验时存在商品编码不存在价目表中或价格为空的数据!");
                        return;
                    }
                    else
                    {
                        DialogResult YRO = MessageBox.Show("校验数据时发现了错误，是否已纠正并继续保存？", "提醒您：", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                        if (YRO == DialogResult.No)
                        {
                            return;
                        }
                    }
                }

            }
            Task.Run(saveDataAsync);
        }


        private async Task saveDataAsync()
        {
            try
            {
                this.Invoke(new Action(() =>
                {

                    uiButton8.Text = "保存中...";
                    uiButton8.Enabled = false;

                }));
                List<U_Plans_WDTOrderItems> lstOrderItems = new List<U_Plans_WDTOrderItems>();
                var datas = JArray.FromObject(uiDataGridView1.DataSource);

                //RestClient client = new RestClient("http://120.78.210.159:3304/api/Wdt/getLogistics");
                //RestRequest request = new RestRequest();
                //request.AddHeader("Content-Type", "application/json; charset=utf-8");
                //request.Method = Method.Post;
                //var response = client.Execute(request);
                //var result = JObject.Parse(response.Content);

                //    if (result["code"].ToString() == "1")
                //    {
                //        lst = JArray.Parse(result["logistics"].ToString());

                //    }
                //    else
                //    {
                //        lst = null;
                //        this.Invoke(new Action(() =>
                //        {

                //            uiButton8.Text = "保存";
                //            uiButton8.Enabled = true;
                //            uiRichTextBox1.AppendText("保存失败,获取物流方式出错！\r\n");

                //        }));
                //        return;

                //    }
                //}

                JArray lst;
                try
                {
                    Dictionary<string, string> dt = new Dictionary<string, string>();

                    // dt.Add("logistics_no", "");
                    var resultobj = JObject.Parse(GetWdtDataBLL.GetDataFromApi(dt, "logistics.php"));
                    if (resultobj["code"].ToString() == "0")
                    {
                        var logislst = JArray.Parse(JsonConvert.SerializeObject(resultobj["logistics_list"])).Where(a => a["is_disabled"].ToString() == "0" && a["logistics_type"].ToString() != "0").Select(a =>
                              new
                              {
                                  //logistics_typeName = a["logistics_name"].ToString(),
                                  logistics_typeName = getLogisticsName(Convert.ToInt32(a["logistics_type"])),
                                  logistics_type = Convert.ToInt32(a["logistics_type"])
                              }).ToList();

                        lst = JArray.Parse(JsonConvert.SerializeObject(logislst));
                    }
                    else
                    {
                        lst = null;
                        this.Invoke(new Action(() =>
                        {

                            uiButton8.Text = "保存";
                            uiButton8.Enabled = true;
                            uiRichTextBox1.AppendText("保存失败,获取物流方式出错！\r\n");

                        }));
                        return;
                    }
                }

                catch (Exception ex)
                {

                    lst = null;
                    this.Invoke(new Action(() =>
                    {

                        uiButton8.Text = "保存";
                        uiButton8.Enabled = true;
                        uiRichTextBox1.AppendText("保存失败,获取物流方式出错！\r\n");

                    }));
                    return;
                }

                #region 将数据反写到源文件
                if (uiCheckBox1.Checked)
                {
                    if (System.IO.File.Exists(uitxtfilepath.Text))
                    {
                        HSSFWorkbook wb;
                        FileStream file;

                        using (file = new FileStream(uitxtfilepath.Text, FileMode.Open, FileAccess.ReadWrite))
                        {

                            wb = new HSSFWorkbook(file);

                            ISheet sheet;
                            sheet = wb.GetSheetAt(0);

                            NPOI.SS.UserModel.IRow rowTemp;

                            int j = 1;
                            datas.ForEach(r =>
                            {

                                rowTemp = sheet.GetRow(j);
                                if (r["店铺名称"] != null)
                                {
                                    rowTemp.GetCell(0).SetCellValue(r["店铺名称"].ToString());
                                }
                                if (r["发货仓库"] != null)
                                {
                                    rowTemp.GetCell(1).SetCellValue(r["发货仓库"].ToString());

                                }
                                if (r["收件人"] != null)
                                {
                                    rowTemp.GetCell(2).SetCellValue(r["收件人"].ToString());
                                }
                                if (r["手机"] != null)
                                {
                                    rowTemp.GetCell(3).SetCellValue(r["手机"].ToString());
                                }
                                if (r["地址"] != null)
                                {
                                    rowTemp.GetCell(4).SetCellValue(r["地址"].ToString());
                                }
                                if (r["商家编码"] != null)
                                {
                                    rowTemp.GetCell(5).SetCellValue(r["商家编码"].ToString());
                                }
                                if (r["货品数量"] != null)
                                {
                                    rowTemp.GetCell(6).SetCellValue(r["货品数量"].ToString());
                                }
                                if (r["客服备注"] != null && r["客服备注"].ToString() != "")
                                {
                                    rowTemp.GetCell(7).SetCellValue(r["客服备注"].ToString());
                                }
                                if (r["平台单号"] != null && r["平台单号"].ToString() != "")
                                {
                                    rowTemp.GetCell(8).SetCellValue(r["平台单号"].ToString());
                                }
                                if (r["物流类型"] != null && r["物流类型"].ToString() != "")
                                {
                                    rowTemp.GetCell(9).SetCellValue(r["物流类型"].ToString());
                                }
                                j++;
                            });



                            sheet.ForceFormulaRecalculation = true;

                            using (FileStream filess = System.IO.File.OpenWrite(uitxtfilepath.Text))
                            {
                                wb.Write(filess);
                            }
                            this.Invoke(new Action(() =>
                            {
                                uiRichTextBox1.AppendText("源文件已回写完成!" + "\r\n");

                            }));

                        }


                    }
                    else
                    {
                        this.Invoke(new Action(() =>
                        {
                            uiRichTextBox1.AppendText("源文件已无法找到!" + "\r\n");

                        }));
                    }

                }
                #endregion


                JObject da = (JObject)datas.First();
                bool useLogistics = false;
                foreach (JProperty prop in da.Properties())
                {
                    if (prop.Name == "物流类型")
                    {
                        useLogistics = true;
                        break;
                    }

                }

                if (useLogistics)
                {

                    datas.ForEach(a =>
                    {
                        lstOrderItems.Add(new U_Plans_WDTOrderItems
                        {
                            FID = -1,
                            Shopname = a["店铺名称"].ToString(),
                            StockName = a["发货仓库"].ToString(),
                            Srcbillno = a["平台单号"].ToString(),
                            WDTSrcbillno = "",
                            Recipienter = a["收件人"].ToString(),
                            Tel = a["手机"].ToString(),
                            Address = a["地址"].ToString(),
                            Conditions = "1",
                            Good_No = a["商家编码"].ToString(),
                            Qty = Convert.ToDecimal(a["货品数量"].ToString()),
                            Remark = a["客服备注"]==null?"":a["客服备注"].ToString(),
                            UserID = Convert.ToInt32(wflogin.fuserId.Trim()),
                            logistics_type = (int)(lst == null ? 0 : (lst.Where(l => l["logistics_typeName"].ToString() == a["物流类型"].ToString()).ToList().Count > 0 ? (lst.Where(l => l["logistics_typeName"].ToString() == a["物流类型"].ToString()).First()["logistics_type"]) : 0)),
                            WDTAddress = a["原地址"] == null ? a["地址"].ToString() : a["原地址"].ToString()

                        });
                    });
                    //this.Invoke(new Action(() =>
                    //{
                    //    uiRichTextBox1.AppendText(JsonConvert.SerializeObject(lstOrderItems) +"\r\n");
                    //}));
                    RestClient client = new RestClient("http://120.78.210.159:3304/api/Wdt/SaveWDTOrderNextSellAndItemApi");
                    RestRequest request = new RestRequest();
                    request.AddHeader("Content-Type", "application/json");
                    request.Method = Method.Post;

                    var obj = new { fid = -1, lstOrderItems = lstOrderItems, userid = Convert.ToInt32(wflogin.fuserId), fCustid = wflogin.fcustId, remark = "" };
                    var str = JsonConvert.SerializeObject(obj);
                    //this.Invoke(new Action(() =>
                    //{
                    //    uiRichTextBox1.AppendText(str + "\r\n");
                    //}));
                    request.AddParameter("application/json", str, ParameterType.RequestBody);
                    request.AddBody(str);
                    var response = client.Execute(request);
                    var result = JObject.Parse(response.Content);
                    if (result["code"].ToString() == "1")
                    {
                        this.Invoke(new Action(() =>
                        {
                            uiRichTextBox1.AppendText("保存成功!\r\n");
                            UIMessageBox.Show("保存成功,将跳转到订单查询页签进行提交!");
                            uiTabControl1.SelectedIndex = 1;
                            getOrderData();
                        }));
                    }
                }
                else
                {
                    this.Invoke(new Action(() =>
                    {
                        uiRichTextBox1.AppendText("保存失败，物流类型列不存在\r\n");
                        uiButton8.Text = "保存";
                        uiButton8.Enabled = true;
                    }));
                }
            }
            catch (Exception er)
            {
                this.Invoke(new Action(() =>
                {
                    uiRichTextBox1.AppendText(er.Message + "\r\n");
                    uiButton8.Text = "保存";
                    uiButton8.Enabled = true;
                    uiRichTextBox1.AppendText("保存结束\r\n");
                }));
            }
            this.Invoke(new Action(() =>
            {

                uiButton8.Text = "保存";
                uiButton8.Enabled = true;
                uiRichTextBox1.AppendText("保存结束\r\n");

            }));
        }

        public async void getOrderData()
        {
            this.Invoke(new Action(() =>
            {
                uiButton15.Text = "查询中...";
                uiButton15.Enabled = false;

            }));

            try
            {
                //using (HttpClient client = new HttpClient())
                //{

                var conobj = new
                {
                    startDate = Convert.ToDateTime(sdt.Value.ToShortDateString()),
                    endDate = Convert.ToDateTime(edt.Value.ToShortDateString()),
                    goodno = uitxtgoodno.Text,
                    custname = uiTextBox11.Text,
                    wdtsrcno = uitxtSrcOrderNo.Text,
                    recipienter = uitxtAddressee.Text,
                    userId = wflogin.fuserId

                };

                var str = JsonConvert.SerializeObject(conobj);
                //HttpContent content = new StringContent(str);
                //content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                //HttpResponseMessage response = await client.PostAsync("http://120.78.210.159:3304/api/Wdt/GetWDTOrderNextSellLogList", content);
                ////  response.EnsureSuccessStatusCode();//用来抛异常的
                //string resultStr = await response.Content.ReadAsStringAsync();
                //var result = JObject.Parse(resultStr);


                RestClient client = new RestClient("http://120.78.210.159:3304/api/Wdt/GetWDTOrderNextSellLogList");
                RestRequest request = new RestRequest();
                request.AddHeader("Content-Type", "application/json");
                request.Method = Method.Post;
                request.AddParameter("application/json", str, ParameterType.RequestBody);
                request.AddBody(str);
                var response = client.Execute(request);
                var result = JObject.Parse(response.Content);


                if (result["code"].ToString() == "1")
                {
                    JArray resultList = JArray.Parse(result["list"].ToString());
                    var datas = resultList.Select(a => new
                    {
                        分销商名称 = a["FCustName"].ToString(),
                        订单编号 = a["FOrderNo"].ToString(),
                        订单日期 = a["FOrderDate"].ToString(),
                        店铺 = a["Shopname"].ToString(),
                        发货仓 = a["StockName"].ToString(),
                        订单状态 = a["FOrderStatus"].ToString() == "C" ? "已审核" : "未审核",
                        平台单号 = a["Srcbillno"].ToString(),
                        原始单号 = a["WDTSrcbillno"].ToString(),
                        收件人 = a["Recipienter"].ToString(),
                        电话 = a["Tel"].ToString(),
                        地址 = a["Address"].ToString(),
                        旺店通状态 = a["FStatus"].ToString(),
                        物流单号 = a["logistics_no"].ToString(),
                        发货时间 = a["Consign_time"],
                        商家编码 = a["Good_No"].ToString(),
                        发货数量 = a["Qty"].ToString(),
                        金额 = a["RecAmount"].ToString(),
                        拆分情况 = a["SplitOrderSstatus"] == null ? "" : a["SplitOrderSstatus"].ToString(),
                        同步旺店通结果 = a["ResultMess"].ToString(),
                        客服备注 = a["Remark"].ToString(),
                        订单ID = a["FID"].ToString(),
                        明细ID = a["ID"].ToString(),

                    }).OrderByDescending(a => a.明细ID).ToList();

                    if (dataGridView2ASC == false)
                    {
                        datas = resultList.Select(a => new
                        {
                            分销商名称 = a["FCustName"].ToString(),
                            订单编号 = a["FOrderNo"].ToString(),
                            订单日期 = a["FOrderDate"].ToString(),
                            店铺 = a["Shopname"].ToString(),
                            发货仓 = a["StockName"].ToString(),
                            订单状态 = a["FOrderStatus"].ToString() == "C" ? "已审核" : "未审核",
                            平台单号 = a["Srcbillno"].ToString(),
                            原始单号 = a["WDTSrcbillno"].ToString(),
                            收件人 = a["Recipienter"].ToString(),
                            电话 = a["Tel"].ToString(),
                            地址 = a["Address"].ToString(),
                            旺店通状态 = a["FStatus"].ToString(),
                            物流单号 = a["logistics_no"].ToString(),
                            发货时间 = a["Consign_time"],
                            商家编码 = a["Good_No"].ToString(),
                            发货数量 = a["Qty"].ToString(),
                            金额 = a["RecAmount"].ToString(),
                            拆分情况 = a["SplitOrderSstatus"] == null ? "" : a["SplitOrderSstatus"].ToString(),
                            同步旺店通结果 = a["ResultMess"].ToString(),
                            客服备注 = a["Remark"].ToString(),
                            订单ID = a["FID"].ToString(),
                            明细ID = a["ID"].ToString(),

                        }).OrderBy(a => a.明细ID).ToList();
                    }

                    this.Invoke(new Action(() =>
                    {
                        dataGridView2.DataSource = datas;
                        // uiRichTextBox1.AppendText(str);
                        //   uiDataGridView5.DataSource = datas;
                        //for (int i = 0; i <= uiDataGridView5.Columns.Count - 1; i++)
                        //{
                        //    uiDataGridView5.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

                        //}
                        // for (int i = 0; i <= uiDataGridView5.Columns.Count - 1; i++)
                        // {
                        //     //store autosized widths
                        //   //  int colw = uiDataGridView5.Columns[i].Width;
                        //     //remove autosizing
                        //   //  uiDataGridView5.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                        //     //set width to calculated by autosize
                        ////     uiDataGridView5.Columns[i].Width = colw;
                        // }

                    }));

                }
                else
                {
                    UIMessageBox.Show(result["msg"].ToString());
                }

            }
            catch (Exception ex)
            {
                UIMessageBox.Show("出现错误:" + ex.Message.ToString());

            }
            this.Invoke(new Action(() =>
            {
                uiButton15.Text = "查询";
                uiButton15.Enabled = true;

            }));
        }


        private void uiDataGridView1_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            Rectangle rectangle = new Rectangle(e.RowBounds.Location.X,
               e.RowBounds.Location.Y,
               uiDataGridView1.RowHeadersWidth,
               e.RowBounds.Height);

            TextRenderer.DrawText(e.Graphics, (e.RowIndex + 1).ToString(),
                uiDataGridView1.RowHeadersDefaultCellStyle.Font,
                rectangle,
                uiDataGridView1.RowHeadersDefaultCellStyle.ForeColor,
                TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
        }

        private void dataGridView2_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            Rectangle rectangle = new Rectangle(e.RowBounds.Location.X,
             e.RowBounds.Location.Y,
             (dataGridView2.RowHeadersWidth + 5),
             e.RowBounds.Height + 2);

            TextRenderer.DrawText(e.Graphics, (e.RowIndex + 1).ToString(),
                dataGridView2.RowHeadersDefaultCellStyle.Font,
                rectangle,
                dataGridView2.RowHeadersDefaultCellStyle.ForeColor,
                TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
        }

        private void dataGridView2_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                var list = JArray.FromObject(dataGridView2.DataSource).ToList();
                if (list != null)
                {
                    Task.Run(getOrderData);
                    dataGridView2ASC = !dataGridView2ASC;
                }

            }
        }

        private void uiButton15_Click(object sender, EventArgs e)
        {
            Task.Run(getOrderData);
        }

        private async void uiButton11_Click(object sender, EventArgs e)
        {
            if (dataGridView2.Rows.Count < 1)
            {
                UIMessageBox.Show("没有可以提交的数据!");
                return;
            }
            if (dataGridView2.CurrentRow.Index < 0)
            {
                UIMessageBox.Show("没有选择表格行!");
                return;
            }
            string orderNo = dataGridView2.Rows[dataGridView2.CurrentRow.Index].Cells["订单编号"].Value.ToString();
            string fOrderStatus = dataGridView2.Rows[dataGridView2.CurrentRow.Index].Cells["订单状态"].Value.ToString();
            if (orderNo.Length < 1)
            {
                UIMessageBox.Show("订单编号不能为空!");
                return;
            }
            if (fOrderStatus != "未审核")
            {
                UIMessageBox.Show("订单状态不符合要求!");
                return;
            }
            using (HttpClient client = new HttpClient())
            {

                var obj = new { forderno = orderNo, userid = Convert.ToInt32(wflogin.fuserId) };
                var str = JsonConvert.SerializeObject(obj);

                HttpContent content = new StringContent(str);
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                HttpResponseMessage response = await client.PostAsync("http://120.78.210.159:3304/Order/SubmitWDTOrderApi", content);
                //  response.EnsureSuccessStatusCode();//用来抛异常的
                string resultStr = await response.Content.ReadAsStringAsync();

                var result = JObject.Parse(resultStr);
                if (result.ContainsKey("code"))
                {
                    if (Convert.ToInt32(result["code"]) == 0)
                    {
                        this.Invoke(new Action(() =>
                        {
                            Task.Run(getOrderData);
                            UIMessageBox.Show("提交成功!");

                        }));
                    }
                    else
                    {
                        UIMessageBox.Show(result["msg"].ToString());
                    }

                }
            }
        }

        private void uiButton1_Click(object sender, EventArgs e)
        {

            if (dataGridView2.Rows.Count < 1)
            {
                UIMessageBox.Show("没有可以删除的数据!");
                return;
            }
            if (dataGridView2.CurrentRow.Index < 0)
            {
                UIMessageBox.Show("没有选择表格行!");
                return;
            }
            DialogResult YRO = MessageBox.Show("您确定要删除选中的记录吗？", "提醒您：", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
            if (YRO == DialogResult.No)
            {
                return;
            }
            string fentryid = dataGridView2.Rows[dataGridView2.CurrentRow.Index].Cells["明细ID"].Value.ToString();
            string fid = dataGridView2.Rows[dataGridView2.CurrentRow.Index].Cells["订单ID"].Value.ToString();
            string fOrderStatus = dataGridView2.Rows[dataGridView2.CurrentRow.Index].Cells["订单状态"].Value.ToString();
            if (fentryid.Length < 1)
            {
                UIMessageBox.Show("订单明细ID不能为空!");
                return;
            }
            if (fOrderStatus != "未审核")
            {
                UIMessageBox.Show("已审核订单不能被删除!");
                return;
            }
            RestClient client = new RestClient("http://120.78.210.159:3304/api/Wdt/DelWDTOrderItem");
            RestRequest request = new RestRequest();
            request.AddHeader("Content-Type", "application/json");
            request.Method = Method.Post;
            RestResponse response;
            // string fentryid,string fid,string userId
            var obj = new { fentryid = fentryid, fid = fid, userId = wflogin.fuserId.ToString() };
            var str = JsonConvert.SerializeObject(obj);

            request.AddBody(str);
            response = client.Execute(request);
            var result = JObject.Parse(response.Content);
            if (result["code"].ToString() == "1")
            {
                Task.Run(getOrderData);
                UIMessageBox.Show("删除成功!");
            }
        }

        private void uiButton10_Click(object sender, EventArgs e)
        {

            if (dataGridView2.Rows.Count < 1)
            {
                UIMessageBox.Show("没有可以删除的数据!");
                return;
            }
            if (dataGridView2.CurrentRow.Index < 0)
            {
                UIMessageBox.Show("没有选择表格行!");
                return;
            }
            DialogResult YRO = MessageBox.Show("您确定要删除选中的记录吗？", "提醒您：", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
            if (YRO == DialogResult.No)
            {
                return;
            }
            string fentryid = dataGridView2.Rows[dataGridView2.CurrentRow.Index].Cells["明细ID"].Value.ToString();
            string fid = dataGridView2.Rows[dataGridView2.CurrentRow.Index].Cells["订单ID"].Value.ToString();
            string fOrderStatus = dataGridView2.Rows[dataGridView2.CurrentRow.Index].Cells["订单状态"].Value.ToString();
            if (fid.Length < 1)
            {
                UIMessageBox.Show("订单ID不能为空!");
                return;
            }
            if (fOrderStatus != "未审核")
            {
                UIMessageBox.Show("已审核订单不能被删除!");
                return;
            }
            RestClient client = new RestClient("http://120.78.210.159:3304/api/Wdt/DelWDTOrder");
            RestRequest request = new RestRequest();
            request.AddHeader("Content-Type", "application/json; charset=utf-8");
            request.Method = Method.Post;
            RestResponse response;
            // string fentryid,string fid,string userId
            var obj = new { fid = fid, userId = wflogin.fuserId.ToString() };
            var str = JsonConvert.SerializeObject(obj);

            request.AddBody(str);
            response = client.Execute(request);
            var result = JObject.Parse(response.Content);
            if (result["code"].ToString() == "1")
            {
                Task.Run(getOrderData);
                UIMessageBox.Show("删除成功!");
            }
        }

        private void uiButton16_Click(object sender, EventArgs e)
        {
            if (dataGridView2.Rows.Count < 1)
            {
                UIMessageBox.Show("没有可以同步的数据!");
                return;
            }
            if (dataGridView2.CurrentRow.Index < 0)
            {
                UIMessageBox.Show("没有选择表格行!");
                return;
            }
            panel1.Visible = true;
            panel1.BringToFront();
            //uiPanel1.Show();
        }

        private void uiButton21_Click(object sender, EventArgs e)
        {
            RestClient client = new RestClient("http://120.78.210.159:3304/api/Wdt/CheckUpPwd");
            RestRequest request = new RestRequest();
            var obj = new { pwd = uiTextBox9.Text };
            request.AddHeader("Content-Type", "application/json");
            request.Method = Method.Post;
            RestResponse response;

            var str = JsonConvert.SerializeObject(obj);

            request.AddBody(str);
            response = client.Execute(request);
            var result = JObject.Parse(response.Content);
            if (result["code"].ToString() == "0")
            {
                UIMessageBox.Show("密码错误！");
                return;
            }

            this.Invoke(new Action(() =>
            {
                uiButton21.Enabled = false;
                uiButton21.Text = "上传中...";
            }));
            string billno = dataGridView2.Rows[dataGridView2.CurrentRow.Index].Cells["订单编号"].Value.ToString();
            client = new RestClient("http://120.78.210.159:3304/api/Wdt/SlekOrderUploadToWDT");
            request = new RestRequest();
            var obj1 = new { billno = billno };
            request.AddHeader("Content-Type", "application/json");
            request.Method = Method.Post;


            str = JsonConvert.SerializeObject(obj1);

            request.AddBody(str);
            response = client.Execute(request);
            result = JObject.Parse(response.Content);
            if (result["code"].ToString() == "1")
            {
                Task.Run(getOrderData);
                UIMessageBox.Show(result["mess"].ToString());
                this.Invoke(new Action(() =>
                {
                    uiButton21.Enabled = true;
                    uiButton21.Text = "同步";
                    panel1.Visible = false;
                    panel1.SendToBack();
                }));
            }
            else
            {
                UIMessageBox.Show(result["mess"].ToString());
                this.Invoke(new Action(() =>
                {
                    uiButton21.Enabled = true;
                    uiButton21.Text = "同步";
                }));
            }

        }

        private void uiButton22_Click(object sender, EventArgs e)
        {
            panel1.Visible = false;
            panel1.SendToBack();
        }

        private void uiButton14_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView2.Rows.Count < 1)
                {
                    UIMessageBox.Show("没有可以更新的数据!");
                    return;
                }
                if (dataGridView2.CurrentRow.Index < 0)
                {
                    UIMessageBox.Show("没有选择表格行!");
                    return;
                }
                string fOrderStatus = dataGridView2.Rows[dataGridView2.CurrentRow.Index].Cells["订单状态"].Value.ToString();
                if (fOrderStatus == "未审核")
                {
                    UIMessageBox.Show("订单数据没有上传旺店通,无法刷新状态!");
                    return;
                }
                string orderno = dataGridView2.Rows[dataGridView2.CurrentRow.Index].Cells["订单编号"].Value.ToString();
                var WDTSrcbillnos = JArray.FromObject(dataGridView2.DataSource).Where(a => a["订单编号"].ToString() == orderno && a["原始单号"] != null && a["原始单号"].ToString() != "").Select(a => a["原始单号"].ToString()).Distinct().ToList();
                //  string WDTSrcbillno = uiDataGridView5.Rows[uiDataGridView5.CurrentRow.Index].Cells["原始单号"].Value.ToString();
                if (WDTSrcbillnos.Count < 1)
                {
                    UIMessageBox.Show("没有原始单号,系统无法更新状态!");
                    return;
                }
                this.Invoke(new Action(() =>
                {
                    uiButton14.Enabled = false;
                    uiButton14.Text = "查询状态...";
                }));
                int uprec = 0;
                RestClient client = new RestClient("http://120.78.210.159:3304/api/Wdt/upWdtOrderStatus");
                RestRequest request = new RestRequest();
                request.AddHeader("Content-Type", "application/json");
                request.Method = Method.Post;
                RestResponse response;
                WDTSrcbillnos.ForEach(d =>
                {

                    client = new RestClient("http://120.78.210.159:3304/api/Wdt/upWdtOrderStatus");
                    request = new RestRequest();
                    request.AddHeader("Content-Type", "application/json");
                    request.Method = Method.Post;
                    var str = JsonConvert.SerializeObject(new { wdtSrcBillNo = d });

                    request.AddBody(str);
                    response = client.Execute(request);
                    var result = JObject.Parse(response.Content);
                    if (result["code"].ToString() == "0" && result["mess"].ToString().Contains("更新成功"))
                    {
                        uprec++;

                    }

                });
                if (uprec > 0)
                {
                    Task.Run(getOrderData);
                    UIMessageBox.Show("总共更新" + uprec.ToString() + "条原始订单记录!");
                }
                else
                {
                    UIMessageBox.Show("没有可更新的记录!");
                }
                this.Invoke(new Action(() =>
                {
                    uiButton14.Enabled = true;
                    uiButton14.Text = "更新状态";
                }));
            }
            catch (Exception ex)
            {
                this.Invoke(new Action(() =>
                {
                    uiButton14.Enabled = true;
                    uiButton14.Text = "更新状态";
                }));

            }
        }

        private void uiButton26_Click(object sender, EventArgs e)
        {
            var datas = JArray.FromObject(dataGridView2.DataSource);
            if (datas.Count < 1)
            {
                UIMessageBox.Show("没有明细数据!");
                return;
            }
            else
            {

                this.Invoke(new Action(() =>
                {
                    uiButton26.Text = "校验中...";
                    uiButton26.Enabled = false;
                }));

                try
                {

                    List<string> listAddress = new List<string>();
                    datas.ForEach(a =>
                    {
                        string WDTBILLNO = a["原始单号"] == null ? "" : a["原始单号"].ToString();
                        if (WDTBILLNO == "")
                        {
                            string FENTRYID = a["明细ID"] == null ? "0" : a["明细ID"].ToString();

                            RestClient client = new RestClient("http://120.78.210.159:3304/api/Wdt/getAddress");
                            RestRequest request = new RestRequest();
                            request.AddHeader("Content-Type", "application/json");
                            request.Method = Method.Post;
                            RestResponse response;

                            var obj = new { fentryid = FENTRYID };
                            var str = JsonConvert.SerializeObject(obj);

                            request.AddBody(str);
                            response = client.Execute(request);
                            var result = JObject.Parse(response.Content);
                            if (result["code"].ToString() == "1")
                            {
                                string address = result["Address"].ToString();
                                listAddress.Add(address);

                            }

                        }

                    });

                    if (listAddress.Count > 0)
                    {
                        var resultadd = listAddress.Distinct().ToList();
                        resultadd.ForEach(j =>
                        {
                            string address = j;
                            string newAddress;
                            using (HttpClient hclient = new HttpClient())
                            {
                                string url = "https://aip.baidubce.com/oauth/2.0/token?grant_type=client_credentials&client_id=2zXXDULe8nqpLsUHQAIeks23&client_secret=pZyzzbZsRv8Gl2i3BMwy7BjdtdQ6VNmG";
                                var hresponse = hclient.GetAsync(url);
                                var stradd = hresponse.Result.Content.ReadAsStringAsync();
                                string token;

                                if (stradd.Result.Contains("access_token"))
                                {
                                    JObject Obj = JObject.Parse(stradd.Result);
                                    token = Obj["access_token"].ToString();
                                    if (address != "" && address != null)
                                    {
                                        url = "https://aip.baidubce.com/rpc/2.0/nlp/v1/address?access_token=" + token + "&text=" + address;
                                        hresponse = hclient.GetAsync(url);
                                        Obj = JObject.Parse(hresponse.Result.Content.ReadAsStringAsync().Result);
                                        newAddress = Obj["province"].ToString() + " " + Obj["city"].ToString() + " " + Obj["county"].ToString() + " " + Obj["town"].ToString() + " " + Obj["detail"].ToString();


                                        RestClient client = new RestClient("http://120.78.210.159:3304/api/Wdt/UpAddress");
                                        RestRequest request = new RestRequest();
                                        request.AddHeader("Content-Type", "application/json");
                                        request.Method = Method.Post;
                                        var objup = new { address = newAddress, oldadd = address };
                                        var str = JsonConvert.SerializeObject(objup);
                                        request.AddBody(str);
                                        RestResponse response = client.Execute(request);
                                        var upresult = JObject.Parse(response.Content);
                                    }

                                };

                            }

                        });

                    }

                    this.Invoke(new Action(() =>
                    {
                        uiButton26.Text = "地址校验";
                        uiButton26.Enabled = true;

                    }));

                }
                catch (Exception ex)
                {

                    UIMessageBox.Show(ex.ToString());
                    this.Invoke(new Action(() =>
                    {
                        uiButton26.Text = "地址校验";
                        uiButton26.Enabled = true;

                    }));
                }
            }
        }

        private void uiButton13_Click(object sender, EventArgs e)
        {
            if (dataGridView2.Rows.Count < 1)
            {
                UIMessageBox.Show("没有可以取消的数据!");
                return;
            }
            if (dataGridView2.CurrentRow.Index < 0)
            {
                UIMessageBox.Show("没有选择表格行!");
                return;
            }
            DialogResult YRO = MessageBox.Show("您确定要取消选中的记录吗？", "提醒您：", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
            if (YRO == DialogResult.No)
            {
                return;
            }
            string fentryid = dataGridView2.Rows[dataGridView2.CurrentRow.Index].Cells["明细ID"].Value.ToString();
            string fid = dataGridView2.Rows[dataGridView2.CurrentRow.Index].Cells["订单ID"].Value.ToString();
            string fOrderStatus = dataGridView2.Rows[dataGridView2.CurrentRow.Index].Cells["订单状态"].Value.ToString();
            string fWdtOrderStatus = dataGridView2.Rows[dataGridView2.CurrentRow.Index].Cells["旺店通状态"].Value.ToString();
            string shopName = dataGridView2.Rows[dataGridView2.CurrentRow.Index].Cells["店铺"].Value.ToString();
            string WDTSrcbillno = dataGridView2.Rows[dataGridView2.CurrentRow.Index].Cells["原始单号"].Value.ToString();

            string[] status = new string[] { "已取消", "已发货", "已完成", "异常发货" };
            if (fid.Length < 1)
            {
                UIMessageBox.Show("订单ID不能为空!");
                return;
            }
            if (fOrderStatus == "未审核")
            {
                UIMessageBox.Show("未审核订单不用取消，可直接删除!");
                return;
            }
            if (status.Contains(fWdtOrderStatus))
            {
                UIMessageBox.Show("旺店通状态不允许取消!");
                return;
            }
            RestClient client = new RestClient("http://120.78.210.159:3304/api/Wdt/WDTTradeOrderStatusChange");
            RestRequest request = new RestRequest();
            request.AddHeader("Content-Type", "application/json");
            request.Method = Method.Post;
            RestResponse response;
            //   string shopname, string wdtsrcbillno, string statusid
            var obj = new { shopname = shopName, wdtsrcbillno = WDTSrcbillno, statusid = 90, fid = fid, fentryid = fentryid };
            var str = JsonConvert.SerializeObject(obj);

            request.AddBody(str);
            response = client.Execute(request);
            var result = JObject.Parse(response.Content);
            if (result["code"].ToString() == "1")
            {
                Task.Run(getOrderData);
                UIMessageBox.Show("取消成功!");
            }
            else
            {
                UIMessageBox.Show("取消失败!");
            }
        }

        private void uiButton20_Click(object sender, EventArgs e)
        {
            if (dataGridView2.Rows.Count < 1)
            {
                UIMessageBox.Show("没有可以导出的数据!");
                return;
            }
            saveFileDialog2.Filter = "Excel 文件(*.xls)| *.xls";
            saveFileDialog2.ShowDialog();
        }

        private void saveFileDialog2_FileOk(object sender, CancelEventArgs e)
        {
            try
            {
                var datas = JArray.FromObject(dataGridView2.DataSource);

                if (saveFileDialog2.FileName != "")
                {
                    string tfilepath = saveFileDialog2.FileName;

                    HSSFWorkbook wb;
                    FileStream file;

                    MemoryStream ms = new MemoryStream();
                    wb = new HSSFWorkbook();

                    ISheet sheet;
                    sheet = wb.CreateSheet("sheet1");
                    ICellStyle cellStyle = wb.CreateCellStyle();

                    NPOI.SS.UserModel.IRow rowTemp;
                    rowTemp = sheet.CreateRow(0);//创建第一行
                    int c = 0;
                    JObject header = (JObject)datas.First();
                    rowTemp = sheet.CreateRow(0);
                    sheet.SetColumnWidth(0, 15 * 256);
                    sheet.SetColumnWidth(1, 20 * 256);
                    sheet.SetColumnWidth(2, 12 * 256);
                    sheet.SetColumnWidth(3, 12 * 256);
                    sheet.SetColumnWidth(4, 12 * 256);
                    sheet.SetColumnWidth(5, 12 * 256);
                    sheet.SetColumnWidth(6, 27 * 256);
                    sheet.SetColumnWidth(7, 27 * 256);
                    sheet.SetColumnWidth(8, 8 * 256);
                    sheet.SetColumnWidth(9, 12 * 256);
                    sheet.SetColumnWidth(10, 10 * 256);
                    sheet.SetColumnWidth(11, 20 * 256);
                    sheet.SetColumnWidth(12, 20 * 256);
                    sheet.SetColumnWidth(13, 15 * 256);
                    sheet.SetColumnWidth(14, 8 * 256);
                    sheet.SetColumnWidth(15, 55 * 256);
                    foreach (JProperty prop in header.Properties())
                    {
                        if (prop.Name != "订单ID" && prop.Name != "明细ID")
                        {
                            rowTemp.CreateCell(c).SetCellValue(prop.Name.ToString());
                            c++;
                        }

                    }
                    int j = 1;
                    datas.ForEach(r =>
                    {
                        rowTemp = sheet.CreateRow(j);
                        JObject dataRow = (JObject)r;
                        c = 0;
                        foreach (JProperty prop in dataRow.Properties())
                        {

                            if (prop.Name != "订单ID" && prop.Name != "明细ID")
                            {
                                rowTemp.CreateCell(c).SetCellValue(prop.Value.ToString());
                                c++;
                            }
                        }
                        j++;
                    });

                    wb.Write(ms);//将Excel写入流
                    ms.Flush();
                    ms.Position = 0;

                    FileStream dumpFile = new FileStream(tfilepath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
                    ms.WriteTo(dumpFile);//将流写入文件
                    dumpFile.Close();
                    this.Invoke(new Action(() =>
                    {
                        UIMessageBox.Show("数据导出完成!");

                    }));

                }
            }
            catch (Exception er)
            {
                UIMessageBox.Show("出现错误:" + er.Message);
            }
        }

        private void uiButton5_Click(object sender, EventArgs e)
        {
            Task.Run(getOutStockData);
        }

        private void getOutStockData()
        {
            try
            {
                //getOutStockInfo
                this.Invoke(new Action(() =>
                {

                    uiButton5.Text = "查询中...";
                    uiButton5.Enabled = false;

                    //uirichtxt.Clear();
                    //uirichtxt.AppendText(DateTime.Now.ToShortTimeString() + ":开始统计...\r\n");
                }));
                RestClient client = new RestClient("http://120.78.210.159:3304/api/Wdt/getNextSellOutStockInfo");
                RestRequest request = new RestRequest();
                request.AddHeader("Content-Type", "application/json");
                request.Method = Method.Post;
                RestResponse response;

                var obj = new
                {
                    userid = wflogin.fuserId.Trim(),
                    fcustId = wflogin.fcustId.Trim(),
                    custName = uiTextBox4.Text,
                    sdate = Convert.ToDateTime(Convert.ToDateTime(ssdt.Value).ToString("yyyy-MM-dd") + " 00:00:00"),
                    edate = Convert.ToDateTime(Convert.ToDateTime(sedt.Value).ToString("yyyy-MM-dd") + " 23:59:59"),
                    src_tid = uiTextBox3.Text,
                    receiver_name = uiTextBox2.Text,
                    receiver_mobile = uiTextBox1.Text
                };
                var str = JsonConvert.SerializeObject(obj);

                request.AddBody(str);
                response = client.Execute(request);
                var result = JObject.Parse(response.Content);
                if (result["code"].ToString() == "1")
                {
                    this.Invoke(new Action(() =>
                    {

                        dataGridView1.DataSource = JArray.Parse(result["sdataList"].ToString());

                    }));

                    UIMessageBox.Show(result["mess"].ToString());
                }
                else
                {
                    UIMessageBox.Show(result["mess"].ToString());
                }
                this.Invoke(new Action(() =>
                {

                    uiButton5.Text = "查询";
                    uiButton5.Enabled = true;
                    //  uirichtxt.AppendText(DateTime.Now.ToShortTimeString() + ":统计结束\r\n");
                }));
            }
            catch (Exception ex)
            {
                this.Invoke(new Action(() =>
                {

                    uiButton5.Text = "查询";
                    uiButton5.Enabled = true;
                    //  uirichtxt.AppendText(DateTime.Now.ToShortTimeString() + ":统计结束\r\n");
                }));

            }
        }

        private void uiButton3_Click(object sender, EventArgs e)
        {
            Task.Run(getReturnStockData);
        }

        private void getReturnStockData()
        {
            this.Invoke(new Action(() =>
            {

                uiButton3.Text = "查询中...";
                uiButton3.Enabled = false;


            }));
            RestClient client = new RestClient("http://120.78.210.159:3304/api/Wdt/getNextSellReStockInfo");
            RestRequest request = new RestRequest();
            request.AddHeader("Content-Type", "application/json");
            request.Method = Method.Post;
            RestResponse response;

            var obj = new
            {
                userid = wflogin.fuserId.Trim(),
                fcustId = wflogin.fcustId.Trim(),
                custName = uiTextBox8.Text,
                sdate = Convert.ToDateTime(Convert.ToDateTime(rsdt.Value).ToString("yyyy - MM - dd") + " 00:00:00"),
                edate = Convert.ToDateTime(Convert.ToDateTime(redt.Value).ToString("yyyy - MM - dd") + " 23:59:59"),
                src_tid = uiTextBox7.Text,
                receiver_name = uiTextBox6.Text,
                receiver_mobile = uiTextBox5.Text
            };
            var str = JsonConvert.SerializeObject(obj);

            request.AddBody(str);
            response = client.Execute(request);
            var result = JObject.Parse(response.Content);
            if (result["code"].ToString() == "1")
            {
                this.Invoke(new Action(() =>
                {

                    dataGridView3.DataSource = JArray.Parse(result["rdataList"].ToString());

                }));

                UIMessageBox.Show(result["mess"].ToString());
            }
            else
            {
                UIMessageBox.Show(result["mess"].ToString());
            }
            this.Invoke(new Action(() =>
            {

                uiButton3.Text = "查询";
                uiButton3.Enabled = true;
                //  uirichtxt.AppendText(DateTime.Now.ToShortTimeString() + ":统计结束\r\n");
            }));
        }

        private void uiButton28_Click(object sender, EventArgs e)
        {
            saveFileDialog4.Filter = "Excel 文件(*.xls)| *.xls";
            saveFileDialog4.ShowDialog();
        }

        private void saveFileDialog4_FileOk(object sender, CancelEventArgs e)
        {
            try
            {
                var datas = JArray.FromObject(dataGridView3.DataSource);

                if (saveFileDialog4.FileName != "")
                {
                    string tfilepath = saveFileDialog4.FileName;

                    HSSFWorkbook wb;
                    FileStream file;

                    MemoryStream ms = new MemoryStream();
                    wb = new HSSFWorkbook();

                    ISheet sheet;
                    sheet = wb.CreateSheet("sheet1");
                    ICellStyle cellStyle = wb.CreateCellStyle();

                    NPOI.SS.UserModel.IRow rowTemp;
                    rowTemp = sheet.CreateRow(0);//创建第一行
                    int c = 0;
                    JObject header = (JObject)datas.First();
                    rowTemp = sheet.CreateRow(0);
                    sheet.SetColumnWidth(0, 30 * 256);
                    sheet.SetColumnWidth(1, 30 * 256);
                    sheet.SetColumnWidth(2, 12 * 256);
                    sheet.SetColumnWidth(3, 12 * 256);

                    foreach (JProperty prop in header.Properties())
                    {
                        if (prop.Name != "订单ID" && prop.Name != "明细ID")
                        {
                            rowTemp.CreateCell(c).SetCellValue(prop.Name.ToString());
                            c++;
                        }

                    }
                    int j = 1;
                    datas.ForEach(r =>
                    {
                        rowTemp = sheet.CreateRow(j);
                        JObject dataRow = (JObject)r;
                        c = 0;
                        foreach (JProperty prop in dataRow.Properties())
                        {

                            if (prop.Name != "订单ID" && prop.Name != "明细ID")
                            {
                                rowTemp.CreateCell(c).SetCellValue(prop.Value.ToString());
                                c++;
                            }
                        }
                        j++;
                    });

                    wb.Write(ms);//将Excel写入流
                    ms.Flush();
                    ms.Position = 0;

                    FileStream dumpFile = new FileStream(tfilepath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
                    ms.WriteTo(dumpFile);//将流写入文件
                    dumpFile.Close();
                    this.Invoke(new Action(() =>
                    {
                        UIMessageBox.Show("数据导出完成!");

                    }));

                }
            }
            catch (Exception er)
            {
                UIMessageBox.Show("出现错误:" + er.Message);
            }
        }

        private void dataGridView3_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            Rectangle rectangle = new Rectangle(e.RowBounds.Location.X,
              e.RowBounds.Location.Y,
              (dataGridView3.RowHeadersWidth + 5),
              e.RowBounds.Height);

            TextRenderer.DrawText(e.Graphics, (e.RowIndex + 1).ToString(),
                dataGridView3.RowHeadersDefaultCellStyle.Font,
                rectangle,
                dataGridView3.RowHeadersDefaultCellStyle.ForeColor,
                TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
        }

        private void uiButton4_Click(object sender, EventArgs e)
        {
            Task.Run(getCustBillsforApi);
        }

        public void getCustBillsforApi()
        {
            try
            {

                this.Invoke(new Action(() =>
                {

                    uiButton4.Text = "统计中...";
                    uiButton4.Enabled = false;

                    uirichtxt.Clear();
                    uirichtxt.AppendText(DateTime.Now.ToShortTimeString() + ":开始统计...\r\n");
                }));
                RestClient client = new RestClient("http://120.78.210.159:3304/api/Wdt/getNextSellCustBills");
                RestRequest request = new RestRequest();
                request.AddHeader("Content-Type", "application/json");
                request.Method = Method.Post;
                RestResponse response;

                var obj = new { userid = wflogin.fuserId.Trim(), sdate = Convert.ToDateTime(dzsdt.Value.ToShortDateString()), edate = Convert.ToDateTime(dzedt.Value.ToShortDateString()) };
                var str = JsonConvert.SerializeObject(obj);

                request.AddBody(str);
                response = client.Execute(request);
                var result = JObject.Parse(response.Content);
                if (result["code"].ToString() == "1")
                {
                    this.Invoke(new Action(() =>
                    {

                        dataGridView4.DataSource = JArray.Parse(result["dataList"].ToString());
                    }));

                    UIMessageBox.Show(result["mess"].ToString());
                }
                else
                {
                    UIMessageBox.Show(result["mess"].ToString());
                }
                this.Invoke(new Action(() =>
                {

                    uiButton4.Text = "统计";
                    uiButton4.Enabled = true;
                    uirichtxt.AppendText(DateTime.Now.ToShortTimeString() + ":统计结束\r\n");
                }));
            }
            catch (Exception ex)
            {

                this.Invoke(new Action(() =>
                {

                    uiButton4.Text = "统计";
                    uiButton4.Enabled = true;
                }));
            }
        }

        private void dataGridView4_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            Rectangle rectangle = new Rectangle(e.RowBounds.Location.X,
               e.RowBounds.Location.Y,
               (dataGridView4.RowHeadersWidth + 5),
               e.RowBounds.Height);

            TextRenderer.DrawText(e.Graphics, (e.RowIndex + 1).ToString(),
                dataGridView4.RowHeadersDefaultCellStyle.Font,
                rectangle,
                dataGridView4.RowHeadersDefaultCellStyle.ForeColor,
                TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
        }

        private void uiButton6_Click(object sender, EventArgs e)
        {
            if (dataGridView4.CurrentRow.Index < 0)
            {
                UIMessageBox.Show("没有选择表格行!");
                return;
            }
            Task.Run(getBillsDetailforApi);
        }

        private void getBillsDetailforApi()
        {
            try
            {

                DateTime fdate = Convert.ToDateTime(dataGridView4.Rows[dataGridView4.CurrentRow.Index].Cells["订单日期"].Value);
                int custId = Convert.ToInt32(dataGridView4.Rows[dataGridView4.CurrentRow.Index].Cells["分销商ID"].Value);
                this.Invoke(new Action(() =>
                {

                    uiButton6.Text = "统计中...";
                    uiButton6.Enabled = false;

                    uirichtxt.Clear();
                    uirichtxt.AppendText(DateTime.Now.ToShortTimeString() + ":开始统计明细...\r\n");
                }));
                RestClient client = new RestClient("http://120.78.210.159:3304/api/Wdt/getNextSellBillDetail");
                RestRequest request = new RestRequest();
                request.AddHeader("Content-Type", "application/json");
                request.Method = Method.Post;
                RestResponse response;

                var obj = new { fdate = fdate, custId = custId, userid = wflogin.fuserId.Trim() };
                var str = JsonConvert.SerializeObject(obj);

                request.AddBody(str);
                response = client.Execute(request);
                var result = JObject.Parse(response.Content);
                if (result["code"].ToString() == "1")
                {
                    this.Invoke(new Action(() =>
                    {
                        dataGridView5.DataSource = JArray.Parse(result["odataList"].ToString());
                        dataGridView6.DataSource = JArray.Parse(result["sdataList"].ToString());
                        dataGridView7.DataSource = JArray.Parse(result["rdataList"].ToString());

                    }));

                    UIMessageBox.Show(result["mess"].ToString());
                }
                else
                {
                    UIMessageBox.Show(result["mess"].ToString());
                }
                this.Invoke(new Action(() =>
                {

                    uiButton6.Text = "明细";
                    uiButton6.Enabled = true;
                    uirichtxt.AppendText(DateTime.Now.ToShortTimeString() + ":统计明细结束\r\n");
                }));
            }
            catch (Exception ex)
            {

                this.Invoke(new Action(() =>
                {
                    uiButton6.Text = "明细";
                    uiButton6.Enabled = true;
                }));
            }

        }

        private void uiButton25_Click(object sender, EventArgs e)
        {
            saveFileDialog3.Filter = "Excel 文件(*.xls)| *.xls";
            saveFileDialog3.ShowDialog();
        }

        private void saveFileDialog3_FileOk(object sender, CancelEventArgs e)
        {
            try
            {
                var datas = JArray.FromObject(dataGridView5.DataSource);
                if (datas.Count < 1)
                {
                    UIMessageBox.Show("没有明细数据!");
                    return;
                }

                if (saveFileDialog3.FileName != "")
                {

                    string tfilepath = saveFileDialog3.FileName;

                    HSSFWorkbook wb;
                    FileStream file;

                    MemoryStream ms = new MemoryStream();
                    wb = new HSSFWorkbook();

                    ISheet sheet;
                    sheet = wb.CreateSheet("订货明细");
                    ICellStyle cellStyle = wb.CreateCellStyle();

                    NPOI.SS.UserModel.IRow rowTemp;
                    rowTemp = sheet.CreateRow(0);//创建第一行
                    int c = 0;
                    JObject header = (JObject)datas.First();
                    rowTemp = sheet.CreateRow(0);
                    sheet.SetColumnWidth(0, 15 * 256);
                    sheet.SetColumnWidth(1, 20 * 256);
                    sheet.SetColumnWidth(2, 12 * 256);
                    sheet.SetColumnWidth(3, 12 * 256);
                    sheet.SetColumnWidth(4, 12 * 256);
                    sheet.SetColumnWidth(5, 12 * 256);
                    sheet.SetColumnWidth(6, 27 * 256);
                    sheet.SetColumnWidth(7, 15 * 256);
                    sheet.SetColumnWidth(8, 8 * 256);
                    sheet.SetColumnWidth(9, 8 * 256);
                    sheet.SetColumnWidth(10, 8 * 256);
                    sheet.SetColumnWidth(11, 8 * 256);
                    sheet.SetColumnWidth(12, 8 * 256);
                    sheet.SetColumnWidth(13, 8 * 256);
                    sheet.SetColumnWidth(14, 8 * 256);
                    sheet.SetColumnWidth(15, 8 * 256);
                    sheet.SetColumnWidth(16, 8 * 256);
                    sheet.SetColumnWidth(17, 8 * 256);
                    foreach (JProperty prop in header.Properties())
                    {
                        if (prop.Name != "订单ID" && prop.Name != "明细ID")
                        {
                            rowTemp.CreateCell(c).SetCellValue(prop.Name.ToString());
                            c++;
                        }

                    }
                    int j = 1;
                    datas.ForEach(r =>
                    {
                        rowTemp = sheet.CreateRow(j);
                        JObject dataRow = (JObject)r;
                        c = 0;
                        foreach (JProperty prop in dataRow.Properties())
                        {

                            if (prop.Name != "订单ID" && prop.Name != "明细ID")
                            {
                                rowTemp.CreateCell(c).SetCellValue(prop.Value.ToString());
                                c++;
                            }
                        }
                        j++;
                    });

                    datas = JArray.FromObject(dataGridView6.DataSource);
                    if (datas.Count > 0)
                    {

                        sheet = wb.CreateSheet("发货明细");
                        cellStyle = wb.CreateCellStyle();


                        rowTemp = sheet.CreateRow(0);//创建第一行
                        c = 0;
                        header = (JObject)datas.First();
                        rowTemp = sheet.CreateRow(0);
                        sheet.SetColumnWidth(0, 12 * 256);
                        sheet.SetColumnWidth(1, 10 * 256);
                        sheet.SetColumnWidth(2, 8 * 256);
                        sheet.SetColumnWidth(3, 8 * 256);
                        sheet.SetColumnWidth(4, 8 * 256);

                        foreach (JProperty prop in header.Properties())
                        {
                            if (prop.Name != "订单ID" && prop.Name != "明细ID")
                            {
                                rowTemp.CreateCell(c).SetCellValue(prop.Name.ToString());
                                c++;
                            }

                        }
                        j = 1;
                        datas.ForEach(r =>
                        {
                            rowTemp = sheet.CreateRow(j);
                            JObject dataRow = (JObject)r;
                            c = 0;
                            foreach (JProperty prop in dataRow.Properties())
                            {

                                if (prop.Name != "订单ID" && prop.Name != "明细ID")
                                {
                                    rowTemp.CreateCell(c).SetCellValue(prop.Value.ToString());
                                    c++;
                                }
                            }
                            j++;
                        });

                    }
                    datas = JArray.FromObject(dataGridView7.DataSource);
                    if (datas.Count > 0)
                    {
                        sheet = wb.CreateSheet("退货明细");
                        cellStyle = wb.CreateCellStyle();


                        rowTemp = sheet.CreateRow(0);//创建第一行
                        c = 0;
                        header = (JObject)datas.First();
                        rowTemp = sheet.CreateRow(0);
                        sheet.SetColumnWidth(0, 12 * 256);
                        sheet.SetColumnWidth(1, 10 * 256);
                        sheet.SetColumnWidth(2, 8 * 256);
                        sheet.SetColumnWidth(3, 8 * 256);


                        foreach (JProperty prop in header.Properties())
                        {
                            if (prop.Name != "订单ID" && prop.Name != "明细ID")
                            {
                                rowTemp.CreateCell(c).SetCellValue(prop.Name.ToString());
                                c++;
                            }

                        }
                        j = 1;
                        datas.ForEach(r =>
                        {
                            rowTemp = sheet.CreateRow(j);
                            JObject dataRow = (JObject)r;
                            c = 0;
                            foreach (JProperty prop in dataRow.Properties())
                            {

                                if (prop.Name != "订单ID" && prop.Name != "明细ID")
                                {
                                    rowTemp.CreateCell(c).SetCellValue(prop.Value.ToString());
                                    c++;
                                }
                            }
                            j++;
                        });
                    }
                    wb.Write(ms);//将Excel写入流
                    ms.Flush();
                    ms.Position = 0;

                    FileStream dumpFile = new FileStream(tfilepath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
                    ms.WriteTo(dumpFile);//将流写入文件
                    dumpFile.Close();
                    this.Invoke(new Action(() =>
                    {
                        UIMessageBox.Show("数据导出完成!");

                    }));

                }
            }
            catch (Exception er)
            {
                UIMessageBox.Show("出现错误:" + er.Message);
            }
        }

        private void uiButton12_Click(object sender, EventArgs e)
        {
            try
            {
                Dictionary<string, string> dt = new Dictionary<string, string>();

                // dt.Add("logistics_no", "");
                var resultobj = JObject.Parse(GetWdtDataBLL.GetDataFromApi(dt, "logistics.php"));
                if (resultobj["code"].ToString() == "0")

                {
                    uirichtxt.AppendText(JsonConvert.SerializeObject(resultobj["logistics_list"]));
                    var logistics_type = JArray.Parse(JsonConvert.SerializeObject(resultobj["logistics_list"])).Where(a => a["is_disabled"].ToString() == "0" && a["logistics_type"].ToString() != "0").Select(a =>
                          new
                          {
                              物流名称 = a["logistics_name"],
                              物流类型名称 = getLogisticsName(Convert.ToInt32(a["logistics_type"])),
                              物流类型ID = Convert.ToInt32(a["logistics_type"]),
                              物流编号 = a["logistics_no"]
                          }).ToList();


                    this.Invoke(new Action(() =>
                    {
                        dataGridView8.DataSource = logistics_type;
                    }));

                }
                else
                {
                    this.Invoke(new Action(() =>
                    {
                        uiRichTextBox2.AppendText("查询失败!" + "\r\n");
                    }));

                }
            }
            catch (Exception er)
            {
                UIMessageBox.Show(er.Message);
            }
        }

        private void uiButton18_Click(object sender, EventArgs e)
        {
            Dictionary<string, string> dt = new Dictionary<string, string>();
            dt = new Dictionary<string, string>();
            dt.Add("page_size", 100.ToString());
            var resultobj = JObject.Parse(GetWdtDataBLL.GetDataFromApi(dt, "warehouse_query.php"));
            if (resultobj["code"].ToString() == "0")
            {
                var warehouses = JArray.Parse(JsonConvert.SerializeObject(resultobj["warehouses"])).Where(a => a["is_disabled"].ToString() == "0").Select(a =>
                       new
                       {
                           仓库编号 = a["warehouse_no"],
                           仓库名称 = a["name"],
                           仓库地址 = a["address"],
                           仓库联系人 = a["contact"],
                           联系人手机 = a["mobile"]
                       }).ToList();
                this.Invoke(new Action(() =>
                {
                    dataGridView8.DataSource = warehouses;
                }));

            }
            else
            {
                this.Invoke(new Action(() =>
                {
                    uiRichTextBox2.AppendText("查询失败!" + "\r\n");
                }));

            }
        }

        private void uiButton19_Click(object sender, EventArgs e)
        {
            Dictionary<string, string> dt = new Dictionary<string, string>();
            dt = new Dictionary<string, string>();
            dt.Add("page_size", 100.ToString());
            var resultobj = JObject.Parse(GetWdtDataBLL.GetDataFromApi(dt, "shop.php"));
            if (resultobj["code"].ToString() == "0")
            {
                var shoplist = JArray.Parse(JsonConvert.SerializeObject(resultobj["shoplist"])).Where(a => a["is_disabled"].ToString() == "0").Select(a =>
                       new
                       {
                           店铺编号 = a["shop_no"],
                           店铺名称 = a["shop_name"]

                       }).ToList();
                this.Invoke(new Action(() =>
                {
                    dataGridView8.DataSource = shoplist;
                }));

            }
            else
            {
                this.Invoke(new Action(() =>
                {
                    uiRichTextBox2.AppendText("查询失败!" + "\r\n");
                }));

            }
        }

        private void uiButton24_Click(object sender, EventArgs e)
        {
            RestClient client = new RestClient("http://120.78.210.159:3304/api/Wdt/GetCustWDTPriceNextSellList");
            RestRequest request = new RestRequest();
            request.AddHeader("Content-Type", "application/json; charset=utf-8");
            request.Method = Method.Post;
            RestResponse response;
            var obj = new { custName = uitxtdzCustName.Text };

            request.AddBody(JsonConvert.SerializeObject(obj));
            response = client.Execute(request);
            var result = JObject.Parse(response.Content);
            if (result["code"].ToString() == "0")
            {
                this.Invoke(new Action(() =>
                {

                    dataGridView8.DataSource = JArray.Parse(result["list"].ToString());
                }));

                UIMessageBox.Show("查询完成!");
            }
            else
            {
                UIMessageBox.Show("查询失败!");
            }
        }

        private void uiButton9_Click(object sender, EventArgs e)
        {
            try
            {
                

                string oldPwd = uiTxtoldpwd.Text.Trim();
                string newPwd = uiTxtnewpwd.Text.Trim();
                string cirmpwd = uiTxtcirmpwd.Text.Trim();


                if (oldPwd== newPwd)
                {
                    UIMessageBox.Show("原密码与新密码一致!");
                    return;
                }

                if (string.IsNullOrEmpty(newPwd))
                {
                    UIMessageBox.Show("新密码不能为空!");
                    return;
                }

                if (newPwd != cirmpwd)
                {
                    UIMessageBox.Show("新密码与确认密码不一致!");
                    return;
                }
                this.Invoke(new Action(() =>
                {
                    uiButton9.Enabled = false;
                    uiButton9.Text = "更新中...";
                }));

                RestClient client = new RestClient("http://120.78.210.159:3304/api/Wdt/UserUpPwd");
                RestRequest request = new RestRequest();
                request.AddHeader("Content-Type", "application/json; charset=utf-8");
                request.Method = Method.Post;
                RestResponse response;
                var obj = new { oldPwd = oldPwd, newPwd = newPwd, userId = wflogin.fuserId };

                request.AddBody(JsonConvert.SerializeObject(obj));
                response = client.Execute(request);
                var result = JObject.Parse(response.Content);

                UIMessageBox.Show(result["mess"].ToString());

                this.Invoke(new Action(() =>
                {
                    uiButton9.Enabled = true;
                    uiButton9.Text = "更新密码";
                }));
            }
            catch (Exception ex)
            {
                UIMessageBox.Show("密码修改失败!");
                this.Invoke(new Action(() =>
                {
                    uiButton9.Enabled = true;
                    uiButton9.Text = "更新密码";
                }));
            }

            uiTxtoldpwd.Text="";
            uiTxtnewpwd.Text="";
            uiTxtcirmpwd.Text="";


        }
    }
}
