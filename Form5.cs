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

namespace SleckEc2KdInterface
{
    public partial class Form5 : UIForm
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
        public Form5()
        {
            InitializeComponent();
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
                        orderno = "",
                        startDate = Convert.ToDateTime(sdt.Value.ToShortDateString()),
                        endDate = Convert.ToDateTime(edt.Value.ToShortDateString()),
                        goodno = uitxtgoodno.Text,
                        custname = uiTextBox11.Text,
                        wdtsrcno =uitxtSrcOrderNo.Text,
                        recipienter = uitxtAddressee.Text,
                        userId = wflogin.fuserId

                    };
 
                    var str = JsonConvert.SerializeObject(conobj);

                    HttpContent content = new StringContent(str);
                    content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                    HttpResponseMessage response = await client.PostAsync("http://120.78.210.159:3304/Order/GetWDTOrderLogList", content);
                  //  response.EnsureSuccessStatusCode();//用来抛异常的
                    string resultStr = await response.Content.ReadAsStringAsync();
                  
                    var result = JObject.Parse(resultStr);
                    if (result.ContainsKey("code"))
                    {
                        if (Convert.ToInt32(result["code"]) == 0)
                        {
                            JArray resultList = JArray.Parse(result["list"].ToString());
                            var datas = resultList.Select(a => new {
                                客户名称 = a["FCustName"].ToString(),
                                订单编号 = a["FOrderNo"].ToString(),
                                订单日期 = a["FOrderDate"].ToString(),
                                店铺 = a["Shopname"].ToString(),
                                发货仓 = a["StockName"].ToString(),
                                订单状态 = a["FOrderStatus"].ToString()=="C" ?"已审核":"未审核",
                                平台单号 = a["Srcbillno"].ToString(),
                                原始单号 = a["WDTSrcbillno"].ToString(),
                                收件人 = a["Recipienter"].ToString(),
                                电话 = a["Tel"].ToString(),
                                旺店通状态 = a["FStatus"].ToString(),
                                物流单号 = a["logistics_no"].ToString(),
                                发货时间 = a["Consign_time"],
                                商家编码 = a["Good_No"].ToString(),
                                发货数量 = a["Qty"].ToString() ,
                                同步旺店通结果 = a["ResultMess"].ToString(),
                                客服备注 = a["Remark"].ToString(),
                                订单ID = a["FID"].ToString(),
                                明细ID = a["ID"].ToString(),

                            }).OrderByDescending(a=>a.明细ID).ToList();
                            this.Invoke(new Action(() =>
                            {
                               // uiRichTextBox1.AppendText(str);
                                uiDataGridView5.DataSource = datas;

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
                UIMessageBox.Show("出现错误:"+ex.Message.ToString());

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
            ICell cell = null;
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
                        sheet = workbook.GetSheetAt(0);//读取第一个sheet，当然也可以循环读取每个sheet  
                        dataTable = new DataTable();
                        if (sheet != null)
                        {
                            int rowCount = sheet.LastRowNum;//总行数  
                            if (rowCount > 0)
                            {
                                IRow firstRow = sheet.GetRow(0);//第一行  
                                int cellCount = firstRow.LastCellNum;//列数  

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
                                                    dataRow[j] = cell.StringCellValue.Trim();
                                                    break;
                                            }
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
            catch (Exception)
            {
                if (fs != null)
                {
                    fs.Close();
                }
                return null;
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
        public  bool  addressResolution(String address)
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

            var obj = new {cusName= uitxtdzCustName.Text,sdate= Convert.ToDateTime(dzsdt.Value.ToShortDateString()),edate= Convert.ToDateTime(dzedt.Value.ToShortDateString()) };
            var str = JsonConvert.SerializeObject(obj);

            request.AddBody(str);
            response = client.Execute(request);
            var result = JObject.Parse(response.Content);
            if (result["code"].ToString() == "1" )
            {
                this.Invoke(new Action(() =>
                {

                    uiDataGridView2.DataSource = JArray.Parse(result["dataList"].ToString());
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
        private void getCustBills()
        {
            this.Invoke(new Action(() =>
            {
                uiButton4.Text = "统计中...";
                uiButton4.Enabled = false;
                uirichtxt.Clear();
                uirichtxt.AppendText(DateTime.Now.ToShortTimeString()+":开始统计...\r\n");
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
                 客户编码=b.FCustId,
                 商品金额=a.RecAmount
                


             }).ToList();
                //订货数据
                var orResult = result.GroupBy(p => new
                {
                    p.订单日期,
                    p.客户名称,
                    p.客户编码
                  
                }
                ).Select(a => new {
                    客户名称=  a.Key.客户名称,
                    客户ID=a.Key.客户编码,
                    订单日期 = a.Key.订单日期,
                    订货数量 = a.Sum(c => Convert.ToDecimal(c.商品数量)),
                    订货金额=  a.Sum(c => Convert.ToDecimal(c.商品金额)),
                    取消数量=a.Where(b=>b.收货状态=="已取消").Sum(c => Convert.ToDecimal(c.商品数量)),
                    取消金额= a.Where(b => b.收货状态 == "已取消").Sum(c => Convert.ToDecimal(c.商品金额)),
                    发货数量 =0,
                    发货金额=0,
                    退货数量=0,
                    退货金额=0


                }).OrderBy(a=>a.订单日期).ToArray();
                this.Invoke(new Action(() =>
                {
                   
                   // uiDataGridView2.DataSource = orResult;
                    uirichtxt.AppendText(DateTime.Now.ToShortTimeString() + ":获取订货数据完成\r\n");
                   
                }));


                //原始单号集合
                var orResultforSrc = result.Where(a=>a.收货状态!="已取消").GroupBy(p => new
                {
                    p.订单日期,
                    p.客户名称,
                    p.客户编码,
                    p.原始单号
                }
               ).Select((a) => new {
                   客户名称 = a.Key.客户名称,
                   客户ID = a.Key.客户编码,
                   订单日期 = a.Key.订单日期,
                   原始单号=a.Key.原始单号,
                   发货金额=0,
                   发货数量 = 0

               }).Distinct().ToList();
                //通过原始单号去核查发货数据
                #region
                var Srcs = orResultforSrc.Select(s=>s.原始单号).ToArray();
             
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
                    }).ToList().GroupBy(a=>new { a.客户ID, a.订单日期 }).Select(b => new
                    {
                        客户ID= b.Key.客户ID,
                        订单日期=b.Key.订单日期,
                        发货数量 = (decimal)b.Sum(x =>x.发货数量 ),
                        发货金额 = (decimal)b.Sum(x => x.发货金额)

                    }).ToList();

                var sendInfos = ( from a in orResult
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
                    
                    uiDataGridView2.DataSource = sendandreturnInfos;
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
            if(wflogin.FUserType.Trim()!="3")
            {
                uiTextBox4.Enabled = true;
                uiTextBox8.Enabled = true;
                uitxtdzCustName.Enabled = true;
                uiTextBox11.Enabled = true;
            }
            else
            {
                uiTextBox4.Enabled =false;
                uiTextBox8.Enabled = false;
                uitxtdzCustName.Enabled = false;
                uiTextBox11.Enabled = false;
            }
            uiTextBox4.Text = wflogin.fcustName;
            uiTextBox8.Text= wflogin.fcustName;
            uitxtdzCustName.Text= wflogin.fcustName;
            uiTextBox11.Text= wflogin.fcustName;
        }

        private void uiButton5_Click(object sender, EventArgs e)
        {
            Task.Run(getOutStockData);
        }
        private void getOutStockData()
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

            var obj = new {
                custName = uiTextBox4.Text, 
                sdate = Convert.ToDateTime(Convert.ToDateTime(ssdt.Value).ToString("yyyy - MM - dd") + " 00:00:00"),
                edate = Convert.ToDateTime(Convert.ToDateTime(sedt.Value).ToString("yyyy - MM - dd") + " 23:59:59") ,
                src_tid = uiTextBox3.Text ,
                receiver_name = uiTextBox2.Text ,
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

                    uiDataGridView3.DataSource = JArray.Parse(result["sdataList"].ToString());
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
        private void uiButton6_Click(object sender, EventArgs e)
        {
            if(uiDataGridView2.CurrentRow.Index <0)
            {
                UIMessageBox.Show("没有选择表格行!");
                return;
            }
            Task.Run(getBillsDetailforApi);



        }
        private void getBillsDetailforApi()
        {
            DateTime fdate = Convert.ToDateTime(uiDataGridView2.Rows[uiDataGridView2.CurrentRow.Index].Cells["订单日期"].Value);
            int custId = Convert.ToInt32(uiDataGridView2.Rows[uiDataGridView2.CurrentRow.Index].Cells["客户ID"].Value);
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

            var obj = new { fdate =fdate, custId = custId };
            var str = JsonConvert.SerializeObject(obj);

            request.AddBody(str);
            response = client.Execute(request);
            var result = JObject.Parse(response.Content);
            if (result["code"].ToString() == "1")
            {
                this.Invoke(new Action(() =>
                {
                    uiDataGridView6.DataSource = JArray.Parse(result["odataList"].ToString());
                    uiDataGridView7.DataSource = JArray.Parse(result["sdataList"].ToString());
                    uiDataGridView8.DataSource = JArray.Parse(result["rdataList"].ToString());

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
        private void getBillsDetail()
        {
            DateTime fdate=Convert.ToDateTime(uiDataGridView2.Rows[uiDataGridView2.CurrentRow.Index].Cells["订单日期"].Value);
            int custId=Convert.ToInt32(uiDataGridView2.Rows[uiDataGridView2.CurrentRow.Index].Cells["客户ID"].Value);

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
                   c.FCUSTID== custId  && b.FDate== Convert.ToDateTime(fdate.ToShortDateString()) 
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

                    uiDataGridView6.DataSource = orResult;
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
               ).Select((a) => new {
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
                        发货单号=p.order_no,
                        商品编码=p.goods_no,
                        发货数量 = p.num,
                        发货金额 = p.share_amount
                    }).ToList();
 
                this.Invoke(new Action(() =>
                {

                    uiDataGridView7.DataSource = saleStockOutItems;
                    uirichtxt.AppendText(DateTime.Now.ToShortTimeString() + ":获取发货数据完成\r\n");

                }));
                #endregion

                //通过原始单号追溯退货数据
                #region

                var saleStockReturns = dbContextEc.SaleReturnStockItemDb.GetList(a => Srcs.Contains(a.tid)).
                   Select(p => new
                   {
                       原始单号 = p.tid,
                       退货入库单号=p.order_no,
                       退货数量 = p.num,
                       退货金额 = p.num*p.src_price
                   }).ToList();


                #endregion
                _ = this.Invoke(new Action(() =>
                  {

                      uiDataGridView8.DataSource = saleStockReturns;
                      uirichtxt.AppendText(DateTime.Now.ToShortTimeString() + ":获取退货数据完成\r\n");

                  }));




            }
            catch (Exception ex)
            {
                UIMessageBox.Show(ex.Message);

            }
            this.Invoke(new Action(() =>
            {
                uiButton6.Text = "明细";
                uiButton6.Enabled = true;

            }));
        }

        private void uiDataGridView2_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            Rectangle rectangle = new Rectangle(e.RowBounds.Location.X,
              e.RowBounds.Location.Y,
              uiDataGridView2.RowHeadersWidth,
              e.RowBounds.Height);

            TextRenderer.DrawText(e.Graphics, (e.RowIndex + 1).ToString(),
                uiDataGridView2.RowHeadersDefaultCellStyle.Font,
                rectangle,
                uiDataGridView2.RowHeadersDefaultCellStyle.ForeColor,
                TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
        }

        private void uiDataGridView4_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            Rectangle rectangle = new Rectangle(e.RowBounds.Location.X,
              e.RowBounds.Location.Y,
              uiDataGridView4.RowHeadersWidth,
              e.RowBounds.Height);

            TextRenderer.DrawText(e.Graphics, (e.RowIndex + 1).ToString(),
                uiDataGridView4.RowHeadersDefaultCellStyle.Font,
                rectangle,
                uiDataGridView4.RowHeadersDefaultCellStyle.ForeColor,
                TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
        }

        private void uiDataGridView3_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            Rectangle rectangle = new Rectangle(e.RowBounds.Location.X,
              e.RowBounds.Location.Y,
              uiDataGridView3.RowHeadersWidth,
              e.RowBounds.Height);

            TextRenderer.DrawText(e.Graphics, (e.RowIndex + 1).ToString(),
                uiDataGridView3.RowHeadersDefaultCellStyle.Font,
                rectangle,
                uiDataGridView3.RowHeadersDefaultCellStyle.ForeColor,
                TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
        }

        private void Form5_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void uiButton7_Click(object sender, EventArgs e)
        {
            if(uiDataGridView1.Rows.Count<1)
            {
                UIMessageBox.Show("没有可以校验的数据!");
                return;
            }
          Task.Run(CheckData);
        }
        private void CheckData()
        {
            errorcount = 0;
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
                        if (a!="" && a!=null)
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
                  if (!System.Text.RegularExpressions.Regex.IsMatch(a["货品数量"].ToString(), @"^([1-9]{1,})$"))
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
                    if (shopS.Where(a => a["shop_name"].ToString().Contains(s.ToString())).ToList().Count < 1)
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
            RestClient client ;
            RestRequest request ;
          
            RestResponse response;
            matNames.ForEach(m =>
            {
              

                try
                {
                    client = new RestClient("http://120.78.210.159:3304/Order/GetCustWDTPriceMatList");
                    request = new RestRequest();
                    request.AddHeader("Content-Type", "application/json;");
                    request.Method = Method.Post;
               
                    var matInfo = new { custid = wflogin.fcustId.Trim(), goodno = m };
                    var str = JsonConvert.SerializeObject(matInfo);

                    request.AddJsonBody(str);
                    response = client.Execute(request);
                    resultobj = JObject.Parse(response.Content);
                    //this.Invoke(new Action(() =>
                    //{
                    //    uiRichTextBox1.AppendText(str +"\r\n"+ response.Content + "\r\n");

                    //}));
                    if (resultobj["code"].ToString() == "1")
                    {
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
                dt.Add("spec_no", m.ToString());
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
                                    uiRichTextBox1.AppendText("第" + r.ToString() + "行商家编码不存在与旺店通" + "\r\n");
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
                    client = new RestClient("http://120.78.210.159:3304/api/Wdt/getLogistics");
                    request = new RestRequest();
                    request.AddHeader("Content-Type", "application/json;");
                    request.Method = Method.Post;
                    response = client.Execute(request);
                    var result = JObject.Parse(response.Content);
                    //this.Invoke(new Action(() =>
                    //{
                    //    uiRichTextBox1.AppendText(response.Content + "\r\n");


                    //}));
                    if (result["code"].ToString() == "1")
                    {
                        var lst = JArray.Parse(result["logistics"].ToString());
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
           }
            catch (Exception er)
            {
                this.Invoke(new Action(() =>
                {
            uiRichTextBox1.AppendText("出现错误," + er.Message + "\r\n");

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

        private void uiButton9_Click(object sender, EventArgs e)
        {
           


        }

        private async void uiButton9_Click_1(object sender, EventArgs e)
        {
            Task.Run(upMat);
         }
        private void  upMat()
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
               
                var result = JArray.Parse(response.Content).Where(a=> ss.Contains(a["FNUMBER"].ToString().Left(1))).ToList();
             
                if (result.Count > 0)
                {
                    this.Invoke(new Action(() =>
                    {
                        uiRichTextBox2.AppendText(result.Count.ToString()+"条记录符合上传条件" + "\r\n");
                    }));
                    result.ForEach(a =>
                    {
                        Thread.Sleep(1000);
                        this.Invoke(new Action(() =>
                        {
                            uiRichTextBox2.AppendText(a["FNUMBER"].ToString() + "查询该物料是否存在于旺店通" + "\r\n");
                        }));
                        Dictionary<string, string> dt = new Dictionary<string, string>();
                        dt.Add("spec_no", a["FNUMBER"].ToString());
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
                                    uiRichTextBox2.AppendText(a["FNUMBER"].ToString() + "同步失败!"+"\r\n");
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
                    uiRichTextBox2.AppendText(er.Message+"\r\n");
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
            if(!checkPass)
            {
                UIMessageBox.Show("请先校验导入的数据!");
                return;
               
            }
            else
            {
                if (errorcount>0)
                {
                    DialogResult YRO = MessageBox.Show("校验数据时发现了错误，是否已纠正并继续保存？", "提醒您：", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                    if (YRO == DialogResult.No)
                    {
                        return;
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
                RestClient client = new RestClient("http://120.78.210.159:3304/api/Wdt/getLogistics");
                RestRequest request = new RestRequest();
                request.AddHeader("Content-Type", "application/json;");
                request.Method = Method.Post;
                var response = client.Execute(request);
                var result = JObject.Parse(response.Content);
                JArray lst;
                if (result["code"].ToString() == "1")
                {
                    lst = JArray.Parse(result["logistics"].ToString());

                }
                else
                {
                   lst = null;
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

                            NPOI.SS.UserModel.IRow rowTemp ;

                            int j =1;
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
                                if (r["平台单号"]!=null && r["平台单号"].ToString()!="")
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
                        Remark = a["客服备注"].ToString(),
                        UserID = Convert.ToInt32(wflogin.fuserId),
                        logistics_type = (int)(lst == null ? 0 : (lst.Where(l => l["logistics_typeName"].ToString() == a["物流类型"].ToString()).ToList().Count > 0 ? (lst.Where(l => l["logistics_typeName"].ToString() == a["物流类型"].ToString()).First()["logistics_type"]):0))

                    });
                });
                //this.Invoke(new Action(() =>
                //{
                //    uiRichTextBox1.AppendText(JsonConvert.SerializeObject(lstOrderItems) +"\r\n");
                //}));
                client = new RestClient("http://120.78.210.159:3304/api/Wdt/SaveWDTOrderAndItemApi");
                request = new RestRequest();
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
                response = client.Execute(request);
                result = JObject.Parse(response.Content);
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
            Rectangle rectangle = new Rectangle(e.RowBounds.Location.X,
                e.RowBounds.Location.Y,
                uiDataGridView5.RowHeadersWidth,
                e.RowBounds.Height);

            TextRenderer.DrawText(e.Graphics, (e.RowIndex + 1).ToString(),
                uiDataGridView5.RowHeadersDefaultCellStyle.Font,
                rectangle,
                uiDataGridView1.RowHeadersDefaultCellStyle.ForeColor,
                TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
        }

        private async void uiButton11_Click(object sender, EventArgs e)
        {
            if (uiDataGridView5.Rows.Count < 1)
            {
                UIMessageBox.Show("没有可以提交的数据!");
                return;
            }
            if (uiDataGridView5.CurrentRow.Index < 0)
            {
                UIMessageBox.Show("没有选择表格行!");
                return;
            }
            string orderNo = uiDataGridView5.Rows[uiDataGridView5.CurrentRow.Index].Cells["订单编号"].Value.ToString();
            string fOrderStatus = uiDataGridView5.Rows[uiDataGridView5.CurrentRow.Index].Cells["订单状态"].Value.ToString();
            if (orderNo.Length<1)
            {
                UIMessageBox.Show("订单编号不能为空!");
                return;
            }
            if (fOrderStatus!="未审核")
            {
                UIMessageBox.Show("订单状态不符合要求!");
                return;
            }
            using (HttpClient client = new HttpClient())
            {

                var obj = new { forderno= orderNo, userid = Convert.ToInt32(wflogin.fuserId)};
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
           
            if (uiDataGridView5.Rows.Count < 1)
            {
                UIMessageBox.Show("没有可以删除的数据!");
                return;
            }
            if (uiDataGridView5.CurrentRow.Index < 0)
            {
                UIMessageBox.Show("没有选择表格行!");
                return;
            }
            DialogResult YRO = MessageBox.Show("您确定要删除选中的记录吗？", "提醒您：", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
            if (YRO == DialogResult.No)
            {
                return;
            }
            string fentryid = uiDataGridView5.Rows[uiDataGridView5.CurrentRow.Index].Cells["明细ID"].Value.ToString();
            string fid = uiDataGridView5.Rows[uiDataGridView5.CurrentRow.Index].Cells["订单ID"].Value.ToString();
            string fOrderStatus = uiDataGridView5.Rows[uiDataGridView5.CurrentRow.Index].Cells["订单状态"].Value.ToString();
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
            var obj = new { fentryid=fentryid, fid=fid, userId =wflogin.fuserId.ToString()};
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
           
            if (uiDataGridView5.Rows.Count < 1)
            {
                UIMessageBox.Show("没有可以删除的数据!");
                return;
            }
            if (uiDataGridView5.CurrentRow.Index < 0)
            {
                UIMessageBox.Show("没有选择表格行!");
                return;
            }
            DialogResult YRO = MessageBox.Show("您确定要删除选中的记录吗？", "提醒您：", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
            if (YRO == DialogResult.No)
            {
                return;
            }
            string fentryid = uiDataGridView5.Rows[uiDataGridView5.CurrentRow.Index].Cells["明细ID"].Value.ToString();
            string fid = uiDataGridView5.Rows[uiDataGridView5.CurrentRow.Index].Cells["订单ID"].Value.ToString();
            string fOrderStatus = uiDataGridView5.Rows[uiDataGridView5.CurrentRow.Index].Cells["订单状态"].Value.ToString();
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
            request.AddHeader("Content-Type", "application/json");
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

        private void uiDataGridView6_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            Rectangle rectangle = new Rectangle(e.RowBounds.Location.X,
              e.RowBounds.Location.Y,
              uiDataGridView6.RowHeadersWidth,
              e.RowBounds.Height);

            TextRenderer.DrawText(e.Graphics, (e.RowIndex + 1).ToString(),
                uiDataGridView6.RowHeadersDefaultCellStyle.Font,
                rectangle,
                uiDataGridView6.RowHeadersDefaultCellStyle.ForeColor,
                TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
        }

        private void uiDataGridView7_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            Rectangle rectangle = new Rectangle(e.RowBounds.Location.X,
              e.RowBounds.Location.Y,
              uiDataGridView7.RowHeadersWidth,
              e.RowBounds.Height);

            TextRenderer.DrawText(e.Graphics, (e.RowIndex + 1).ToString(),
                uiDataGridView7.RowHeadersDefaultCellStyle.Font,
                rectangle,
                uiDataGridView7.RowHeadersDefaultCellStyle.ForeColor,
                TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
        }

        private void uiDataGridView8_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            Rectangle rectangle = new Rectangle(e.RowBounds.Location.X,
              e.RowBounds.Location.Y,
              uiDataGridView8.RowHeadersWidth,
              e.RowBounds.Height);

            TextRenderer.DrawText(e.Graphics, (e.RowIndex + 1).ToString(),
                uiDataGridView8.RowHeadersDefaultCellStyle.Font,
                rectangle,
                uiDataGridView8.RowHeadersDefaultCellStyle.ForeColor,
                TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
        }

        private string getLogisticsName(int logisticsid)
        {
            RestClient client = new RestClient("http://120.78.210.159:3304/api/Wdt/getLogisticsName");
            RestRequest request = new RestRequest();
            request.AddHeader("Content-Type", "application/json");
            request.Method = Method.Post;
            RestResponse response;
            //   string shopname, string wdtsrcbillno, string statusid
            var obj = new { logisticsid= logisticsid };
            var str = JsonConvert.SerializeObject(obj);

            request.AddBody(str);
            response = client.Execute(request);
            var result = JObject.Parse(response.Content);
         
            return result["name"].ToString();
          
        }
        private void uiButton12_Click(object sender, EventArgs e)
        {
            Dictionary<string, string>  dt = new Dictionary<string, string>();

         // dt.Add("logistics_no", "");
            var  resultobj = JObject.Parse(GetWdtDataBLL.GetDataFromApi(dt, "logistics.php"));
            if (resultobj["code"].ToString() == "0")
            {
              var logistics_type =  JArray.Parse(JsonConvert.SerializeObject(resultobj["logistics_list"])).Where(a => a["is_disabled"].ToString() == "0" && a["logistics_type"].ToString()!="0").Select(a =>
                    new { 
                        物流名称 = a["logistics_name"],
                        物流类型名称 = getLogisticsName(Convert.ToInt32(a["logistics_type"])), 
                        物流类型ID = Convert.ToInt32(a["logistics_type"]),
                        物流编号 = a["logistics_no"] }).ToList();
                this.Invoke(new Action(() =>
                {
                    uiDataGridView9.DataSource = logistics_type;
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

        private void uiButton13_Click(object sender, EventArgs e)
        {
            if (uiDataGridView5.Rows.Count < 1)
            {
                UIMessageBox.Show("没有可以取消的数据!");
                return;
            }
            if (uiDataGridView5.CurrentRow.Index < 0)
            {
                UIMessageBox.Show("没有选择表格行!");
                return;
            }
            DialogResult YRO = MessageBox.Show("您确定要取消选中的记录吗？", "提醒您：", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
            if (YRO == DialogResult.No)
            {
                return;
            }
            string fentryid = uiDataGridView5.Rows[uiDataGridView5.CurrentRow.Index].Cells["明细ID"].Value.ToString();
            string fid = uiDataGridView5.Rows[uiDataGridView5.CurrentRow.Index].Cells["订单ID"].Value.ToString();
            string fOrderStatus = uiDataGridView5.Rows[uiDataGridView5.CurrentRow.Index].Cells["订单状态"].Value.ToString();
            string fWdtOrderStatus = uiDataGridView5.Rows[uiDataGridView5.CurrentRow.Index].Cells["旺店通状态"].Value.ToString();
            string shopName = uiDataGridView5.Rows[uiDataGridView5.CurrentRow.Index].Cells["店铺"].Value.ToString();
            string WDTSrcbillno = uiDataGridView5.Rows[uiDataGridView5.CurrentRow.Index].Cells["原始单号"].Value.ToString();
          
            string[] status=new string[]{ "已取消", "已发货", "已完成", "异常发货" };
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
            var obj = new { shopname = shopName, wdtsrcbillno = WDTSrcbillno, statusid =90};
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
            if (uiDataGridView5.Rows.Count < 1)
            {
                UIMessageBox.Show("没有可以更新的数据!");
                return;
            }
            if (uiDataGridView5.CurrentRow.Index < 0)
            {
                UIMessageBox.Show("没有选择表格行!");
                return;
            }
            string fOrderStatus = uiDataGridView5.Rows[uiDataGridView5.CurrentRow.Index].Cells["订单状态"].Value.ToString();
            if (fOrderStatus == "未审核")
            {
                UIMessageBox.Show("订单数据没有上传旺店通,无法刷新状态!");
                return;
            }
            string orderno = uiDataGridView5.Rows[uiDataGridView5.CurrentRow.Index].Cells["订单编号"].Value.ToString();
            var WDTSrcbillnos = JArray.FromObject(uiDataGridView5.DataSource).Where(a=>a["订单编号"].ToString()== orderno && a["原始单号"]!=null && a["原始单号"].ToString()!="").Select(a=>a["原始单号"].ToString()).Distinct().ToList();
          //  string WDTSrcbillno = uiDataGridView5.Rows[uiDataGridView5.CurrentRow.Index].Cells["原始单号"].Value.ToString();
            if(WDTSrcbillnos.Count<1)
            {
                UIMessageBox.Show("没有原始单号,系统无法更新状态!");
                return;
            }
            this.Invoke(new Action(() =>
            {
                uiButton14.Enabled=false;
                uiButton14.Text = "查询状态...";
            }));
            int uprec = 0;
            WDTSrcbillnos.ForEach(d=>
            {
                RestClient client = new RestClient("http://120.78.210.159:3304/api/Wdt/upWdtOrderStatus");
                RestRequest request = new RestRequest();
                request.AddHeader("Content-Type", "application/json");
                request.Method = Method.Post;
                RestResponse response;


                //  var str = JsonConvert.SerializeObject(obj);

                request.AddBody(d);
                response = client.Execute(request);
                var result = JObject.Parse(response.Content);
                if (result["code"].ToString() == "0" && result["mess"].ToString().Contains("更新成功"))
                {
                    uprec++;
                   
                }
               
            });
            if(uprec>0)
            {
                Task.Run(getOrderData);
                UIMessageBox.Show("总共更新"+ uprec.ToString()+"条原始订单记录!");
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

        private void uiButton16_Click(object sender, EventArgs e)
        {

            if (uiDataGridView5.Rows.Count < 1)
            {
                UIMessageBox.Show("没有可以同步的数据!");
                return;
            }
            if (uiDataGridView5.CurrentRow.Index < 0)
            {
                UIMessageBox.Show("没有选择表格行!");
                return;
            }
            uiPanel1.Visible = true;
          
        }

        private void uiButton17_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "Excel 文件(*.xls)| *.xls";
            saveFileDialog1.ShowDialog();
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
                       new {
                           仓库编号 = a["warehouse_no"],
                           仓库名称 =a["name"],
                           仓库地址 = a["address"],
                           仓库联系人 = a["contact"],
                           联系人手机 = a["mobile"]
                       }).ToList();
                this.Invoke(new Action(() =>
                {
                    uiDataGridView9.DataSource = warehouses;
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
                       new {
                           店铺编号 = a["shop_no"],
                           店铺名称 = a["shop_name"]
                         
                       }).ToList();
                this.Invoke(new Action(() =>
                {
                    uiDataGridView9.DataSource = shoplist;
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

                    uiDataGridView4.DataSource = JArray.Parse(result["rdataList"].ToString());
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
                    UIMessageBox.Show("模板复制成功！存放路径为:"+ tfilepath);
                }
            }
            catch(Exception er)
            {
                UIMessageBox.Show("出现错误:"+er.Message);
            }
        }

        private void uiButton20_Click(object sender, EventArgs e)
        {
            if (uiDataGridView5.Rows.Count < 1)
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
                var datas = JArray.FromObject(uiDataGridView5.DataSource);
                 
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
                            if(prop.Name!="订单ID" && prop.Name != "明细ID")
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
            uiPanel1.Visible = false;
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
            string billno = uiDataGridView5.Rows[uiDataGridView5.CurrentRow.Index].Cells["订单编号"].Value.ToString();
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
                    uiPanel1.Visible = false;
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
            var datas = JArray.FromObject(uiDataGridView5.DataSource).Select(a => new {
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
            var datas = JArray.FromObject(uiDataGridView5.DataSource).Select(a => new {
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
    }
}
