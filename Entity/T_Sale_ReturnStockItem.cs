using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace SleckEc2KdInterface.Entity
{
    public class T_Sale_ReturnStockItem
    {


        /// <summary>
        /// 注释自行添加
        /// </summary>

        public string Order_No
        {
            get;
            set;
        }


        string _src_order_detail_id = string.Empty;
        /// <summary>
        /// 注释自行添加
        /// </summary>
        public string Src_Order_Detail_Id
        {
            get { return _src_order_detail_id; }
            set { _src_order_detail_id = value; }
        }


        string _spec_no = string.Empty;
        /// <summary>
        /// 注释自行添加
        /// </summary>
        public string Spec_No
        {
            get { return _spec_no; }
            set { _spec_no = value; }
        }


        string _goods_count = string.Empty;
        /// <summary>
        /// 注释自行添加
        /// </summary>
        public string Goods_Count
        {
            get { return _goods_count; }
            set { _goods_count = value; }
        }


        string _discount = string.Empty;
        /// <summary>
        /// 注释自行添加
        /// </summary>
        public string Discount
        {
            get { return _discount; }
            set { _discount = value; }
        }


        string _cost_price = string.Empty;
        /// <summary>
        /// 注释自行添加
        /// </summary>
        public string Cost_Price
        {
            get { return _cost_price; }
            set { _cost_price = value; }
        }


        DateTime _production_date = DateTime.Now;
        /// <summary>
        /// 注释自行添加
        /// </summary>
        public DateTime Production_Date
        {
            get { return _production_date; }
            set { _production_date = value; }
        }


        string _validity_days = string.Empty;
        /// <summary>
        /// 注释自行添加
        /// </summary>
        public string Validity_Days
        {
            get { return _validity_days; }
            set { _validity_days = value; }
        }


        DateTime _expire_date = DateTime.Now;
        /// <summary>
        /// 注释自行添加
        /// </summary>
        public DateTime Expire_Date
        {
            get { return _expire_date; }
            set { _expire_date = value; }
        }


        string _src_price = string.Empty;
        /// <summary>
        /// 注释自行添加
        /// </summary>
        public string Src_Price
        {
            get { return _src_price; }
            set { _src_price = value; }
        }


        string _tax_price = string.Empty;
        /// <summary>
        /// 注释自行添加
        /// </summary>
        public string Tax_Price
        {
            get { return _tax_price; }
            set { _tax_price = value; }
        }


        string _tax_amount = string.Empty;
        /// <summary>
        /// 注释自行添加
        /// </summary>
        public string Tax_Amount
        {
            get { return _tax_amount; }
            set { _tax_amount = value; }
        }


        string _tax = string.Empty;
        /// <summary>
        /// 注释自行添加
        /// </summary>
        public string Tax
        {
            get { return _tax; }
            set { _tax = value; }
        }


        string _total_cost = string.Empty;
        /// <summary>
        /// 注释自行添加
        /// </summary>
        public string Total_Cost
        {
            get { return _total_cost; }
            set { _total_cost = value; }
        }


        string _remark = string.Empty;
        /// <summary>
        /// 注释自行添加
        /// </summary>
        public string Remark
        {
            get { return _remark; }
            set { _remark = value; }
        }


        string _adjust_num = string.Empty;
        /// <summary>
        /// 注释自行添加
        /// </summary>
        public string Adjust_Num
        {
            get { return _adjust_num; }
            set { _adjust_num = value; }
        }


        string _adjust_price = string.Empty;
        /// <summary>
        /// 注释自行添加
        /// </summary>
        public string Adjust_Price
        {
            get { return _adjust_price; }
            set { _adjust_price = value; }
        }


        string _right_num = string.Empty;
        /// <summary>
        /// 注释自行添加
        /// </summary>
        public string Right_Num
        {
            get { return _right_num; }
            set { _right_num = value; }
        }


        string _right_price = string.Empty;
        /// <summary>
        /// 注释自行添加
        /// </summary>
        public string Right_Price
        {
            get { return _right_price; }
            set { _right_price = value; }
        }


        string _num = string.Empty;
        /// <summary>
        /// 注释自行添加
        /// </summary>
        public string Num
        {
            get { return _num; }
            set { _num = value; }
        }


        string _num2 = string.Empty;
        /// <summary>
        /// 注释自行添加
        /// </summary>
        public string Num2
        {
            get { return _num2; }
            set { _num2 = value; }
        }


        string _right_cost = string.Empty;
        /// <summary>
        /// 注释自行添加
        /// </summary>
        public string Right_Cost
        {
            get { return _right_cost; }
            set { _right_cost = value; }
        }


        string _brand_no = string.Empty;
        /// <summary>
        /// 注释自行添加
        /// </summary>
        public string Brand_No
        {
            get { return _brand_no; }
            set { _brand_no = value; }
        }


        string _goods_no = string.Empty;
        /// <summary>
        /// 注释自行添加
        /// </summary>
        public string Goods_No
        {
            get { return _goods_no; }
            set { _goods_no = value; }
        }


        string _spec_name = string.Empty;
        /// <summary>
        /// 注释自行添加
        /// </summary>
        public string Spec_Name
        {
            get { return _spec_name; }
            set { _spec_name = value; }
        }


        string _spec_code = string.Empty;
        /// <summary>
        /// 注释自行添加
        /// </summary>
        public string Spec_Code
        {
            get { return _spec_code; }
            set { _spec_code = value; }
        }


        string _goods_unit = string.Empty;
        /// <summary>
        /// 注释自行添加
        /// </summary>
        public string Goods_Unit
        {
            get { return _goods_unit; }
            set { _goods_unit = value; }
        }


        string _batch_no = string.Empty;
        /// <summary>
        /// 注释自行添加
        /// </summary>
        public string Batch_No
        {
            get { return _batch_no; }
            set { _batch_no = value; }
        }


        string _batch_remark = string.Empty;
        /// <summary>
        /// 注释自行添加
        /// </summary>
        public string Batch_Remark
        {
            get { return _batch_remark; }
            set { _batch_remark = value; }
        }


        string _position_no = string.Empty;
        /// <summary>
        /// 注释自行添加
        /// </summary>
        public string Position_No
        {
            get { return _position_no; }
            set { _position_no = value; }
        }


        string _tid = string.Empty;
        /// <summary>
        /// 注释自行添加
        /// </summary>
        public string Tid
        {
            get { return _tid; }
            set { _tid = value; }
        }


        string _oid = string.Empty;
        /// <summary>
        /// 注释自行添加
        /// </summary>
        public string Oid
        {
            get { return _oid; }
            set { _oid = value; }
        }


        string _src_order_type = string.Empty;
        /// <summary>
        /// 注释自行添加
        /// </summary>
        public string Src_Order_Type
        {
            get { return _src_order_type; }
            set { _src_order_type = value; }
        }


        string _org_stockin_detail_id = string.Empty;
        /// <summary>
        /// 注释自行添加
        /// </summary>
        public string Org_Stockin_Detail_Id
        {
            get { return _org_stockin_detail_id; }
            set { _org_stockin_detail_id = value; }
        }


        string _spec_id = string.Empty;
        /// <summary>
        /// 注释自行添加
        /// </summary>
        public string Spec_Id
        {
            get { return _spec_id; }
            set { _spec_id = value; }
        }


        string _batch_id = string.Empty;
        /// <summary>
        /// 注释自行添加
        /// </summary>
        public string Batch_Id
        {
            get { return _batch_id; }
            set { _batch_id = value; }
        }


        string _position_id = string.Empty;
        /// <summary>
        /// 注释自行添加
        /// </summary>
        public string Position_Id
        {
            get { return _position_id; }
            set { _position_id = value; }
        }


        string _unit_id = string.Empty;
        /// <summary>
        /// 注释自行添加
        /// </summary>
        public string Unit_Id
        {
            get { return _unit_id; }
            set { _unit_id = value; }
        }


        string _base_unit_id = string.Empty;
        /// <summary>
        /// 注释自行添加
        /// </summary>
        public string Base_Unit_Id
        {
            get { return _base_unit_id; }
            set { _base_unit_id = value; }
        }


        string _unit_ratio = string.Empty;
        /// <summary>
        /// 注释自行添加
        /// </summary>
        public string Unit_Ratio
        {
            get { return _unit_ratio; }
            set { _unit_ratio = value; }
        }


        string _expect_num = string.Empty;
        /// <summary>
        /// 注释自行添加
        /// </summary>
        public string Expect_Num
        {
            get { return _expect_num; }
            set { _expect_num = value; }
        }


        string _cost_price2 = string.Empty;
        /// <summary>
        /// 注释自行添加
        /// </summary>
        public string Cost_Price2
        {
            get { return _cost_price2; }
            set { _cost_price2 = value; }
        }


        string _share_post_cost = string.Empty;
        /// <summary>
        /// 注释自行添加
        /// </summary>
        public string Share_Post_Cost
        {
            get { return _share_post_cost; }
            set { _share_post_cost = value; }
        }


        string _share_post_total = string.Empty;
        /// <summary>
        /// 注释自行添加
        /// </summary>
        public string Share_Post_Total
        {
            get { return _share_post_total; }
            set { _share_post_total = value; }
        }

    }
}
