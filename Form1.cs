using SleckEc2KdInterface.Entity;
using SleckEc2KdInterface.DAL;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Windows.Forms;
using SleckWDTInterface.JOB;
using Newtonsoft.Json.Linq;

namespace SleckEc2KdInterface
{
    public partial class Form1 : Form
    {
        DbContext dbContext = new DbContext(false);

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
                    
          
            //var lst=dbContext.DeliverInfoDb.GetList(t => t.Platform_Code.Contains(textBox1.Text) || t.Code.Contains(textBox1.Text));      
            //dataGridView6.DataSource = lst;

           this.richTextBox1.Text = SaveOneAudioSaleOrder().ToString();

      //          var list = Db.Queryable<Student, School>((st, sc) => new object[] {
      //  JoinType.Left,st.SchoolId==sc.Id})
      //.Select((st, sc) => new { Id = st.Id, Name = st.Name, SchoolName = sc.Name }).ToList();
      //      var s11 = Db.Queryable<Student, School>((st, sc) => st.SchoolId == sc.Id)
      //                  .Select<ViewModelStudent3>().ToList();
        }

        //固定的json
        public JObject JsonHeader(bool IsAuto)
        {
            JObject jsonRoot = new JObject();
            // Creator: 创建用户
            jsonRoot.Add("Creator", "");
            // NeedUpDateFields: 哪些字段需要更新？为空则表示参数中全部字段，均需要更新
            jsonRoot.Add("NeedUpDateFields", new JArray(""));
            jsonRoot.Add("NeedReturnFields", new JArray("FWDTOrderNo"));
            jsonRoot.Add("IsDeleteEntry", "true");
            if (IsAuto)
            {
                jsonRoot.Add("IsAutoSubmitAndAudit", "true"); //是否提交审核
            }
            jsonRoot.Add("SubSystemId", "");
            jsonRoot.Add("IsVerifyBaseDataField", "false");
            jsonRoot.Add("IsEntryBatchFill", "true");
            jsonRoot.Add("ValidateFlag", "true");
            jsonRoot.Add("NumberSearch", "true");
            jsonRoot.Add("InterationFlags", "");

            return jsonRoot;


        }


        public JObject SaveOneAudioSaleOrder()
        {
            JObject jsonRoot = new JObject();
            string strSql = @"select shop_no,min(stock_check_time) check_time  from T_Sale_StockOutInfo where isnull(issynchro,0)<>1 and goods_total_amount<>0 and  shop_no='TM01' and convert(nvarchar(10),stock_check_time ,120)='2021-02-02'
            group by shop_no,convert(nvarchar(10),stock_check_time,120)";
            DataTable dsOrder = DBHelper.GetDataTable(strSql);
            foreach (DataRow item in dsOrder.Rows)
            {
                jsonRoot = JsonHeader(true);
                JArray modelRows = new JArray();
                jsonRoot.Add("Model", modelRows);
                // Model: 单据详细数据参数
                JObject model = new JObject();
                modelRows.Add(model);

                #region 表头.基本信息
                string custno = item["shop_no"].ToString();
                // custno = "DY01";
                switch (custno)
                {
                    case "YZ01": //有赞舒蕾商城
                        custno = ConfigurationManager.AppSettings["yzcustno"].ToString(); //有赞客户编码
                        break;
                    case "KS01": //快手舒蕾个护
                        custno = ConfigurationManager.AppSettings["kscustno"].ToString(); //快手客户编码
                        break;
                    case "DY01": //抖音舒美个护专营店
                        custno = ConfigurationManager.AppSettings["dycustno"].ToString(); //抖音客户编码
                        break;
                    default: //舒蕾洗护旗舰店 ,淘宝个人店
                        custno = ConfigurationManager.AppSettings["tmcustno"].ToString(); //天猫客户编码
                        break;
                }
                string shop_no = item["shop_no"].ToString();
                // 单据主键：必须填写，系统据此判断是新增还是修改单据；新增单据，填0
                model.Add("FID", 0);
                //单据类型：标准销售订单
                JObject basedata = new JObject();
                basedata.Add("FNumber", "XSDD01_SYS");
                model.Add("FBillTypeID", basedata);
                //销售日期（FDate）
                model.Add("FDate", item["check_time"].ToString());
                // 销售组织：舒美
                basedata = new JObject();
                basedata.Add("FNumber", ConfigurationManager.AppSettings["useorgno"].ToString());
                model.Add("FSaleOrgId", basedata);
                // 客户名称=抖音
                basedata = new JObject();
                basedata.Add("FNumber", custno);
                model.Add("FCustId", basedata);
                // 收货方
                basedata = new JObject();
                basedata.Add("FNumber", custno);
                model.Add("FReceiveId", basedata);
                // 收货地址
                //basedata = new JObject();
                //basedata.Add("FNumber", "ZD001");
                //model.Add("FHEADLOCID", basedata);
                // 销售部门=电商经营部
                basedata = new JObject();
                basedata.Add("FNumber", ConfigurationManager.AppSettings["saledeptno"].ToString());
                model.Add("FSaleDeptId", basedata);
                // 销售员
                basedata = new JObject();
                basedata.Add("FNumber", ConfigurationManager.AppSettings["salerno"].ToString());
                model.Add("FSalerId", basedata);
                //联系电话
                // model.Add("FLinkPhone", "13904331186");
                //收货方地址
                //model.Add("FReceiveAddress", "长春市二道区东环城路500号");
                //结算方
                basedata = new JObject();
                basedata.Add("FNumber", custno);
                model.Add("FSettleId", basedata);
                //供货联系人
                basedata = new JObject();
                basedata.Add("FName", ConfigurationManager.AppSettings["khcontact"].ToString());
                model.Add("FReceiveContact", basedata);
                //付款方
                basedata = new JObject();
                basedata.Add("FNumber", custno);
                model.Add("FChargeId", basedata);
                //是否期初单据
                model.Add("FISINIT", "false");
                //来自移动
                model.Add("FIsMobile", false);
                //渠道
                basedata = new JObject();
                basedata.Add("FNumber", "QD01");
                model.Add("F_FC_Assistant", basedata);
                //大区
                basedata = new JObject();
                basedata.Add("FNumber", "DQ05");
                model.Add("F_FC_Assistant1", basedata);
                //省份部门
                basedata = new JObject();
                basedata.Add("FNumber", "SXS29");
                model.Add("F_FC_Base", basedata);
                //旺店通同步标识=1
                model.Add("FWDTFlag", 1);
                //旺店通入库单号
                model.Add("FWDTOrderNo", (item["shop_no"].ToString() + ";" + Convert.ToDateTime(item["check_time"]).ToString("yyyy-MM-dd")));
                #endregion 

                #region 表头.财务信息
                JObject financedata = new JObject();
                model.Add("FSaleOrderFinance", financedata);
                //结算币别=默认人民币
                basedata = new JObject();
                basedata.Add("FNumber", "PRE001");
                financedata.Add("FSettleCurrId", basedata);
                //汇率类型=固定汇率
                basedata = new JObject();
                basedata.Add("FNumber", "HLTX01_SYS");
                financedata.Add("FExchangeTypeId", basedata);
                //汇率
                financedata.Add("FExchangeRate", 1);
                //是否含税
                financedata.Add("FIsIncludedTax", "true");
                //价外税
                financedata.Add("FISPRICEEXCLUDETAX", "true");
                //寄售生成跨组织调拨
                financedata.Add("FOverOrgTransDirect", false);
                #endregion

                #region 单据体.明细信息
                JArray entryRows = new JArray();
                // 把单据体行集合，添加到model中，以单据体Key为标识
                string entityKey = "FSaleOrderEntry";
                model.Add(entityKey, entryRows);
                strSql = string.Format(@"select goods_no,sum(num) num,sum(sell_price) sell_price ,gift_type from T_Sale_StockOutInfo a
inner join T_Sale_StockOutItem b on a.order_no=b.order_no
where isnull(issynchro,0)<>1 and a.shop_no='{0}' and convert(nvarchar(10),stock_check_time,120)='{1}' AND sell_price<>0
group by goods_no,gift_type;", item["shop_no"].ToString(), Convert.ToDateTime(item["check_time"]).ToString("yyyy-MM-dd"));
                // 通过循环创建单据体行：示例代码仅创建一行
                DataTable dsPurEntrty = DBHelper.GetDataTable(strSql);

                // 通过循环创建单据体行：示例代码仅创建一行
                foreach (DataRow curren in dsPurEntrty.Rows)
                {
                    // 添加新行，把新行加入到单据体行集合
                    JObject entryRow = new JObject();
                    entryRows.Add(entryRow);
                    //给新行，设置关键字段值
                    //单据体主键：必须填写，系统据此判断是新增还是修改行
                    entryRow.Add("FEntryID", 0);
                    //产品类型=标准产品
                    entryRow.Add("FRowType", "Standard");
                    //物料(FMaterialId)：基础资料，填写编码
                    basedata = new JObject();
                    basedata.Add("FNumber", curren["goods_no"].ToString());
                    entryRow.Add("FMaterialId", basedata);
                    // 数量(FQty)：数量字段
                    entryRow.Add("FQty", Convert.ToInt32(curren["num"]));
                    //单位(FUnitId)：基础资料，填写编码
                    basedata = new JObject();
                    basedata.Add("FNumber", ConfigurationManager.AppSettings["unitno"].ToString());
                    entryRow.Add("FUnitId", basedata);

                    // 计价数量：数量字段
                    entryRow.Add("FPriceUnitQty", Convert.ToInt32(curren["num"]));
                    //计价单位：基础资料，填写编码
                    basedata = new JObject();
                    basedata.Add("FNumber", ConfigurationManager.AppSettings["unitno"].ToString());
                    entryRow.Add("FPriceUnitId", basedata);
                    //计价基本数量：数量字段
                    entryRow.Add("FPriceBaseQty", Convert.ToInt32(curren["num"]));

                    //是否赠品  0非赠品 1自动赠送 2手工赠送
                    entryRow.Add("FIsFree", curren["gift_type"].ToString() == "0" ? "false" : "true");
                    //税率%
                    entryRow.Add("FEntryTaxRate", "13");
                    //保质期
                    entryRow.Add("FExpPeriod", 4);
                    //保质期单位
                    entryRow.Add("FExpUnit", "Y");
                    // 单价(FPrice)：单价字段
                    //entryRow.Add("FPrice", 2.946903);
                    //含税单价
                    entryRow.Add("FTaxPrice", (Convert.ToDecimal(curren["sell_price"])/ Convert.ToDecimal(curren["num"])).ToString("0.0000"));
                    //要货日期
                    entryRow.Add("FDeliveryDate", Convert.ToDateTime(item["check_time"]).ToString("yyyy-MM-dd hh:mm:ss"));

                    //库存组织 
                    basedata = new JObject();
                    basedata.Add("FNumber", ConfigurationManager.AppSettings["useorgno"].ToString());
                    entryRow.Add("FStockOrgId", basedata);
                    //结算组织
                    basedata = new JObject();
                    basedata.Add("FNumber", ConfigurationManager.AppSettings["useorgno"].ToString());
                    entryRow.Add("FEntrySettleOrgId", basedata);
                    //供应组织
                    basedata = new JObject();
                    basedata.Add("FNumber", ConfigurationManager.AppSettings["useorgno"].ToString());
                    entryRow.Add("FSupplyOrgId", basedata);
                    //货主类型=业务组织
                    entryRow.Add("FOwnerTypeId", "BD_OwnerOrg");
                    //货主
                    basedata = new JObject();
                    basedata.Add("FNumber", ConfigurationManager.AppSettings["useorgno"].ToString());
                    entryRow.Add("FOwnerId", basedata);
                    //预留类型=弱预留
                    entryRow.Add("FReserveType", "1");
                    // 计价基本数量：数量字段
                    //entryRow.Add("FPriceBaseQty", 240);
                    //库存单位
                    //basedata = new JObject();
                    //basedata.Add("FNumber", ConfigurationManager.AppSettings["unitno"].ToString());
                    //entryRow.Add("FStockUnitID", basedata);
                    ////库存数量
                    //entryRow.Add("FStockQty", "240");
                    ////库存基本数量
                    //entryRow.Add("FStockBaseQty", "240");
                    //超发控制单位
                    //basedata = new JObject();
                    //basedata.Add("FNumber", ConfigurationManager.AppSettings["unitno"].ToString());
                    //entryRow.Add("FOutLmtUnitID", basedata);
                    //已计划运算
                    entryRow.Add("FISMRP", false);

                    #region 子单据体.交货明细
                    JArray childentryRows = new JArray();
                    // 把单据体行集合，添加到model中，以单据体Key为标识
                    string childentityKey = "FOrderEntryPlan";
                    entryRow.Add(childentityKey, childentryRows);
                    // 通过循环创建单据体行：示例代码仅创建一行
                    for (int j = 0; j <= 0; j++)
                    {
                        // 添加新行，把新行加入到子单据体行集合
                        JObject cdentryRow = new JObject();
                        ////交货地点
                        //basedata = new JObject();
                        //basedata.Add("FNumber", "ZD001");
                        //cdentryRow.Add("FDetailLocId", basedata);

                        //cdentryRow.Add("FDetailLocAddress", "长春市二道区东环城路500号"); //交货地址
                        //销售单位
                        basedata = new JObject();
                        basedata.Add("FNumber", ConfigurationManager.AppSettings["unitno"].ToString());
                        cdentryRow.Add("FPlanUnitID", basedata);
                        cdentryRow.Add("FPlanDate", Convert.ToDateTime(item["check_time"]).ToString("yyyy-MM-dd hh:mm:ss")); //要货日期
                        //数量
                        cdentryRow.Add("FPlanQty", Convert.ToInt32(curren["num"]));
                        childentryRows.Add(cdentryRow);
                    }
                    #endregion
                }
                #endregion


               // ExecClient(jsonRoot, "SAL_SaleOrder");

            }


            return jsonRoot;
        }



        private void dataGridView6_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1)
            {

              string fid=dataGridView6.Rows[e.RowIndex].Cells[0].Value.ToString();
              var itemlst = dbContext.DeliverItemDb.GetList(t => t.FID== fid);
              dataGridView1.DataSource = itemlst;

            }
        }
    }

}
