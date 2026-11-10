using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NPOI.HSSF.UserModel;
using NPOI.SS.Formula;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.Util;
using NPOI.XSSF.UserModel;
using NPOI.XWPF.UserModel;
using QRCoder;
using RestSharp;
using SleckEc2KdInterface.BLL;
using SleckEc2KdInterface.DAL;
using SleckEc2KdInterface.Entity;
using SleckEc2KdInterface.OPENAPI;
using SlekGift.Models;
using SqlSugar;
using Sunny.UI;
using Sunny.UI.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
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
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Xml.Linq;
using static log4net.Appender.RollingFileAppender;
using static NPOI.HSSF.Util.HSSFColor;
using static System.Windows.Forms.AxHost;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using BorderStyle = NPOI.SS.UserModel.BorderStyle;
using HorizontalAlignment = NPOI.SS.UserModel.HorizontalAlignment;
using VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment;
using UIMessageBox = SleckEc2KdInterface.UI.NexusMessageBox;
using MessageBox = SleckEc2KdInterface.UI.NexusMessageBox;

namespace SleckEc2KdInterface
{
    public partial class Form5 : UI.NexusWindow
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

        public Form5()
        {
            InitializeComponent();
            UI.NexusShell.ApplyMain(this, uiTabControl1, false);
            UI.NexusPageLayouts.Apply(this, false);
        }

        private void uiButton1_Click(object sender, EventArgs e)
        {





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
                using (HttpClient client = new HttpClient())
                {

                    var conobj = new
                    {
                        page = 1,
                        limit = 1000000,
                        orderno = uiTextBox17.Text,
                        startDate = Convert.ToDateTime(sdt.Value.ToShortDateString()),
                        endDate = Convert.ToDateTime(edt.Value.ToShortDateString()),
                        goodno = uitxtgoodno.Text,
                        custname = uiTextBox11.Text,
                        wdtsrcno = uitxtSrcOrderNo.Text,
                        recipienter = uitxtAddressee.Text,
                        logisticsno = uitxtlogisticsno.Text,
                        userId = wflogin.fuserId

                    };

                    var str = JsonConvert.SerializeObject(conobj);
                    HttpContent content = new StringContent(str);
                    content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                    HttpResponseMessage response = await client.PostAsync("http://120.78.210.159:3304/Order/GetWDTOrderDetailList", content);
                    //  response.EnsureSuccessStatusCode();//用来抛异常的
                    string resultStr = await response.Content.ReadAsStringAsync();
                    var result = JObject.Parse(resultStr);
                    if (result.ContainsKey("code"))
                    {
                        if (Convert.ToInt32(result["code"]) == 0)
                        {
                            JArray resultList = JArray.Parse(result["list"].ToString());
                            var datas = resultList.Select(a => new
                            {
                                客户名称 = a["FCustName"].ToString(),
                                分销商名称 = a["FSellersname"].ToString(),
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
                                客服备注 = a["Remark"].ToString(),
                                拆分情况 = a["SplitOrderSstatus"] == null ? "" : a["SplitOrderSstatus"].ToString(),
                                同步旺店通结果 = a["ResultMess"].ToString(),
                                订单ID = a["FID"].ToString(),
                                明细ID = a["ID"].ToString(),
                                制单人ID = a["FUserID"].ToString(),

                            }).OrderByDescending(a => a.明细ID).ToList();
                            if (dataGridView2ASC == false)
                            {
                                datas = resultList.Select(a => new
                                {
                                    客户名称 = a["FCustName"].ToString(),
                                    分销商名称 = a["FSellersname"].ToString(),
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
                                    客服备注 = a["Remark"].ToString(),
                                    拆分情况 = a["SplitOrderSstatus"] == null ? "" : a["SplitOrderSstatus"].ToString(),
                                    同步旺店通结果 = a["ResultMess"].ToString(),
                                    订单ID = a["FID"].ToString(),
                                    明细ID = a["ID"].ToString(),
                                    制单人ID = a["FUserID"].ToString()

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
                    else
                    {
                        UIMessageBox.Show("无法访问系统,请关注是否网络问题!");
                    }
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


        public static DataTable ExcelToDataTable(string filePath, bool isColumnName)
        {
            DataTable dataTable = null;
            FileStream fs = null;
            DataColumn column = null;
            DataRow dataRow = null;
            IWorkbook workbook = null;
            ISheet sheet = null;
            IRow row = null;
            NPOI.SS.UserModel.ICell cell = null;
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

        public static DataTable ExcelToDataTableForlog(string filePath, bool isColumnName)
        {
            DataTable dataTable = null;
            FileStream fs = null;
            DataColumn column = null;
            DataRow dataRow = null;
            IWorkbook workbook = null;
            ISheet sheet = null;
            IRow row = null;
            NPOI.SS.UserModel.ICell cell = null;
            int startRow = 0;

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

                    if (workbook != null)
                    {
                        sheet = workbook.GetSheetAt(0);//读取第一个sheet
                        dataTable = new DataTable();

                        if (sheet != null)
                        {
                            int rowCount = sheet.LastRowNum;//总行数，注意：LastRowNum是最后一行的索引，不是行数

                            // 检查是否有数据行
                            if (rowCount >= 0) // 至少有一行数据（包括表头）
                            {
                                IRow firstRow = sheet.GetRow(0);//第一行  
                                if (firstRow == null)
                                {
                                    // 第一行为空，尝试寻找第一个非空行
                                    for (int i = 0; i <= rowCount; i++)
                                    {
                                        firstRow = sheet.GetRow(i);
                                        if (firstRow != null)
                                        {
                                            rowCount = sheet.LastRowNum - i; // 重新计算行数
                                            break;
                                        }
                                    }
                                }

                                if (firstRow == null)
                                {
                                    return dataTable; // 返回空的DataTable而不是null
                                }

                                int cellCount = firstRow.LastCellNum;//列数，LastCellNum已经是实际列数了

                                //构建datatable的列  
                                if (isColumnName)
                                {
                                    startRow = 1;//如果第一行是列名，则从第二行开始读取  
                                    for (int i = firstRow.FirstCellNum; i < cellCount; i++)
                                    {
                                        cell = firstRow.GetCell(i);
                                        string columnName = "";

                                        if (cell != null)
                                        {
                                            switch (cell.CellType)
                                            {
                                                case CellType.String:
                                                    columnName = cell.StringCellValue?.Trim() ?? "";
                                                    break;
                                                case CellType.Numeric:
                                                    columnName = cell.NumericCellValue.ToString();
                                                    break;
                                                case CellType.Boolean:
                                                    columnName = cell.BooleanCellValue.ToString();
                                                    break;
                                                case CellType.Formula:
                                                    try
                                                    {
                                                        columnName = cell.StringCellValue?.Trim() ?? "";
                                                    }
                                                    catch
                                                    {
                                                        columnName = cell.NumericCellValue.ToString();
                                                    }
                                                    break;
                                                default:
                                                    columnName = $"Column{i + 1}";
                                                    break;
                                            }
                                        }

                                        // 如果列名为空，使用默认名称
                                        if (string.IsNullOrEmpty(columnName))
                                        {
                                            columnName = $"Column{i + 1}";
                                        }

                                        // 确保列名唯一
                                        string originalColumnName = columnName;
                                        int suffix = 1;
                                        while (dataTable.Columns.Contains(columnName))
                                        {
                                            columnName = $"{originalColumnName}_{suffix}";
                                            suffix++;
                                        }

                                        column = new DataColumn(columnName);
                                        dataTable.Columns.Add(column);
                                    }
                                }
                                else
                                {
                                    for (int i = firstRow.FirstCellNum; i < cellCount; i++)
                                    {
                                        column = new DataColumn("column" + (i + 1));
                                        dataTable.Columns.Add(column);
                                    }
                                }

                                //填充行数据
                                for (int i = startRow; i <= rowCount; i++)
                                {
                                    row = sheet.GetRow(i);
                                    if (row == null) continue;

                                    // 检查行是否为空
                                    bool isEmptyRow = true;
                                    for (int j = row.FirstCellNum; j < cellCount; j++)
                                    {
                                        cell = row.GetCell(j);
                                        if (cell != null && cell.CellType != CellType.Blank)
                                        {
                                            string cellValue = GetCellValue(cell);
                                            if (!string.IsNullOrWhiteSpace(cellValue))
                                            {
                                                isEmptyRow = false;
                                                break;
                                            }
                                        }
                                    }

                                    // 跳过空行
                                    if (isEmptyRow) continue;

                                    dataRow = dataTable.NewRow();
                                    for (int j = row.FirstCellNum; j < cellCount && j < dataTable.Columns.Count; j++)
                                    {
                                        cell = row.GetCell(j);
                                        if (cell == null)
                                        {
                                            dataRow[j] = "";
                                        }
                                        else
                                        {
                                            dataRow[j] = GetCellValue(cell);
                                        }
                                    }
                                    dataTable.Rows.Add(dataRow);
                                }
                            }
                        }
                    }
                }
                return dataTable ?? new DataTable(); // 确保不返回null
            }
            catch (Exception ex)
            {
                if (fs != null)
                {
                    fs.Close();
                }
                // 记录异常信息，方便调试
                System.Diagnostics.Debug.WriteLine($"读取Excel文件异常: {ex.Message}");
                return new DataTable(); // 返回空的DataTable而不是null
            }
        }

        /// <summary>
        /// 获取单元格的值
        /// </summary>
        /// <param name="cell">单元格</param>
        /// <returns>单元格值的字符串表示</returns>
        private static string GetCellValue(NPOI.SS.UserModel.ICell cell)
        {
            if (cell == null) return "";

            switch (cell.CellType)
            {
                case CellType.Blank:
                    return "";
                case CellType.Numeric:
                    // 检查是否为日期格式
                    if (NPOI.SS.UserModel.DateUtil.IsCellDateFormatted(cell))
                    {
                        return cell.DateCellValue.ToString("yyyy-MM-dd HH:mm:ss");
                    }
                    else
                    {
                        // 处理数字格式，避免科学计数法
                        double numValue = cell.NumericCellValue;
                        if (numValue == Math.Floor(numValue))
                        {
                            // 整数
                            return ((long)numValue).ToString();
                        }
                        else
                        {
                            // 小数
                            return numValue.ToString();
                        }
                    }
                case CellType.String:
                    return cell.StringCellValue?.Trim().Replace("'", "") ?? "";
                case CellType.Boolean:
                    return cell.BooleanCellValue.ToString();
                case CellType.Formula:
                    try
                    {
                        // 尝试获取公式计算后的值
                        switch (cell.CachedFormulaResultType)
                        {
                            case CellType.Numeric:
                                if (NPOI.SS.UserModel.DateUtil.IsCellDateFormatted(cell))
                                {
                                    return cell.DateCellValue.ToString("yyyy-MM-dd HH:mm:ss");
                                }
                                else
                                {
                                    double numValue = cell.NumericCellValue;
                                    if (numValue == Math.Floor(numValue))
                                    {
                                        return ((long)numValue).ToString();
                                    }
                                    else
                                    {
                                        return numValue.ToString();
                                    }
                                }
                            case CellType.String:
                                return cell.StringCellValue?.Trim().Replace("'", "") ?? "";
                            case CellType.Boolean:
                                return cell.BooleanCellValue.ToString();
                            default:
                                return "";
                        }
                    }
                    catch
                    {
                        return "";
                    }
                case CellType.Error:
                    return "";
                default:
                    return "";
            }
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            uitxtfilepath.Text = openFileDialog1.FileName;
            Task.Run(exceltodata);
            //  DataTable dt = ExcelToDataTable(openFileDialog1.FileName, true);
            //  uiDataGridView1.DataSource = dt;

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

        //地址校验
        public bool addressResolution(String address)
        {

            String regex = "(?<province>[^省]+^自治区|.*?自治区|.*?省|.*?行政区|.*?市)(?<city>上海城区|北京城区|天津城区|重庆城区|重庆郊县|[^市]+自治州|.*?地区|.*?行政单位|.+盟|市辖区|香港岛|九龙|新界|澳门半岛|离岛|.*?市|.*?县)";
            String countyRegex = "(?<county>121团炮台镇|井陉县|正定县|行唐县|灵寿县|高邑县|深泽县|赞皇县|无极县|平山县|元氏县|赵县|辛集市|晋州市|新乐市|滦县|滦南县|乐亭县|迁西县|玉田县|遵化市|迁安市|青龙满族自治县|昌黎县|卢龙县|临漳县|成安县|大名县|涉县|磁县|邱县|鸡泽县|广平县|馆陶县|魏县|曲周县|武安市|邢台县|临城县|内丘县|柏乡县|隆尧县|任县|南和县|宁晋县|巨鹿县|新河县|广宗县|平乡县|威县|清河县|临西县|南宫市|沙河市|涞水县|阜平县|定兴县|唐县|高阳县|容城县|涞源县|望都县|安新县|易县|曲阳县|蠡县|顺平县|博野县|雄县|涿州市|定州市|安国市|高碑店市|张北县|康保县|沽源县|尚义县|蔚县|阳原县|怀安县|怀来县|涿鹿县|赤城县|承德县|兴隆县|滦平县|隆化县|丰宁满族自治县|宽城满族自治县|围场满族蒙古族自治县|平泉市|沧县|青县|东光县|海兴县|盐山县|肃宁县|南皮县|吴桥县|献县|孟村回族自治县|泊头市|任丘市|黄骅市|河间市|固安县|永清县|香河县|大城县|文安县|大厂回族自治县|霸州市|三河市|枣强县|武邑县|武强县|饶阳县|安平县|故城县|景县|阜城县|深州市|清徐县|阳曲县|娄烦县|古交市|阳高县|天镇县|广灵县|灵丘县|浑源县|左云县|大同县|平定县|盂县|长治县|襄垣县|屯留县|平顺县|黎城县|壶关县|长子县|武乡县|沁县|沁源县|潞城市|沁水县|阳城县|陵川县|泽州县|高平市|山阴县|应县|右玉县|怀仁县|榆社县|左权县|和顺县|昔阳县|寿阳县|太谷县|祁县|平遥县|灵石县|介休市|临猗县|万荣县|闻喜县|稷山县|新绛县|绛县|垣曲县|夏县|平陆县|芮城县|永济市|河津市|定襄县|五台县|代县|繁峙县|宁武县|静乐县|神池县|五寨县|岢岚县|河曲县|保德县|偏关县|原平市|曲沃县|翼城县|襄汾县|洪洞县|古县|安泽县|浮山县|吉县|乡宁县|大宁县|隰县|永和县|蒲县|汾西县|侯马市|霍州市|文水县|交城县|兴县|临县|柳林县|石楼县|岚县|方山县|中阳县|交口县|孝义市|汾阳市|土默特左旗|托克托县|和林格尔县|清水河县|武川县|土默特右旗|固阳县|达尔罕茂明安联合旗|阿鲁科尔沁旗|巴林左旗|巴林右旗|林西县|克什克腾旗|翁牛特旗|喀喇沁旗|宁城县|敖汉旗|科尔沁左翼中旗|科尔沁左翼后旗|开鲁县|库伦旗|奈曼旗|扎鲁特旗|霍林郭勒市|达拉特旗|准格尔旗|鄂托克前旗|鄂托克旗|杭锦旗|乌审旗|伊金霍洛旗|阿荣旗|莫力达瓦达斡尔族自治旗|鄂伦春自治旗|鄂温克族自治旗|陈巴尔虎旗|新巴尔虎左旗|新巴尔虎右旗|满洲里市|牙克石市|扎兰屯市|额尔古纳市|根河市|五原县|磴口县|乌拉特前旗|乌拉特中旗|乌拉特后旗|杭锦后旗|卓资县|化德县|商都县|兴和县|凉城县|察哈尔右翼前旗|察哈尔右翼中旗|察哈尔右翼后旗|四子王旗|丰镇市|乌兰浩特市|阿尔山市|科尔沁右翼前旗|科尔沁右翼中旗|扎赉特旗|突泉县|二连浩特市|锡林浩特市|阿巴嘎旗|苏尼特左旗|苏尼特右旗|东乌珠穆沁旗|西乌珠穆沁旗|太仆寺旗|镶黄旗|正镶白旗|正蓝旗|多伦县|阿拉善左旗|阿拉善右旗|额济纳旗|康平县|法库县|新民市|长海县|瓦房店市|庄河市|台安县|岫岩满族自治县|海城市|抚顺县|新宾满族自治县|清原满族自治县|本溪满族自治县|桓仁满族自治县|宽甸满族自治县|东港市|凤城市|黑山县|义县|凌海市|北镇市|盖州市|大石桥市|阜新蒙古族自治县|彰武县|辽阳县|灯塔市|盘山县|铁岭县|西丰县|昌图县|调兵山市|开原市|朝阳县|建平县|喀喇沁左翼蒙古族自治县|北票市|凌源市|绥中县|建昌县|兴城市|农安县|榆树市|德惠市|永吉县|蛟河市|桦甸市|舒兰市|磐石市|梨树县|伊通满族自治县|公主岭市|双辽市|东丰县|东辽县|通化县|辉南县|柳河县|梅河口市|集安市|抚松县|靖宇县|长白朝鲜族自治县|临江市|前郭尔罗斯蒙古族自治县|长岭县|乾安县|扶余市|镇赉县|通榆县|洮南市|大安市|延吉市|图们市|敦化市|珲春市|龙井市|和龙市|汪清县|安图县|依兰县|方正县|宾县|巴彦县|木兰县|通河县|延寿县|尚志市|五常市|龙江县|依安县|泰来县|甘南县|富裕县|克山县|克东县|拜泉县|讷河市|鸡东县|虎林市|密山市|萝北县|绥滨县|集贤县|友谊县|宝清县|饶河县|肇州县|肇源县|林甸县|杜尔伯特蒙古族自治县|嘉荫县|铁力市|桦南县|桦川县|汤原县|同江市|富锦市|抚远市|勃利县|林口县|绥芬河市|海林市|宁安市|穆棱市|东宁市|嫩江县|逊克县|孙吴县|北安市|五大连池市|望奎县|兰西县|青冈县|庆安县|明水县|绥棱县|安达市|肇东市|海伦市|呼玛县|塔河县|漠河县|江阴市|宜兴市|丰县|沛县|睢宁县|新沂市|邳州市|溧阳市|常熟市|张家港市|昆山市|太仓市|海安县|如东县|启东市|如皋市|海门市|东海县|灌云县|灌南县|涟水县|盱眙县|金湖县|响水县|滨海县|阜宁县|射阳县|建湖县|东台市|宝应县|仪征市|高邮市|丹阳市|扬中市|句容市|兴化市|靖江市|泰兴市|沭阳县|泗阳县|泗洪县|桐庐县|淳安县|建德市|象山县|宁海县|余姚市|慈溪市|永嘉县|平阳县|苍南县|文成县|泰顺县|瑞安市|乐清市|嘉善县|海盐县|海宁市|平湖市|桐乡市|德清县|长兴县|安吉县|新昌县|诸暨市|嵊州市|武义县|浦江县|磐安县|兰溪市|义乌市|东阳市|永康市|常山县|开化县|龙游县|江山市|岱山县|嵊泗县|三门县|天台县|仙居县|温岭市|临海市|玉环市|青田县|缙云县|遂昌县|松阳县|云和县|庆元县|景宁畲族自治县|龙泉市|长丰县|肥东县|肥西县|庐江县|巢湖市|芜湖县|繁昌县|南陵县|无为县|怀远县|五河县|固镇县|凤台县|寿县|当涂县|含山县|和县|濉溪县|枞阳县|怀宁县|潜山县|太湖县|宿松县|望江县|岳西县|桐城市|歙县|休宁县|黟县|祁门县|来安县|全椒县|定远县|凤阳县|天长市|明光市|临泉县|太和县|阜南县|颍上县|界首市|砀山县|萧县|灵璧县|泗县|霍邱县|舒城县|金寨县|霍山县|涡阳县|蒙城县|利辛县|东至县|石台县|青阳县|郎溪县|广德县|泾县|绩溪县|旌德县|宁国市|闽侯县|连江县|罗源县|闽清县|永泰县|平潭县|福清市|仙游县|明溪县|清流县|宁化县|大田县|尤溪县|沙县|将乐县|泰宁县|建宁县|永安市|惠安县|安溪县|永春县|德化县|金门县|石狮市|晋江市|南安市|云霄县|漳浦县|诏安县|长泰县|东山县|南靖县|平和县|华安县|龙海市|顺昌县|浦城县|光泽县|松溪县|政和县|邵武市|武夷山市|建瓯市|长汀县|上杭县|武平县|连城县|漳平市|霞浦县|古田县|屏南县|寿宁县|周宁县|柘荣县|福安市|福鼎市|南昌县|安义县|进贤县|浮梁县|乐平市|莲花县|上栗县|芦溪县|武宁县|修水县|永修县|德安县|都昌县|湖口县|彭泽县|瑞昌市|共青城市|庐山市|分宜县|余江县|贵溪市|信丰县|大余县|上犹县|崇义县|安远县|龙南县|定南县|全南县|宁都县|于都县|兴国县|会昌县|寻乌县|石城县|瑞金市|吉安县|吉水县|峡江县|新干县|永丰县|泰和县|遂川县|万安县|安福县|永新县|井冈山市|奉新县|万载县|上高县|宜丰县|靖安县|铜鼓县|丰城市|樟树市|高安市|南城县|黎川县|南丰县|崇仁县|乐安县|宜黄县|金溪县|资溪县|广昌县|玉山县|铅山县|横峰县|弋阳县|余干县|鄱阳县|万年县|婺源县|德兴市|平阴县|济阳县|商河县|胶州市|平度市|莱西市|桓台县|高青县|沂源县|滕州市|利津县|广饶县|长岛县|龙口市|莱阳市|莱州市|蓬莱市|招远市|栖霞市|海阳市|临朐县|昌乐县|青州市|诸城市|寿光市|安丘市|高密市|昌邑市|微山县|鱼台县|金乡县|嘉祥县|汶上县|泗水县|梁山县|曲阜市|邹城市|宁阳县|东平县|新泰市|肥城市|荣成市|乳山市|五莲县|莒县|沂南县|郯城县|沂水县|兰陵县|费县|平邑县|莒南县|蒙阴县|临沭县|宁津县|庆云县|临邑县|齐河县|平原县|夏津县|武城县|乐陵市|禹城市|阳谷县|莘县|茌平县|东阿县|冠县|高唐县|临清市|惠民县|阳信县|无棣县|博兴县|邹平县|曹县|单县|成武县|巨野县|郓城县|鄄城县|东明县|中牟县|巩义市|荥阳市|新密市|新郑市|登封市|杞县|通许县|尉氏县|兰考县|孟津县|新安县|栾川县|嵩县|汝阳县|宜阳县|洛宁县|伊川县|偃师市|宝丰县|叶县|鲁山县|郏县|舞钢市|汝州市|安阳县|汤阴县|滑县|内黄县|林州市|浚县|淇县|新乡县|获嘉县|原阳县|延津县|封丘县|长垣县|卫辉市|辉县市|修武县|博爱县|武陟县|温县|沁阳市|孟州市|清丰县|南乐县|范县|台前县|濮阳县|鄢陵县|襄城县|禹州市|长葛市|舞阳县|临颍县|渑池县|卢氏县|义马市|灵宝市|南召县|方城县|西峡县|镇平县|内乡县|淅川县|社旗县|唐河县|新野县|桐柏县|邓州市|民权县|睢县|宁陵县|柘城县|虞城县|夏邑县|永城市|罗山县|光山县|新县|商城县|固始县|潢川县|淮滨县|息县|扶沟县|西华县|商水县|沈丘县|郸城县|淮阳县|太康县|鹿邑县|项城市|西平县|上蔡县|平舆县|正阳县|确山县|泌阳县|汝南县|遂平县|新蔡县|阳新县|大冶市|郧西县|竹山县|竹溪县|房县|丹江口市|远安县|兴山县|秭归县|长阳土家族自治县|五峰土家族自治县|宜都市|当阳市|枝江市|南漳县|谷城县|保康县|老河口市|枣阳市|宜城市|京山县|沙洋县|钟祥市|孝昌县|大悟县|云梦县|应城市|安陆市|汉川市|公安县|监利县|江陵县|石首市|洪湖市|松滋市|团风县|红安县|罗田县|英山县|浠水县|蕲春县|黄梅县|麻城市|武穴市|嘉鱼县|通城县|崇阳县|通山县|赤壁市|随县|广水市|恩施市|利川市|建始县|巴东县|宣恩县|咸丰县|来凤县|鹤峰县|长沙县|浏阳市|宁乡市|宁乡县|株洲县|攸县|茶陵县|炎陵县|醴陵市|湘潭县|湘乡市|韶山市|衡阳县|衡南县|衡山县|衡东县|祁东县|耒阳市|常宁市|邵东县|新邵县|邵阳县|隆回县|洞口县|绥宁县|新宁县|城步苗族自治县|武冈市|岳阳县|华容县|湘阴县|平江县|汨罗市|临湘市|安乡县|汉寿县|澧县|临澧县|桃源县|石门县|津市市|慈利县|桑植县|南县|桃江县|安化县|沅江市|桂阳县|宜章县|永兴县|嘉禾县|临武县|汝城县|桂东县|安仁县|资兴市|祁阳县|东安县|双牌县|道县|江永县|宁远县|蓝山县|新田县|江华瑶族自治县|中方县|沅陵县|辰溪县|溆浦县|会同县|麻阳苗族自治县|新晃侗族自治县|芷江侗族自治县|靖州苗族侗族自治县|通道侗族自治县|洪江市|双峰县|新化县|冷水江市|涟源市|吉首市|泸溪县|凤凰县|花垣县|保靖县|古丈县|永顺县|龙山县|始兴县|仁化县|翁源县|乳源瑶族自治县|新丰县|乐昌市|南雄市|澳门大学横琴校区(由澳门管辖)|南澳县|台山市|开平市|鹤山市|恩平市|遂溪县|徐闻县|廉江市|雷州市|吴川市|高州市|化州市|信宜市|广宁县|怀集县|封开县|德庆县|四会市|博罗县|惠东县|龙门县|大埔县|丰顺县|五华县|平远县|蕉岭县|兴宁市|海丰县|陆河县|陆丰市|紫金县|龙川县|连平县|和平县|东源县|阳西县|阳春市|佛冈县|阳山县|连山壮族瑶族自治县|连南瑶族自治县|英德市|连州市|饶平县|揭西县|惠来县|普宁市|新兴县|郁南县|罗定市|隆安县|马山县|上林县|宾阳县|横县|柳城县|鹿寨县|融安县|融水苗族自治县|三江侗族自治县|阳朔县|灵川县|全州县|兴安县|永福县|灌阳县|龙胜各族自治县|资源县|平乐县|荔浦县|恭城瑶族自治县|苍梧县|藤县|蒙山县|岑溪市|合浦县|上思县|东兴市|灵山县|浦北县|平南县|桂平市|容县|陆川县|博白县|兴业县|北流市|田东县|平果县|德保县|那坡县|凌云县|乐业县|田林县|西林县|隆林各族自治县|靖西市|昭平县|钟山县|富川瑶族自治县|南丹县|天峨县|凤山县|东兰县|罗城仫佬族自治县|环江毛南族自治县|巴马瑶族自治县|都安瑶族自治县|大化瑶族自治县|忻城县|象州县|武宣县|金秀瑶族自治县|合山市|扶绥县|宁明县|龙州县|大新县|天等县|凭祥市|西沙群岛|南沙群岛|中沙群岛的岛礁及其海域|城口县|丰都县|垫江县|忠县|云阳县|奉节县|巫山县|巫溪县|石柱土家族自治县|秀山土家族苗族自治县|酉阳土家族苗族自治县|彭水苗族土家族自治县|金堂县|大邑县|蒲江县|新津县|都江堰市|彭州市|邛崃市|崇州市|简阳市|荣县|富顺县|米易县|盐边县|泸县|合江县|叙永县|古蔺县|中江县|广汉市|什邡市|绵竹市|三台县|盐亭县|梓潼县|北川羌族自治县|平武县|江油市|旺苍县|青川县|剑阁县|苍溪县|蓬溪县|射洪县|大英县|威远县|资中县|隆昌市|犍为县|井研县|夹江县|沐川县|峨边彝族自治县|马边彝族自治县|峨眉山市|南部县|营山县|蓬安县|仪陇县|西充县|阆中市|仁寿县|洪雅县|丹棱县|青神县|宜宾县|江安县|长宁县|高县|珙县|筠连县|兴文县|屏山县|岳池县|武胜县|邻水县|华蓥市|宣汉县|开江县|大竹县|渠县|万源市|荥经县|汉源县|石棉县|天全县|芦山县|宝兴县|通江县|南江县|平昌县|安岳县|乐至县|马尔康市|汶川县|理县|茂县|松潘县|九寨沟市|金川县|小金县|黑水县|壤塘县|阿坝县|若尔盖县|红原县|康定市|泸定县|丹巴县|九龙县|雅江县|道孚县|炉霍县|甘孜县|新龙县|德格县|白玉县|石渠县|色达县|理塘县|巴塘县|乡城县|稻城县|得荣县|西昌市|木里藏族自治县|盐源县|德昌县|会理县|会东县|宁南县|普格县|布拖县|金阳县|昭觉县|喜德县|冕宁县|越西县|甘洛县|美姑县|雷波县|开阳县|息烽县|修文县|清镇市|水城县|盘州市|桐梓县|绥阳县|正安县|道真仡佬族苗族自治县|务川仡佬族苗族自治县|凤冈县|湄潭县|余庆县|习水县|赤水市|仁怀市|普定县|镇宁布依族苗族自治县|关岭布依族苗族自治县|紫云苗族布依族自治县|大方县|黔西县|金沙县|织金县|纳雍县|威宁彝族回族苗族自治县|赫章县|江口县|玉屏侗族自治县|石阡县|思南县|印江土家族苗族自治县|德江县|沿河土家族自治县|松桃苗族自治县|兴义市|兴仁县|普安县|晴隆县|贞丰县|望谟县|册亨县|安龙县|凯里市|黄平县|施秉县|三穗县|镇远县|岑巩县|天柱县|锦屏县|剑河县|台江县|黎平县|榕江县|从江县|雷山县|麻江县|丹寨县|都匀市|福泉市|荔波县|贵定县|瓮安县|独山县|平塘县|罗甸县|长顺县|龙里县|惠水县|三都水族自治县|富民县|宜良县|石林彝族自治县|嵩明县|禄劝彝族苗族自治县|寻甸回族彝族自治县|安宁市|马龙县|陆良县|师宗县|罗平县|富源县|会泽县|宣威市|澄江县|通海县|华宁县|易门县|峨山彝族自治县|新平彝族傣族自治县|元江哈尼族彝族傣族自治县|施甸县|龙陵县|昌宁县|腾冲市|鲁甸县|巧家县|盐津县|大关县|永善县|绥江县|镇雄县|彝良县|威信县|水富县|玉龙纳西族自治县|永胜县|华坪县|宁蒗彝族自治县|宁洱哈尼族彝族自治县|墨江哈尼族自治县|景东彝族自治县|景谷傣族彝族自治县|镇沅彝族哈尼族拉祜族自治县|江城哈尼族彝族自治县|孟连傣族拉祜族佤族自治县|澜沧拉祜族自治县|西盟佤族自治县|凤庆县|云县|永德县|镇康县|双江拉祜族佤族布朗族傣族自治县|耿马傣族佤族自治县|沧源佤族自治县|楚雄市|双柏县|牟定县|南华县|姚安县|大姚县|永仁县|元谋县|武定县|禄丰县|个旧市|开远市|蒙自市|弥勒市|屏边苗族自治县|建水县|石屏县|泸西县|元阳县|红河县|金平苗族瑶族傣族自治县|绿春县|河口瑶族自治县|文山市|砚山县|西畴县|麻栗坡县|马关县|丘北县|广南县|富宁县|景洪市|勐海县|勐腊县|大理市|漾濞彝族自治县|祥云县|宾川县|弥渡县|南涧彝族自治县|巍山彝族回族自治县|永平县|云龙县|洱源县|剑川县|鹤庆县|瑞丽市|芒市|梁河县|盈江县|陇川县|泸水市|福贡县|贡山独龙族怒族自治县|兰坪白族普米族自治县|香格里拉市|德钦县|维西傈僳族自治县|林周县|当雄县|尼木县|曲水县|墨竹工卡县|南木林县|江孜县|定日县|萨迦县|拉孜县|昂仁县|谢通门县|白朗县|仁布县|康马县|定结县|仲巴县|亚东县|吉隆县|聂拉木县|萨嘎县|岗巴县|江达县|贡觉县|类乌齐县|丁青县|察雅县|八宿县|左贡县|芒康县|洛隆县|边坝县|工布江达县|米林县|墨脱县|波密县|察隅县|朗县|扎囊县|贡嘎县|桑日县|琼结县|曲松县|措美县|洛扎县|加查县|隆子县|错那县|浪卡子县|嘉黎县|比如县|聂荣县|安多县|申扎县|索县|班戈县|巴青县|尼玛县|双湖县|普兰县|札达县|噶尔县|日土县|革吉县|改则县|措勤县|蓝田县|周至县|宜君县|凤翔县|岐山县|扶风县|眉县|陇县|千阳县|麟游县|凤县|太白县|三原县|泾阳县|乾县|礼泉县|永寿县|彬县|长武县|旬邑县|淳化县|武功县|兴平市|潼关县|大荔县|合阳县|澄城县|蒲城县|白水县|富平县|韩城市|华阴市|延长县|延川县|子长县|志丹县|吴起县|甘泉县|富县|洛川县|宜川县|黄龙县|黄陵县|城固县|洋县|西乡县|勉县|宁强县|略阳县|镇巴县|留坝县|佛坪县|府谷县|靖边县|定边县|绥德县|米脂县|佳县|吴堡县|清涧县|子洲县|神木市|汉阴县|石泉县|宁陕县|紫阳县|岚皋县|平利县|镇坪县|旬阳县|白河县|洛南县|丹凤县|商南县|山阳县|镇安县|柞水县|永登县|皋兰县|榆中县|永昌县|靖远县|会宁县|景泰县|清水县|秦安县|甘谷县|武山县|张家川回族自治县|民勤县|古浪县|天祝藏族自治县|肃南裕固族自治县|民乐县|临泽县|高台县|山丹县|泾川县|灵台县|崇信县|华亭县|庄浪县|静宁县|金塔县|瓜州县|肃北蒙古族自治县|阿克塞哈萨克族自治县|玉门市|敦煌市|庆城县|环县|华池县|合水县|正宁县|宁县|镇原县|通渭县|陇西县|渭源县|临洮县|漳县|岷县|成县|文县|宕昌县|康县|西和县|礼县|徽县|两当县|临夏市|临夏县|康乐县|永靖县|广河县|和政县|东乡族自治县|积石山保安族东乡族撒拉族自治县|合作市|临潭县|卓尼县|舟曲县|迭部县|玛曲县|碌曲县|夏河县|大通回族土族自治县|湟中县|湟源县|民和回族土族自治县|互助土族自治县|化隆回族自治县|循化撒拉族自治县|门源回族自治县|祁连县|海晏县|刚察县|同仁县|尖扎县|泽库县|河南蒙古族自治县|共和县|同德县|贵德县|兴海县|贵南县|玛沁县|班玛县|甘德县|达日县|久治县|玛多县|玉树市|杂多县|称多县|治多县|囊谦县|曲麻莱县|格尔木市|德令哈市|乌兰县|都兰县|天峻县|海西蒙古族藏族自治州直辖|永宁县|贺兰县|灵武市|平罗县|盐池县|同心县|青铜峡市|西吉县|隆德县|泾源县|彭阳县|中宁县|海原县|乌鲁木齐县|鄯善县|托克逊县|巴里坤哈萨克自治县|伊吾县|昌吉市|阜康市|呼图壁县|玛纳斯县|奇台县|吉木萨尔县|木垒哈萨克自治县|博乐市|阿拉山口市|精河县|温泉县|库尔勒市|轮台县|尉犁县|若羌县|且末县|焉耆回族自治县|和静县|和硕县|博湖县|阿克苏市|温宿县|库车县|沙雅县|新和县|拜城县|乌什县|阿瓦提县|柯坪县|阿图什市|阿克陶县|阿合奇县|乌恰县|喀什市|疏附县|疏勒县|英吉沙县|泽普县|莎车县|叶城县|麦盖提县|岳普湖县|伽师县|巴楚县|塔什库尔干塔吉克自治县|和田市|和田县|墨玉县|皮山县|洛浦县|策勒县|于田县|民丰县|伊宁市|奎屯市|霍尔果斯市|伊宁县|察布查尔锡伯自治县|霍城县|巩留县|新源县|昭苏县|特克斯县|尼勒克县|塔城市|乌苏市|额敏县|沙湾县|托里县|裕民县|和布克赛尔蒙古自治县|阿勒泰市|布尔津县|富蕴县|福海县|哈巴河县|青河县|吉木乃县|喀尔交镇|恰勒什海乡|托普铁热克镇|别斯铁热克乡|吉木乃镇|托斯特乡|兵团一八六团|乌拉斯特镇|兵团一五二团|向阳街道|红山街道|东城街道|石河子乡|老街街道|北泉镇|新城街道|阿拉尔农场|幸福路街道|兵团第一师幸福农场|兵团七团|兵团十一团|兵团第一师水利水电工程处|青松路街道|托喀依乡|兵团八团|中心监狱|兵团第一师塔里木灌区水利管理处|兵团十四团|兵团十二团|兵团十三团|金银川路街道|兵团十六团|兵团十团|南口街道|兵团四十九团|兵团图木舒克市永安坝|兵团五十一团|前海街道|兵团图木舒克市喀拉拜勒镇|永安坝街道|齐干却勒街道|兵团五十团|兵团五十三团|兵团四十四团|兵团一零二团|军垦路街道|人民路街道|兵团一零一团|兵团一零三团|青湖路街道|兵团二十九团|农二师三十团|兵团六十八团|都拉塔口岸|兵团六十六团|兵团六十三团|兵团六十七团|兵团六十四团|乌尔其乡|普恰克其乡|兵团皮山农场|兵团二二四团|兵团四十七团|博斯坦乡|喀拉喀什镇|阔依其乡|兵团一牧场|乌鲁克萨依乡|奴尔乡|兵团一零三|坡头镇|梨林镇|思礼镇|大峪镇|五龙口镇|王屋镇|玉泉街道|轵城镇|济水街道|沁园街道|下冶镇|天坛街道|克井镇|邵原镇|北海街道|承留镇|沙湖镇|工业园区|畜禽良种场|豆河镇|通海口镇|胡场镇|长倘口镇|五湖渔场|杨林尾镇|干河街道|西流河镇|赵西垸林场|九合垸原种场|彭场镇|沔城回族镇|龙华山街道|沙湖原种场|陈场镇|郭河镇|排湖风景区|郑场镇|沙嘴街道|张沟镇|毛嘴镇|三伏潭镇|积玉口镇|广华街道|泰丰街道|潜江经济开发区|周矶管理区|周矶街道|总口管理区|高场街道|王场镇|运粮湖管理区|园林街道|白鹭湖管理区|竹根滩镇|渔洋镇|熊口镇|后湖管理区|熊口管理区|江汉石油管理局|张金镇|杨市街道|老新镇|龙湾镇|浩口原种场|浩口镇|高石碑镇|胡市镇|多祥镇|黄潭镇|沉湖管委会|横林镇|马湾镇|干驿镇|蒋湖农场|小板镇|多宝镇|岳口镇|蒋场镇|石家河镇|彭市镇|竟陵街道|九真镇|佛子山镇|侨乡街道开发区|麻洋镇|杨林街道|白茅湖农场|汪场镇|渔薪镇|净潭乡|卢市镇|皂市镇|拖市镇|张港镇|下谷坪土家族乡|木鱼镇|新华镇|九湖镇|宋洛乡|松柏镇|红坪镇|阳日镇|常平镇|莞城街道|望牛墩镇|大朗镇|麻涌镇|黄江镇|凤岗镇|樟木头镇|东莞生态园|松山湖管委会|桥头镇|石龙镇|寮步镇|高埗镇|塘厦镇|厚街镇|谢岗镇|虎门镇|南城街道|虎门港管委会|横沥镇|企石镇|东坑镇|东城街道|石排镇|洪梅镇|沙田镇|长安镇|道滘镇|大岭山镇|清溪镇|茶山镇|中堂镇|万江街道|石碣镇|横栏镇|三角镇|五桂山街道|东升镇|神湾镇|火炬开发区街道|小榄镇|南朗镇|古镇镇|民众镇|港口镇|三乡镇|石岐区街道|大涌镇|南头镇|黄圃镇|东区街道|阜沙镇|板芙镇|西区街道|坦洲镇|南区街道|沙溪镇|东凤镇|洋浦经济开发区|光村镇|兰洋镇|海头镇|和庆镇|华南热作学院|国营蓝洋农场|中和镇|王五镇|木棠镇|东成镇|新州镇|南丰镇|排浦镇|国营西培农场|雅星镇|国营八一农场|国营西联农场|白马井镇|那大镇|峨蔓镇|大成镇|三都镇|南圣镇|毛阳镇|国营畅好农场|番阳镇|水满乡|通什镇|畅好乡|毛道乡|嘉积镇|会山镇|国营东升农场|国营东太农场|彬村山华侨农场|万泉镇|大路镇|潭门镇|国营东红农场|中原镇|长坡镇|龙江镇|塔洋镇|博鳌镇|阳江镇|石壁镇|东阁镇|国营罗豆农场|文教镇|会文镇|锦山镇|翁田镇|东郊镇|国营东路农场|铺前镇|龙楼镇|冯坡镇|国营南阳农场|昌洒镇|公坡镇|蓬莱镇|抱罗镇|潭牛镇|东路镇|文城镇|重兴镇|礼纪镇|国营东兴农场|后安镇|国营东和农场|万城镇|和乐镇|大茂镇|山根镇|龙滚镇|兴隆华侨农场|三更罗镇|北大镇|长丰镇|地方国营六连林场|国营新中农场|南桥镇|东澳镇|东方华侨农场|新龙镇|江边乡|国营广坝农场|天安乡|东河镇|感城镇|三家镇|四更镇|大田镇|板桥镇|八所镇|黄竹镇|新竹镇|国营中瑞农场|富文镇|岭口镇|定城镇|雷鸣镇|翰林镇|国营南海农场|龙河镇|国营金鸡岭农场|龙湖镇|龙门镇|国营中建农场|屯城镇|南吕镇|坡心镇|新兴镇|国营中坤农场|西昌镇|枫木镇|南坤镇|乌坡镇|永发镇|福山镇|中兴镇|桥头镇|金江镇|老城镇|文儒镇|国营红岗农场|瑞溪镇|国营红光农场|大丰镇|加乐镇|国营昆仑农场|国营金安农场|国营西达农场|国营和岭农场|仁兴镇|南宝镇|博厚镇|和舍镇|东英镇|调楼镇|波莲镇|多文镇|国营加来农场|新盈镇|临城镇|皇桐镇|国营红华农场|七坊镇|金波乡|南开乡|荣邦乡|青松乡|国营龙江农场|邦溪镇|元门乡|国营白沙农场|牙叉镇|细水乡|打安镇|阜龙乡|国营邦溪农场|国营霸王岭林场|十月田镇|石碌镇|海南矿业联合有限公司|七叉镇|乌烈镇|叉河镇|王下乡|海尾镇|国营红林农场|昌化镇|国营山荣农场|莺歌海镇|国营乐光农场|万冲镇|利国镇|国营保国农场|抱由镇|九所镇|国营莺歌海盐场|黄流镇|大安镇|佛罗镇|尖峰镇|千家镇|志仲镇|国营尖峰岭林业公司|椰林镇|黎安镇|群英乡|新村镇|国营吊罗山林业公司|本号镇|光坡镇|文罗镇|国营南平农场|隆广镇|提蒙乡|三才镇|国营岭门农场|英州镇|海南保亭热带作物研究所|新政镇|加茂镇|国营新星农场|保城镇|国营金江农场|南林乡|国营三道农场|毛感乡|响水镇|什玲镇|三道镇|六弓乡|吊罗山乡|国营加钗农场|和平镇|国营乌石农场|什运乡|湾岭镇|国营阳江农场|国营黎母山林业公司|营根镇|上安乡|中平镇|黎母山镇|国营长征农场|红毛镇|长征镇|峪泉镇|新城镇|文殊镇|雄关区|镜铁区|长城区|[^区]+区)";
            String detailAddress = "?(?<detail>.*)";

            string matcher = String.Concat(regex + countyRegex + detailAddress);
            Regex check = new Regex(matcher);
            String province, city, county, detail;
            if (check.IsMatch(address))
            {
                return true;
            }
            else
            {
                return false;
            }


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

        private void uiDataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

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
                RestClient client = new RestClient("http://120.78.210.159:3304/api/Wdt/getCustBills");
                RestRequest request = new RestRequest();
                request.AddHeader("Content-Type", "application/json");
                request.Method = Method.Post;
                RestResponse response;

                var obj = new { cusName = uitxtdzCustName.Text, sdate = Convert.ToDateTime(dzsdt.Value.ToShortDateString()), edate = Convert.ToDateTime(dzedt.Value.ToShortDateString()) };
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
        private void getCustBills()
        {
            this.Invoke(new Action(() =>
            {
                uiButton4.Text = "统计中...";
                uiButton4.Enabled = false;
                uirichtxt.Clear();
                uirichtxt.AppendText(DateTime.Now.ToShortTimeString() + ":开始统计...\r\n");
            }));
            try
            {

                var result = dbContext.Db.Queryable<U_Plans_WDTOrderItems, U_Plans_WDTOrder, V_CUST, V_MAT, U_Plans_suites>((a, b, c, d, f) => new object[]
                {
                JoinType.Left,a.FID==b.FID,
                JoinType.Left,b.FCustId==c.FCUSTID,
                JoinType.Left,a.Good_No==d.FNUMBER && d.FUSEORGID==100018,
                JoinType.Left,a.Good_No==f.suite_no

               }).Where((a, b, c, d, f) =>
                    c.FNAME.Contains(uitxtdzCustName.Text)



                 && b.FDate >= Convert.ToDateTime(dzsdt.Value.ToShortDateString()) && b.FDate <= Convert.ToDateTime(dzedt.Value.ToShortDateString())
                 ).Select((a, b, c, d, f) => new
                 {

                     订单日期 = b.FDate,
                     订单编号 = b.FOrderNo,
                     原始单号 = a.WDTSrcbillno,
                     客户名称 = c.FNAME,
                     收货人 = a.Recipienter,
                     收货人手机号 = a.Tel,
                     收货地址 = a.Address,
                     商品编码 = a.Good_No,
                     商品名称 = f.suite_name != null ? f.suite_name : d.FNAME,
                     商品规格 = d.FSpecification != null ? d.FSpecification : "",
                     商品数量 = a.Qty,
                     发货快递 = "",
                     单号 = a.logistics_no,
                     收货状态 = a.FStatus,
                     客户编码 = b.FCustId,
                     商品金额 = a.RecAmount



                 }).ToList();
                //订货数据
                var orResult = result.GroupBy(p => new
                {
                    p.订单日期,
                    p.客户名称,
                    p.客户编码

                }
                ).Select(a => new
                {
                    客户名称 = a.Key.客户名称,
                    客户ID = a.Key.客户编码,
                    订单日期 = a.Key.订单日期,
                    订货数量 = a.Sum(c => Convert.ToDecimal(c.商品数量)),
                    订货金额 = a.Sum(c => Convert.ToDecimal(c.商品金额)),
                    取消数量 = a.Where(b => b.收货状态 == "已取消").Sum(c => Convert.ToDecimal(c.商品数量)),
                    取消金额 = a.Where(b => b.收货状态 == "已取消").Sum(c => Convert.ToDecimal(c.商品金额)),
                    发货数量 = 0,
                    发货金额 = 0,
                    退货数量 = 0,
                    退货金额 = 0


                }).OrderBy(a => a.订单日期).ToArray();
                this.Invoke(new Action(() =>
                {

                    // uiDataGridView2.DataSource = orResult;
                    uirichtxt.AppendText(DateTime.Now.ToShortTimeString() + ":获取订货数据完成\r\n");

                }));


                //原始单号集合
                var orResultforSrc = result.Where(a => a.收货状态 != "已取消").GroupBy(p => new
                {
                    p.订单日期,
                    p.客户名称,
                    p.客户编码,
                    p.原始单号
                }
               ).Select((a) => new
               {
                   客户名称 = a.Key.客户名称,
                   客户ID = a.Key.客户编码,
                   订单日期 = a.Key.订单日期,
                   原始单号 = a.Key.原始单号,
                   发货金额 = 0,
                   发货数量 = 0

               }).Distinct().ToList();
                //通过原始单号去核查发货数据
                #region
                var Srcs = orResultforSrc.Select(s => s.原始单号).ToArray();

                var saleStockOutItems = dbContextEc.SaleStockOutItemDb.GetList(a => Srcs.Contains(a.src_tid)).GroupBy(a => a.src_tid).
                    Select(p => new
                    {
                        src_tid = p.Key,
                        totalnum = (decimal)p.Sum(x => x.num),
                        totalamount = (decimal)p.Sum(x => x.share_amount)
                    }).ToList();
                var sendInfogroup = orResultforSrc.Join(
                    saleStockOutItems,
                    a => a.原始单号,
                    b => b.src_tid,
                    (a, b) => new
                    {
                        客户ID = a.客户ID,
                        订单日期 = a.订单日期,
                        原始单号 = a.原始单号,
                        发货数量 = b.totalnum,
                        发货金额 = b.totalamount
                    }).ToList().GroupBy(a => new { a.客户ID, a.订单日期 }).Select(b => new
                    {
                        客户ID = b.Key.客户ID,
                        订单日期 = b.Key.订单日期,
                        发货数量 = (decimal)b.Sum(x => x.发货数量),
                        发货金额 = (decimal)b.Sum(x => x.发货金额)

                    }).ToList();

                var sendInfos = (from a in orResult
                                 join s in sendInfogroup
                                 on new { a.客户ID, a.订单日期 } equals new { s.客户ID, s.订单日期 } into rs
                                 from b in rs.DefaultIfEmpty()
                                 select new
                                 {
                                     客户ID = a.客户ID,
                                     订单日期 = a.订单日期,
                                     客户名称 = a.客户名称,
                                     订货数量 = a.订货数量,
                                     订货金额 = a.订货金额,
                                     取消数量 = a.取消数量,
                                     取消金额 = a.取消金额,
                                     发货数量 = b?.发货数量,
                                     发货金额 = b?.发货金额,
                                     退货数量 = 0.0,
                                     退货金额 = 0.0
                                 }).ToList();
                #region join方法
                //var sendInfos = orResult.Join(
                //   sendInfogroup,
                //   a => new {
                //       客户ID=a.客户ID,
                //       订单日期=a.订单日期
                //   },
                //   b => new {
                //       客户ID = b.客户ID,
                //       订单日期 = b.订单日期
                //   },
                //   (a, b) => new       
                //   {
                //       客户ID = a.客户ID,
                //       订单日期 = a.订单日期,
                //       客户名称 = a.客户名称,
                //       订货数量 = a.订货数量,
                //       订货金额 = a.订货金额,
                //       取消数量 = a.取消数量,
                //       取消金额 = a.取消金额,
                //       发货数量 = b.发货数量,
                //       发货金额 = b.发货金额,
                //       退货数量 = 0.0,
                //       退货金额 = 0.0
                //   }).ToList();
                #endregion
                this.Invoke(new Action(() =>
                {


                    uirichtxt.AppendText(DateTime.Now.ToShortTimeString() + ":获取发货数据完成\r\n");

                }));
                #endregion

                //通过原始单号追溯退货数据
                #region

                var saleStockReturns = dbContextEc.SaleReturnStockInfoDb.GetList(a => Srcs.Contains(a.tid)).
                   Select(p => new
                   {
                       src_tid = p.tid,
                       totalnum = p.goods_count,
                       totalamount = p.actual_refund_amount
                   }).ToList();
                var returnInfogroup = orResultforSrc.Join(
                     saleStockReturns,
                     a => a.原始单号,
                     b => b.src_tid,
                     (a, b) => new
                     {
                         客户ID = a.客户ID,
                         订单日期 = a.订单日期,
                         原始单号 = a.原始单号,
                         退货数量 = b.totalnum,
                         退货金额 = b.totalamount
                     }).ToList().GroupBy(a => new { a.客户ID, a.订单日期 }).Select(b => new
                     {
                         客户ID = b.Key.客户ID,
                         订单日期 = b.Key.订单日期,
                         退货数量 = (decimal)b.Sum(x => x.退货数量),
                         退货金额 = (decimal)b.Sum(x => x.退货金额)

                     }).ToList();

                var sendandreturnInfos = (from a in sendInfos
                                          join r in returnInfogroup
                                          on new { a.客户ID, a.订单日期 } equals new { r.客户ID, r.订单日期 } into rs
                                          from b in rs.DefaultIfEmpty()
                                          select new
                                          {
                                              客户ID = a.客户ID,
                                              订单日期 = a.订单日期,
                                              客户名称 = a.客户名称,
                                              订货数量 = a.订货数量,
                                              订货金额 = a.订货金额,
                                              取消数量 = a.取消数量,
                                              取消金额 = a.取消金额,
                                              发货数量 = a.发货数量,
                                              发货金额 = a.发货金额,
                                              退货数量 = b?.退货数量,
                                              退货金额 = b?.退货金额
                                          }).ToList();

                #endregion
                this.Invoke(new Action(() =>
                {

                    dataGridView4.DataSource = sendandreturnInfos;
                    //uiDataGridView3.DataSource = sendInfos;
                    //uiDataGridView4.DataSource = returnInfogroup;
                    uirichtxt.AppendText(DateTime.Now.ToShortTimeString() + ":获取退货数据完成\r\n");

                }));




            }
            catch (Exception ex)
            {
                UIMessageBox.Show(ex.Message);

            }
            this.Invoke(new Action(() =>
            {
                uiButton4.Text = "统计";
                uiButton4.Enabled = true;

            }));
        }

        private void Form5_Load(object sender, EventArgs e)
        {
            sdt.Value = DateTime.Today;
            edt.Value = DateTime.Today;
            dzsdt.Value = DateTime.Today;
            dzedt.Value = DateTime.Today;
            ssdt.Value = DateTime.Today;
            sedt.Value = DateTime.Today;
            rsdt.Value = DateTime.Today;
            redt.Value = DateTime.Today;
            if (wflogin.FUserType.Trim() != "3")
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

            // 关闭所有 DataGridView 的自动列宽/行高，防止数据绑定时撑出容器边界
            var allGrids = new System.Windows.Forms.DataGridView[]
            {
                uiDataGridView1, uiDataGridView5,
                dataGridView1, dataGridView2, dataGridView3, dataGridView4,
                dataGridView5, dataGridView6, dataGridView7, dataGridView8,
                dataGridView9, dataGridView10, dataGridView11, dataGridView12,
                dataGridView13, dataGridView14, dataGridView15
            };
            foreach (var dgv in allGrids)
            {
                dgv.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.None;
                dgv.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.None;
            }

            // dataGridView2：父容器 tabPage6，顶部有工具栏约 110px，填满剩余空间
            Action fixDgv2 = () =>
            {
                int top = dataGridView2.Location.Y;
                int h = tabPage6.ClientSize.Height - top;
                if (h > 0) dataGridView2.Size = new System.Drawing.Size(tabPage6.ClientSize.Width - dataGridView2.Location.X - 3, h);
            };
            fixDgv2();
            tabPage6.Resize += (s, ev) => fixDgv2();

            // dataGridView10：右边不能覆盖 richTextBox1（x=1116），左边从 x=503 开始
            // dataGridView9 / dataGridView10 / richTextBox1 三者高度保持一致
            Action fixDgv10 = () =>
            {
                int top = dataGridView10.Location.Y;
                int right = richTextBox1.Location.X - 3;
                int h = tabPage10.ClientSize.Height - top - 3;
                if (h <= 0) return;
                int w10 = right - dataGridView10.Location.X;
                int w9  = dataGridView9.Width; // 宽度不变，只同步高度
                if (w10 > 0) dataGridView10.Size = new System.Drawing.Size(w10, h);
                dataGridView9.Size = new System.Drawing.Size(w9, h);
                richTextBox1.Size  = new System.Drawing.Size(richTextBox1.Width, h);
            };
            fixDgv10();
            tabPage10.Resize += (s, ev) => fixDgv10();

            // dataGridView11：右边不能覆盖 richTextBox2（x=1111），左边从 x=502 开始
            // dataGridView13 / dataGridView11 / richTextBox2 三者高度保持一致
            Action fixDgv11 = () =>
            {
                int top = dataGridView11.Location.Y;
                int right = richTextBox2.Location.X - 3;
                int h = tabPage11.ClientSize.Height - top - 3;
                if (h <= 0) return;
                int w11  = right - dataGridView11.Location.X;
                int w13  = dataGridView13.Width; // 宽度不变，只同步高度
                if (w11 > 0) dataGridView11.Size = new System.Drawing.Size(w11, h);
                dataGridView13.Size = new System.Drawing.Size(w13, h);
                richTextBox2.Size   = new System.Drawing.Size(richTextBox2.Width, h);
            };
            fixDgv11();
            tabPage11.Resize += (s, ev) => fixDgv11();
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
                RestClient client = new RestClient("http://120.78.210.159:3304/api/Wdt/getOutStockInfo");
                RestRequest request = new RestRequest();
                request.AddHeader("Content-Type", "application/json");
                request.Method = Method.Post;
                RestResponse response;

                var obj = new
                {
                    custName = uiTextBox4.Text,
                    sdate = Convert.ToDateTime(Convert.ToDateTime(ssdt.Value).ToString("yyyy - MM - dd") + " 00:00:00"),
                    edate = Convert.ToDateTime(Convert.ToDateTime(sedt.Value).ToString("yyyy - MM - dd") + " 23:59:59"),
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
                int custId = Convert.ToInt32(dataGridView4.Rows[dataGridView4.CurrentRow.Index].Cells["客户ID"].Value);
                string sellername = dataGridView4.Rows[dataGridView4.CurrentRow.Index].Cells["分销商名称"].Value.ToString();
                this.Invoke(new Action(() =>
                {

                    uiButton6.Text = "统计中...";
                    uiButton6.Enabled = false;

                    uirichtxt.Clear();
                    uirichtxt.AppendText(DateTime.Now.ToShortTimeString() + ":开始统计明细...\r\n");
                }));
                RestClient client = new RestClient("http://120.78.210.159:3304/api/Wdt/getBillDetail");
                RestRequest request = new RestRequest();
                request.AddHeader("Content-Type", "application/json");
                request.Method = Method.Post;
                RestResponse response;

                var obj = new { fdate = fdate, custId = custId, sellername = sellername };
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
        private void getBillsDetail()
        {
            DateTime fdate = Convert.ToDateTime(dataGridView4.Rows[dataGridView4.CurrentRow.Index].Cells["订单日期"].Value);
            int custId = Convert.ToInt32(dataGridView4.Rows[dataGridView4.CurrentRow.Index].Cells["客户ID"].Value);

            this.Invoke(new Action(() =>
            {

                uiButton6.Text = "查询中...";
                uiButton6.Enabled = false;

                uirichtxt.Clear();
                uirichtxt.AppendText(DateTime.Now.ToShortTimeString() + ":开始查询...\r\n");
            }));
            try
            {

                var orResult = dbContext.Db.Queryable<U_Plans_WDTOrderItems, U_Plans_WDTOrder, V_CUST, V_MAT, U_Plans_suites>((a, b, c, d, f) => new object[]
                {
                JoinType.Left,a.FID==b.FID,
                JoinType.Left,b.FCustId==c.FCUSTID,
                JoinType.Left,a.Good_No==d.FNUMBER && d.FUSEORGID==100018,
                JoinType.Left,a.Good_No==f.suite_no

               }).Where((a, b, c, d, f) =>
                   c.FCUSTID == custId && b.FDate == Convert.ToDateTime(fdate.ToShortDateString())
                 ).Select((a, b, c, d, f) => new
                 {

                     订单日期 = b.FDate,
                     订单编号 = b.FOrderNo,
                     原始单号 = a.WDTSrcbillno,
                     客户名称 = c.FNAME,
                     收货人 = a.Recipienter,
                     收货人手机号 = a.Tel,
                     收货地址 = a.Address,
                     商品编码 = a.Good_No,
                     商品名称 = f.suite_name != null ? f.suite_name : d.FNAME,
                     商品规格 = d.FSpecification != null ? d.FSpecification : "",
                     商品数量 = a.Qty,
                     发货快递 = "",
                     单号 = a.logistics_no,
                     收货状态 = a.FStatus,
                     客户编码 = b.FCustId,
                     商品金额 = a.RecAmount



                 }).ToList();
                //订货数据

                this.Invoke(new Action(() =>
                {

                    dataGridView5.DataSource = orResult;
                    uirichtxt.AppendText(DateTime.Now.ToShortTimeString() + ":获取订货数据完成\r\n");

                }));


                //原始单号集合
                var orResultforSrc = orResult.Where(a => a.收货状态 != "已取消").GroupBy(p => new
                {
                    p.订单日期,
                    p.客户名称,
                    p.客户编码,
                    p.原始单号
                }
               ).Select((a) => new
               {
                   客户名称 = a.Key.客户名称,
                   客户ID = a.Key.客户编码,
                   订单日期 = a.Key.订单日期,
                   原始单号 = a.Key.原始单号,
                   发货金额 = 0,
                   发货数量 = 0

               }).Distinct().ToList();
                //通过原始单号去核查发货数据
                #region
                var Srcs = orResultforSrc.Select(s => s.原始单号).ToArray();

                var saleStockOutItems = dbContextEc.SaleStockOutItemDb.GetList(a => Srcs.Contains(a.src_tid)).
                    Select(p => new
                    {
                        原始单号 = p.src_tid,
                        发货单号 = p.order_no,
                        商品编码 = p.goods_no,
                        发货数量 = p.num,
                        发货金额 = p.share_amount
                    }).ToList();

                this.Invoke(new Action(() =>
                {

                    dataGridView6.DataSource = saleStockOutItems;
                    uirichtxt.AppendText(DateTime.Now.ToShortTimeString() + ":获取发货数据完成\r\n");

                }));
                #endregion

                //通过原始单号追溯退货数据
                #region

                var saleStockReturns = dbContextEc.SaleReturnStockItemDb.GetList(a => Srcs.Contains(a.tid)).
                   Select(p => new
                   {
                       原始单号 = p.tid,
                       退货入库单号 = p.order_no,
                       退货数量 = p.num,
                       退货金额 = p.num * p.src_price
                   }).ToList();


                #endregion
                _ = this.Invoke(new Action(() =>
                  {

                      dataGridView7.DataSource = saleStockReturns;
                      uirichtxt.AppendText(DateTime.Now.ToShortTimeString() + ":获取退货数据完成\r\n");

                  }));

            }
            catch (Exception ex)
            {
                UIMessageBox.Show(ex.Message);
                uiButton6.Text = "明细";
                uiButton6.Enabled = true;

            }
            this.Invoke(new Action(() =>
            {
                uiButton6.Text = "明细";
                uiButton6.Enabled = true;

            }));
        }

        //private void uiDataGridView2_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        //{
        //    Rectangle rectangle = new Rectangle(e.RowBounds.Location.X,
        //      e.RowBounds.Location.Y,
        //      uiDataGridView2.RowHeadersWidth,
        //      e.RowBounds.Height);

        //    TextRenderer.DrawText(e.Graphics, (e.RowIndex + 1).ToString(),
        //        uiDataGridView2.RowHeadersDefaultCellStyle.Font,
        //        rectangle,
        //        uiDataGridView2.RowHeadersDefaultCellStyle.ForeColor,
        //        TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
        //}

        //private void uiDataGridView4_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        //{
        //    Rectangle rectangle = new Rectangle(e.RowBounds.Location.X,
        //      e.RowBounds.Location.Y,
        //      uiDataGridView4.RowHeadersWidth,
        //      e.RowBounds.Height);

        //    TextRenderer.DrawText(e.Graphics, (e.RowIndex + 1).ToString(),
        //        uiDataGridView4.RowHeadersDefaultCellStyle.Font,
        //        rectangle,
        //        uiDataGridView4.RowHeadersDefaultCellStyle.ForeColor,
        //        TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
        //}

        //private void uiDataGridView3_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        //{
        //    Rectangle rectangle = new Rectangle(e.RowBounds.Location.X,
        //      e.RowBounds.Location.Y,
        //      uiDataGridView3.RowHeadersWidth,
        //      e.RowBounds.Height);

        //    TextRenderer.DrawText(e.Graphics, (e.RowIndex + 1).ToString(),
        //        uiDataGridView3.RowHeadersDefaultCellStyle.Font,
        //        rectangle,
        //        uiDataGridView3.RowHeadersDefaultCellStyle.ForeColor,
        //        TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
        //}

        private void Form5_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
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

                    }
                    ;

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
                        client = new RestClient("http://120.78.210.159:3304/api/Wdt/GetCustWDTPriceMatList");
                        request = new RestRequest();
                        request.AddHeader("Content-Type", "application/json; charset=utf-8");
                        request.Method = Method.Post;

                        var matInfo = new { custid = wflogin.fcustId.Trim(), goodno = m };
                        var str = JsonConvert.SerializeObject(matInfo);

                        //request.AddJsonBody(str);
                        request.AddBody(str);
                        response = client.Execute(request);
                        var resultobj1 = JObject.Parse(response.Content);


                        if (resultobj1["code"].ToString() == "1")
                        {
                            noerrorcount++;
                            errorcount++;
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
                            noerrorcount++;
                            errorcount++;
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

        private void uiButton9_Click(object sender, EventArgs e)
        {



        }

        private async void uiButton9_Click_1(object sender, EventArgs e)
        {
            Task.Run(upMat);
        }
        private void upMat()
        {
            try
            {
                this.Invoke(new Action(() =>
                {
                    uiButton9.Text = "同步中...";
                    uiButton9.Enabled = false;
                    uiRichTextBox2.AppendText("开始同步...\r\n");
                }));
                DateTime aprtime = uiDatePicker1.Value;

                RestClient client = new RestClient($"http://120.78.210.159:3304/api/Wdt/GetMatList?FApproveDate= {aprtime}");
                RestRequest request = new RestRequest();
                request.AddHeader("Content-Type", "application/json; charset=utf-8");

                request.Method = Method.Post;
                RestResponse response;

                response = client.Execute(request);

                var ss = "1,2,3,4,5,6".Split(",").ToList();

                var result = JArray.Parse(response.Content).Where(a => ss.Contains(a["FNUMBER"].ToString().Left(1))).ToList();

                if (result.Count > 0)
                {
                    this.Invoke(new Action(() =>
                    {
                        uiRichTextBox2.AppendText(result.Count.ToString() + "条记录符合上传条件" + "\r\n");
                    }));
                    result.ForEach(a =>
                    {
                        Thread.Sleep(1000);
                        this.Invoke(new Action(() =>
                        {
                            uiRichTextBox2.AppendText(a["FNUMBER"].ToString() + "查询该物料是否存在于旺店通" + "\r\n");
                        }));
                        Dictionary<string, string> dt = new Dictionary<string, string>();
                        dt.Add("goods_no", a["FNUMBER"].ToString());
                        var resultobj = JObject.Parse(GetWdtDataBLL.GetDataFromApi(dt, "goods_query.php"));

                        if (resultobj["total_count"].ToString() != "1")
                        {
                            this.Invoke(new Action(() =>
                            {
                                uiRichTextBox2.AppendText(a["FNUMBER"].ToString() + "不存在于旺店通!" + "\r\n");
                            }));
                            string fname = result[0]["FNAME"].ToString();
                            var goods_list = new[]
                            {
                                 new
                                 {
                                        goods_no=a["FNUMBER"].ToString(),
                                        goods_type=1,
                                        goods_name=a["FNAME"].ToString(),

                                        spec_list=new[]
                                        {
                                            new
                                            {
                                                spec_no=a["FNUMBER"].ToString(),
                                                spec_code =a["FNUMBER"].ToString(),
                                                spec_name = a["FNAME"].ToString(),
                                                barcode=a["FNUMBER"].ToString(),
                                                weight=Convert.ToDecimal(a["FWeight"].ToString())
                                            }

                                        }
                                 }
                            };
                            dt = new Dictionary<string, string>();

                            dt.Add("goods_list", JsonConvert.SerializeObject(goods_list));
                            resultobj = JObject.Parse(GetWdtDataBLL.GetDataFromApi(dt, "goods_push.php"));
                            if (resultobj["code"].ToString() == "0")
                            {
                                this.Invoke(new Action(() =>
                                {
                                    uiRichTextBox2.AppendText(a["FNUMBER"].ToString() + "同步成功!" + "\r\n");
                                }));

                            }
                            else
                            {
                                this.Invoke(new Action(() =>
                                {
                                    uiRichTextBox2.AppendText(a["FNUMBER"].ToString() + "同步失败!" + "\r\n");
                                }));

                            }
                        }
                        else
                        {
                            this.Invoke(new Action(() =>
                            {
                                uiRichTextBox2.AppendText(a["FNUMBER"].ToString() + "已存在于旺店通!" + "\r\n");
                            }));
                        }

                    });
                    this.Invoke(new Action(() =>
                    {
                        uiRichTextBox2.AppendText("没有需要同步的货品" + "\r\n");
                        uiButton9.Text = "同步货品";
                        uiButton9.Enabled = true;
                        uiRichTextBox2.AppendText("结束同步\r\n");
                    }));

                }
                else
                {
                    this.Invoke(new Action(() =>
                    {
                        uiRichTextBox2.AppendText("没有需要同步的货品" + "\r\n");
                        uiButton9.Text = "同步货品";
                        uiButton9.Enabled = true;
                        uiRichTextBox2.AppendText("结束同步\r\n");
                    }));

                }

            }
            catch (Exception er)
            {
                this.Invoke(new Action(() =>
                {
                    uiRichTextBox2.AppendText(er.Message + "\r\n");
                    uiButton9.Text = "同步货品";
                    uiButton9.Enabled = true;
                    uiRichTextBox2.AppendText("结束同步\r\n");
                }));

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
                            Remark = a["客服备注"] == null ? "" : a["客服备注"].ToString(),
                            UserID = Convert.ToInt32(wflogin.fuserId.Trim()),
                            logistics_type = (int)(lst == null ? 0 : (lst.Where(l => l["logistics_typeName"].ToString() == a["物流类型"].ToString()).ToList().Count > 0 ? (lst.Where(l => l["logistics_typeName"].ToString() == a["物流类型"].ToString()).First()["logistics_type"]) : 0)),
                            WDTAddress = a["原地址"] == null ? a["地址"].ToString() : a["原地址"].ToString()

                        });
                    });
                    //this.Invoke(new Action(() =>
                    //{
                    //    uiRichTextBox1.AppendText(JsonConvert.SerializeObject(lstOrderItems) +"\r\n");
                    //}));
                    RestClient client = new RestClient("http://120.78.210.159:3304/api/Wdt/SaveWDTOrderAndItemApi");
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

        private void uiButton15_Click(object sender, EventArgs e)
        {
            Task.Run(getOrderData);
        }

        private void uiButton10_Click(object sender, EventArgs e)
        {

        }

        private void uiDataGridView5_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            //Rectangle rectangle = new Rectangle(e.RowBounds.Location.X,
            //    e.RowBounds.Location.Y,
            //    uiDataGridView5.RowHeadersWidth,
            //    e.RowBounds.Height);

            //TextRenderer.DrawText(e.Graphics, (e.RowIndex + 1).ToString(),
            //    uiDataGridView5.RowHeadersDefaultCellStyle.Font,
            //    rectangle,
            //    uiDataGridView1.RowHeadersDefaultCellStyle.ForeColor,
            //    TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
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
            string fOrderuserid = dataGridView2.Rows[dataGridView2.CurrentRow.Index].Cells["制单人ID"].Value.ToString();
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
            if (wflogin.FUserType == "3")
            {
                if (fOrderuserid != wflogin.fuserId)
                {
                    UIMessageBox.Show("提交失败，订单是分销商创建，只能对应分销商提交!");
                    return;
                }
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

        private void uiButton1_Click_1(object sender, EventArgs e)
        {
            string fOrderuserid = dataGridView2.Rows[dataGridView2.CurrentRow.Index].Cells["制单人ID"].Value.ToString();
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

            if (wflogin.FUserType == "3")
            {
                if (fOrderuserid != wflogin.fuserId)
                {
                    UIMessageBox.Show("删除明细失败，订单是分销商创建，只能对应分销商删除!");
                    return;
                }
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

        private void uiButton10_Click_1(object sender, EventArgs e)
        {
            string fOrderuserid = dataGridView2.Rows[dataGridView2.CurrentRow.Index].Cells["制单人ID"].Value.ToString();
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
            if (wflogin.FUserType == "3")
            {
                if (fOrderuserid != wflogin.fuserId)
                {
                    UIMessageBox.Show("整单删除失败，订单是分销商创建，只能对应分销商删除!");
                    return;
                }
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

        //private void uiDataGridView6_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        //{
        //    Rectangle rectangle = new Rectangle(e.RowBounds.Location.X,
        //      e.RowBounds.Location.Y,
        //      uiDataGridView6.RowHeadersWidth,
        //      e.RowBounds.Height);

        //    TextRenderer.DrawText(e.Graphics, (e.RowIndex + 1).ToString(),
        //        uiDataGridView6.RowHeadersDefaultCellStyle.Font,
        //        rectangle,
        //        uiDataGridView6.RowHeadersDefaultCellStyle.ForeColor,
        //        TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
        //}

        //private void uiDataGridView7_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        //{
        //    Rectangle rectangle = new Rectangle(e.RowBounds.Location.X,
        //      e.RowBounds.Location.Y,
        //      uiDataGridView7.RowHeadersWidth,
        //      e.RowBounds.Height);

        //    TextRenderer.DrawText(e.Graphics, (e.RowIndex + 1).ToString(),
        //        uiDataGridView7.RowHeadersDefaultCellStyle.Font,
        //        rectangle,
        //        uiDataGridView7.RowHeadersDefaultCellStyle.ForeColor,
        //        TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
        //}

        //private void uiDataGridView8_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        //{
        //    Rectangle rectangle = new Rectangle(e.RowBounds.Location.X,
        //      e.RowBounds.Location.Y,
        //      uiDataGridView8.RowHeadersWidth,
        //      e.RowBounds.Height);

        //    TextRenderer.DrawText(e.Graphics, (e.RowIndex + 1).ToString(),
        //        uiDataGridView8.RowHeadersDefaultCellStyle.Font,
        //        rectangle,
        //        uiDataGridView8.RowHeadersDefaultCellStyle.ForeColor,
        //        TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
        //}

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
        private void uiButton12_Click(object sender, EventArgs e)
        {
            try
            {
                Dictionary<string, string> dt = new Dictionary<string, string>();
                string txtname = uiTxtName.Text.ToString().Trim();

                // dt.Add("logistics_no", "");
                var resultobj = JObject.Parse(GetWdtDataBLL.GetDataFromApi(dt, "logistics.php"));
                if (resultobj["code"].ToString() == "0")

                {
                    uirichtxt.AppendText(JsonConvert.SerializeObject(resultobj["logistics_list"]));
                    var logistics_type = JArray.Parse(JsonConvert.SerializeObject(resultobj["logistics_list"])).Where(a => a["is_disabled"].ToString() == "0" && a["logistics_type"].ToString() != "0" && getLogisticsName(Convert.ToInt32(a["logistics_type"])).Contains(txtname)).Select(a =>
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

            string fOrderuserid = dataGridView2.Rows[dataGridView2.CurrentRow.Index].Cells["制单人ID"].Value.ToString();
            if (wflogin.FUserType == "3")
            {
                if (fOrderuserid != wflogin.fuserId)
                {
                    UIMessageBox.Show("取消失败，订单是分销商创建，只能对应分销商取消!");
                    return;
                }
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

        private void uiButton16_Click(object sender, EventArgs e)
        {

            string fOrderuserid = dataGridView2.Rows[dataGridView2.CurrentRow.Index].Cells["制单人ID"].Value.ToString();
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
            if (wflogin.FUserType == "3")
            {
                if (fOrderuserid != wflogin.fuserId)
                {
                    UIMessageBox.Show("同步失败，订单是分销商创建，只能对应分销商同步该数据!");
                    return;
                }
            }
            panel1.Visible = true;
            panel1.BringToFront();
            //uiPanel1.Show();



        }

        private void uiButton17_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "Excel 文件(*.xls)| *.xls";
            saveFileDialog1.ShowDialog();

        }

        private void uiButton18_Click(object sender, EventArgs e)
        {
            string txtname = uiTxtName.Text.ToString().Trim();

            Dictionary<string, string> dt = new Dictionary<string, string>();
            dt = new Dictionary<string, string>();
            dt.Add("page_size", 100.ToString());
            var resultobj = JObject.Parse(GetWdtDataBLL.GetDataFromApi(dt, "warehouse_query.php"));
            if (resultobj["code"].ToString() == "0")
            {
                var warehouses = JArray.Parse(JsonConvert.SerializeObject(resultobj["warehouses"])).Where(a => a["is_disabled"].ToString() == "0" && a["name"].ToString().Contains(txtname)).Select(a =>
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
            try
            {
                string txtname = uiTxtName.Text.ToString().Trim();

                Dictionary<string, string> dt = new Dictionary<string, string>();
                dt = new Dictionary<string, string>();
                dt.Add("page_size", 100.ToString());
                var resultobj = JObject.Parse(GetWdtDataBLL.GetDataFromApi(dt, "shop.php"));
                if (resultobj["code"].ToString() == "0")
                {
                    var shoplist = JArray.Parse(JsonConvert.SerializeObject(resultobj["shoplist"])).Where(a => a["is_disabled"].ToString() == "0" && a["shop_name"].ToString().Contains(txtname)).Select(a =>
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
            catch (Exception ex)
            {

                this.Invoke(new Action(() =>
                {
                    uiRichTextBox2.AppendText("查询失败!" + ex.Message.ToString() + "\r\n");
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
            RestClient client = new RestClient("http://120.78.210.159:3304/api/Wdt/getReStockInfo");
            RestRequest request = new RestRequest();
            request.AddHeader("Content-Type", "application/json");
            request.Method = Method.Post;
            RestResponse response;

            var obj = new
            {
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

        private void uiButton22_Click(object sender, EventArgs e)
        {
            panel1.Visible = false;
            panel1.SendToBack();
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

        private void uiDataGridView5_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            var datas = JArray.FromObject(uiDataGridView5.DataSource).Select(a => new
            {
                客户名称 = a["客户名称"].ToString(),
                订单编号 = a["订单编号"].ToString(),
                订单日期 = a["订单日期"].ToString(),
                店铺 = a["店铺"].ToString(),
                发货仓 = a["发货仓"].ToString(),
                订单状态 = a["订单状态"].ToString(),
                平台单号 = a["平台单号"].ToString(),
                原始单号 = a["原始单号"].ToString(),
                收件人 = a["收件人"].ToString(),
                电话 = a["电话"].ToString(),
                旺店通状态 = a["旺店通状态"].ToString(),
                物流单号 = a["物流单号"].ToString(),
                发货时间 = a["发货时间"],
                商家编码 = a["商家编码"].ToString(),
                发货数量 = a["发货数量"].ToString(),
                同步旺店通结果 = a["同步旺店通结果"].ToString(),
                客服备注 = a["客服备注"].ToString(),
                订单ID = a["订单ID"].ToString(),
                明细ID = a["明细ID"].ToString(),

            }).OrderBy(a => a.明细ID).ToList();
            uiDataGridView5.DataSource = datas;
        }

        private void 记录排训_Opening(object sender, CancelEventArgs e)
        {

        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            var datas = JArray.FromObject(uiDataGridView5.DataSource).Select(a => new
            {
                客户名称 = a["客户名称"].ToString(),
                订单编号 = a["订单编号"].ToString(),
                订单日期 = a["订单日期"].ToString(),
                店铺 = a["店铺"].ToString(),
                发货仓 = a["发货仓"].ToString(),
                订单状态 = a["订单状态"].ToString(),
                平台单号 = a["平台单号"].ToString(),
                原始单号 = a["原始单号"].ToString(),
                收件人 = a["收件人"].ToString(),
                电话 = a["电话"].ToString(),
                旺店通状态 = a["旺店通状态"].ToString(),
                物流单号 = a["物流单号"].ToString(),
                发货时间 = a["发货时间"],
                商家编码 = a["商家编码"].ToString(),
                发货数量 = a["发货数量"].ToString(),
                同步旺店通结果 = a["同步旺店通结果"].ToString(),
                客服备注 = a["客服备注"].ToString(),
                订单ID = a["订单ID"].ToString(),
                明细ID = a["明细ID"].ToString(),

            }).OrderByDescending(a => a.明细ID).ToList();
            uiDataGridView5.DataSource = datas;
        }

        private void uiButton23_Click(object sender, EventArgs e)
        {
            RestClient client = new RestClient("http://120.78.210.159:3304/api/Wdt/getBanlance");
            RestRequest request = new RestRequest();
            //request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddHeader("Content-Type", "application/json");
            request.Method = Method.Post;
            RestResponse response;

            var obj = new { custName = uitxtdzCustName.Text.Trim() };

            request.AddBody(JsonConvert.SerializeObject(obj));
            response = client.Execute(request);
            var result = JObject.Parse(response.Content);
            uirichtxt.Clear();
            if (result["code"].ToString() == "1")
            {
                var lst = JArray.Parse(result["sdataList"].ToString());
                if (lst.Count > 0)
                {
                    lst.ForEach(a =>
                    {
                        uirichtxt.AppendText(a["客户"].ToString() + "的可用余额为:" + a["可用余额"].ToString() + "\r\n");
                    });
                    UIMessageBox.Show(result["mess"].ToString());
                }
                else
                {
                    UIMessageBox.Show("没有查到记录！");
                }
            }
            else
            {
                UIMessageBox.Show(result["mess"].ToString());
            }
        }

        private void uiButton24_Click(object sender, EventArgs e)
        {
            RestClient client = new RestClient("http://120.78.210.159:3304/api/Wdt/GetCustWDTPriceList");
            RestRequest request = new RestRequest();
            request.AddHeader("Content-Type", "application/json; charset=utf-8");
            request.Method = Method.Post;
            RestResponse response;
            var obj = new { custName = uitxtdzCustName.Text, selname = uiTxtName.Text.ToString().Trim() };

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

        private void uiButton25_Click(object sender, EventArgs e)
        {
            saveFileDialog3.Filter = "Excel 文件(*.xls)| *.xls";
            saveFileDialog3.ShowDialog();
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

                                }
                                ;

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

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

            //Rectangle myRectangle = new Rectangle(0, 0, this.Width, this.Height);
            //ControlPaint.DrawBorder(e.Graphics, myRectangle,
            //    System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(160)))), ((int)(((byte)(255))))), 1, ButtonBorderStyle.Solid,  //left
            //    System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(160)))), ((int)(((byte)(255))))), 1, ButtonBorderStyle.Solid,  //top
            //    System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(160)))), ((int)(((byte)(255))))), 1, ButtonBorderStyle.Solid,  //right
            //    System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(160)))), ((int)(((byte)(255))))), 1, ButtonBorderStyle.Solid   //bottom
            //);
        }



        /// <summary>
        /// 点击列头部
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        private void uiButton29_Click(object sender, EventArgs e)
        {
            saveFileDialog5.Filter = "Excel 文件(*.xls)| *.xls";
            saveFileDialog5.ShowDialog();
        }

        private void saveFileDialog5_FileOk(object sender, CancelEventArgs e)
        {
            try
            {
                var datas = JArray.FromObject(dataGridView1.DataSource);

                if (saveFileDialog5.FileName != "")
                {
                    string tfilepath = saveFileDialog5.FileName;

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

        private void uiButton31_Click(object sender, EventArgs e)
        {
            dataGridView10.DataSource = null; // 先清空
            dataGridView12.DataSource = null; // 先清空
            uiTabControl1.SelectedIndex = 6;
            DeduplicateAndRebindDataSource();

        }
        private List<object> ExtractAndDeduplicateOrderData()
        {
            if (dataGridView2.DataSource is IEnumerable<object> dataSource)
            {
                var extractedData = dataSource.Select(item =>
                {
                    var itemType = item.GetType();
                    return new
                    {
                        原始单号 = itemType.GetProperty("原始单号")?.GetValue(item)?.ToString() ?? "",
                        地址 = itemType.GetProperty("地址")?.GetValue(item)?.ToString() ?? "",
                        收件人 = itemType.GetProperty("收件人")?.GetValue(item)?.ToString() ?? "",
                        电话 = itemType.GetProperty("电话")?.GetValue(item)?.ToString() ?? "",
                        旺店通状态 = itemType.GetProperty("旺店通状态")?.GetValue(item)?.ToString() ?? "",
                        发货仓 = itemType.GetProperty("发货仓")?.GetValue(item)?.ToString() ?? ""
                    };
                })
                .Where(x => !string.IsNullOrEmpty(x.原始单号) && x.旺店通状态 != "已取消" && x.发货仓 == "唯品会电商仓") // 过滤空的原始单号
                .GroupBy(x => x.原始单号) // 按原始单号分组
                .Select(g => g.First()) // 取每组的第一条记录
                .ToList<object>();

                return extractedData;
            }

            return new List<object>();
        }
        private void DeduplicateAndRebindDataSource()
        {
            try
            {
                // 提取并去重数据
                var deduplicatedData = ExtractAndDeduplicateOrderData();

                // 重新设置数据源
                dataGridView9.DataSource = null; // 先清空
                dataGridView9.DataSource = deduplicatedData;
                dataGridView13.DataSource = null; // 先清空
                dataGridView13.DataSource = deduplicatedData;
                // UIMessageBox.Show($"去重完成！当前显示 {deduplicatedData.Count} 条唯一订单数据");

            }
            catch (Exception ex)
            {
                UIMessageBox.Show($"去重处理时发生错误: {ex.Message}");

            }
        }
        private List<object> ProcessSuiteGoods(List<object> orderTradeGoods)
        {
            var finalResults = new List<object>();
            var uniqueSuites = new HashSet<string>();

            // 获取所有唯一的组合装号
            foreach (var item in orderTradeGoods)
            {
                string suiteNo = GetPropertyValue(item, new[] { "组合装号", "货号", "goods_no" });
                if (!string.IsNullOrEmpty(suiteNo))
                {
                    uniqueSuites.Add(suiteNo);
                }
            }

            this.Invoke(new Action(() =>
            {
                richTextBox2.AppendText($"发现 {uniqueSuites.Count} 个唯一组合装，开始查询组合装详情...\r\n");
            }));

            // 查询所有组合装信息并缓存
            int suiteIndex = 0;
            foreach (string suiteNo in uniqueSuites)
            {
                suiteIndex++;
                if (!suiteInfoCache.ContainsKey(suiteNo))
                {
                    this.Invoke(new Action(() =>
                    {
                        richTextBox2.AppendText($"查询组合装 {suiteIndex}/{uniqueSuites.Count}: {suiteNo}\r\n");
                    }));

                    var suiteInfo = GetWdtSuitesInfo(suiteNo);
                    suiteInfoCache[suiteNo] = suiteInfo;

                    this.Invoke(new Action(() =>
                    {
                        richTextBox2.AppendText($"组合装{suiteNo}缓存到本地\r\n");
                    }));

                    // 让UI有机会更新
                    Application.DoEvents();
                }
                else
                {
                    this.Invoke(new Action(() =>
                    {
                        richTextBox2.AppendText($"使用缓存的组合装信息: {suiteNo}\r\n");
                    }));
                }
            }

            // 处理每个订单商品
            foreach (var orderItem in orderTradeGoods)
            {
                string 原始单号 = GetPropertyValue(orderItem, new[] { "原始单号", "tid" });
                string 组合装号 = GetPropertyValue(orderItem, new[] { "组合装号", "货号", "goods_no" });
                string 组合装商家编码 = GetPropertyValue(orderItem, new[] { "商家编码", "spec_no" });
                string 组合装名称 = GetPropertyValue(orderItem, new[] { "货品名称", "goods_name" });
                string 组合装数量字符串 = GetPropertyValue(orderItem, new[] { "组合装数量", "数量", "num" });

                int 组合装数量 = 0;
                if (int.TryParse(组合装数量字符串, out 组合装数量))
                {
                    // 检查缓存中是否有组合装信息
                    if (suiteInfoCache.ContainsKey(组合装号) && suiteInfoCache[组合装号].Count > 0)
                    {
                        // 有组合装信息，拆分为单品
                        foreach (var suiteGoods in suiteInfoCache[组合装号])
                        {
                            string 单品数量字符串 = GetPropertyValue(suiteGoods, new[] { "数量", "num" });
                            int 单品在组合装中的数量 = 0;

                            if (int.TryParse(单品数量字符串, out 单品在组合装中的数量))
                            {
                                var finalGoods = new
                                {
                                    原始单号 = 原始单号,
                                    货号 = GetPropertyValue(suiteGoods, new[] { "货号", "goods_no" }),
                                    商家编码 = GetPropertyValue(suiteGoods, new[] { "商家编码", "spec_no" }),
                                    货品名称 = GetPropertyValue(suiteGoods, new[] { "货品名称", "goods_name" }),
                                    单品数量 = (组合装数量 * 单品在组合装中的数量).ToString()
                                };
                                finalResults.Add(finalGoods);
                            }
                        }
                    }
                    else
                    {
                        // 没有组合装信息，直接存储原始数据
                        var directGoods = new
                        {
                            原始单号 = 原始单号,
                            货号 = 组合装号,
                            商家编码 = 组合装商家编码,
                            货品名称 = 组合装名称,
                            单品数量 = 组合装数量.ToString()
                        };
                        finalResults.Add(directGoods);
                    }
                }
                else
                {
                    this.Invoke(new Action(() =>
                    {
                        richTextBox2.AppendText($"转化组合装数量失败: {组合装数量字符串}\r\n");
                    }));
                }
            }

            this.Invoke(new Action(() =>
            {
                richTextBox2.AppendText($"转化组合装完成\r\n");
            }));

            return finalResults;
        }
        private List<object> ProcessSuiteGoodsForlog(List<object> orderTradeGoods)
        {
            var finalResults = new List<object>();
            var uniqueSuites = new HashSet<string>();

            // 获取所有唯一的组合装号
            foreach (var item in orderTradeGoods)
            {
                string suiteNo = GetPropertyValue(item, new[] { "组合装号", "货号", "goods_no" });
                if (!string.IsNullOrEmpty(suiteNo))
                {
                    uniqueSuites.Add(suiteNo);
                }
            }

            this.Invoke(new Action(() =>
            {
                richTextBox1.AppendText($"发现 {uniqueSuites.Count} 个唯一组合装，开始查询组合装详情...\r\n");
            }));

            // 查询所有组合装信息并缓存
            int suiteIndex = 0;
            foreach (string suiteNo in uniqueSuites)
            {
                suiteIndex++;
                if (!suiteInfoCache.ContainsKey(suiteNo))
                {
                    this.Invoke(new Action(() =>
                    {
                        richTextBox1.AppendText($"查询组合装 {suiteIndex}/{uniqueSuites.Count}: {suiteNo}\r\n");
                    }));

                    var suiteInfo = GetWdtSuitesInfo(suiteNo);
                    suiteInfoCache[suiteNo] = suiteInfo;

                    this.Invoke(new Action(() =>
                    {
                        richTextBox1.AppendText($"组合装{suiteNo}缓存到本地\r\n");
                    }));

                    // 让UI有机会更新
                    Application.DoEvents();
                }
                else
                {
                    this.Invoke(new Action(() =>
                    {
                        richTextBox1.AppendText($"使用缓存的组合装信息: {suiteNo}\r\n");
                    }));
                }
            }

            // 处理每个订单商品
            foreach (var orderItem in orderTradeGoods)
            {
                string 原始单号 = GetPropertyValue(orderItem, new[] { "原始单号", "tid" });
                string 组合装号 = GetPropertyValue(orderItem, new[] { "组合装号", "货号", "goods_no" });
                string 组合装商家编码 = GetPropertyValue(orderItem, new[] { "商家编码", "spec_no" });
                string 组合装名称 = GetPropertyValue(orderItem, new[] { "货品名称", "goods_name" });
                string 组合装数量字符串 = GetPropertyValue(orderItem, new[] { "组合装数量", "数量", "num" });

                int 组合装数量 = 0;
                if (int.TryParse(组合装数量字符串, out 组合装数量))
                {
                    // 检查缓存中是否有组合装信息
                    if (suiteInfoCache.ContainsKey(组合装号) && suiteInfoCache[组合装号].Count > 0)
                    {
                        // 有组合装信息，拆分为单品
                        foreach (var suiteGoods in suiteInfoCache[组合装号])
                        {
                            string 单品数量字符串 = GetPropertyValue(suiteGoods, new[] { "数量", "num" });
                            int 单品在组合装中的数量 = 0;

                            if (int.TryParse(单品数量字符串, out 单品在组合装中的数量))
                            {
                                var finalGoods = new
                                {
                                    原始单号 = 原始单号,
                                    组合装编码 = 组合装号,
                                    组合装名称 = 组合装名称,
                                    组合装数量 = 组合装数量.ToString(),
                                    货号 = GetPropertyValue(suiteGoods, new[] { "货号", "goods_no" }),
                                    商家编码 = GetPropertyValue(suiteGoods, new[] { "商家编码", "spec_no" }),
                                    货品名称 = GetPropertyValue(suiteGoods, new[] { "货品名称", "goods_name" }),
                                    单品数量 = (组合装数量 * 单品在组合装中的数量).ToString()
                                };
                                finalResults.Add(finalGoods);
                            }
                        }
                    }
                    else
                    {
                        // 没有组合装信息，直接存储原始数据
                        var directGoods = new
                        {
                            原始单号 = 原始单号,
                            组合装编码 = 组合装号,
                            组合装名称 = 组合装名称,
                            组合装数量 = 组合装数量.ToString(),
                            货号 = 组合装号,
                            商家编码 = 组合装商家编码,
                            货品名称 = 组合装名称,
                            单品数量 = 组合装数量.ToString()
                        };
                        finalResults.Add(directGoods);
                    }
                }
                else
                {
                    this.Invoke(new Action(() =>
                    {
                        richTextBox1.AppendText($"转化组合装数量失败: {组合装数量字符串}\r\n");
                    }));
                }
            }

            this.Invoke(new Action(() =>
            {
                richTextBox1.AppendText($"转化组合装完成\r\n");
            }));

            return finalResults;
        }
        private void uiButton30_Click(object sender, EventArgs e)
        {

            try
            {
                // 验证数据源
                if (dataGridView2.DataSource == null)
                {
                    MessageBox.Show("请先加载订单数据到主表格");
                    return;
                }

                // 清空结果表格并显示查询状态
                this.Invoke(new Action(() =>
                {
                    dataGridView10.DataSource = null;
                    dataGridView10.Rows.Clear();
                    richTextBox1.AppendText("=== 开始批量查询旺店通订单商品信息 ===\r\n");
                }));

                // 禁用查询按钮防止重复点击
                uiButton30.Enabled = false;

                var resultSuites = BatchQueryWithProgress(progressBar1);
                //var tradeGoods = new
                //{
                //    原始单号 = tradeTid,
                //    订单号 = "",
                //    组合装号 = goods["goods_no"]?.ToString() ?? "",
                //    商家编码 = goods["spec_no"]?.ToString() ?? "",
                //    货品名称 = goods["goods_name"]?.ToString() ?? "",
                //    组合装数量 = ((int)Convert.ToDecimal(goods["num"])).ToString() ?? ""
                //};
                //将组合装转成单品
                var results = ProcessSuiteGoodsForlog(resultSuites);
                //var directGoods = new
                //{
                //    原始单号 = 原始单号,
                //    货号 = 组合装号,
                //    商家编码 = 组合装商家编码,
                //    货品名称 = 组合装名称,
                //    单品数量 = 组合装数量.ToString()
                //};

                // 绑定结果并格式化表格
                this.Invoke(new Action(() =>
                {
                    if (results != null && results.Count > 0)
                    {
                        // 处理结果数据，添加箱规和箱数
                        var processedResults = ProcessResultsWithBoxInfo(results);

                        dataGridView10.DataSource = processedResults;

                        // 设置列头和格式
                        FormatDataGridView10();

                        // 统计信息
                        var uniqueOrders = processedResults.Cast<object>()
                            .Select(item => item.GetType().GetProperty("原始单号")?.GetValue(item)?.ToString())
                            .Distinct().Count();

                        richTextBox1.AppendText($"查询完成统计:\r\n");
                        richTextBox1.AppendText($"- 商品记录总数: {processedResults.Count} 条\r\n");
                        richTextBox1.AppendText($"- 涉及订单数量: {uniqueOrders} 个\r\n");
                        richTextBox1.AppendText($"- 结果已显示在表格中\r\n");
                        richTextBox1.AppendText("=== 查询完成 ===\r\n\r\n");

                        MessageBox.Show($"查询完成！\n商品记录: {processedResults.Count} 条\n订单数量: {uniqueOrders} 个",
                            "查询结果", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        richTextBox1.AppendText("未查询到任何商品数据\r\n");
                        richTextBox1.AppendText("=== 查询完成 ===\r\n\r\n");
                        MessageBox.Show("未查询到任何数据，请检查订单号是否正确",
                            "查询结果", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }));
            }
            catch (Exception ex)
            {
                this.Invoke(new Action(() =>
                {
                    richTextBox1.AppendText($"批量查询出现异常: {ex.Message}\r\n");
                    richTextBox1.AppendText("=== 查询异常结束 ===\r\n\r\n");
                }));
                MessageBox.Show($"批量查询失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // 重新启用查询按钮
                this.Invoke(new Action(() =>
                {
                    uiButton30.Enabled = true;
                }));
            }
        }

        // 添加一个新方法用于处理结果数据，添加箱规和箱数信息
        private List<object> ProcessResultsWithBoxInfo(List<object> results)
        {
            //results = new
            //{
            //    原始单号 = 原始单号,
            //    货号 = 组合装号,
            //    商家编码 = 组合装商家编码,
            //    货品名称 = 组合装名称,
            //    单品数量 = 组合装数量.ToString()
            //};
            // 创建一个字典用于缓存已查询过的货号对应的箱规
            Dictionary<string, MatInfo> boxSpecCache = new Dictionary<string, MatInfo>();
            List<object> processedResults = new List<object>();

            this.Invoke(new Action(() =>
            {
                richTextBox1.AppendText($"开始处理商品箱规信息，共 {results.Count} 条记录...\r\n");
            }));

            try
            {
                // 第一步：按原始单号+货号进行分组汇总
                this.Invoke(new Action(() =>
                {
                    richTextBox1.AppendText("开始按原始单号和货号汇总商品数量...\r\n");
                }));

                var aggregatedGoods = results
                    .GroupBy(item => new
                    {
                        原始单号 = item.GetType().GetProperty("原始单号")?.GetValue(item)?.ToString() ?? "",
                        组合装编码 = item.GetType().GetProperty("组合装编码")?.GetValue(item)?.ToString() ?? "",
                        组合装名称 = item.GetType().GetProperty("组合装名称")?.GetValue(item)?.ToString() ?? "",
                        组合装数量 = item.GetType().GetProperty("组合装数量")?.GetValue(item)?.ToString() ?? "",
                        货号 = item.GetType().GetProperty("货号")?.GetValue(item)?.ToString() ?? "",
                        商家编码 = item.GetType().GetProperty("商家编码")?.GetValue(item)?.ToString() ?? "",
                        货品名称 = item.GetType().GetProperty("货品名称")?.GetValue(item)?.ToString() ?? ""
                    })
                    .Select(group =>
                    {
                        int totalQuantity = 0;
                        // 遍历分组中的所有项，对数量进行求和
                        foreach (var item in group)
                        {
                            string quantityStr = item.GetType().GetProperty("单品数量")?.GetValue(item)?.ToString() ?? "0";
                            if (int.TryParse(quantityStr, out int quantity))
                            {
                                totalQuantity += quantity;
                            }
                            else
                            {
                                this.Invoke(new Action(() =>
                                {
                                    richTextBox1.AppendText($"警告: 商品 {group.Key.货号} 的数量格式无效: {quantityStr}\r\n");
                                }));
                            }
                        }

                        // 创建包含汇总信息的匿名类型
                        return new
                        {
                            原始单号 = group.Key.原始单号,
                            组合装编码 = group.Key.组合装编码,
                            组合装名称 = group.Key.组合装名称,
                            组合装数量 = group.Key.组合装数量,
                            货号 = group.Key.货号,
                            商家编码 = group.Key.商家编码,
                            货品名称 = group.Key.货品名称,
                            数量 = totalQuantity,
                            原始记录数 = group.Count()
                        };
                    })
                    .Where(item => !string.IsNullOrEmpty(item.货号) && item.数量 > 0)
                    .ToList();

                this.Invoke(new Action(() =>
                {
                    richTextBox1.AppendText($"数量汇总完成，原始 {results.Count} 条记录合并为 {aggregatedGoods.Count} 条唯一记录\r\n");
                }));

                // 第二步：遍历汇总后的集合，查询箱规并计算相关信息
                foreach (var item in aggregatedGoods)
                {
                    string goodsNo = item.货号;
                    int quantity = item.数量;

                    // 记录处理日志
                    this.Invoke(new Action(() =>
                    {
                        richTextBox1.AppendText($"处理商品: {goodsNo}, 原始单号: {item.原始单号}, 汇总数量: {quantity} (合并了{item.原始记录数}条记录)\r\n");
                    }));

                    // 获取箱规
                    MatInfo boxSpec = new MatInfo { changNum = 0, FWeight = 0 };

                    if (!string.IsNullOrEmpty(goodsNo))
                    {
                        // 检查缓存中是否已有该货号的箱规
                        if (boxSpecCache.ContainsKey(goodsNo))
                        {
                            boxSpec = boxSpecCache[goodsNo];
                            this.Invoke(new Action(() =>
                            {
                                richTextBox1.AppendText($"使用缓存的箱规: {goodsNo} -> {boxSpec.changNum}\r\n");
                            }));
                        }
                        else
                        {
                            try
                            {
                                // 调用函数获取箱规
                                boxSpec = getMatChangeNum(goodsNo);
                                // 将结果加入缓存
                                boxSpecCache[goodsNo] = boxSpec;
                                this.Invoke(new Action(() =>
                                {
                                    richTextBox1.AppendText($"查询到箱规: {goodsNo} -> {boxSpec.changNum}\r\n");
                                }));
                            }
                            catch (Exception ex)
                            {
                                this.Invoke(new Action(() =>
                                {
                                    richTextBox1.AppendText($"查询箱规出错: {ex.Message}\r\n");
                                }));
                            }
                        }
                    }

                    // 计算箱数
                    int fullBoxCount = 0;
                    int looseQuantity = 0;
                    if (boxSpec.changNum > 0)
                    {
                        fullBoxCount = quantity / boxSpec.changNum;  // 整箱数
                        looseQuantity = quantity % boxSpec.changNum;  // 散数
                    }

                    // 计算重量合计
                    decimal totalWeight = quantity * boxSpec.FWeight;

                    // 使用匿名类型而不是ExpandoObject，更简单可靠
                    var newItem = new
                    {
                        原始单号 = item.原始单号,
                        组合装编码 = item.组合装编码,
                        组合装名称 = item.组合装名称,
                        组合装数量 = item.组合装数量,
                        货号 = goodsNo,
                        商家编码 = item.商家编码,
                        货品名称 = item.货品名称,
                        数量 = quantity,
                        箱规 = boxSpec.changNum,
                        毛重 = boxSpec.FWeight,
                        整箱数 = fullBoxCount,
                        散数 = looseQuantity,
                        重量合计 = totalWeight
                    };

                    processedResults.Add(newItem);
                }

                this.Invoke(new Action(() =>
                {
                    richTextBox1.AppendText($"商品箱规信息处理完成，共处理 {processedResults.Count} 条记录\r\n");
                }));

                return processedResults;
            }
            catch (Exception ex)
            {
                this.Invoke(new Action(() =>
                {
                    richTextBox1.AppendText($"处理商品箱规信息时出错: {ex.Message}\r\n");
                    richTextBox1.AppendText($"错误详情: {ex.StackTrace}\r\n");
                }));

                // 如果处理失败，返回原始结果
                return results;
            }
        }
        private void ProcessResultsWithBoxInfoForTran()
        {
            var results = ((IEnumerable<object>)dataGridView11.DataSource).ToList();
            // 创建一个字典用于缓存已查询过的货号对应的箱规
            Dictionary<string, MatInfo> boxSpecCache = new Dictionary<string, MatInfo>();
            List<object> processedResults = new List<object>();

            this.Invoke(new Action(() =>
            {
                richTextBox2.Clear();
                richTextBox2.AppendText($"开始处理商品箱规信息，共 {results.Count} 条记录...\r\n");
            }));

            try
            {
                // 第一步：按货号对数量进行求和
                this.Invoke(new Action(() =>
                {
                    richTextBox2.AppendText("开始按货号汇总商品数量...\r\n");
                }));

                var aggregatedGoods = results
                    .GroupBy(item => new
                    {
                        货号 = item.GetType().GetProperty("货号")?.GetValue(item)?.ToString() ?? "",
                        货品名称 = item.GetType().GetProperty("货品名称")?.GetValue(item)?.ToString() ?? ""
                    })
                    .Select(group =>
                    {
                        int totalQuantity = 0;
                        // 遍历分组中的所有项，对数量进行求和
                        foreach (var item in group)
                        {
                            string quantityStr = item.GetType().GetProperty("单品数量")?.GetValue(item)?.ToString() ?? "0";
                            if (int.TryParse(quantityStr, out int quantity))
                            {
                                totalQuantity += quantity;
                            }
                        }

                        // 创建一个包含汇总信息的匿名类型
                        return new
                        {
                            货号 = group.Key.货号,
                            货品名称 = group.Key.货品名称,
                            数量 = totalQuantity
                        };
                    })
                    .Where(item => !string.IsNullOrEmpty(item.货号) && item.数量 > 0)
                    .ToList();

                this.Invoke(new Action(() =>
                {
                    richTextBox2.AppendText($"商品数量已汇总，共有 {aggregatedGoods.Count} 个唯一货号需要处理。\r\n");
                }));

                // 第二步：遍历汇总后的集合，查询库存、箱规，计算净需求和调拨数量
                foreach (var item in aggregatedGoods)
                {
                    string goodsNo = item.货号;
                    string goodesName = item.货品名称;
                    int quantity = item.数量;

                    this.Invoke(new Action(() =>
                    {
                        richTextBox2.AppendText($"处理商品: {goodsNo}, 汇总数量: {quantity}\r\n");
                    }));

                    // 查询库存

                    int stockQuantity = getMatStockNum(goodsNo);
                    this.Invoke(new Action(() =>
                    {
                        richTextBox2.AppendText($"查询: {goodsNo} 的库存量{stockQuantity}\r\n");
                    }));
                    // 计算净需求量
                    int netDemand = quantity - stockQuantity;

                    // 获取箱规
                    MatInfo boxSpec = new MatInfo { changNum = 0, FWeight = 0, FSaleBaseUnitNo = "支" };

                    if (!string.IsNullOrEmpty(goodsNo))
                    {
                        // 检查缓存中是否已有该货号的箱规
                        if (boxSpecCache.ContainsKey(goodsNo))
                        {
                            boxSpec = boxSpecCache[goodsNo];
                            this.Invoke(new Action(() =>
                            {
                                richTextBox2.AppendText($"使用缓存的箱规: {goodsNo} -> {boxSpec.changNum}\r\n");
                            }));
                        }
                        else
                        {
                            try
                            {
                                // 调用函数获取箱规
                                boxSpec = getMatChangeNum(goodsNo);
                                // 将结果加入缓存
                                boxSpecCache[goodsNo] = boxSpec;
                                this.Invoke(new Action(() =>
                                {
                                    richTextBox2.AppendText($"查询到箱规: {goodsNo} -> {boxSpec.changNum}\r\n");
                                }));
                            }
                            catch (Exception ex)
                            {
                                this.Invoke(new Action(() =>
                                {
                                    richTextBox2.AppendText($"查询箱规出错: {ex.Message}\r\n");
                                }));
                            }
                        }
                    }

                    // 计算调拨箱数和调拨数量
                    int allocatedBoxes = 0;
                    int allocatedQuantity = 0;
                    int orderBoxes = 0;
                    int orderLooseQuantity = 0;
                    if (boxSpec.changNum > 0 && netDemand > 0)
                    {
                        // 向上取整计算调拨箱数
                        allocatedBoxes = (int)Math.Ceiling((double)netDemand / boxSpec.changNum);
                        // 调拨数量 = 调拨箱数 * 箱规
                        allocatedQuantity = allocatedBoxes * boxSpec.changNum;
                    }
                    if (boxSpec.changNum > 0)
                    {
                        // 向下取整计算箱数
                        orderBoxes = (int)Math.Floor((double)quantity / boxSpec.changNum);
                        // 散装数量
                        orderLooseQuantity = quantity % boxSpec.changNum;
                    }
                    // 使用匿名类型来构建最终结果
                    var newItem = new
                    {
                        货号 = goodsNo,
                        货品名称 = goodesName,
                        单位 = boxSpec.FSaleBaseUnitNo,
                        数量 = quantity, // 原始需求总数量
                        整箱数 = orderBoxes,
                        散数 = orderLooseQuantity,
                        库存量 = stockQuantity,
                        净需求量 = netDemand,
                        箱规 = boxSpec.changNum,
                        调拨箱数 = allocatedBoxes,
                        调拨数量 = allocatedQuantity
                    };

                    processedResults.Add(newItem);
                }

                this.Invoke(new Action(() =>
                {
                    dataGridView11.DataSource = processedResults;
                    richTextBox2.AppendText($"商品箱规信息处理完成，共处理 {processedResults.Count} 个唯一货号。\r\n");
                }));
            }
            catch (Exception ex)
            {
                this.Invoke(new Action(() =>
                {
                    richTextBox2.AppendText($"处理商品箱规信息时出错: {ex.Message}\r\n");
                    richTextBox2.AppendText($"错误详情: {ex.StackTrace}\r\n");
                }));
            }
        }
        private void FormatDataGridView10()
        {
            try
            {
                if (dataGridView10.Columns.Count > 0)
                {
                    // 设置列头文本
                    if (dataGridView10.Columns.Contains("原始单号"))
                        dataGridView10.Columns["原始单号"].HeaderText = "原始单号";
                    if (dataGridView10.Columns.Contains("组合装编码"))
                        dataGridView10.Columns["组合装编码"].HeaderText = "组合装编码";
                    if (dataGridView10.Columns.Contains("组合装名称"))
                        dataGridView10.Columns["组合装名称"].HeaderText = "组合装名称";
                    if (dataGridView10.Columns.Contains("组合装数量"))
                        dataGridView10.Columns["组合装数量"].HeaderText = "组合装数量";
                    if (dataGridView10.Columns.Contains("货号"))
                        dataGridView10.Columns["货号"].HeaderText = "货号";
                    if (dataGridView10.Columns.Contains("商家编码"))
                        dataGridView10.Columns["商家编码"].HeaderText = "商家编码";
                    if (dataGridView10.Columns.Contains("货品名称"))
                        dataGridView10.Columns["货品名称"].HeaderText = "货品名称";
                    if (dataGridView10.Columns.Contains("数量"))
                        dataGridView10.Columns["数量"].HeaderText = "数量";
                    // 添加新列的表头设置
                    if (dataGridView10.Columns.Contains("箱规"))
                        dataGridView10.Columns["箱规"].HeaderText = "箱规";
                    if (dataGridView10.Columns.Contains("毛重"))
                        dataGridView10.Columns["毛重"].HeaderText = "毛重";
                    if (dataGridView10.Columns.Contains("整箱数"))
                        dataGridView10.Columns["整箱数"].HeaderText = "整箱数";
                    if (dataGridView10.Columns.Contains("散数"))
                        dataGridView10.Columns["散数"].HeaderText = "散数";
                    if (dataGridView10.Columns.Contains("重量合计"))
                        dataGridView10.Columns["重量合计"].HeaderText = "重量合计";

                    // 自动调整列宽
                    foreach (DataGridViewColumn column in dataGridView10.Columns)
                    {
                        column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    }

                    // 设置特定列的宽度（可选）
                    if (dataGridView10.Columns.Contains("货品名称"))
                    {
                        dataGridView10.Columns["货品名称"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                        dataGridView10.Columns["货品名称"].MinimumWidth = 150;
                    }

                    // 设置表格样式
                    dataGridView10.AlternatingRowsDefaultCellStyle.BackColor = Color.LightGray;
                    dataGridView10.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                    dataGridView10.ReadOnly = true; // 设置为只读

                    richTextBox1.AppendText("表格格式设置完成\r\n");
                }
            }
            catch (Exception ex)
            {
                richTextBox1.AppendText($"表格格式设置出错: {ex.Message}\r\n");
            }
        }

        private string GetWdtOrderInfo(string tid)
        {

            try
            {
                ApiRateController.WaitForNextCall1();
                this.Invoke(new Action(() =>
                {
                    richTextBox1.AppendText($"查询{tid}系统订单信息开始...\r\n");
                }));

              

             
                var dt = new Dictionary<string, string>();
                dt.Add("src_tid", tid);
               
                this.Invoke(new Action(() =>
                {
                    richTextBox1.AppendText($"开始传参查询...\r\n");
                }));
                var resultobj = JObject.Parse(GetWdtDataBLL.GetDataFromApi(dt, "sales_trade_query.php"));

                // 直接在这里处理结果
                if (resultobj["code"] != null && Convert.ToInt32(resultobj["code"]) != 0)
                {
                    // code不为0，提示错误
                    string message = resultobj["message"]?.ToString() ?? "未知错误";
                    this.Invoke(new Action(() =>
                    {
                        richTextBox1.AppendText($"旺店通API返回错误: {message}\r\n");
                    }));
                    //  MessageBox.Show($"查询失败: {message}");
                    return ""; // 或者根据你的方法返回类型调整
                }
                else
                {
                    // code为0，提取数据
                    JArray tradeList = JArray.Parse(resultobj["trades"].ToString());
                    string trade_no = "";
                    foreach (var trade in tradeList)
                    {
                        trade_no = trade["trade_no"]?.ToString() ?? "";

                    }

                    this.Invoke(new Action(() =>
                    {
                        richTextBox1.AppendText($"查询{tid}系统订单信息成功\r\n");
                    }));

                    return trade_no;
                }
            }
            catch (Exception er)
            {
                this.Invoke(new Action(() =>
                {
                    richTextBox1.AppendText($"查询{tid}系统订单信息出现错误:{er.Message}\r\n");
                }));
                return ""; // 返回空集合
            }

        }
        private List<object> GetWdtOrderStockOutInfo(string tid)
        {

            try
            {
                ApiRateController.WaitForNextCall();
                this.Invoke(new Action(() =>
                {
                    richTextBox1.AppendText($"查询{tid}系统销售出库单信息开始...\r\n");
                }));


                var dt = new Dictionary<string, string>();
                dt.Add("src_tid", tid);

                this.Invoke(new Action(() =>
                {
                    richTextBox1.AppendText($"开始传参查询...\r\n");
                }));
                var resultobj = JObject.Parse(GetWdtDataBLL.GetDataFromApi(dt, "stockout_order_query_trade.php"));

                // 直接在这里处理结果
                if (resultobj["code"] != null && Convert.ToInt32(resultobj["code"]) != 0)
                {
                    // code不为0，提示错误
                    string message = resultobj["message"]?.ToString() ?? "未知错误";
                    this.Invoke(new Action(() =>
                    {
                        richTextBox1.AppendText($"旺店通API返回错误: {message}\r\n");
                    }));
                    //  MessageBox.Show($"查询失败: {message}");
                    return new List<object>(); // 或者根据你的方法返回类型调整
                }
                else
                {
                    // code为0，提取数据
                    JArray tradeList = JArray.Parse(resultobj["stockout_list"].ToString());
                    var tradeGoodsList = new List<object>();

                    foreach (var trade in tradeList)
                    {
                        string tradeTid = trade["src_trade_no"]?.ToString() ?? "";
                        string outer_no = trade["outer_no"]?.ToString() ?? "";
                        JArray goodsList = JArray.Parse(trade["details_list"].ToString());

                        foreach (var goods in goodsList)
                        {
                            var tradeGoods = new
                            {
                                原始单号 = tradeTid,
                                外部单号 = outer_no,
                                货号 = goods["goods_no"]?.ToString() ?? "",
                                商家编码 = goods["spec_no"]?.ToString() ?? "",
                                货品名称 = goods["goods_name"]?.ToString() ?? "",
                                数量 = ((int)Convert.ToDecimal(goods["goods_count"])).ToString() ?? ""
                            };

                            tradeGoodsList.Add(tradeGoods);
                        }
                    }

                    this.Invoke(new Action(() =>
                    {
                        richTextBox1.AppendText($"查询{tid}系统销售出库单信息成功，返回 {tradeGoodsList.Count} 条商品记录\r\n");
                    }));

                    return tradeGoodsList;
                }
            }
            catch (Exception er)
            {
                this.Invoke(new Action(() =>
                {
                    richTextBox1.AppendText($"查询{tid}系统销售出库单信息出现错误:{er.Message}\r\n");
                }));
                return new List<object>(); // 返回空集合
            }

        }

        private List<object> BatchQueryWithProgressFromTid(System.Windows.Forms.ProgressBar progressBar = null)
        {
            var allOrderGoods = new List<object>();
            var processedTids = new HashSet<string>();

            try
            {
                if (dataGridView2.DataSource == null)
                {
                    MessageBox.Show("没有数据需要查询");
                    return allOrderGoods;
                }

                if (dataGridView9.DataSource is IEnumerable<object> dataSource)
                {
                    var tids = dataSource.Select(item =>
                    {
                        var itemType = item.GetType();
                        return itemType.GetProperty("原始单号")?.GetValue(item)?.ToString() ?? "";
                    })
                    .Where(tid => !string.IsNullOrEmpty(tid) && !processedTids.Contains(tid))
                    .ToList();

                    // 设置进度条
                    if (progressBar != null)
                    {
                        this.Invoke(new Action(() =>
                        {
                            progressBar.Visible = true;
                            progressBar.Minimum = 0;
                            progressBar.Maximum = tids.Count;
                            progressBar.Value = 0;
                        }));
                    }

                    this.Invoke(new Action(() =>
                    {
                        richTextBox1.AppendText($"开始批量查询，共 {tids.Count} 个唯一原始单号\r\n");
                    }));

                    for (int i = 0; i < tids.Count; i++)
                    {
                        string tid = tids[i];
                        processedTids.Add(tid);

                        this.Invoke(new Action(() =>
                        {
                            richTextBox1.AppendText($"正在查询第 {i + 1}/{tids.Count} 个订单: {tid}\r\n");

                            // 更新进度条
                            if (progressBar != null)
                            {
                                progressBar.Value = i + 1;
                            }
                        }));

                        //  DateTime dateTime = GetWdtSOrderInfo(tids[i]);
                        //  var orderGoods = GetWdtOrderInfo(tid, dateTime);

                        var orderGoods = GetWdtOrderStockOutInfo(tid);
                        if (orderGoods != null && orderGoods.Count > 0)
                        {
                            allOrderGoods.AddRange(orderGoods);
                        }

                        // 让UI有机会更新
                        Application.DoEvents();
                    }

                    this.Invoke(new Action(() =>
                    {
                        richTextBox1.AppendText($"批量查询完成，共获取 {allOrderGoods.Count} 条商品记录\r\n");

                        // 重置进度条
                        if (progressBar != null)
                        {
                            progressBar.Value = progressBar.Maximum;
                            progressBar.Visible = false;
                        }
                    }));
                }
            }
            catch (Exception ex)
            {
                this.Invoke(new Action(() =>
                {
                    richTextBox1.AppendText($"批量查询出错: {ex.Message}\r\n");
                    progressBar.Visible = false;
                }));
                MessageBox.Show($"批量查询出错: {ex.Message}");
            }

            return allOrderGoods;
        }
        private List<object> BatchQueryWithProgress(System.Windows.Forms.ProgressBar progressBar = null)
        {
            var allOrderGoods = new List<object>();
            var processedTids = new HashSet<string>();

            try
            {
                if (dataGridView2.DataSource == null)
                {
                    MessageBox.Show("没有数据需要查询");
                    return allOrderGoods;
                }

                if (dataGridView9.DataSource is IEnumerable<object> dataSource)
                {
                    var tids = dataSource.Select(item =>
                    {
                        var itemType = item.GetType();
                        return itemType.GetProperty("原始单号")?.GetValue(item)?.ToString() ?? "";
                    })
                    .Where(tid => !string.IsNullOrEmpty(tid) && !processedTids.Contains(tid))
                    .ToList();

                    // 设置进度条
                    if (progressBar != null)
                    {
                        this.Invoke(new Action(() =>
                        {
                            progressBar.Visible = true;
                            progressBar.Minimum = 0;
                            progressBar.Maximum = tids.Count;
                            progressBar.Value = 0;
                        }));
                    }

                    this.Invoke(new Action(() =>
                    {
                        richTextBox1.AppendText($"开始批量查询，共 {tids.Count} 个唯一原始单号\r\n");
                    }));

                    for (int i = 0; i < tids.Count; i++)
                    {
                        string tid = tids[i];
                        processedTids.Add(tid);

                        this.Invoke(new Action(() =>
                        {
                            richTextBox1.AppendText($"正在查询第 {i + 1}/{tids.Count} 个订单: {tid}\r\n");

                            // 更新进度条
                            if (progressBar != null)
                            {
                                progressBar.Value = i + 1;
                            }
                        }));

                        //  DateTime dateTime = GetWdtSOrderInfo(tids[i]);
                        //  var orderGoods = GetWdtOrderInfo(tid, dateTime);

                        //    var orderGoods = GetWdtOrderStockOutInfo(tid);
                        var orderGoods = GetWdtSOrderInfoForlog(tid);
                        if (orderGoods != null && orderGoods.Count > 0)
                        {
                            allOrderGoods.AddRange(orderGoods);
                        }

                        // 让UI有机会更新
                        Application.DoEvents();
                    }

                    this.Invoke(new Action(() =>
                    {
                        richTextBox1.AppendText($"批量查询完成，共获取 {allOrderGoods.Count} 条商品记录\r\n");

                        // 重置进度条
                        if (progressBar != null)
                        {
                            progressBar.Value = progressBar.Maximum;
                            progressBar.Visible = false;
                        }
                    }));
                }
            }
            catch (Exception ex)
            {
                this.Invoke(new Action(() =>
                {
                    richTextBox1.AppendText($"批量查询出错: {ex.Message}\r\n");
                    progressBar.Visible = false;
                }));
                MessageBox.Show($"批量查询出错: {ex.Message}");
            }

            return allOrderGoods;
        }
        // 组合装信息缓存
        private Dictionary<string, List<object>> suiteInfoCache = new Dictionary<string, List<object>>();

        private List<object> BatchQueryWithProgressForTran(System.Windows.Forms.ProgressBar progressBar = null)
        {
            var allOrderGoods = new List<object>();
            var processedTids = new HashSet<string>();

            try
            {
                if (dataGridView13.DataSource == null)
                {
                    MessageBox.Show("没有数据需要查询");
                    return allOrderGoods;
                }

                if (dataGridView13.DataSource is IEnumerable<object> dataSource)
                {
                    var tids = dataSource.Select(item =>
                    {
                        var itemType = item.GetType();
                        return itemType.GetProperty("原始单号")?.GetValue(item)?.ToString() ?? "";
                    })
                    .Where(tid => !string.IsNullOrEmpty(tid) && !processedTids.Contains(tid))
                    .ToList();

                    // 设置进度条
                    if (progressBar != null)
                    {
                        this.Invoke(new Action(() =>
                        {
                            progressBar.Visible = true;
                            progressBar.Minimum = 0;
                            progressBar.Maximum = tids.Count;
                            progressBar.Value = 0;
                        }));
                    }

                    this.Invoke(new Action(() =>
                    {
                        richTextBox2.AppendText($"开始批量查询，共 {tids.Count} 个唯一原始单号\r\n");
                    }));

                    // 第一步：获取所有订单的组合装信息
                    var orderTradeGoods = new List<object>();
                    for (int i = 0; i < tids.Count; i++)
                    {
                        string tid = tids[i];
                        processedTids.Add(tid);

                        this.Invoke(new Action(() =>
                        {
                            richTextBox2.AppendText($"正在查询第 {i + 1}/{tids.Count} 个订单: {tid}\r\n");

                            // 更新进度条
                            if (progressBar != null)
                            {
                                progressBar.Value = i + 1;
                            }
                        }));

                        var orderGoods = GetWdtSOrderInfo(tid);
                        if (orderGoods != null && orderGoods.Count > 0)
                        {
                            orderTradeGoods.AddRange(orderGoods);
                        }

                        // 让UI有机会更新
                        Application.DoEvents();
                    }

                    this.Invoke(new Action(() =>
                    {
                        dataGridView11.DataSource = orderTradeGoods;
                        richTextBox2.AppendText($"第一步完成，获取到 {orderTradeGoods.Count} 条组合装记录\r\n");
                        richTextBox2.AppendText($"开始处理组合装信息...\r\n");
                    }));

                    // 第二步：处理组合装信息
                    var finalResults = ProcessSuiteGoods(orderTradeGoods, progressBar);
                    allOrderGoods.AddRange(finalResults);

                    this.Invoke(new Action(() =>
                    {
                        richTextBox2.AppendText($"批量查询完成，最终获取 {allOrderGoods.Count} 条商品记录\r\n");

                        // 重置进度条
                        if (progressBar != null)
                        {
                            progressBar.Value = progressBar.Maximum;
                            progressBar.Visible = false;
                        }
                    }));
                }
            }
            catch (Exception ex)
            {
                this.Invoke(new Action(() =>
                {
                    richTextBox2.AppendText($"批量查询出错: {ex.Message}\r\n");
                    if (progressBar != null)
                        progressBar.Visible = false;
                }));
                MessageBox.Show($"批量查询出错: {ex.Message}");
            }

            return allOrderGoods;
        }
        private string GetPropertyValue(object obj, string[] possibleNames)
        {
            if (obj == null) return "";

            var type = obj.GetType();
            var properties = type.GetProperties();

            foreach (string name in possibleNames)
            {
                var property = properties.FirstOrDefault(p => p.Name == name);
                if (property != null)
                {
                    return property.GetValue(obj)?.ToString() ?? "";
                }
            }

            return "";
        }
        private List<object> ProcessSuiteGoods(List<object> orderTradeGoods, System.Windows.Forms.ProgressBar progressBar = null)
        {
            var finalResults = new List<object>();
            var uniqueSuites = new HashSet<string>();

            // 获取所有唯一的组合装号
            foreach (var item in orderTradeGoods)
            {
                string suiteNo = GetPropertyValue(item, new[] { "组合装号", "货号", "goods_no" });
                if (!string.IsNullOrEmpty(suiteNo))
                {
                    uniqueSuites.Add(suiteNo);
                }
            }

            this.Invoke(new Action(() =>
            {
                richTextBox2.AppendText($"发现 {uniqueSuites.Count} 个唯一组合装，开始查询组合装详情...\r\n");
            }));

            // 查询所有组合装信息并缓存
            int suiteIndex = 0;
            foreach (string suiteNo in uniqueSuites)
            {
                suiteIndex++;
                if (!suiteInfoCache.ContainsKey(suiteNo))
                {
                    this.Invoke(new Action(() =>
                    {
                        richTextBox2.AppendText($"查询组合装 {suiteIndex}/{uniqueSuites.Count}: {suiteNo}\r\n");
                    }));

                    var suiteInfo = GetWdtSuitesInfo(suiteNo);
                    suiteInfoCache[suiteNo] = suiteInfo;

                    this.Invoke(new Action(() =>
                    {
                        richTextBox2.AppendText($"组合装{suiteNo}缓存到本地\r\n");
                    }));

                    // 让UI有机会更新
                    Application.DoEvents();
                }
                else
                {
                    this.Invoke(new Action(() =>
                    {
                        richTextBox2.AppendText($"使用缓存的组合装信息: {suiteNo}\r\n");
                    }));
                }
            }

            // 处理每个订单商品
            foreach (var orderItem in orderTradeGoods)
            {
                string 原始单号 = GetPropertyValue(orderItem, new[] { "原始单号", "tid" });
                string 组合装号 = GetPropertyValue(orderItem, new[] { "组合装号", "货号", "goods_no" });
                string 组合装商家编码 = GetPropertyValue(orderItem, new[] { "商家编码", "spec_no" });
                string 组合装名称 = GetPropertyValue(orderItem, new[] { "货品名称", "goods_name" });
                string 组合装数量字符串 = GetPropertyValue(orderItem, new[] { "组合装数量", "数量", "num" });

                int 组合装数量 = 0;
                if (int.TryParse(组合装数量字符串, out 组合装数量))
                {
                    // 检查缓存中是否有组合装信息
                    if (suiteInfoCache.ContainsKey(组合装号) && suiteInfoCache[组合装号].Count > 0)
                    {
                        // 有组合装信息，拆分为单品
                        foreach (var suiteGoods in suiteInfoCache[组合装号])
                        {
                            string 单品数量字符串 = GetPropertyValue(suiteGoods, new[] { "数量", "num" });
                            int 单品在组合装中的数量 = 0;

                            if (int.TryParse(单品数量字符串, out 单品在组合装中的数量))
                            {
                                var finalGoods = new
                                {
                                    原始单号 = 原始单号,
                                    货号 = GetPropertyValue(suiteGoods, new[] { "货号", "goods_no" }),
                                    商家编码 = GetPropertyValue(suiteGoods, new[] { "商家编码", "spec_no" }),
                                    货品名称 = GetPropertyValue(suiteGoods, new[] { "货品名称", "goods_name" }),
                                    单品数量 = (组合装数量 * 单品在组合装中的数量).ToString()
                                };
                                finalResults.Add(finalGoods);
                            }
                        }
                    }
                    else
                    {
                        // 没有组合装信息，直接存储原始数据
                        var directGoods = new
                        {
                            原始单号 = 原始单号,
                            货号 = 组合装号,
                            商家编码 = 组合装商家编码,
                            货品名称 = 组合装名称,
                            单品数量 = 组合装数量.ToString()
                        };
                        finalResults.Add(directGoods);
                    }
                }
                else
                {
                    this.Invoke(new Action(() =>
                    {
                        richTextBox2.AppendText($"转化组合装数量失败: {组合装数量字符串}\r\n");
                    }));
                }
            }

            this.Invoke(new Action(() =>
            {
                richTextBox2.AppendText($"转化组合装完成\r\n");
            }));

            return finalResults;
        }


        private List<object> GetWdtSuitesInfo(string suite_no)
        {
            try
            {
                ApiRateController.WaitForNextCall();
                this.Invoke(new Action(() =>
                {
                    richTextBox2.AppendText($"查询{suite_no}组合信息开始...\r\n");
                }));

                var dt = new Dictionary<string, string>();
                dt.Add("suite_no", suite_no);

                var resultobj = JObject.Parse(GetWdtDataBLL.GetDataFromApi(dt, "suites_query.php"));

                // 直接在这里处理结果
                if (resultobj["code"] != null && Convert.ToInt32(resultobj["code"]) != 0)
                {
                    // code不为0，提示错误
                    string message = resultobj["message"]?.ToString() ?? "未知错误";
                    this.Invoke(new Action(() =>
                    {
                        richTextBox2.AppendText($"旺店通API返回错误: {message}\r\n");
                    }));
                    return new List<object>();
                }
                else
                {
                    // code为0，提取数据
                    JArray suitesList = JArray.Parse(resultobj["suites"].ToString());

                    if (suitesList.Count > 0)
                    {
                        var suiteGoodsList = new List<object>();

                        foreach (var suite in suitesList)
                        {
                            JArray goodsList = JArray.Parse(suite["specs_list"].ToString());

                            foreach (var goods in goodsList)
                            {
                                var suiteGoods = new
                                {
                                    货号 = goods["goods_no"]?.ToString() ?? "",
                                    商家编码 = goods["spec_no"]?.ToString() ?? "",
                                    货品名称 = goods["goods_name"]?.ToString() ?? "",
                                    数量 = ((int)Convert.ToDecimal(goods["num"])).ToString() ?? ""
                                };

                                suiteGoodsList.Add(suiteGoods);
                            }
                        }

                        this.Invoke(new Action(() =>
                        {
                            richTextBox2.AppendText($"组合装{suite_no}返回信息:{suiteGoodsList.ToJsonString()}\r\n");
                        }));
                        return suiteGoodsList;
                    }
                    else
                    {
                        this.Invoke(new Action(() =>
                        {
                            richTextBox2.AppendText($"未找到组合装{suite_no}的原始信息\r\n");
                        }));
                        return new List<object>();
                    }
                }
            }
            catch (Exception er)
            {
                this.Invoke(new Action(() =>
                {
                    richTextBox2.AppendText($"查询{suite_no}组合装信息出现错误:{er.Message}\r\n");
                }));
                return new List<object>();
            }
        }

        private List<object> GetWdtSOrderInfo(string tid)
        {
            try
            {
                ApiRateController.WaitForNextCall();
                this.Invoke(new Action(() =>
                {
                    richTextBox2.AppendText($"查询{tid}原始订单信息开始...\r\n");
                }));

                string starttime = sdt.Value.Date.AddSeconds(1).ToString("yyyy-MM-dd HH:mm:ss");
                DateTime calculatedEndTime = edt.Value.Date.AddDays(2).AddSeconds(-1);
                DateTime currentTime = DateTime.Now;
                string endtime = (calculatedEndTime > currentTime ? currentTime : calculatedEndTime)
                                 .ToString("yyyy-MM-dd HH:mm:ss");

                var dt = new Dictionary<string, string>();
                dt.Add("tid", tid);
                dt.Add("start_time", starttime);
                dt.Add("end_time", endtime);

                var resultobj = JObject.Parse(GetWdtDataBLL.GetDataFromApi(dt, "vip_api_trade_query.php"));

                // 直接在这里处理结果
                if (resultobj["code"] != null && Convert.ToInt32(resultobj["code"]) != 0)
                {
                    // code不为0，提示错误
                    string message = resultobj["message"]?.ToString() ?? "未知错误";
                    this.Invoke(new Action(() =>
                    {
                        richTextBox2.AppendText($"旺店通API返回错误: {message}\r\n");
                    }));
                    return new List<object>();
                }
                else
                {
                    // code为0，提取数据
                    JArray tradeList = JArray.Parse(resultobj["trade_list"].ToString());

                    if (tradeList.Count > 0)
                    {
                        var tradeGoodsList = new List<object>();

                        foreach (var trade in tradeList)
                        {
                            string tradeTid = trade["tid"]?.ToString() ?? "";
                            string stockout_no = trade["stockout_no"]?.ToString() ?? "";
                            JArray goodsList = JArray.Parse(trade["goods_list"].ToString());

                            foreach (var goods in goodsList)
                            {
                                var tradeGoods = new
                                {
                                    原始单号 = tradeTid,
                                    组合装号 = goods["goods_no"]?.ToString() ?? "",
                                    商家编码 = goods["spec_no"]?.ToString() ?? "",
                                    货品名称 = goods["goods_name"]?.ToString() ?? "",
                                    组合装数量 = ((int)Convert.ToDecimal(goods["num"])).ToString() ?? ""
                                };

                                tradeGoodsList.Add(tradeGoods);
                            }
                        }
                        return tradeGoodsList;
                    }
                    else
                    {
                        this.Invoke(new Action(() =>
                        {
                            richTextBox2.AppendText($"未找到订单{tid}的原始信息\r\n");
                        }));
                        return new List<object>();
                    }
                }
            }
            catch (Exception er)
            {
                this.Invoke(new Action(() =>
                {
                    richTextBox2.AppendText($"查询{tid}原始订单信息出现错误:{er.Message}\r\n");
                }));
                return new List<object>();
            }
        }
        private List<object> GetWdtSOrderInfoForlog(string tid)
        {
            try
            {
                ApiRateController.WaitForNextCall();
                this.Invoke(new Action(() =>
                {
                    richTextBox1.AppendText($"查询{tid}原始订单信息开始...\r\n");
                }));

                string starttime = sdt.Value.Date.AddSeconds(1).ToString("yyyy-MM-dd HH:mm:ss");
                DateTime calculatedEndTime = edt.Value.Date.AddDays(2).AddSeconds(-1);
                DateTime currentTime = DateTime.Now;
                string endtime = (calculatedEndTime > currentTime ? currentTime : calculatedEndTime)
                                 .ToString("yyyy-MM-dd HH:mm:ss");

                var dt = new Dictionary<string, string>();
                dt.Add("tid", tid);
                dt.Add("start_time", starttime);
                dt.Add("end_time", endtime);

                var resultobj = JObject.Parse(GetWdtDataBLL.GetDataFromApi(dt, "vip_api_trade_query.php"));

                // 直接在这里处理结果
                if (resultobj["code"] != null && Convert.ToInt32(resultobj["code"]) != 0)
                {
                    // code不为0，提示错误
                    string message = resultobj["message"]?.ToString() ?? "未知错误";
                    this.Invoke(new Action(() =>
                    {
                        richTextBox1.AppendText($"旺店通API返回错误: {message}\r\n");
                    }));
                    return new List<object>();
                }
                else
                {
                    // code为0，提取数据
                    JArray tradeList = JArray.Parse(resultobj["trade_list"].ToString());

                    if (tradeList.Count > 0)
                    {
                        var tradeGoodsList = new List<object>();

                        foreach (var trade in tradeList)
                        {
                            string tradeTid = trade["tid"]?.ToString() ?? "";
                            string stockout_no = trade["stockout_no"]?.ToString() ?? "";
                            JArray goodsList = JArray.Parse(trade["goods_list"].ToString());

                            foreach (var goods in goodsList)
                            {
                                var tradeGoods = new
                                {
                                    原始单号 = tradeTid,
                                    订单号 = "",
                                    组合装号 = goods["goods_no"]?.ToString() ?? "",
                                    商家编码 = goods["spec_no"]?.ToString() ?? "",
                                    货品名称 = goods["goods_name"]?.ToString() ?? "",
                                    组合装数量 = ((int)Convert.ToDecimal(goods["num"])).ToString() ?? ""
                                };

                                tradeGoodsList.Add(tradeGoods);
                            }
                        }
                        return tradeGoodsList;
                    }
                    else
                    {
                        this.Invoke(new Action(() =>
                        {
                            richTextBox1.AppendText($"未找到订单{tid}的原始信息\r\n");
                        }));
                        return new List<object>();
                    }
                }
            }
            catch (Exception er)
            {
                this.Invoke(new Action(() =>
                {
                    richTextBox1.AppendText($"查询{tid}原始订单信息出现错误:{er.Message}\r\n");
                }));
                return new List<object>();
            }
        }
        private List<object> ProcessOrdersWithSuiteInfo(System.Windows.Forms.ProgressBar progressBar = null)
        {
            var finalResults = new List<object>();

            try
            {
                // 第一步：获取所有订单的组合装信息
                var orderTradeGoods = BatchQueryWithProgressForTran(progressBar);

                if (orderTradeGoods.Count == 0)
                {
                    return finalResults;
                }
                this.Invoke(new Action(() =>
                {
                    dataGridView14.DataSource = orderTradeGoods;
                }));
                // 获取所有唯一的组合装号
                var uniqueSuites = orderTradeGoods
                    .Select(item => ((dynamic)item).组合装号?.ToString() ?? "")
                    .Where(suiteNo => !string.IsNullOrEmpty(suiteNo))
                    .Distinct()
                    .ToList();

                this.Invoke(new Action(() =>
                {
                    richTextBox2.AppendText($"发现 {uniqueSuites.Count} 个唯一组合装，开始查询组合装详情...\r\n");
                }));

                // 设置组合装查询进度条
                if (progressBar != null)
                {
                    this.Invoke(new Action(() =>
                    {
                        progressBar.Visible = true;
                        progressBar.Minimum = 0;
                        progressBar.Maximum = uniqueSuites.Count;
                        progressBar.Value = 0;
                    }));
                }

                // 第二步：查询所有组合装信息并缓存
                for (int i = 0; i < uniqueSuites.Count; i++)
                {
                    string suiteNo = uniqueSuites[i];

                    this.Invoke(new Action(() =>
                    {
                        richTextBox2.AppendText($"查询组合装 {i + 1}/{uniqueSuites.Count}: {suiteNo}\r\n");

                        if (progressBar != null)
                        {
                            progressBar.Value = i + 1;
                        }
                    }));

                    if (!suiteInfoCache.ContainsKey(suiteNo))
                    {

                        var suiteInfo = GetWdtSuitesInfo(suiteNo);
                        suiteInfoCache[suiteNo] = suiteInfo;
                        this.Invoke(new Action(() =>
                        {
                            richTextBox2.AppendText($"将查询的组合装: {suiteNo}缓存到本地\r\n");
                        }));
                    }
                    else
                    {
                        this.Invoke(new Action(() =>
                        {
                            richTextBox2.AppendText($"组合装:{suiteNo}已缓存在本地\r\n");
                        }));
                    }

                    // 让UI有机会更新
                    Application.DoEvents();
                }

                // 第三步：处理每个订单商品，生成最终结果
                this.Invoke(new Action(() =>
                {
                    richTextBox2.AppendText($"开始生成最终商品清单...\r\n");
                }));

                foreach (var orderItem in orderTradeGoods)
                {
                    dynamic item = orderItem;
                    string 原始单号 = item.原始单号?.ToString() ?? "";
                    string 组合装号 = item.组合装号?.ToString() ?? "";
                    string 组合装商家编码 = item.商家编码?.ToString() ?? "";
                    string 组合装名称 = item.货品名称?.ToString() ?? "";

                    if (!int.TryParse(item.组合装数量?.ToString(), out int 组合装数量))
                    {
                        组合装数量 = 0;
                    }

                    // 检查缓存中是否有组合装信息
                    if (suiteInfoCache.ContainsKey(组合装号) && suiteInfoCache[组合装号].Count > 0)
                    {
                        // 有组合装信息，拆分为单品
                        foreach (var suiteGoods in suiteInfoCache[组合装号])
                        {
                            dynamic goods = suiteGoods;

                            if (!int.TryParse(goods.数量?.ToString(), out int 单品在组合装中的数量))
                            {
                                单品在组合装中的数量 = 0;
                            }

                            var finalGoods = new
                            {
                                原始单号 = 原始单号,
                                货号 = goods.货号?.ToString() ?? "",
                                商家编码 = goods.商家编码?.ToString() ?? "",
                                货品名称 = goods.货品名称?.ToString() ?? "",
                                单品数量 = (组合装数量 * 单品在组合装中的数量).ToString()
                            };
                            finalResults.Add(finalGoods);
                        }
                    }
                    else
                    {
                        // 没有组合装信息，直接存储原始数据
                        var directGoods = new
                        {
                            原始单号 = 原始单号,
                            货号 = 组合装号,
                            商家编码 = 组合装商家编码,
                            货品名称 = 组合装名称,
                            单品数量 = 组合装数量.ToString()
                        };
                        finalResults.Add(directGoods);
                    }
                }

                this.Invoke(new Action(() =>
                {
                    richTextBox2.AppendText($"最终处理完成，生成 {finalResults.Count} 条商品记录\r\n");

                    if (progressBar != null)
                    {
                        progressBar.Visible = false;
                    }
                }));

            }
            catch (Exception ex)
            {
                this.Invoke(new Action(() =>
                {
                    richTextBox2.AppendText($"处理组合装信息出错: {ex.Message}\r\n");
                    if (progressBar != null)
                        progressBar.Visible = false;
                }));
                // MessageBox.Show($"处理组合装信息出错: {ex.Message}");
            }

            return finalResults;
        }
        private void uiButton32_Click(object sender, EventArgs e)
        {
            CalculateTotalBoxes();
        }
        private void CalculateTotalBoxes()
        {
            try
            {
                if (dataGridView9.DataSource == null || dataGridView10.DataSource == null)
                {
                    MessageBox.Show("请先完成数据查询");
                    return;
                }

                // 获取用户输入的箱规
                int specifiedBoxSpec = 0;
                if (!int.TryParse(uiTextBox12.Text, out specifiedBoxSpec) || specifiedBoxSpec <= 0)
                {
                    MessageBox.Show("请输入有效的箱规数值");
                    return;
                }

                // 获取dataGridView10的数据
                var grid10Data = (IEnumerable<object>)dataGridView10.DataSource;

                // 检查uiTextBox18是否有值，并处理特定货号的逻辑
                string[] specificProductCodes = null;
                bool hasSpecificProducts = false;
                
                if (!string.IsNullOrWhiteSpace(uiTextBox18.Text))
                {
                    string specificProductText = uiTextBox18.Text.Trim();
                   // if (specificProductText.Contains(","))
                  //  {
                        // 以逗号分隔，转换成数组
                        specificProductCodes = specificProductText.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                            .Select(s => s.Trim()).ToArray();
                        hasSpecificProducts = true;
                      //  richTextBox1.Text = hasSpecificProducts.ToString()+"\r\n";
                        // 验证所有货号是否存在于grid10Data中
                        foreach (var productCode in specificProductCodes)
                        {
                            bool productExists = grid10Data.Any(x => 
                                x.GetType().GetProperty("货号")?.GetValue(x)?.ToString() == productCode);
                            
                            if (!productExists)
                            {
                                UIMessageBox.Show($"货号 '{productCode}' 不存在于订单明细中，请检查后重试");
                                return;
                            }
                        }
                   // }
                }

                // 处理dataGridView9的数据
                var grid9Data = (IEnumerable<object>)dataGridView9.DataSource;
                var processedData = new List<dynamic>();

                foreach (var item in grid9Data)
                {
                    var itemType = item.GetType();
                    string tid = itemType.GetProperty("原始单号")?.GetValue(item)?.ToString() ?? "";

                    // 在dataGridView10中查找对应原始单号的记录
                    var relatedItems = grid10Data.Where(x =>
                        x.GetType().GetProperty("原始单号")?.GetValue(x)?.ToString() == tid).ToList();

                    // 计算整箱数和散数
                    int totalFullBoxes = 0;
                    int totalLooseQuantity = 0;
                    decimal totalFWeight = 0;
                    
                    if (hasSpecificProducts)
                    {
                        // 分开处理特定货号和其他货号
                        var specificItems = new List<object>();
                        var otherItems = new List<object>();
                        
                        foreach (var relatedItem in relatedItems)
                        {
                            var relatedType = relatedItem.GetType();
                            string productCode = relatedType.GetProperty("货号")?.GetValue(relatedItem)?.ToString() ?? "";
                            
                            if (specificProductCodes.Contains(productCode))
                            {
                                specificItems.Add(relatedItem);
                               // richTextBox1.Text = relatedItem.ToString() + "\r\n";
                            }
                            else
                            {
                                otherItems.Add(relatedItem);
                            }
                        }

                        
                        // 处理非特定货号的商品（使用通用箱规）
                        foreach (var otherItem in otherItems)
                        {
                            var relatedType = otherItem.GetType();
                           // int fullBoxes = Convert.ToInt32(relatedType.GetProperty("整箱数")?.GetValue(otherItem) ?? 0);
                         //   int looseQuantity = Convert.ToInt32(relatedType.GetProperty("散数")?.GetValue(otherItem) ?? 0);
                            decimal fWeight = Convert.ToDecimal(relatedType.GetProperty("重量合计")?.GetValue(otherItem) ?? 0);
                            
                           // totalFullBoxes += fullBoxes;
                          //  totalLooseQuantity += looseQuantity;
                            totalFWeight += fWeight;
                        }
                        
                        // 计算非特定货号的散数箱数（使用通用箱规）
                        int looseBoxesForOthers = totalLooseQuantity > 0 ? 
                            (int)Math.Ceiling((double)totalLooseQuantity / specifiedBoxSpec) : 0;
                        
                        // 处理特定货号的商品（使用各自的箱规）
                        int specificLooseBoxes = 0;
                        
                        foreach (var specificItem in specificItems)
                        {
                            var relatedType = specificItem.GetType();
                          //  int fullBoxes = Convert.ToInt32(relatedType.GetProperty("整箱数")?.GetValue(specificItem) ?? 0);
                         //   int looseQuantity = Convert.ToInt32(relatedType.GetProperty("散数")?.GetValue(specificItem) ?? 0);
                            decimal fWeight = Convert.ToDecimal(relatedType.GetProperty("重量合计")?.GetValue(specificItem) ?? 0);
                            int itemBoxSpec = Convert.ToInt32(relatedType.GetProperty("箱规")?.GetValue(specificItem) ?? specifiedBoxSpec);
                            
                            // 如果箱规为0，使用通用箱规
                            if (itemBoxSpec <= 0)
                            {
                                itemBoxSpec = specifiedBoxSpec;
                            }
                            
                            // 计算该特定货号的散数箱数
                          //  int itemLooseBoxes = looseQuantity > 0 ? 
                           //     (int)Math.Ceiling((double)looseQuantity / itemBoxSpec) : 0;
                            
                          //  specificLooseBoxes += itemLooseBoxes;
                          //  totalFullBoxes += fullBoxes;
                            totalFWeight += fWeight;
                            // 不累加散数，因为已经按各自箱规计算了散数箱数
                        }
                        
                        // 总箱数 = 整箱数 + 非特定货号散数箱数 + 特定货号散数箱数
                        int totalBoxes = totalFullBoxes + looseBoxesForOthers + specificLooseBoxes;
                        
                        // 创建新对象，包含原有属性和新增的统计信息
                        dynamic newItem = new
                        {
                            原始单号 = itemType.GetProperty("原始单号")?.GetValue(item)?.ToString() ?? "",
                            地址 = itemType.GetProperty("地址")?.GetValue(item)?.ToString() ?? "",
                            收件人 = itemType.GetProperty("收件人")?.GetValue(item)?.ToString() ?? "",
                            电话 = itemType.GetProperty("电话")?.GetValue(item)?.ToString() ?? "",
                            旺店通状态 = itemType.GetProperty("旺店通状态")?.GetValue(item)?.ToString() ?? "",
                          //  整箱数 = totalFullBoxes,
                         //   散数 = totalLooseQuantity,
                         //   散数箱数 = looseBoxesForOthers + specificLooseBoxes,
                         //   总箱数 = totalBoxes,
                            总重量 = totalFWeight
                        };
                        
                        processedData.Add(newItem);
                    }
                    else
                    {
                        // 原有逻辑，处理所有货号
                        foreach (var relatedItem in relatedItems)
                        {
                            var relatedType = relatedItem.GetType();
                          //  int fullBoxes = Convert.ToInt32(relatedType.GetProperty("整箱数")?.GetValue(relatedItem) ?? 0);
                         //   int looseQuantity = Convert.ToInt32(relatedType.GetProperty("散数")?.GetValue(relatedItem) ?? 0);
                            decimal fWeight = Convert.ToDecimal(relatedType.GetProperty("重量合计")?.GetValue(relatedItem) ?? 0);
                          //  totalFullBoxes += fullBoxes;
                          //  totalLooseQuantity += looseQuantity;
                            totalFWeight += fWeight;
                        }

                        // 计算散数箱数（向上取整）
                        int looseBoxes = (int)Math.Ceiling((double)totalLooseQuantity / specifiedBoxSpec);

                        // 总箱数
                        int totalBoxes = totalFullBoxes + looseBoxes;

                        // 创建新对象，包含原有属性和新增的统计信息
                        dynamic newItem = new
                        {
                            原始单号 = itemType.GetProperty("原始单号")?.GetValue(item)?.ToString() ?? "",
                            地址 = itemType.GetProperty("地址")?.GetValue(item)?.ToString() ?? "",
                            收件人 = itemType.GetProperty("收件人")?.GetValue(item)?.ToString() ?? "",
                            电话 = itemType.GetProperty("电话")?.GetValue(item)?.ToString() ?? "",
                            旺店通状态 = itemType.GetProperty("旺店通状态")?.GetValue(item)?.ToString() ?? "",
                          //  整箱数 = totalFullBoxes,
                          //  散数 = totalLooseQuantity,
                          //  散数箱数 = looseBoxes,
                          //  总箱数 = totalBoxes,
                            总重量 = totalFWeight
                        };

                        processedData.Add(newItem);
                    }
                }

                // 更新dataGridView9的数据源
                dataGridView9.DataSource = null;
                dataGridView9.DataSource = processedData;

                // 设置列头
                if (dataGridView9.Columns.Contains("整箱数"))
                    dataGridView9.Columns["整箱数"].HeaderText = "整箱数";
                if (dataGridView9.Columns.Contains("散数"))
                    dataGridView9.Columns["散数"].HeaderText = "散数";
                if (dataGridView9.Columns.Contains("散数箱数"))
                    dataGridView9.Columns["散数箱数"].HeaderText = "散数箱数";
                if (dataGridView9.Columns.Contains("总箱数"))
                    dataGridView9.Columns["总箱数"].HeaderText = "总箱数";
                if (dataGridView9.Columns.Contains("总重量"))
                    dataGridView9.Columns["总重量"].HeaderText = "总重量";

                UIMessageBox.Show($"箱数计算完成！共处理 {processedData.Count} 条订单数据");
            }
            catch (Exception ex)
            {
                UIMessageBox.Show($"计算箱数时出错: {ex.Message}");
            }
        }
        private MatInfo getMatChangeNum(string FNumber)
        {
            //Thread.Sleep(500);

            try
            {
                RestClient client = new RestClient("http://120.78.210.159:3304/api/Wdt/QueryMatChangNum");
                RestRequest request = new RestRequest();
                request.AddHeader("Content-Type", "application/json");
                request.Method = Method.Post;
                RestResponse response;

                var obj = new { FNumber = FNumber };
                var str = JsonConvert.SerializeObject(obj);

                request.AddBody(str);
                response = client.Execute(request);
                var result = JObject.Parse(response.Content);

                return new MatInfo
                {
                    changNum = Convert.ToInt16(result["changNum"]),
                    FWeight = Convert.ToDecimal(result["FWeight"]),
                    FSaleBaseUnitNo = result["FSaleBaseUnitNo"].ToString()

                };
            }
            catch (Exception er)
            {
                return new MatInfo
                {
                    changNum = 0,
                    FWeight = 0,
                    FSaleBaseUnitNo = "支"
                };
            }


        }
        private int getMatStockNum(string FNumber)
        {
            //Thread.Sleep(500);

            try
            {
                RestClient client = new RestClient("http://120.78.210.159:3304/api/Wdt/ExecuteBillQueryRequest");
                RestRequest request = new RestRequest();
                request.AddHeader("Content-Type", "application/json");
                request.Method = Method.Post;
                RestResponse response;

                var obj = new
                {
                    data = new
                    {
                        FormId = "STK_Inventory",
                        FieldKeys = "FBASEQTY",
                        FilterString = $"FBASEQTY>0 and FSTOCKID.FNumber='SCK023' and FSTOCKSTATUSID.FName='合格' and FMATERIALID.FNUMBER='{FNumber}'"

                    }
                };
                var str = JsonConvert.SerializeObject(obj);

                request.AddBody(str);
                response = client.Execute(request);
                var result = JArray.Parse(response.Content);

                // 使用 LINQ 表达式累加所有值
                decimal totalQuantity = result

                    .SelectMany(innerArray => (JArray)innerArray)

                    .Sum(token => token.ToObject<decimal>());

                // 将最终结果转换为 int 类型
                int finalTotal = (int)Math.Round(totalQuantity);
                //  richTextBox2.AppendText(finalTotal.ToString());
                return finalTotal;
            }
            catch (Exception er)
            {
                richTextBox2.AppendText($"查询{FNumber}库存出错:" + er.Message);
                return 0;
            }


        }
        private void uiButton33_Click(object sender, EventArgs e)
        {

            saveFileDialog6.Filter = "Excel 文件(*.xls)|*.xls";
            saveFileDialog6.ShowDialog();
        }

        private void saveFileDialog6_FileOk(object sender, CancelEventArgs e)
        {
            try
            {
                if (dataGridView9.DataSource == null || dataGridView10.DataSource == null)
                {
                    UIMessageBox.Show("请先完成数据查询和计算");
                    return;
                }

                if (saveFileDialog6.FileName == "") return;

                var grid9Data = ((IEnumerable<object>)dataGridView9.DataSource).ToList();
                var grid10Data = ((IEnumerable<object>)dataGridView10.DataSource).ToList();

                // 按收件人分组（收件人值如"华东收货组"等）
                var groups = grid9Data
                    .GroupBy(item => item.GetType().GetProperty("收件人")?.GetValue(item)?.ToString() ?? "")
                    .ToList();

                string tfilepath = saveFileDialog6.FileName;
                MemoryStream ms = new MemoryStream();
                HSSFWorkbook wb = new HSSFWorkbook();

                // 公共样式
                ICellStyle titleStyle = wb.CreateCellStyle();
                IFont titleFont = wb.CreateFont();
                titleFont.FontHeightInPoints = 16;
                titleFont.IsBold = true;
                titleStyle.SetFont(titleFont);
                titleStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                titleStyle.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;

                ICellStyle subTitleStyle = wb.CreateCellStyle();
                IFont subTitleFont = wb.CreateFont();
                subTitleFont.FontHeightInPoints = 13;
                subTitleFont.IsBold = true;
                subTitleStyle.SetFont(subTitleFont);
                subTitleStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                subTitleStyle.VerticalAlignment = VerticalAlignment.Center;

                ICellStyle labelStyle = wb.CreateCellStyle();
                IFont labelFont = wb.CreateFont();
                labelFont.FontHeightInPoints = 11;
                labelStyle.SetFont(labelFont);
                labelStyle.VerticalAlignment = VerticalAlignment.Center;

                ICellStyle headerStyle = wb.CreateCellStyle();
                IFont headerFont = wb.CreateFont();
                headerFont.FontHeightInPoints = 11;
                headerFont.IsBold = true;
                headerStyle.SetFont(headerFont);
                headerStyle.Alignment = HorizontalAlignment.Center;
                headerStyle.VerticalAlignment = VerticalAlignment.Center;
                headerStyle.BorderTop = BorderStyle.Thin;
                headerStyle.BorderBottom = BorderStyle.Thin;
                headerStyle.BorderLeft = BorderStyle.Thin;
                headerStyle.BorderRight = BorderStyle.Thin;

                ICellStyle dataStyle = wb.CreateCellStyle();
                IFont dataFont = wb.CreateFont();
                dataFont.FontHeightInPoints = 11;
                dataStyle.SetFont(dataFont);
                dataStyle.Alignment = HorizontalAlignment.Center;
                dataStyle.VerticalAlignment = VerticalAlignment.Center;
                dataStyle.BorderTop = BorderStyle.Thin;
                dataStyle.BorderBottom = BorderStyle.Thin;
                dataStyle.BorderLeft = BorderStyle.Thin;
                dataStyle.BorderRight = BorderStyle.Thin;

                ICellStyle totalStyle = wb.CreateCellStyle();
                IFont totalFont = wb.CreateFont();
                totalFont.FontHeightInPoints = 11;
                totalFont.IsBold = true;
                totalStyle.SetFont(totalFont);
                totalStyle.Alignment = HorizontalAlignment.Center;
                totalStyle.VerticalAlignment = VerticalAlignment.Center;
                totalStyle.BorderTop = BorderStyle.Thin;
                totalStyle.BorderBottom = BorderStyle.Thin;
                totalStyle.BorderLeft = BorderStyle.Thin;
                totalStyle.BorderRight = BorderStyle.Thin;

                ICellStyle footerStyle = wb.CreateCellStyle();
                IFont footerFont = wb.CreateFont();
                footerFont.FontHeightInPoints = 11;
                footerStyle.SetFont(footerFont);
                footerStyle.VerticalAlignment = VerticalAlignment.Center;
                footerStyle.BorderTop = BorderStyle.Thin;
                footerStyle.BorderBottom = BorderStyle.Thin;
                footerStyle.BorderLeft = BorderStyle.Thin;
                footerStyle.BorderRight = BorderStyle.Thin;

                string today = DateTime.Now.ToString("yyyy-MM-dd");

                foreach (var grp in groups)
                {
                    string sheetName = grp.Key;
                    if (string.IsNullOrEmpty(sheetName)) sheetName = "未知";
                    // Excel sheet名不能超过31字符
                    if (sheetName.Length > 31) sheetName = sheetName.Substring(0, 31);

                    ISheet sheet = wb.CreateSheet(sheetName);
                    sheet.SetColumnWidth(0, 18 * 256); // 组合编码
                    sheet.SetColumnWidth(1, 22 * 256); // 组合名称
                    sheet.SetColumnWidth(2, 10 * 256); // 套数
                    sheet.SetColumnWidth(3, 10 * 256); // 箱数
                    sheet.SetColumnWidth(4, 12 * 256); // 重量
                    sheet.SetColumnWidth(5, 18 * 256); // 备注

                    int rowIdx = 0;

                    // 获取该收货组的所有原始单号
                    var tids = grp.Select(item => item.GetType().GetProperty("原始单号")?.GetValue(item)?.ToString() ?? "").ToList();

                    // 获取该收货组的地址（取第一条）
                    string address = grp.First().GetType().GetProperty("地址")?.GetValue(grp.First())?.ToString() ?? "";
                    string phone = grp.First().GetType().GetProperty("电话")?.GetValue(grp.First())?.ToString() ?? "";

                    // 获取该收货组在dataGridView10中的组合装数据，按组合装编码分组汇总
                    var suiteItems = grid10Data
                        .Where(x => tids.Contains(x.GetType().GetProperty("原始单号")?.GetValue(x)?.ToString() ?? ""))
                        .GroupBy(x => new
                        {
                            组合装编码 = x.GetType().GetProperty("组合装编码")?.GetValue(x)?.ToString() ?? "",
                            组合装名称 = x.GetType().GetProperty("组合装名称")?.GetValue(x)?.ToString() ?? ""
                        })
                        .Select(g =>
                        {
                            // 套数：取组合装数量之和（每个原始单号只取一次）
                            var distinctTids = g.GroupBy(x => x.GetType().GetProperty("原始单号")?.GetValue(x)?.ToString() ?? "");
                            int totalSuites = 0;
                            decimal totalWeight = 0;
                            foreach (var tidGrp in distinctTids)
                            {
                                var first = tidGrp.First();
                                var firstType = first.GetType();
                                string qtyStr = firstType.GetProperty("组合装数量")?.GetValue(first)?.ToString() ?? "0";
                                if (int.TryParse(qtyStr, out int qty)) totalSuites += qty;
                                // 重量：该原始单号下该组合装的重量合计
                                foreach (var x in tidGrp)
                                {
                                    var xType = x.GetType();
                                    decimal w = Convert.ToDecimal(xType.GetProperty("重量合计")?.GetValue(x) ?? 0);
                                    totalWeight += w;
                                }
                            }
                            return new
                            {
                                组合装编码 = g.Key.组合装编码,
                                组合名称 = g.Key.组合装名称,
                                套数 = totalSuites,
                                重量 = totalWeight
                            };
                        })
                        .Where(x => !string.IsNullOrEmpty(x.组合装编码))
                        .ToList();

                    // 行1：公司名称（A-F合并居中）
                    var row0 = sheet.CreateRow(rowIdx++);
                    row0.Height = 600;
                    for (int c = 0; c < 6; c++) { var cell = row0.CreateCell(c); cell.CellStyle = titleStyle; }
                    row0.GetCell(0).SetCellValue("舒蕾生物科技有限公司");
                    sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(rowIdx - 1, rowIdx - 1, 0, 5));

                    // 行2：送货单（A-F合并居中）
                    var row1 = sheet.CreateRow(rowIdx++);
                    row1.Height = 500;
                    for (int c = 0; c < 6; c++) { var cell = row1.CreateCell(c); cell.CellStyle = subTitleStyle; }
                    row1.GetCell(0).SetCellValue("送货单");
                    sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(rowIdx - 1, rowIdx - 1, 0, 5));

                    // 行3：客户 + 订单日期
                    var row2 = sheet.CreateRow(rowIdx++);
                    row2.Height = 400;
                    for (int c = 0; c < 6; c++) { var cell = row2.CreateCell(c); cell.CellStyle = labelStyle; }
                    row2.GetCell(0).SetCellValue("客户：唯品会");
                    sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(rowIdx - 1, rowIdx - 1, 0, 2));
                    row2.GetCell(3).SetCellValue("订单日期：" + today);
                    sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(rowIdx - 1, rowIdx - 1, 3, 5));

                    // 行4：地址
                    var row3 = sheet.CreateRow(rowIdx++);
                    row3.Height = 400;
                    for (int c = 0; c < 6; c++) { var cell = row3.CreateCell(c); cell.CellStyle = labelStyle; }
                    row3.GetCell(0).SetCellValue("地址：" + address);
                    sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(rowIdx - 1, rowIdx - 1, 0, 5));

                    // 行5：收件人 + 电话
                    var row4 = sheet.CreateRow(rowIdx++);
                    row4.Height = 400;
                    for (int c = 0; c < 6; c++) { var cell = row4.CreateCell(c); cell.CellStyle = labelStyle; }
                    row4.GetCell(0).SetCellValue("收件人：" + sheetName);
                    sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(rowIdx - 1, rowIdx - 1, 0, 2));
                    row4.GetCell(3).SetCellValue("电话：" + phone);
                    sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(rowIdx - 1, rowIdx - 1, 3, 5));

                    // 行6：表格列头
                    var headerRow = sheet.CreateRow(rowIdx++);
                    headerRow.Height = 450;
                    string[] headers = { "组合编码", "组合名称", "套数", "箱数", "重量", "备注" };
                    for (int c = 0; c < headers.Length; c++)
                    {
                        var cell = headerRow.CreateCell(c);
                        cell.SetCellValue(headers[c]);
                        cell.CellStyle = headerStyle;
                    }

                    // 数据行
                    int dataStartRow = rowIdx;
                    decimal totalSuitesAll = 0;
                    decimal totalWeightAll = 0;
                    foreach (var si in suiteItems)
                    {
                        var dataRow = sheet.CreateRow(rowIdx++);
                        dataRow.Height = 400;
                        dataRow.CreateCell(0).SetCellValue(si.组合装编码); dataRow.GetCell(0).CellStyle = dataStyle;
                        dataRow.CreateCell(1).SetCellValue(si.组合名称);   dataRow.GetCell(1).CellStyle = dataStyle;
                        dataRow.CreateCell(2).SetCellValue(si.套数);       dataRow.GetCell(2).CellStyle = dataStyle;
                        dataRow.CreateCell(3).SetCellValue("");             dataRow.GetCell(3).CellStyle = dataStyle; // 箱数空置
                        dataRow.CreateCell(4).SetCellValue((double)si.重量); dataRow.GetCell(4).CellStyle = dataStyle;
                        dataRow.CreateCell(5).SetCellValue("");             dataRow.GetCell(5).CellStyle = dataStyle;
                        totalSuitesAll += si.套数;
                        totalWeightAll += si.重量;
                    }

                    // 合计行
                    var totalRow = sheet.CreateRow(rowIdx++);
                    totalRow.Height = 400;
                    totalRow.CreateCell(0).SetCellValue("合计"); totalRow.GetCell(0).CellStyle = totalStyle;
                    totalRow.CreateCell(1).SetCellValue("");     totalRow.GetCell(1).CellStyle = totalStyle;
                    totalRow.CreateCell(2).SetCellValue((double)totalSuitesAll); totalRow.GetCell(2).CellStyle = totalStyle;
                    totalRow.CreateCell(3).SetCellValue("");     totalRow.GetCell(3).CellStyle = totalStyle;
                    totalRow.CreateCell(4).SetCellValue((double)totalWeightAll); totalRow.GetCell(4).CellStyle = totalStyle;
                    totalRow.CreateCell(5).SetCellValue("");     totalRow.GetCell(5).CellStyle = totalStyle;

                    // 空行
                    sheet.CreateRow(rowIdx++);

                    // 底部签收区 - 第一行标签
                    var footerRow1 = sheet.CreateRow(rowIdx++);
                    footerRow1.Height = 400;
                    for (int c = 0; c < 6; c++) { var cell = footerRow1.CreateCell(c); cell.CellStyle = footerStyle; }
                    footerRow1.GetCell(0).SetCellValue("仓库");
                    sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(rowIdx - 1, rowIdx - 1, 0, 1));
                    footerRow1.GetCell(2).SetCellValue("承运商");
                    sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(rowIdx - 1, rowIdx - 1, 2, 3));
                    footerRow1.GetCell(4).SetCellValue("收货方");
                    sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(rowIdx - 1, rowIdx - 1, 4, 5));

                    // 底部签收区 - 第二行
                    var footerRow2 = sheet.CreateRow(rowIdx++);
                    footerRow2.Height = 400;
                    for (int c = 0; c < 6; c++) { var cell = footerRow2.CreateCell(c); cell.CellStyle = footerStyle; }
                    footerRow2.GetCell(0).SetCellValue("交接人：");
                    sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(rowIdx - 1, rowIdx - 1, 0, 1));
                    footerRow2.GetCell(2).SetCellValue("承运人：");
                    sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(rowIdx - 1, rowIdx - 1, 2, 3));
                    footerRow2.GetCell(4).SetCellValue("签收人：");
                    sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(rowIdx - 1, rowIdx - 1, 4, 5));

                    // 底部签收区 - 第三行（日期）
                    var footerRow3 = sheet.CreateRow(rowIdx++);
                    footerRow3.Height = 400;
                    for (int c = 0; c < 6; c++) { var cell = footerRow3.CreateCell(c); cell.CellStyle = footerStyle; }
                    footerRow3.GetCell(0).SetCellValue("日期：");
                    sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(rowIdx - 1, rowIdx - 1, 0, 1));
                    footerRow3.GetCell(2).SetCellValue("日期：");
                    sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(rowIdx - 1, rowIdx - 1, 2, 3));
                    footerRow3.GetCell(4).SetCellValue("日期：");
                    sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(rowIdx - 1, rowIdx - 1, 4, 5));
                }

                wb.Write(ms);
                ms.Flush();
                ms.Position = 0;

                FileStream dumpFile = new FileStream(tfilepath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
                ms.WriteTo(dumpFile);
                dumpFile.Close();
                this.Invoke(new Action(() =>
                {
                    UIMessageBox.Show("数据导出完成!");
                }));
            }
            catch (Exception er)
            {
                UIMessageBox.Show("出现错误:" + er.Message);
            }
        }
      
        private void uiButton34_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView9.DataSource == null || dataGridView10.DataSource == null)
                {
                    MessageBox.Show("请先加载订单数据和商品数据");
                    return;
                }

                // 获取数据源并转换为列表
                var grid9Data = ((IEnumerable<object>)dataGridView9.DataSource).ToList();
                var grid10Data = ((IEnumerable<object>)dataGridView10.DataSource).ToList();

                // 获取所有商品并按货号排序
                var allProducts = grid10Data
                    .Select(x =>
                    {
                        var type = x.GetType();
                        return new
                        {
                            Name = type.GetProperty("货品名称")?.GetValue(x)?.ToString() ?? "",
                            No = type.GetProperty("货号")?.GetValue(x)?.ToString() ?? ""
                        };
                    })
                    .Where(x => !string.IsNullOrEmpty(x.Name) && !string.IsNullOrEmpty(x.No))
                    .GroupBy(x => x.Name)
                    .Select(g => g.First())
                    .OrderBy(x => x.No)
                    .ToList();

                // 创建数据表作为数据源
                // 创建数据表作为数据源
                DataTable dt = new DataTable();
                dt.Columns.Add("序号", typeof(int));
                dt.Columns.Add("收货人", typeof(string));
                dt.Columns.Add("原始单号", typeof(string)); // 新增外部订单号列

                // 添加商品列
                foreach (var product in allProducts)
                {
                    dt.Columns.Add($"{product.No}{product.Name}", typeof(int));
                }

                // 添加总箱数列
                dt.Columns.Add("总箱数", typeof(int));

                // 填充数据
                int rowIndex = 1;
                foreach (var order in grid9Data)
                {
                    var orderType = order.GetType();
                    string tid = orderType.GetProperty("原始单号")?.GetValue(order)?.ToString() ?? "";
                    string contact = orderType.GetProperty("收件人")?.GetValue(order)?.ToString() ?? "";

                    // 从grid10Data中获取外部订单号
                    var firstItem = grid10Data.FirstOrDefault(x =>
                        x.GetType().GetProperty("原始单号")?.GetValue(x)?.ToString() == tid);
                    string outerNo = firstItem?.GetType().GetProperty("原始单号")?.GetValue(firstItem)?.ToString() ?? "";

                    DataRow row = dt.NewRow();
                    row["序号"] = rowIndex++;
                    row["收货人"] = contact;
                    row["原始单号"] = outerNo;

                    // 初始化所有商品数量为0
                    foreach (var product in allProducts)
                    {
                        row[$"{product.No}{product.Name}"] = 0;
                    }

                    int totalBoxes = 0; // 计算总箱数

                    // 查找并填充商品数量
                    var orderItems = grid10Data
                        .Where(x => x.GetType().GetProperty("原始单号")?.GetValue(x)?.ToString() == tid)
                        .ToList();

                    foreach (var item in orderItems)
                    {
                        var itemType = item.GetType();
                        string goodsNo = itemType.GetProperty("货号")?.GetValue(item)?.ToString() ?? "";
                        string goodsName = itemType.GetProperty("货品名称")?.GetValue(item)?.ToString() ?? "";
                        string quantityStr = itemType.GetProperty("整箱数")?.GetValue(item)?.ToString() ?? "0";

                        if (int.TryParse(quantityStr, out int quantity))
                        {
                            string columnName = $"{goodsNo}{goodsName}";
                            if (dt.Columns.Contains(columnName))
                            {
                                row[columnName] = (int)row[columnName] + quantity;
                                totalBoxes += quantity; // 累加总箱数
                            }
                        }
                    }

                    row["总箱数"] = totalBoxes; // 设置总箱数
                    dt.Rows.Add(row);
                }

                // 绑定数据到dataGridView12
                dataGridView12.DataSource = null;
                dataGridView12.DataSource = dt;

                // 设置表格样式
                dataGridView12.AlternatingRowsDefaultCellStyle.BackColor = Color.LightGray;
                dataGridView12.ReadOnly = true;
                saveFileDialog7.Filter = "Excel 文件(*.xls)|*.xls";
                saveFileDialog7.ShowDialog();
            }
            catch (Exception ex)
            {
                UIMessageBox.Show("出现错误:" + ex.Message);
            }
        }
        private void uiButton46_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView9.DataSource == null || dataGridView10.DataSource == null)
                {
                    MessageBox.Show("请先完成数据查询和计算");
                    return;
                }

                var grid9Data = ((IEnumerable<object>)dataGridView9.DataSource).ToList();
                var grid10Data = ((IEnumerable<object>)dataGridView10.DataSource).ToList();

                // 收集所有唯一的组合装编码+货号组合（保持原始顺序）
                var allSpecs = grid10Data
                    .Select(x =>
                    {
                        var t = x.GetType();
                        return new
                        {
                            组合装编码 = t.GetProperty("组合装编码")?.GetValue(x)?.ToString() ?? "",
                            货号 = t.GetProperty("货号")?.GetValue(x)?.ToString() ?? ""
                        };
                    })
                    .Where(x => !string.IsNullOrEmpty(x.货号))
                    .GroupBy(x => new { x.组合装编码, x.货号 })
                    .Select(g => g.Key)
                    .OrderBy(x => x.组合装编码).ThenBy(x => x.货号)
                    .ToList();

                // 按组合装编码分组，得到每个组合装编码下的货号列表
                var specGroups = allSpecs
                    .GroupBy(x => x.组合装编码)
                    .Select(g => new { 组合装编码 = g.Key, 货号列表 = g.Select(x => x.货号).ToList() })
                    .ToList();

                string tfilepath = "";
                using (var sfd = new System.Windows.Forms.SaveFileDialog())
                {
                    sfd.Filter = "Excel 文件(*.xls)|*.xls";
                    if (sfd.ShowDialog() != DialogResult.OK) return;
                    tfilepath = sfd.FileName;
                }

                HSSFWorkbook wb = new HSSFWorkbook();
                ISheet sheet = wb.CreateSheet("VIP拣货单");

                // 样式
                ICellStyle headerStyle = wb.CreateCellStyle();
                headerStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                headerStyle.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;
                headerStyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                headerStyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                headerStyle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                headerStyle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                IFont hFont = wb.CreateFont();
                hFont.IsBold = true;
                headerStyle.SetFont(hFont);

                ICellStyle dataStyle = wb.CreateCellStyle();
                dataStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                dataStyle.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;
                dataStyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                dataStyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                dataStyle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                dataStyle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;

                sheet.SetColumnWidth(0, 22 * 256);
                sheet.SetColumnWidth(1, 14 * 256);

                // 预建每个订单的 qtyMap（组合装编码|货号 -> 数量）
                var allQtyMaps = new Dictionary<string, Dictionary<string, int>>();
                foreach (var order in grid9Data)
                {
                    var orderType = order.GetType();
                    string tid = orderType.GetProperty("原始单号")?.GetValue(order)?.ToString() ?? "";
                    var qtyMap = new Dictionary<string, int>();
                    var orderItems = grid10Data
                        .Where(x => x.GetType().GetProperty("原始单号")?.GetValue(x)?.ToString() == tid)
                        .ToList();
                    foreach (var item in orderItems)
                    {
                        var it = item.GetType();
                        string suiteNo = it.GetProperty("组合装编码")?.GetValue(item)?.ToString() ?? "";
                        string goodsNo = it.GetProperty("货号")?.GetValue(item)?.ToString() ?? "";
                        string qtyStr = it.GetProperty("数量")?.GetValue(item)?.ToString() ?? "0";
                        string key = suiteNo + "|" + goodsNo;
                        if (!string.IsNullOrEmpty(goodsNo) && int.TryParse(qtyStr, out int qty))
                        {
                            if (qtyMap.ContainsKey(key)) qtyMap[key] += qty;
                            else qtyMap[key] = qty;
                        }
                    }
                    allQtyMaps[tid] = qtyMap;
                }

                // 计算总货号列数
                int totalGoodsCols = specGroups.Sum(g => g.货号列表.Count);
                const int MAX_COLS = 8; // 每块最多货号列数

                // 将 specGroups 分配到各块，严格保证每块不超过 MAX_COLS 列，同一组合装不拆散
                var blocks = new List<List<dynamic>>();
                var currentBlock = new List<dynamic>();
                int currentBlockCols = 0;
                foreach (var g in specGroups)
                {
                    int groupCols = g.货号列表.Count;
                    // 只要加入后会超过上限，且当前块已有内容，就切块
                    if (currentBlockCols > 0 && currentBlockCols + groupCols > MAX_COLS)
                    {
                        blocks.Add(currentBlock);
                        currentBlock = new List<dynamic>();
                        currentBlockCols = 0;
                    }
                    currentBlock.Add(g);
                    currentBlockCols += groupCols;
                }
                if (currentBlock.Count > 0) blocks.Add(currentBlock);

                // 写每个块，块间空2行
                int startRow = 0;
                foreach (var block in blocks)
                {
                    // 表头行0：原始单号 | 收件人 | 组合装编码（合并）
                    // 表头行1：空 | 空 | 货号
                    var row0 = sheet.CreateRow(startRow);
                    var row1 = sheet.CreateRow(startRow + 1);

                    var c00 = row0.CreateCell(0); c00.SetCellValue("原始单号"); c00.CellStyle = headerStyle;
                    var c01 = row0.CreateCell(1); c01.SetCellValue("收件人"); c01.CellStyle = headerStyle;
                    sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(startRow, startRow + 1, 0, 0));
                    sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(startRow, startRow + 1, 1, 1));
                    var c10 = row1.CreateCell(0); c10.CellStyle = headerStyle;
                    var c11 = row1.CreateCell(1); c11.CellStyle = headerStyle;

                    int colIdx = 2;
                    foreach (var grp in block)
                    {
                        int startCol = colIdx;
                        foreach (var goodsNo in grp.货号列表)
                        {
                            var c1 = row1.CreateCell(colIdx);
                            c1.SetCellValue((string)goodsNo);
                            c1.CellStyle = headerStyle;
                            sheet.SetColumnWidth(colIdx, 10 * 256);
                            colIdx++;
                        }
                        int endCol = colIdx - 1;
                        for (int ci = startCol; ci <= endCol; ci++)
                        {
                            var c0 = row0.CreateCell(ci);
                            c0.SetCellValue(ci == startCol ? (string)grp.组合装编码 : "");
                            c0.CellStyle = headerStyle;
                        }
                        if (endCol > startCol)
                            sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(startRow, startRow, startCol, endCol));
                    }

                    // 数据行
                    int rowNum = startRow + 2;
                    foreach (var order in grid9Data)
                    {
                        var orderType = order.GetType();
                        string tid = orderType.GetProperty("原始单号")?.GetValue(order)?.ToString() ?? "";
                        string contact = orderType.GetProperty("收件人")?.GetValue(order)?.ToString() ?? "";

                        var dataRow = sheet.CreateRow(rowNum++);
                        var tidCell = dataRow.CreateCell(0); tidCell.SetCellValue(tid); tidCell.CellStyle = dataStyle;
                        var contactCell = dataRow.CreateCell(1); contactCell.SetCellValue(contact); contactCell.CellStyle = dataStyle;

                        var qtyMap = allQtyMaps.ContainsKey(tid) ? allQtyMaps[tid] : new Dictionary<string, int>();
                        int dc = 2;
                        foreach (var grp in block)
                        {
                            foreach (var goodsNo in grp.货号列表)
                            {
                                string key = (string)grp.组合装编码 + "|" + (string)goodsNo;
                                int qty = qtyMap.ContainsKey(key) ? qtyMap[key] : 0;
                                var cell = dataRow.CreateCell(dc);
                                cell.SetCellValue(qty);
                                cell.CellStyle = dataStyle;
                                dc++;
                            }
                        }
                    }

                    // 下一块起始行 = 当前块结束行 + 2空行 + 1
                    startRow = startRow + 2 + grid9Data.Count + 2;
                }

                MemoryStream ms = new MemoryStream();
                wb.Write(ms);
                ms.Flush();
                ms.Position = 0;
                using (FileStream fs = new FileStream(tfilepath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    ms.WriteTo(fs);
                }
                UIMessageBox.Show("VIP拣货单导出完成!");
            }
            catch (Exception ex)
            {
                UIMessageBox.Show("导出出错: " + ex.Message);
            }
        }

private void uiButton35_Click(object sender, EventArgs e)
{
    // ... existing code ...
    try
    {
        int specifiedBoxSpec = 0;
        if (!int.TryParse(uiTextBox12.Text, out specifiedBoxSpec) || specifiedBoxSpec <= 0)
        {
            MessageBox.Show("请输入有效的箱规数值");
            return;
        }
        if (dataGridView9.DataSource == null || dataGridView10.DataSource == null)
        {
            MessageBox.Show("请先加载订单数据和商品数据");
            return;
        }

        // 获取数据源并转换为列表
        var grid9Data = ((IEnumerable<object>)dataGridView9.DataSource).ToList();
        var grid10Data = ((IEnumerable<object>)dataGridView10.DataSource).ToList();

        // 处理 uiTextBox18 指定的货号列表（以逗号分隔），并校验是否存在
        string[] specificProductCodes = null;
        bool hasSpecificProducts = false;
        if (!string.IsNullOrWhiteSpace(uiTextBox18.Text))
        {
            string specificProductText = uiTextBox18.Text.Trim();
            // 统一按逗号分隔；支持单个或多个货号
            specificProductCodes = specificProductText
                .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim())
                .ToArray();

            if (specificProductCodes.Length > 0)
            {
                hasSpecificProducts = true;
                // 校验每个货号是否在 grid10Data 中存在
                foreach (var code in specificProductCodes)
                {
                    bool exists = grid10Data.Any(x =>
                        x.GetType().GetProperty("货号")?.GetValue(x)?.ToString() == code);
                    if (!exists)
                    {
                        UIMessageBox.Show($"货号 '{code}' 不存在于订单明细中，请检查后重试");
                        return;
                    }
                }
            }
        }

        // 获取所有商品并按货号排序（用于生成列）
        var allProducts = grid10Data
            .Select(x =>
            {
                var type = x.GetType();
                return new
                {
                    Name = type.GetProperty("货品名称")?.GetValue(x)?.ToString() ?? "",
                    No = type.GetProperty("货号")?.GetValue(x)?.ToString() ?? ""
                };
            })
            .Where(x => !string.IsNullOrEmpty(x.Name) && !string.IsNullOrEmpty(x.No))
            .GroupBy(x => x.Name)
            .Select(g => g.First())
            .OrderBy(x => x.No)
            .ToList();

        // 创建数据表作为数据源
        DataTable dt = new DataTable();
        dt.Columns.Add("序号", typeof(int));
        dt.Columns.Add("收货人", typeof(string));
        dt.Columns.Add("原始单号", typeof(string));

        // 添加商品列
        foreach (var product in allProducts)
        {
            dt.Columns.Add($"{product.No}{product.Name}", typeof(int));
        }

        // 添加合计数列
        dt.Columns.Add("支数", typeof(int));
        dt.Columns.Add("箱数", typeof(int));

        // 填充数据
        int rowIndex = 1;
        foreach (var order in grid9Data)
        {
            var orderType = order.GetType();
            string tid = orderType.GetProperty("原始单号")?.GetValue(order)?.ToString() ?? "";
            string contact = orderType.GetProperty("收件人")?.GetValue(order)?.ToString() ?? "";

            // 从grid10Data中获取原始单号
            var firstItem = grid10Data.FirstOrDefault(x =>
                x.GetType().GetProperty("原始单号")?.GetValue(x)?.ToString() == tid);
            string outerNo = firstItem?.GetType().GetProperty("原始单号")?.GetValue(firstItem)?.ToString() ?? "";

            DataRow row = dt.NewRow();
            row["序号"] = rowIndex++;
            row["收货人"] = contact;
            row["原始单号"] = outerNo;

            // 初始化所有商品数量为0
            foreach (var product in allProducts)
            {
                row[$"{product.No}{product.Name}"] = 0;
            }

            int totalLooseQtyAll = 0; // 全部散数总和（用于“支数”）

            // 查找该订单的商品明细
            var orderItems = grid10Data
                .Where(x => x.GetType().GetProperty("原始单号")?.GetValue(x)?.ToString() == tid)
                .ToList();

            // 逐项填充每个商品的散数到对应列，并累计总散数
            foreach (var item in orderItems)
            {
                var itemType = item.GetType();
                string goodsNo = itemType.GetProperty("货号")?.GetValue(item)?.ToString() ?? "";
                string goodsName = itemType.GetProperty("货品名称")?.GetValue(item)?.ToString() ?? "";
                string quantityStr = itemType.GetProperty("散数")?.GetValue(item)?.ToString() ?? "0";

                if (int.TryParse(quantityStr, out int quantity) && quantity > 0)
                {
                    string columnName = $"{goodsNo}{goodsName}";
                    if (dt.Columns.Contains(columnName))
                    {
                        row[columnName] = (int)row[columnName] + quantity;
                        totalLooseQtyAll += quantity;
                    }
                }
            }

            // 根据是否指定特定货号来计算“箱数”
            int totalBoxes = 0;
            if (hasSpecificProducts)
            {
                // 拆分为特定货号与其他货号
                var specificItems = new List<object>();
                var otherItems = new List<object>();

                foreach (var item in orderItems)
                {
                    var relatedType = item.GetType();
                    string goodsNo = relatedType.GetProperty("货号")?.GetValue(item)?.ToString() ?? "";
                    if (specificProductCodes.Contains(goodsNo))
                        specificItems.Add(item);
                    else
                        otherItems.Add(item);
                }

                // 其他货号：使用通用箱规
                int otherLooseQty = otherItems.Sum(oi =>
                {
                    var t = oi.GetType();
                    int q = 0;
                    int.TryParse(t.GetProperty("散数")?.GetValue(oi)?.ToString() ?? "0", out q);
                    return q;
                });
                int boxesOthers = otherLooseQty > 0 ? (int)Math.Ceiling((double)otherLooseQty / specifiedBoxSpec) : 0;

                // 特定货号：使用各自箱规
                int boxesSpecificSum = 0;
                foreach (var si in specificItems)
                {
                    var t = si.GetType();
                    int looseQty = 0;
                    int.TryParse(t.GetProperty("散数")?.GetValue(si)?.ToString() ?? "0", out looseQty);

                    int itemBoxSpec = 0;
                    int.TryParse(t.GetProperty("箱规")?.GetValue(si)?.ToString() ?? "0", out itemBoxSpec);
                    if (itemBoxSpec <= 0) itemBoxSpec = specifiedBoxSpec;

                    int itemBoxes = looseQty > 0 ? (int)Math.Ceiling((double)looseQty / itemBoxSpec) : 0;
                    boxesSpecificSum += itemBoxes;
                }

                totalBoxes = boxesOthers + boxesSpecificSum;
            }
            else
            {
                // 原有逻辑：全部散数按通用箱规计算
                totalBoxes = totalLooseQtyAll > 0
                    ? (int)Math.Ceiling((double)totalLooseQtyAll / specifiedBoxSpec)
                    : 0;
            }

            row["支数"] = totalLooseQtyAll;
            row["箱数"] = totalBoxes;

            dt.Rows.Add(row);
        }

        // 绑定数据到dataGridView12
        dataGridView12.DataSource = null;
        dataGridView12.DataSource = dt;

        // 设置表格样式与保存
        dataGridView12.AlternatingRowsDefaultCellStyle.BackColor = Color.LightGray;
        dataGridView12.ReadOnly = true;
        saveFileDialog8.Filter = "Excel 文件(*.xls)|*.xls";
        saveFileDialog8.ShowDialog();
    }
    catch (Exception ex)
    {
        UIMessageBox.Show("出现错误:" + ex.Message);
    }
   
}
        private void saveFileDialog7_FileOk(object sender, CancelEventArgs e)
        {
            try
            {
                var datas = JArray.FromObject(dataGridView12.DataSource);

                if (saveFileDialog7.FileName != "")
                {
                    string tfilepath = saveFileDialog7.FileName;

                    HSSFWorkbook wb;
                    FileStream file;

                    MemoryStream ms = new MemoryStream();
                    wb = new HSSFWorkbook();

                    ISheet sheet;
                    sheet = wb.CreateSheet("sheet1");

                    // 设置打印格式
                    sheet.PrintSetup.PaperSize = (short)PaperSize.A4_Small;           // A4纸张
                    sheet.PrintSetup.Landscape = true;                   // 横向打印
                    sheet.PrintSetup.Scale = 100;                        // 设置缩放比例为100%

                    sheet.PrintSetup.UsePage = false;                   // 禁用自动页面调整

                    sheet.PrintSetup.FitWidth = 1;                              // 适应1页宽度
                    sheet.PrintSetup.FitHeight = 0;                             // 高度不限制，允许多页

                    // 设置页边距（单位：英寸，1cm = 0.393701英寸）
                    sheet.SetMargin(MarginType.TopMargin, 0.393701);     // 上边距1cm
                    sheet.SetMargin(MarginType.BottomMargin, 0.393701);  // 下边距1cm
                    sheet.SetMargin(MarginType.LeftMargin, 0.393701);    // 左边距1cm
                    sheet.SetMargin(MarginType.RightMargin, 0.393701);   // 右边距1cm

                    // 设置页眉页脚为0
                    sheet.SetMargin(MarginType.HeaderMargin, 0);
                    sheet.SetMargin(MarginType.FooterMargin, 0);

                    // 设置水平居中打印
                    sheet.HorizontallyCenter = true;

                    sheet.RepeatingRows = CellRangeAddress.ValueOf("1:1"); //设置第1行（表头行）在每页都重复显示

                    ICellStyle cellStyle = wb.CreateCellStyle();
                    NPOI.SS.UserModel.IRow rowTemp;
                    rowTemp = sheet.CreateRow(0);//创建第一行
                    short defaultHeight = rowTemp.Height; // 获取默认行高

                    JObject header = (JObject)datas.First();
                    rowTemp = sheet.CreateRow(0);
                    rowTemp.Height = (short)(defaultHeight * 7); // 表头行高为默认7倍

                    // 动态设置列宽
                    int columnCount = header.Properties().Count();
                    for (int i = 0; i < columnCount; i++)
                    {
                        sheet.SetColumnWidth(i, i == 0 ? 4 * 256 : 8 * 256);
                    }
                    sheet.SetColumnWidth(2, 8 * 256);
                    sheet.SetColumnWidth(1, 9 * 256);
                    // 表头样式 - 添加边框
                    ICellStyle headerStyle = wb.CreateCellStyle();
                    IFont headerFont = wb.CreateFont();
                    headerFont.FontHeightInPoints = 10; // 设置表头字体大小为10号
                    headerStyle.SetFont(headerFont);
                    headerStyle.WrapText = true;
                    headerStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                    headerStyle.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;

                    // 为表头添加边框
                    headerStyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                    headerStyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                    headerStyle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                    headerStyle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                    headerStyle.TopBorderColor = IndexedColors.Black.Index;
                    headerStyle.BottomBorderColor = IndexedColors.Black.Index;
                    headerStyle.LeftBorderColor = IndexedColors.Black.Index;
                    headerStyle.RightBorderColor = IndexedColors.Black.Index;

                    // 数据样式 - 添加边框
                    ICellStyle dataStyle = wb.CreateCellStyle();
                    IFont dataFont = wb.CreateFont();
                    dataFont.FontHeightInPoints = 10; // 设置数据字体大小为10号
                    dataStyle.SetFont(dataFont);
                    dataStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                    dataStyle.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;

                    // 为数据单元格添加边框
                    dataStyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                    dataStyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                    dataStyle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                    dataStyle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                    dataStyle.TopBorderColor = IndexedColors.Black.Index;
                    dataStyle.BottomBorderColor = IndexedColors.Black.Index;
                    dataStyle.LeftBorderColor = IndexedColors.Black.Index;
                    dataStyle.RightBorderColor = IndexedColors.Black.Index;

                    int c = 0;
                    foreach (JProperty prop in header.Properties())
                    {
                        var cell = rowTemp.CreateCell(c);
                        cell.SetCellValue(prop.Name.ToString());
                        cell.CellStyle = headerStyle;
                        c++;
                    }


                    int j = 1;
                    // 创建一个字典用于存储每列的数值总和
                    Dictionary<int, decimal> columnSums = new Dictionary<int, decimal>();
                    // 创建一个字典用于标记哪些列是数值列
                    Dictionary<int, bool> isNumericColumn = new Dictionary<int, bool>();

                    datas.ForEach(r =>
                    {
                        JObject dataRow = (JObject)r;
                        // 检查总箱数是否为0
                        if (dataRow["总箱数"]?.ToString() != "0")
                        {
                            rowTemp = sheet.CreateRow(j);
                            rowTemp.Height = (short)(defaultHeight * 3); // 数据行高为默认3倍

                            c = 0;
                            foreach (JProperty prop in dataRow.Properties())
                            {
                                var cell = rowTemp.CreateCell(c);
                                // 如果是外部订单号列，生成二维码
                                if (prop.Name == "原始单号")
                                {
                                    // 创建二维码图片
                                    using (var originalQrCodeImage = GenerateQRCode(prop.Value.ToString()))
                                    {
                                        // 调整二维码图片大小
                                        using (var resizedQrCodeImage = new Bitmap(originalQrCodeImage, new Size(50, 50)))
                                        {
                                            // 将图片添加到工作簿
                                            var pictureIdx = wb.AddPicture(ImageToByteArray(resizedQrCodeImage), NPOI.SS.UserModel.PictureType.PNG);
                                            var drawing = sheet.CreateDrawingPatriarch();
                                            var anchor = wb.GetCreationHelper().CreateClientAnchor();

                                            // 设置图片位置和大小
                                            anchor.Col1 = c;           // 起始列
                                            anchor.Row1 = j;           // 起始行
                                            anchor.Col2 = c;       // 结束列
                                            anchor.Row2 = j;       // 结束行

                                            // 设置图片在单元格内的位置（使用EMU单位）
                                            anchor.Dx1 = 75;    // 左边距
                                            anchor.Dy1 = 4;    // 上边距
                                            anchor.Dx2 = 1;    // 右边距  
                                            anchor.Dy2 = 1;    // 下边距

                                            // 创建图片对象
                                            var picture = drawing.CreatePicture(anchor, pictureIdx);
                                            picture.Resize(); // 自动调整图片大小以适应单元格
                                        }
                                    }
                                }
                                else
                                {
                                    string valueStr = prop.Value.ToString();
                                    cell.SetCellValue(valueStr);

                                    // 检查是否为数值列并累加
                                    decimal numValue;
                                    bool isNumeric = decimal.TryParse(valueStr, out numValue);

                                    // 如果是第一行数据，初始化该列是否为数值列
                                    if (j == 1)
                                    {
                                        isNumericColumn[c] = isNumeric;
                                    }

                                    // 如果是数值列，累加总和
                                    if (isNumeric && (isNumericColumn.ContainsKey(c) && isNumericColumn[c]))
                                    {
                                        if (!columnSums.ContainsKey(c))
                                        {
                                            columnSums[c] = 0;
                                        }
                                        columnSums[c] += numValue;
                                    }
                                    else
                                    {
                                        // 如果有一行不是数值，则标记该列不是数值列
                                        isNumericColumn[c] = false;
                                    }
                                }

                                cell.CellStyle = dataStyle;
                                c++;
                            }
                            j++;
                        }
                    });

                    // 创建汇总行样式
                    ICellStyle summaryStyle = wb.CreateCellStyle();
                    IFont summaryFont = wb.CreateFont();
                    summaryFont.FontHeightInPoints = 10; // 设置汇总行字体大小为10号
                    summaryFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold; // 设置为粗体
                    summaryStyle.SetFont(summaryFont);
                    summaryStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                    summaryStyle.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;

                    // 为汇总行添加边框
                    summaryStyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                    summaryStyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                    summaryStyle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                    summaryStyle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                    summaryStyle.TopBorderColor = IndexedColors.Black.Index;
                    summaryStyle.BottomBorderColor = IndexedColors.Black.Index;
                    summaryStyle.LeftBorderColor = IndexedColors.Black.Index;
                    summaryStyle.RightBorderColor = IndexedColors.Black.Index;

                    // 设置背景色为浅灰色
                    summaryStyle.FillForegroundColor = IndexedColors.Grey25Percent.Index;
                    summaryStyle.FillPattern = FillPattern.SolidForeground;

                    // 创建汇总行
                    rowTemp = sheet.CreateRow(j);
                    rowTemp.Height = (short)(defaultHeight * 3); // 汇总行高为默认3倍

                    // 添加汇总行数据
                    for (int i = 0; i < columnCount; i++)
                    {
                        var cell = rowTemp.CreateCell(i);

                        if (i == 0)
                        {
                            cell.SetCellValue("合计");
                        }
                        else if (columnSums.ContainsKey(i) && isNumericColumn[i])
                        {
                            // 如果是数值列，显示汇总值
                            cell.SetCellValue(columnSums[i].ToString());
                        }

                        cell.CellStyle = summaryStyle;
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
                        // 导出完成后自动打开Excel文件并进入打印菜单
                        try
                        {
                            // 启动Excel应用程序
                            Process excelProcess = new Process();
                            excelProcess.StartInfo.FileName = tfilepath;
                            excelProcess.StartInfo.UseShellExecute = true;
                            excelProcess.Start();

                            // 等待Excel启动
                            System.Threading.Thread.Sleep(3000);

                            // 发送Ctrl+P快捷键打开打印菜单
                            SendKeys.SendWait("^p");
                        }
                        catch (Exception ex)
                        {
                            UIMessageBox.Show("文件已导出，但无法自动打开打印菜单: " + ex.Message);
                        }
                    }));
                }
            }
            catch (Exception er)
            {
                UIMessageBox.Show("出现错误:" + er.Message);
            }
        }

        private void saveFileDialog8_FileOk(object sender, CancelEventArgs e)
        {
            try
            {
                var datas = JArray.FromObject(dataGridView12.DataSource);

                if (saveFileDialog8.FileName != "")
                {
                    string tfilepath = saveFileDialog8.FileName;

                    HSSFWorkbook wb;
                    FileStream file;

                    MemoryStream ms = new MemoryStream();
                    wb = new HSSFWorkbook();

                    ISheet sheet;
                    sheet = wb.CreateSheet("sheet1");
                    // 设置打印格式
                    sheet.PrintSetup.PaperSize = (short)PaperSize.A4_Small;           // A4纸张
                    sheet.PrintSetup.Landscape = true;                   // 横向打印
                    sheet.PrintSetup.Scale = 100;                        // 设置缩放比例为100%

                    sheet.PrintSetup.UsePage = false;                   // 禁用自动页面调整

                    sheet.PrintSetup.FitWidth = 1;                              // 适应1页宽度
                    sheet.PrintSetup.FitHeight = 0;                             // 高度不限制，允许多页

                    // 设置页边距（单位：英寸，1cm = 0.393701英寸）
                    sheet.SetMargin(MarginType.TopMargin, 0.393701);     // 上边距1cm
                    sheet.SetMargin(MarginType.BottomMargin, 0.393701);  // 下边距1cm
                    sheet.SetMargin(MarginType.LeftMargin, 0.393701);    // 左边距1cm
                    sheet.SetMargin(MarginType.RightMargin, 0.393701);   // 右边距1cm

                    // 设置页眉页脚为0
                    sheet.SetMargin(MarginType.HeaderMargin, 0);
                    sheet.SetMargin(MarginType.FooterMargin, 0);

                    // 设置水平居中打印
                    sheet.HorizontallyCenter = true;

                    sheet.RepeatingRows = CellRangeAddress.ValueOf("1:1"); //设置第1行（表头行）在每页都重复显示

                    ICellStyle cellStyle = wb.CreateCellStyle();
                    NPOI.SS.UserModel.IRow rowTemp;
                    rowTemp = sheet.CreateRow(0);//创建第一行
                    short defaultHeight = rowTemp.Height; // 获取默认行高

                    JObject header = (JObject)datas.First();
                    rowTemp = sheet.CreateRow(0);
                    rowTemp.Height = (short)(defaultHeight * 7); // 表头行高为默认7倍

                    // 动态设置列宽
                    int columnCount = header.Properties().Count();
                    for (int i = 0; i < columnCount; i++)
                    {
                        sheet.SetColumnWidth(i, i == 0 ? 4 * 256 : 8 * 256);
                    }
                    sheet.SetColumnWidth(2, 8 * 256);
                    sheet.SetColumnWidth(1, 9 * 256);
                    // 表头样式 - 添加边框
                    ICellStyle headerStyle = wb.CreateCellStyle();
                    IFont headerFont = wb.CreateFont();
                    headerFont.FontHeightInPoints = 10; // 设置表头字体大小为10号
                    headerStyle.SetFont(headerFont);
                    headerStyle.WrapText = true;
                    headerStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                    headerStyle.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;

                    // 为表头添加边框
                    headerStyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                    headerStyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                    headerStyle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                    headerStyle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                    headerStyle.TopBorderColor = IndexedColors.Black.Index;
                    headerStyle.BottomBorderColor = IndexedColors.Black.Index;
                    headerStyle.LeftBorderColor = IndexedColors.Black.Index;
                    headerStyle.RightBorderColor = IndexedColors.Black.Index;

                    // 数据样式 - 添加边框
                    ICellStyle dataStyle = wb.CreateCellStyle();
                    IFont dataFont = wb.CreateFont();
                    dataFont.FontHeightInPoints = 10; // 设置数据字体大小为10号
                    dataStyle.SetFont(dataFont);
                    dataStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                    dataStyle.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;

                    // 为数据单元格添加边框
                    dataStyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                    dataStyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                    dataStyle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                    dataStyle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                    dataStyle.TopBorderColor = IndexedColors.Black.Index;
                    dataStyle.BottomBorderColor = IndexedColors.Black.Index;
                    dataStyle.LeftBorderColor = IndexedColors.Black.Index;
                    dataStyle.RightBorderColor = IndexedColors.Black.Index;

                    int c = 0;
                    foreach (JProperty prop in header.Properties())
                    {
                        var cell = rowTemp.CreateCell(c);
                        cell.SetCellValue(prop.Name.ToString());
                        cell.CellStyle = headerStyle;
                        c++;
                    }

                    int j = 1;
                    // 创建一个字典用于存储每列的数值总和
                    Dictionary<int, decimal> columnSums = new Dictionary<int, decimal>();
                    // 创建一个字典用于标记哪些列是数值列
                    Dictionary<int, bool> isNumericColumn = new Dictionary<int, bool>();

                    datas.ForEach(r =>
                    {
                        JObject dataRow = (JObject)r;
                        // 检查支数是否为0
                        if (dataRow["支数"]?.ToString() != "0")
                        {
                            rowTemp = sheet.CreateRow(j);
                            rowTemp.Height = (short)(defaultHeight * 3); // 数据行高为默认3倍

                            c = 0;
                            foreach (JProperty prop in dataRow.Properties())
                            {
                                var cell = rowTemp.CreateCell(c);
                                // 如果是外部订单号列，生成二维码
                                if (prop.Name == "原始单号")
                                {
                                    // 创建二维码图片
                                    using (var originalQrCodeImage = GenerateQRCode(prop.Value.ToString()))
                                    {
                                        // 调整二维码图片大小
                                        using (var resizedQrCodeImage = new Bitmap(originalQrCodeImage, new Size(50, 50)))
                                        {
                                            // 将图片添加到工作簿
                                            var pictureIdx = wb.AddPicture(ImageToByteArray(resizedQrCodeImage), NPOI.SS.UserModel.PictureType.PNG);
                                            var drawing = sheet.CreateDrawingPatriarch();
                                            var anchor = wb.GetCreationHelper().CreateClientAnchor();

                                            // 设置图片位置和大小
                                            anchor.Col1 = c;           // 起始列
                                            anchor.Row1 = j;           // 起始行
                                            anchor.Col2 = c;       // 结束列
                                            anchor.Row2 = j;       // 结束行

                                            // 设置图片在单元格内的位置（使用EMU单位）
                                            anchor.Dx1 = 75;    // 左边距
                                            anchor.Dy1 = 4;    // 上边距
                                            anchor.Dx2 = 1;    // 右边距  
                                            anchor.Dy2 = 1;    // 下边距

                                            // 创建图片对象
                                            var picture = drawing.CreatePicture(anchor, pictureIdx);
                                            picture.Resize(); // 自动调整图片大小以适应单元格
                                        }
                                    }
                                }
                                else
                                {
                                    string valueStr = prop.Value.ToString();
                                    cell.SetCellValue(valueStr);

                                    // 检查是否为数值列并累加
                                    decimal numValue;
                                    bool isNumeric = decimal.TryParse(valueStr, out numValue);

                                    // 如果是第一行数据，初始化该列是否为数值列
                                    if (j == 1)
                                    {
                                        isNumericColumn[c] = isNumeric;
                                    }

                                    // 如果是数值列，累加总和
                                    if (isNumeric && (isNumericColumn.ContainsKey(c) && isNumericColumn[c]))
                                    {
                                        if (!columnSums.ContainsKey(c))
                                        {
                                            columnSums[c] = 0;
                                        }
                                        columnSums[c] += numValue;
                                    }
                                    else
                                    {
                                        // 如果有一行不是数值，则标记该列不是数值列
                                        isNumericColumn[c] = false;
                                    }
                                }

                                cell.CellStyle = dataStyle;
                                c++;
                            }
                            j++;
                        }
                    });

                    // 创建汇总行样式
                    ICellStyle summaryStyle = wb.CreateCellStyle();
                    IFont summaryFont = wb.CreateFont();
                    summaryFont.FontHeightInPoints = 10; // 设置汇总行字体大小为10号
                    summaryFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold; // 设置为粗体
                    summaryStyle.SetFont(summaryFont);
                    summaryStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                    summaryStyle.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;

                    // 为汇总行添加边框
                    summaryStyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                    summaryStyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                    summaryStyle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                    summaryStyle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                    summaryStyle.TopBorderColor = IndexedColors.Black.Index;
                    summaryStyle.BottomBorderColor = IndexedColors.Black.Index;
                    summaryStyle.LeftBorderColor = IndexedColors.Black.Index;
                    summaryStyle.RightBorderColor = IndexedColors.Black.Index;

                    // 设置背景色为浅灰色
                    summaryStyle.FillForegroundColor = IndexedColors.Grey25Percent.Index;
                    summaryStyle.FillPattern = FillPattern.SolidForeground;

                    // 创建汇总行
                    rowTemp = sheet.CreateRow(j);
                    rowTemp.Height = (short)(defaultHeight * 3); // 汇总行高为默认3倍

                    // 添加汇总行数据
                    for (int i = 0; i < columnCount; i++)
                    {
                        var cell = rowTemp.CreateCell(i);

                        if (i == 0)
                        {
                            cell.SetCellValue("合计");
                        }
                        else if (columnSums.ContainsKey(i) && isNumericColumn[i])
                        {
                            // 如果是数值列，显示汇总值
                            cell.SetCellValue(columnSums[i].ToString());
                        }

                        cell.CellStyle = summaryStyle;
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
                        // 导出完成后自动打开Excel文件并进入打印菜单
                        try
                        {
                            // 启动Excel应用程序
                            Process excelProcess = new Process();
                            excelProcess.StartInfo.FileName = tfilepath;
                            excelProcess.StartInfo.UseShellExecute = true;
                            excelProcess.Start();

                            // 等待Excel启动
                            System.Threading.Thread.Sleep(3000);

                            // 发送Ctrl+P快捷键打开打印菜单
                            SendKeys.SendWait("^p");
                        }
                        catch (Exception ex)
                        {
                            UIMessageBox.Show("文件已导出，但无法自动打开打印菜单: " + ex.Message);
                        }
                    }));
                }
            }
            catch (Exception er)
            {
                UIMessageBox.Show("出现错误:" + er.Message);
            }
        }

        private void saveFileDialog9_FileOk(object sender, CancelEventArgs e)
        {
            try
            {
                var datas = JArray.FromObject(dataGridView11.DataSource);

                if (saveFileDialog9.FileName != "")
                {
                    string tfilepath = saveFileDialog9.FileName;

                    HSSFWorkbook wb;
                    FileStream file;

                    MemoryStream ms = new MemoryStream();
                    wb = new HSSFWorkbook();

                    ISheet sheet;
                    sheet = wb.CreateSheet("sheet1");

                    ICellStyle headerStyle = wb.CreateCellStyle();
                    IFont headerFont = wb.CreateFont();
                    headerFont.FontHeightInPoints = 12; // 设置表头字体大小为10号
                    headerStyle.SetFont(headerFont);
                    headerStyle.WrapText = true;

                    headerStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                    headerStyle.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;

                    // 为表头添加边框
                    headerStyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                    headerStyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                    headerStyle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                    headerStyle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                    headerStyle.TopBorderColor = IndexedColors.Black.Index;
                    headerStyle.BottomBorderColor = IndexedColors.Black.Index;
                    headerStyle.LeftBorderColor = IndexedColors.Black.Index;
                    headerStyle.RightBorderColor = IndexedColors.Black.Index;

                    // 数据样式 - 添加边框
                    ICellStyle dataStyle = wb.CreateCellStyle();
                    IFont dataFont = wb.CreateFont();
                    dataFont.FontHeightInPoints = 10; // 设置数据字体大小为10号
                    dataStyle.SetFont(dataFont);
                    dataStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                    dataStyle.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;

                    // 为数据单元格添加边框
                    dataStyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                    dataStyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                    dataStyle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                    dataStyle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                    dataStyle.TopBorderColor = IndexedColors.Black.Index;
                    dataStyle.BottomBorderColor = IndexedColors.Black.Index;
                    dataStyle.LeftBorderColor = IndexedColors.Black.Index;
                    dataStyle.RightBorderColor = IndexedColors.Black.Index;

                    NPOI.SS.UserModel.IRow rowTemp;
                    rowTemp = sheet.CreateRow(0);//创建第一行
                    int c = 0;
                    JObject header = (JObject)datas.First();

                    sheet.SetColumnWidth(0, 10 * 256);
                    sheet.SetColumnWidth(1, 50 * 256);
                    sheet.SetColumnWidth(2, 12 * 256);
                    sheet.SetColumnWidth(3, 12 * 256);
                    sheet.SetColumnWidth(4, 12 * 256);
                    sheet.SetColumnWidth(5, 12 * 256);

                    rowTemp = sheet.CreateRow(0);
                    short defaultHeight = rowTemp.Height;
                    rowTemp.Height = (short)(defaultHeight * 2);
                    c = 0;
                    foreach (JProperty prop in header.Properties())
                    {

                        if (prop.Name != "库存量" && prop.Name != "净需求量" && prop.Name != "箱规" && prop.Name != "调拨箱数" && prop.Name != "调拨数量")
                        {
                            var cellh = rowTemp.CreateCell(c);
                            cellh.SetCellValue(prop.Name.ToString());
                            cellh.CellStyle = headerStyle;
                            c++;
                        }

                    }
                    int j = 1;
                    datas.ForEach(r =>
                    {
                        rowTemp = sheet.CreateRow(j);
                        rowTemp.Height = (short)(defaultHeight * 1.5);
                        JObject dataRow = (JObject)r;
                        c = 0;
                        foreach (JProperty prop in dataRow.Properties())
                        {
                            if (prop.Name != "库存量" && prop.Name != "净需求量" && prop.Name != "箱规" && prop.Name != "调拨箱数" && prop.Name != "调拨数量")
                            {
                                var celld = rowTemp.CreateCell(c);
                                celld.SetCellValue(prop.Value.ToString());
                                celld.CellStyle = dataStyle;
                                c++;
                            }

                        }
                        j++;
                    });
                    rowTemp = sheet.CreateRow(j);
                    var cell = rowTemp.CreateCell(1);
                    cell.SetCellValue("签收数量：");
                    cell = rowTemp.CreateCell(4);
                    cell.SetCellValue("签收数量：");
                    rowTemp = sheet.CreateRow(j + 2);
                    cell = rowTemp.CreateCell(1);
                    cell.SetCellValue("发货员签字：");
                    cell = rowTemp.CreateCell(4);
                    cell.SetCellValue("收货员签字：");
                    rowTemp = sheet.CreateRow(j + 4);
                    cell = rowTemp.CreateCell(1);
                    cell.SetCellValue("日期：");
                    cell = rowTemp.CreateCell(4);
                    cell.SetCellValue("日期：");
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
        private System.Drawing.Image GenerateQRCode(string text)
        {
            try
            {
                var qrGenerator = new QRCodeGenerator();
                var qrCodeData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
                var qrCode = new QRCode(qrCodeData);

                // 生成更高质量的二维码图片
                return qrCode.GetGraphic(5, Color.Black, Color.White, true);
            }
            catch (Exception ex)
            {
                // 如果二维码生成失败，返回一个空白图片
                var errorBitmap = new Bitmap(80, 80);
                using (var g = Graphics.FromImage(errorBitmap))
                {
                    g.Clear(Color.White);
                    g.DrawString("QR Error", new Font("Arial", 8), Brushes.Red, 5, 35);
                }
                return errorBitmap;
            }
        }

        // 改进的图片转字节数组方法
        private byte[] ImageToByteArray(System.Drawing.Image image)
        {
            if (image == null) return new byte[0];

            using (var ms = new MemoryStream())
            {
                // 使用PNG格式保存，质量更好
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                return ms.ToArray();
            }
        }

        private void uiButton36_Click(object sender, EventArgs e)
        {
            try
            {
                // 验证数据源
                if (dataGridView2.DataSource == null)
                {
                    MessageBox.Show("请先加载订单数据到主表格");
                    return;
                }

                // 清空结果表格并显示查询状态
                this.Invoke(new Action(() =>
                {
                    dataGridView11.DataSource = null;
                    dataGridView11.Rows.Clear();
                    dataGridView14.DataSource = null;
                    dataGridView14.Rows.Clear();
                    richTextBox2.AppendText("=== 开始批量查询旺店通订单商品信息 ===\r\n");
                }));

                // 禁用查询按钮防止重复点击
                uiButton36.Enabled = false;
                var finalResults = BatchQueryWithProgressForTran(progressBar2);
                // var finalResults = ProcessOrdersWithSuiteInfo(progressBar2);

                // 绑定结果并格式化表格
                this.Invoke(new Action(() =>
                {
                    if (finalResults != null && finalResults.Count > 0)
                    {

                        dataGridView11.DataSource = finalResults;

                    }
                    else
                    {
                        richTextBox2.AppendText("未查询到任何商品数据\r\n");
                        richTextBox2.AppendText("=== 查询完成 ===\r\n\r\n");

                    }
                }));
            }
            catch (Exception ex)
            {
                this.Invoke(new Action(() =>
                {
                    richTextBox2.AppendText($"批量查询出现异常: {ex.Message}\r\n");
                    richTextBox2.AppendText("=== 查询异常结束 ===\r\n\r\n");
                }));
                MessageBox.Show($"批量查询失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // 重新启用查询按钮
                this.Invoke(new Action(() =>
                {
                    uiButton36.Enabled = true;
                }));
            }
        }

        private void uiButton37_Click(object sender, EventArgs e)
        {
            dataGridView13.DataSource = null; // 先清空
            dataGridView11.DataSource = null; // 先清空
            uiTabControl1.SelectedIndex = 7;
            DeduplicateAndRebindDataSource();
        }

        private void uiButton38_Click(object sender, EventArgs e)
        {
            GetWdtSuitesInfo(textBox1.Text);
            //  var sl= getMatStockNum(textBox1.Text);

        }

        private void tabPage6_Click(object sender, EventArgs e)
        {

        }

        private void uiButton39_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView11.DataSource == null)
                {
                    MessageBox.Show("请先完成数据查询");
                    return;
                }

                Task.Run(ProcessResultsWithBoxInfoForTran);

            }
            catch (Exception er)
            {
                MessageBox.Show($"出现错误{er.Message}");
            }

        }
        public void PushTransferApplyToKCloud(List<TransferApplyRow> transferRows, string FORGNUMBER, string fromWarehouseCode, string toWarehouseCode)
        {
            try
            {
                if (transferRows == null || !transferRows.Any())
                {
                    MessageBox.Show("提交失败: 调拨申请单明细为空");
                }

                // Initialize JSON root
                JObject jsonRoot = JsonHeader(true);
                JObject model = new JObject();
                jsonRoot.Add("Model", model);

                #region Header Information (FBillHead)
                // Mandatory fields
                model.Add("FDate", DateTime.Now.ToString("yyyy-MM-dd"));
                model.Add("FTRANSTYPE", " InnerOrgTransfer"); // Default transfer type, adjust as needed
                JObject basedata = new JObject();
                basedata.Add("FNUMBER", "DBSQD01_SYS");
                model.Add("FBillTypeID", basedata);
                // 调出货主类型 - 只添加一次
                model.Add("FOwnerTypeIdHead", "BD_OwnerOrg");
                // 调入货主类型 - 只添加一次
                model.Add("FOwnerTypeInIdHead", "BD_OwnerOrg");

                basedata = new JObject();
                basedata.Add("FNumber", FORGNUMBER);
                model.Add("FAPPORGID", basedata); // Mandatory: Applying organization

                // Optional fields with defaults
                model.Add("FRemarks", "调往唯品会电商仓");
                model.Add("FIsReturnWare", "false");
                model.Add("FZJNote", "");
                model.Add("FIsWDT", true);

                JArray entryRows = new JArray();
                model.Add("FEntity", entryRows);

                foreach (var item in transferRows)
                {


                    JObject entryRow = new JObject();
                    entryRow.Add("FEntryID", 0); // New entry

                    basedata = new JObject();
                    basedata.Add("FNumber", item.MatNumber);
                    entryRow.Add("FMATERIALID", basedata); // Material code
                    entryRow.Add("FQty", item.ApplyQty); // Apply quantity
                    entryRow.Add("FUNITID", new JObject { { "FNumber", item?.FSaleBaseUnitNo ?? "支" } });
                    entryRow.Add("FStockOrgId", new JObject { { "FNumber", FORGNUMBER } }); // Transfer out organization
                    entryRow.Add("FStockOrgInId", new JObject { { "FNumber", FORGNUMBER } }); // Transfer in organization
                    entryRow.Add("FStockId", new JObject { { "FNUMBER", fromWarehouseCode } });
                    entryRow.Add("FStockInId", new JObject { { "FNUMBER", toWarehouseCode } });
                    entryRow.Add("FOwnerTypeId", "BD_OwnerOrg"); // Owner type for transfer out
                    entryRow.Add("FOwnerTypeInId", "BD_OwnerOrg"); // Owner type for transfer in
                    entryRow.Add("FBaseUnitID", new JObject { { "FNumber", item?.FSaleBaseUnitNo ?? "支" } });

                    // Optional fields
                    entryRow.Add("FOwnerId", new JObject { { "FNumber", FORGNUMBER } });
                    entryRow.Add("FOwnerInId", new JObject { { "FNumber", FORGNUMBER } });
                    // entryRow.Add("FNote", $"Qps: {item.Qps}");

                    entryRows.Add(entryRow);
                }
                #endregion

                RestClient client = new RestClient("http://120.78.210.159:3304/api/Wdt/ExecuteBillSaveRequest");
                RestRequest request = new RestRequest();
                request.AddHeader("Content-Type", "application/json");
                request.Method = Method.Post;
                RestResponse response;
                var obj = new
                {
                    formid = "STK_TransferApply",
                    data = jsonRoot
                };
                var str = JsonConvert.SerializeObject(obj);
                //this.Invoke(new Action(() =>
                //{
                //    richTextBox2.AppendText($"调拨申请单传送Json:{str}\r\n");
                //}));

                request.AddBody(str);
                response = client.Execute(request);
                //  var result = JArray.Parse(response.Content);
                this.Invoke(new Action(() =>
                {

                    richTextBox2.AppendText($"调拨申请单执行结果:{response.Content}\r\n");
                }));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"提交失败: {ex.Message}");
            }
        }
        public JObject JsonHeader(bool IsAuto)
        {
            JObject jsonRoot = new JObject();
            // Creator: 创建用户
            jsonRoot.Add("Creator", "");
            // NeedUpDateFields: 哪些字段需要更新？为空则表示参数中全部字段，均需要更新
            jsonRoot.Add("NeedUpDateFields", new JArray(""));
            jsonRoot.Add("NeedReturnFields", new JArray("FNumber"));
            jsonRoot.Add("IsDeleteEntry", "true");
            if (IsAuto)
            {
                jsonRoot.Add("IsAutoSubmitAndAudit", "true"); //是否提交审核
                //jsonRoot.Add("IsSubmit", "true"); //是否提交审核
            }
            jsonRoot.Add("SubSystemId", "");
            jsonRoot.Add("IsVerifyBaseDataField", "false");
            jsonRoot.Add("IsEntryBatchFill", "true");
            jsonRoot.Add("ValidateFlag", "true");
            jsonRoot.Add("NumberSearch", "true");
            jsonRoot.Add("InterationFlags", "");

            return jsonRoot;


        }

        private void uiButton40_Click(object sender, EventArgs e)
        {
            if (dataGridView11.DataSource == null)
            {
                MessageBox.Show("提交失败: 调拨申请单明细为空");
                return;
            }
            var results = (IEnumerable<object>)dataGridView11.DataSource;
            List<TransferApplyRow> lstTr = new List<TransferApplyRow>();
            int skippedCount = 0; // 记录跳过的数量

            foreach (var item in results)
            {
                var itemType = item.GetType();

                // 获取净需求量并检查是否为0
                string netDemandStr = itemType.GetProperty("净需求量")?.GetValue(item)?.ToString() ?? "0";
                if (int.TryParse(netDemandStr, out int netDemand) && netDemand <= 0)
                {
                    skippedCount++;
                    continue; // 跳过净需求量为0或负数的记录
                }

                TransferApplyRow tr = new TransferApplyRow();
                tr.MatNumber = itemType.GetProperty("货号")?.GetValue(item)?.ToString() ?? "";
                tr.ApplyQty = Convert.ToInt16(itemType.GetProperty("调拨数量")?.GetValue(item)?.ToString());
                tr.FSaleBaseUnitNo = itemType.GetProperty("单位")?.GetValue(item)?.ToString() ?? "";

                lstTr.Add(tr);
            }

            // 检查是否有有效记录
            if (lstTr.Count == 0)
            {
                MessageBox.Show($"提交失败: 所有记录的净需求量均为0，没有需要调拨的商品\n(已跳过 {skippedCount} 条记录)",
                    "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 显示跳过的记录数（如果有）
            if (skippedCount > 0)
            {
                richTextBox2.AppendText($"提示: 已自动排除 {skippedCount} 条净需求量为0的记录，实际提交 {lstTr.Count} 条记录\r\n");
            }

            PushTransferApplyToKCloud(lstTr, "102", "SCK012", "SCK023");
        }

        private void uiButton41_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView10.DataSource == null)
                {
                    MessageBox.Show("请先完成数据查询");
                    return;
                }
                if (dataGridView9.DataSource == null)
                {
                    MessageBox.Show("请先完成数据查询和计算箱数");
                    return;
                }
                // 获取数据源并转换为列表
                var grid9Data = ((IEnumerable<object>)dataGridView9.DataSource).ToList();
                var grid10Data = ((IEnumerable<object>)dataGridView10.DataSource).ToList();
                DataTable dt = new DataTable();
                dt.Columns.Add("序号", typeof(int));
                dt.Columns.Add("收货人", typeof(string));
                dt.Columns.Add("原始单号", typeof(string)); // 新增外部订单号列
                int rowindex = 1;
                foreach (var order in grid9Data)
                {
                    var orderType = order.GetType();
                    string tid = orderType.GetProperty("原始单号")?.GetValue(order)?.ToString() ?? "";
                    string contact = orderType.GetProperty("收件人")?.GetValue(order)?.ToString() ?? "";

                    // 从grid10Data中获取外部订单号
                    var firstItem = grid10Data.FirstOrDefault(x =>
                        x.GetType().GetProperty("原始单号")?.GetValue(x)?.ToString() == tid);
                    string outerNo = firstItem?.GetType().GetProperty("原始单号")?.GetValue(firstItem)?.ToString() ?? "";

                    DataRow row = dt.NewRow();
                    row["序号"] = rowindex++;
                    row["收货人"] = contact;
                    row["原始单号"] = outerNo;
                    dt.Rows.Add(row);
                }

                var viewModelList = new List<U_Plans_WdtSOderDetailViewModel>();

                // 如果DataSource是IEnumerable类型
                if (dataGridView10.DataSource is IEnumerable dataSource)
                {
                   
                    foreach (var item in dataSource)
                    {
                        if (item == null) continue;

                        var itemType = item.GetType();
                        string srcTid = itemType.GetProperty("原始单号")?.GetValue(item)?.ToString() ?? "";

                        // 通过原始单号从dt中查询对应的序号
                        int matchedRowIndex = 0;
                        DataRow[] foundRows = dt.Select($"原始单号 = '{srcTid.Replace("'", "''")}'");
                        if (foundRows.Length > 0)
                        {
                            matchedRowIndex = Convert.ToInt32(foundRows[0]["序号"]);
                        }


                        var viewModel = new U_Plans_WdtSOderDetailViewModel
                        {
                            rowindex = matchedRowIndex,
                            src_tid = itemType.GetProperty("原始单号")?.GetValue(item)?.ToString() ?? "",
                            goods_no = itemType.GetProperty("货号")?.GetValue(item)?.ToString() ?? "",
                            num = int.Parse(itemType.GetProperty("数量")?.GetValue(item)?.ToString()),
                            wbnum = int.Parse(itemType.GetProperty("整箱数")?.GetValue(item)?.ToString()),
                            snum = int.Parse(itemType.GetProperty("散数")?.GetValue(item)?.ToString()),
                            specnum = int.Parse(itemType.GetProperty("箱规")?.GetValue(item)?.ToString()),
                            calspecnum = int.Parse(uiTextBox12.Text), // 固定值
                            fweight = Decimal.Parse(itemType.GetProperty("毛重")?.GetValue(item)?.ToString()),
                            totalweight = Decimal.Parse(itemType.GetProperty("重量合计")?.GetValue(item)?.ToString())
                        };

                        viewModelList.Add(viewModel);
                        rowindex++;
                    }
                    // var obj = new { lst = viewModelList };
                    richTextBox1.Clear();
                    richTextBox1.AppendText(JsonConvert.SerializeObject(new { lst = viewModelList }));

                    RestClient client = new RestClient("http://120.78.210.159:3304/api/Wdt/SaveCalOrderResult");
                    RestRequest request = new RestRequest();
                    request.AddHeader("Content-Type", "application/json");
                    request.Method = Method.Post;
                    RestResponse response;

                    var obj = new { lst = viewModelList };
                    var str = JsonConvert.SerializeObject(obj);

                    request.AddBody(str);
                    response = client.Execute(request);
                    var result = JObject.Parse(response.Content);
                    if (result.ContainsKey("code"))
                    {
                        if (Convert.ToInt32(result["code"]) == 1)
                        {
                            MessageBox.Show(result["msg"].ToJsonString());
                            richTextBox1.AppendText(response.Content);

                        }
                        else
                        {
                            MessageBox.Show($"保存失败:{result["msg"].ToJsonString()}");
                            richTextBox1.AppendText(response.Content + "\r\n");
                        }
                    }
                }
            }
            catch (Exception er)
            {
                MessageBox.Show($"保存失败:{er.Message}");
            }
        }

        private void uiButton42_Click(object sender, EventArgs e)
        {
            saveFileDialog9.Filter = "Excel 文件(*.xls)|*.xls";
            saveFileDialog9.ShowDialog();
        }

        // 1. 按钮点击事件（文件选择）
        private void uiButton43_Click(object sender, EventArgs e)
        {
            try
            {
                // 支持多种Excel格式
                openFileDialog2.Filter = "EXCEL文件|*.xls;*.xlsx|所有文件|*.*";
                openFileDialog2.Title = "请选择要导入的Excel文件";
                openFileDialog2.Multiselect = false; // 只允许选择一个文件

                DialogResult result = openFileDialog2.ShowDialog();

                if (result == DialogResult.OK)
                {
                    this.Invoke(new Action(() =>
                    {
                        richTextBox1.AppendText($"已选择文件：{Path.GetFileName(openFileDialog2.FileName)}\r\n");
                    }));
                }
                else
                {
                    this.Invoke(new Action(() =>
                    {
                        richTextBox1.AppendText("用户取消了文件选择\r\n");
                    }));
                }
            }
            catch (Exception er)
            {
                UIMessageBox.ShowError($"选择文件时发生错误：{er.Message}");
                this.Invoke(new Action(() =>
                {
                    richTextBox1.AppendText($"选择文件时发生错误：{er.Message}\r\n");
                }));
            }
        }

        // 2. 文件确认事件
        private void openFileDialog2_FileOk(object sender, CancelEventArgs e)
        {
            try
            {
                string fileName = openFileDialog2.FileName;

                // 验证文件路径
                if (string.IsNullOrEmpty(fileName) || !File.Exists(fileName))
                {
                    UIMessageBox.ShowError("选择的文件不存在或路径无效");
                    e.Cancel = true; // 取消关闭对话框
                    return;
                }

                // 验证文件扩展名
                string extension = Path.GetExtension(fileName).ToLower();
                if (extension != ".xls" && extension != ".xlsx")
                {
                    UIMessageBox.ShowError("请选择有效的Excel文件（.xls或.xlsx格式）");
                    e.Cancel = true;
                    return;
                }

                // 验证文件是否被占用
                try
                {
                    using (FileStream fs = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        // 文件可以正常打开
                    }
                }
                catch (IOException)
                {
                    UIMessageBox.ShowError("文件正在被其他程序使用，请关闭Excel后重试");
                    e.Cancel = true;
                    return;
                }

                this.Invoke(new Action(() =>
                {
                    richTextBox1.AppendText($"正在读取Excel文件：{Path.GetFileName(fileName)}\r\n");
                    richTextBox1.AppendText("请稍候...\r\n");
                }));

                // 使用异步方式处理，避免阻塞UI
                Task.Run(() =>
                {
                    try
                    {
                        ProcessExcelFile(fileName);
                    }
                    catch (Exception ex)
                    {
                        this.Invoke(new Action(() =>
                        {
                            richTextBox1.AppendText($"处理文件时发生异常：{ex.Message}\r\n");
                        }));
                        UIMessageBox.ShowError($"处理Excel文件时发生错误：{ex.Message}");
                    }
                });
            }
            catch (Exception er)
            {
                UIMessageBox.ShowError($"验证文件时发生错误：{er.Message}");
                e.Cancel = true; // 发生错误时不关闭对话框
            }
        }

        // 3. 处理Excel文件的主方法
        private void ProcessExcelFile(string fileName)
        {
            try
            {
                // 读取Excel文件数据
                DataTable dt = ExcelToDataTableForlog(fileName, true);

                this.Invoke(new Action(() =>
                {
                    richTextBox1.AppendText($"文件读取完成 - 列数:{dt?.Columns.Count}, 行数:{dt?.Rows.Count}\r\n");
                }));

                // 验证数据
                if (dt == null)
                {
                    this.Invoke(new Action(() =>
                    {
                        richTextBox1.AppendText("读取Excel文件失败，请检查文件格式\r\n");
                    }));
                    UIMessageBox.ShowError("读取Excel文件失败，请检查文件格式是否正确");
                    return;
                }

                if (dt.Rows.Count == 0)
                {
                    this.Invoke(new Action(() =>
                    {
                        richTextBox1.AppendText("Excel文件中没有数据行\r\n");
                    }));
                    UIMessageBox.ShowError("Excel文件中没有数据");
                    return;
                }

                // 处理数据
                ProcessExcelData(dt, fileName);
            }
            catch (Exception ex)
            {
                this.Invoke(new Action(() =>
                {
                    richTextBox1.AppendText($"处理Excel文件时发生异常：{ex.Message}\r\n");
                }));
                UIMessageBox.ShowError($"处理Excel文件时发生错误：{ex.Message}");
            }
        }

        // 数据行处理逻辑
        private void ProcessExcelData(DataTable dt, string fileName)
        {
            try
            {
                this.Invoke(new Action(() =>
                {
                    richTextBox1.AppendText("=".PadRight(50, '=') + "\r\n");
                    richTextBox1.AppendText($"开始分析Excel数据结构...\r\n");
                }));

                // 显示所有列名，便于调试
                this.Invoke(new Action(() =>
                {
                    richTextBox1.AppendText("发现的列：");
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        richTextBox1.AppendText($"[{i}]{dt.Columns[i].ColumnName}");
                        if (i < dt.Columns.Count - 1) richTextBox1.AppendText(", ");
                    }
                    richTextBox1.AppendText("\r\n");
                }));

                // 检查是否包含必要的列
                bool hasSrcTid = false;
                bool hasLogisticsNo = false;
                bool hasLogisticsCode = false;
                string srcTidColumn = "";
                string logisticsNoColumn = "";
                string logisticsCodeColumn = "";

                foreach (DataColumn col in dt.Columns)
                {
                    string colName = col.ColumnName.ToLower();

                    // 检查原始单号列
                    if (colName.Contains("原始单号") || colName.Contains("src_tid") ||
                        colName.Contains("订单号") || colName.Contains("order") ||
                        colName.Contains("单号"))
                    {
                        if (!hasSrcTid) // 取第一个匹配的列
                        {
                            hasSrcTid = true;
                            srcTidColumn = col.ColumnName;
                        }
                    }

                    // 检查物流单号列
                    if (colName.Contains("物流单号") || colName.Contains("logistics_no") ||
                        colName.Contains("express") || colName.Contains("快递单号") ||
                        colName.Contains("运单号") || colName.Contains("tracking"))
                    {
                        if (!hasLogisticsNo)
                        {
                            hasLogisticsNo = true;
                            logisticsNoColumn = col.ColumnName;
                        }
                    }

                    // 检查物流公司列
                    if (colName.Contains("物流公司") || colName.Contains("logistics_code") ||
                        colName.Contains("快递公司") || colName.Contains("承运商") ||
                        colName.Contains("courier"))
                    {
                        if (!hasLogisticsCode)
                        {
                            hasLogisticsCode = true;
                            logisticsCodeColumn = col.ColumnName;
                        }
                    }
                }

                // 验证必要的列
                if (!hasSrcTid || !hasLogisticsNo)
                {
                    string missingColumns = "";
                    if (!hasSrcTid) missingColumns += "'原始单号' ";
                    if (!hasLogisticsNo) missingColumns += "'物流单号' ";

                    this.Invoke(new Action(() =>
                    {
                        richTextBox1.AppendText($"错误：Excel文件中缺少必要的列：{missingColumns}\r\n");
                        richTextBox1.AppendText("请确保Excel文件包含以下列名之一：\r\n");
                        richTextBox1.AppendText("- 原始单号相关：原始单号、src_tid、订单号、order、单号\r\n");
                        richTextBox1.AppendText("- 物流单号相关：物流单号、logistics_no、express、快递单号、运单号、tracking\r\n");
                    }));

                    UIMessageBox.ShowError($"Excel文件中缺少必要的列：{missingColumns}\r\n\r\n" +
                        "请确保Excel文件包含：\r\n" +
                        "1. 原始单号列（列名包含：原始单号、src_tid、订单号等）\r\n" +
                        "2. 物流单号列（列名包含：物流单号、logistics_no、快递单号等）");
                    return;
                }

                // 显示找到的列信息
                this.Invoke(new Action(() =>
                {
                    richTextBox1.AppendText($"✓ 找到必要的列：\r\n");
                    richTextBox1.AppendText($"  - 原始单号列：'{srcTidColumn}'\r\n");
                    richTextBox1.AppendText($"  - 物流单号列：'{logisticsNoColumn}'\r\n");
                    if (hasLogisticsCode)
                    {
                        richTextBox1.AppendText($"  - 物流公司列：'{logisticsCodeColumn}'\r\n");
                    }
                    else
                    {
                        richTextBox1.AppendText($"  - 物流公司列：未找到（将使用默认值）\r\n");
                    }
                    richTextBox1.AppendText($"准备处理{dt.Rows.Count}行数据...\r\n");
                    richTextBox1.AppendText("=".PadRight(50, '=') + "\r\n");
                }));

                // 处理数据行
                ProcessDataRows(dt, srcTidColumn, logisticsNoColumn, logisticsCodeColumn);
            }
            catch (Exception ex)
            {
                this.Invoke(new Action(() =>
                {
                    richTextBox1.AppendText($"分析Excel数据时发生错误：{ex.Message}\r\n");
                }));
                UIMessageBox.ShowError($"分析Excel数据时发生错误：{ex.Message}");
            }
        }

        // 5. ProcessDataRows的完整实现
        private void ProcessDataRows(DataTable dt, string srcTidColumn, string logisticsNoColumn, string logisticsCodeColumn)
        {
            int successCount = 0;
            int failCount = 0;
            int skipCount = 0; // 跳过的空行数
            StringBuilder failMessage = new StringBuilder();

            // 用于统计处理进度
            int totalRows = dt.Rows.Count;
            int processedRows = 0;

            try
            {
                this.Invoke(new Action(() =>
                {
                    richTextBox1.AppendText($"开始逐行处理数据...\r\n");
                }));

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    processedRows++;

                    try
                    {
                        // 获取行数据
                        string srcTid = dt.Rows[i][srcTidColumn]?.ToString().Trim();
                        string logisticsNo = dt.Rows[i][logisticsNoColumn]?.ToString().Trim();
                        string logisticsCode = "";

                        if (!string.IsNullOrEmpty(logisticsCodeColumn))
                        {
                            logisticsCode = dt.Rows[i][logisticsCodeColumn]?.ToString().Trim();
                        }

                        // 数据验证
                        if (string.IsNullOrEmpty(srcTid) && string.IsNullOrEmpty(logisticsNo))
                        {
                            // 完全空行，跳过
                            skipCount++;
                            continue;
                        }

                        if (string.IsNullOrEmpty(srcTid))
                        {
                            failCount++;
                            string errorMsg = $"第{i + 1}行：原始单号为空";
                            failMessage.AppendLine(errorMsg);

                            this.Invoke(new Action(() =>
                            {
                                richTextBox1.AppendText($"✗ {errorMsg}\r\n");
                            }));
                            continue;
                        }

                        if (string.IsNullOrEmpty(logisticsNo))
                        {
                            failCount++;
                            string errorMsg = $"第{i + 1}行：物流单号为空（原始单号：{srcTid}）";
                            failMessage.AppendLine(errorMsg);

                            this.Invoke(new Action(() =>
                            {
                                richTextBox1.AppendText($"✗ {errorMsg}\r\n");
                            }));
                            continue;
                        }

                        // 显示当前处理的行
                        this.Invoke(new Action(() =>
                        {
                            richTextBox1.AppendText($"[{processedRows}/{totalRows}] 处理：{srcTid} -> {logisticsNo}");
                            if (!string.IsNullOrEmpty(logisticsCode))
                            {
                                richTextBox1.AppendText($" ({logisticsCode})");
                            }
                            richTextBox1.AppendText("\r\n");
                        }));

                        // 1. 先通过原始单号获取系统订单号
                        string tradeNo = GetWdtOrderInfo(srcTid);

                        if (string.IsNullOrEmpty(tradeNo))
                        {
                            failCount++;
                            string errorMsg = $"第{i + 1}行：无法获取原始单号 {srcTid} 对应的系统订单号";
                            failMessage.AppendLine(errorMsg);

                            this.Invoke(new Action(() =>
                            {
                                richTextBox1.AppendText($"  ✗ {errorMsg}\r\n");
                            }));
                            continue;
                        }

                        this.Invoke(new Action(() =>
                        {
                            richTextBox1.AppendText($"  ✓ 获取到系统订单号：{tradeNo}\r\n");
                        }));

                        // 2. 调用vip_trade_modify.php接口更新物流信息
                        bool updateResult = UpdateTradeLogistics(tradeNo, logisticsCode, logisticsNo, srcTid, i + 1);

                        if (updateResult)
                        {
                            successCount++;
                            this.Invoke(new Action(() =>
                            {
                                richTextBox1.AppendText($"  ✓ 第{i + 1}行处理成功：订单 {tradeNo} 物流信息已更新\r\n");
                            }));
                        }
                        else
                        {
                            failCount++;
                            string errorMsg = $"第{i + 1}行：订单 {tradeNo} 物流信息更新失败";
                            failMessage.AppendLine(errorMsg);

                            this.Invoke(new Action(() =>
                            {
                                richTextBox1.AppendText($"  ✗ {errorMsg}\r\n");
                            }));
                        }

                        // 每处理10行显示一次进度
                        if (processedRows % 10 == 0 || processedRows == totalRows)
                        {
                            this.Invoke(new Action(() =>
                            {
                                richTextBox1.AppendText($"--- 进度：{processedRows}/{totalRows}，成功：{successCount}，失败：{failCount}，跳过：{skipCount} ---\r\n");
                            }));
                        }
                    }
                    catch (Exception rowEx)
                    {
                        failCount++;
                        string errorMsg = $"第{i + 1}行：处理异常 - {rowEx.Message}";
                        failMessage.AppendLine(errorMsg);

                        this.Invoke(new Action(() =>
                        {
                            richTextBox1.AppendText($"  ✗ {errorMsg}\r\n");
                        }));
                    }
                }

                // 显示最终处理结果
                this.Invoke(new Action(() =>
                {
                    richTextBox1.AppendText("\r\n" + "=".PadRight(50, '=') + "\r\n");
                    richTextBox1.AppendText("处理完成！\r\n");
                    richTextBox1.AppendText($"总计：{totalRows}行\r\n");
                    richTextBox1.AppendText($"成功：{successCount}行\r\n");
                    richTextBox1.AppendText($"失败：{failCount}行\r\n");
                    richTextBox1.AppendText($"跳过：{skipCount}行（空行）\r\n");
                    richTextBox1.AppendText($"成功率：{(totalRows - skipCount > 0 ? (double)successCount / (totalRows - skipCount) * 100 : 0):F1}%\r\n");
                    richTextBox1.AppendText("=".PadRight(50, '=') + "\r\n");
                }));

                // 构建结果消息
                string resultMsg = $"批量更新完成！\r\n\r\n" +
                                  $"总计：{totalRows}行\r\n" +
                                  $"成功：{successCount}行\r\n" +
                                  $"失败：{failCount}行\r\n" +
                                  $"跳过：{skipCount}行（空行）\r\n" +
                                  $"成功率：{(totalRows - skipCount > 0 ? (double)successCount / (totalRows - skipCount) * 100 : 0):F1}%";

                if (failCount > 0 && failMessage.Length > 0)
                {
                    resultMsg += $"\r\n\r\n失败详情：\r\n{failMessage.ToString()}";
                }

                // 根据结果显示不同的消息框
                if (failCount == 0)
                {
                    UIMessageBox.ShowSuccess(resultMsg);
                }
                else if (successCount > 0)
                {
                    UIMessageBox.ShowWarning(resultMsg);
                }
                else
                {
                    UIMessageBox.ShowError(resultMsg);
                }
            }
            catch (Exception ex)
            {
                this.Invoke(new Action(() =>
                {
                    richTextBox1.AppendText($"\r\n处理数据行时发生严重错误：{ex.Message}\r\n");
                }));
                UIMessageBox.ShowError($"处理数据时发生严重错误：{ex.Message}");
            }
        }

        // 6. 优化的更新物流信息方法
        private bool UpdateTradeLogistics(string tradeNo, string logisticsCode, string logisticsNo, string srcTid, int rowNumber)
        {
            try
            {
                ApiRateController.WaitForNextCall1();

                var dt = new Dictionary<string, string>();
                dt.Add("trade_no", tradeNo);
                dt.Add("logistics_no", logisticsNo);

                // 如果有物流公司编号则添加，否则可以使用默认值
                if (!string.IsNullOrEmpty(logisticsCode))
                {
                    dt.Add("logistics_code", logisticsCode);
                }
                else
                {
                    // 如果没有物流公司编号，可以使用一个默认值或者不传递该参数
                    // 根据旺店通API文档，logistics_code是可选的
                    this.Invoke(new Action(() =>
                    {
                        richTextBox1.AppendText($"    注意：第{rowNumber}行未指定物流公司编号\r\n");
                    }));
                }

                var resultobj = JObject.Parse(GetWdtDataBLL.GetDataFromApi(dt, "vip_trade_modify.php"));

                // 处理返回结果
                if (resultobj["code"] != null && Convert.ToInt32(resultobj["code"]) == 0)
                {
                    // code为0表示成功
                    return true;
                }
                else
                {
                    // code不为0，更新失败
                    string message = resultobj["message"]?.ToString() ?? "未知错误";
                    this.Invoke(new Action(() =>
                    {
                        richTextBox1.AppendText($"    API返回错误: {message}\r\n");
                    }));
                    return false;
                }
            }
            catch (Exception ex)
            {
                this.Invoke(new Action(() =>
                {
                    richTextBox1.AppendText($"    更新异常: {ex.Message}\r\n");
                }));
                return false;
            }
        }

        private void uiLabel28_Click(object sender, EventArgs e)
        {

        }

        private void uiButton44_Click(object sender, EventArgs e)
        {
            if (uiTextBox14.Text.Trim()=="" && uiTextBox13.Text.Trim()==""&& uiTextBox15.Text.Trim()=="" && uiTextBox16.Text.Trim()=="")
            {
                UIMessageBox.Show("请输入查询条件，至少输入一项！");
                return;
            }
            Task.Run(getOrderPackData);
        }
        public async void getOrderPackData()
        {
            this.Invoke(new Action(() =>
            {
                uiButton44.Text = "查询中...";
                uiButton44.Enabled = false;

            }));

            try
            {
                using (HttpClient client = new HttpClient())
                {

                    var obj = new
                    {

                        logistics_no = uiTextBox14.Text,
                        WDTSrcbillno = uiTextBox13.Text,
                        Recipienter = uiTextBox15.Text,
                        goods_no = uiTextBox16.Text


                    };

                    var str = JsonConvert.SerializeObject(obj);
                    HttpContent content = new StringContent(str);
                    content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                    HttpResponseMessage response = await client.PostAsync("http://120.78.210.159:3304/api/wdt/QueryPackingDataLog", content);
                    //  response.EnsureSuccessStatusCode();//用来抛异常的
                    string resultStr = await response.Content.ReadAsStringAsync();
                   // richTextBox2.Text = resultStr;
                    var result = JObject.Parse(resultStr);
                    if (result.ContainsKey("code"))
                    {
                        if (Convert.ToInt32(result["code"]) == 1)
                        {
                            JArray resultList = JArray.Parse(result["lst"].ToString());
                            var datas = resultList.Select(a => new
                            {
                              
                                装箱单号 = a["pack_no"].ToString(),
                                装箱类型 = a["isWholebox"].ToString()=="1"?"整箱":"散装",
                                物流单号 = a["logistics_no"].ToString(),
                                原始单号 = a["WDTSrcbillno"].ToString(),
                                收货人 = a["Recipienter"].ToString(),
                                货号 = a["goods_no"].ToString(),
                                条码 = a["barcode"].ToString(),
                                装箱时间 =Convert.ToDateTime(a["packtime"]).ToString("F") ,
                                收货地址 = a["Address"].ToString()

                               

                            }).OrderByDescending(a => a.装箱时间).ToList();
                          
                            this.Invoke(new Action(() =>
                            {
                                dataGridView15.DataSource = datas;
                              
                            }));

                        }
                        else
                        {
                            UIMessageBox.Show(result["mess"].ToString());
                        }
                    }
                    else
                    {
                        UIMessageBox.Show("无法访问系统,请关注是否网络问题!");
                    }
                }



            }
            catch (Exception ex)
            {
                UIMessageBox.Show("出现错误:" + ex.Message.ToString());

            }
            this.Invoke(new Action(() =>
            {
                uiButton44.Text = "查询";
                uiButton44.Enabled = true;

            }));
        }

        private void uiButton45_Click(object sender, EventArgs e)
        {
            saveFileDialog10.Filter = "Excel 文件(*.xls)|*.xls";
            saveFileDialog10.ShowDialog();
        }

        private void saveFileDialog10_FileOk(object sender, CancelEventArgs e)
        {
            try
            {
                var datas = JArray.FromObject(dataGridView15.DataSource);

                if (saveFileDialog10.FileName != "")
                {
                    string tfilepath = saveFileDialog10.FileName;

                    HSSFWorkbook wb;
                    FileStream file;

                    MemoryStream ms = new MemoryStream();
                    wb = new HSSFWorkbook();

                    ISheet sheet;
                    sheet = wb.CreateSheet("sheet1");

                    ICellStyle headerStyle = wb.CreateCellStyle();
                    IFont headerFont = wb.CreateFont();
                    headerFont.FontHeightInPoints = 12; // 设置表头字体大小为10号
                    headerStyle.SetFont(headerFont);
                    headerStyle.WrapText = true;

                    headerStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                    headerStyle.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;

                    // 为表头添加边框
                    headerStyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                    headerStyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                    headerStyle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                    headerStyle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                    headerStyle.TopBorderColor = IndexedColors.Black.Index;
                    headerStyle.BottomBorderColor = IndexedColors.Black.Index;
                    headerStyle.LeftBorderColor = IndexedColors.Black.Index;
                    headerStyle.RightBorderColor = IndexedColors.Black.Index;

                    // 数据样式 - 添加边框
                    ICellStyle dataStyle = wb.CreateCellStyle();
                    IFont dataFont = wb.CreateFont();
                    dataFont.FontHeightInPoints = 10; // 设置数据字体大小为10号
                    dataStyle.SetFont(dataFont);
                    dataStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                    dataStyle.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;

                    // 为数据单元格添加边框
                    dataStyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                    dataStyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                    dataStyle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                    dataStyle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                    dataStyle.TopBorderColor = IndexedColors.Black.Index;
                    dataStyle.BottomBorderColor = IndexedColors.Black.Index;
                    dataStyle.LeftBorderColor = IndexedColors.Black.Index;
                    dataStyle.RightBorderColor = IndexedColors.Black.Index;

                    NPOI.SS.UserModel.IRow rowTemp;
                    rowTemp = sheet.CreateRow(0);//创建第一行
                    int c = 0;
                    JObject header = (JObject)datas.First();

                    sheet.SetColumnWidth(0, 15 * 256);
                    sheet.SetColumnWidth(1, 10 * 256);
                    sheet.SetColumnWidth(2, 15 * 256);
                    sheet.SetColumnWidth(3, 20 * 256);
                    sheet.SetColumnWidth(4, 12 * 256);
                    sheet.SetColumnWidth(5, 12 * 256);
                    sheet.SetColumnWidth(6, 15 * 256);
                    sheet.SetColumnWidth(7, 22 * 256);
                    sheet.SetColumnWidth(8, 100 * 256);
                    rowTemp = sheet.CreateRow(0);
                    short defaultHeight = rowTemp.Height;
                    rowTemp.Height = (short)(defaultHeight * 2);
                    c = 0;
                    foreach (JProperty prop in header.Properties())
                    {

                      //  if (prop.Name != "库存量" && prop.Name != "净需求量" && prop.Name != "箱规" && prop.Name != "调拨箱数" && prop.Name != "调拨数量")
                      //  {
                            var cellh = rowTemp.CreateCell(c);
                            cellh.SetCellValue(prop.Name.ToString());
                            cellh.CellStyle = headerStyle;
                            c++;
                      //  }

                    }
                    int j = 1;
                    datas.ForEach(r =>
                    {
                        rowTemp = sheet.CreateRow(j);
                        rowTemp.Height = (short)(defaultHeight * 1.5);
                        JObject dataRow = (JObject)r;
                        c = 0;
                        foreach (JProperty prop in dataRow.Properties())
                        {
                           // if (prop.Name != "库存量" && prop.Name != "净需求量" && prop.Name != "箱规" && prop.Name != "调拨箱数" && prop.Name != "调拨数量")
                          //  {
                                var celld = rowTemp.CreateCell(c);
                                celld.SetCellValue(prop.Value.ToString());
                                celld.CellStyle = dataStyle;
                                c++;
                           // }

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

    }
    public class MatInfo
    {
       public  int changNum { get; set;}
       public  decimal FWeight { get; set; }
       public string FSaleBaseUnitNo { get; set; }
    }

    public class ApiRateController
    {
        private static DateTime lastCallTime = DateTime.MinValue;
        private static readonly object lockObject = new object();
        private static readonly object lockObject1 = new object();
        public static void WaitForNextCall()
        {
            lock (lockObject)
            {
                var timeSinceLastCall = DateTime.Now - lastCallTime;
                var minInterval = TimeSpan.FromSeconds(0.2); // 每秒最多1次调用

                if (timeSinceLastCall < minInterval)
                {
                    var waitTime = minInterval - timeSinceLastCall;
                    Thread.Sleep(waitTime);
                }

                lastCallTime = DateTime.Now;
            }
        }
        public static void WaitForNextCall1()
        {
            lock (lockObject1)
            {
                var timeSinceLastCall = DateTime.Now - lastCallTime;
                var minInterval = TimeSpan.FromSeconds(1); // 每秒最多1次调用

                if (timeSinceLastCall < minInterval)
                {
                    var waitTime = minInterval - timeSinceLastCall;
                    Thread.Sleep(waitTime);
                }

                lastCallTime = DateTime.Now;
            }
        }
    }
}
public class TransferApplyRow
{
    public string MatNumber { get; set; }
    public int ApplyQty { get; set; }

    public int Qps { get; set; }

    public string WarehouseCode { get; set; }
    public string FSaleBaseUnitNo { get; set; }

    public Boolean Adeq_Inv { get; set; }
}
public class U_Plans_WdtSOderDetailViewModel
{
    public int rowindex { get; set; }
    public string src_tid { get; set; }

    public string goods_no { get; set; }
    public int? num { get; set; }
    public int? wbnum { get; set; }
    public int? snum { get; set; }
    public int? specnum { get; set; }
    public int? calspecnum { get; set; }
    public decimal? fweight { get; set; }
    public decimal? totalweight { get; set; }

}

