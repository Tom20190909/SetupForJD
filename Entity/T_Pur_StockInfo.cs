using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace SleckEc2KdInterface.Entity
{
    [SugarTable("T_Pur_StockInfo")]
    public class PurStockInfo
    {


        string _order_no = string.Empty;
        /// <summary>
        /// 注释自行添加
        /// </summary>
        public string order_no
        {
            get { return _order_no; }
            set { _order_no = value; }
        }


        string _warehouse_no = string.Empty;
        /// <summary>
        /// 注释自行添加
        /// </summary>
        public string Warehouse_No
        {
            get { return _warehouse_no; }
            set { _warehouse_no = value; }
        }


        public string _check_time;
        /// <summary>
        /// 注释自行添加
        /// </summary>
        public string Check_Time
        {
            get { return _check_time; }
            set { _check_time = value; }
        }


        string _src_order_no = string.Empty;
        /// <summary>
        /// 注释自行添加
        /// </summary>
        public string Src_Order_No
        {
            get { return _src_order_no; }
            set { _src_order_no = value; }
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


        string _stockin_reason = string.Empty;
        /// <summary>
        /// 注释自行添加
        /// </summary>
        public string Stockin_Reason
        {
            get { return _stockin_reason; }
            set { _stockin_reason = value; }
        }


        string _order_type = string.Empty;
        /// <summary>
        /// 注释自行添加
        /// </summary>
        public string Order_Type
        {
            get { return _order_type; }
            set { _order_type = value; }
        }


        string _order_type_name = string.Empty;
        /// <summary>
        /// 注释自行添加
        /// </summary>
        public string Order_Type_Name
        {
            get { return _order_type_name; }
            set { _order_type_name = value; }
        }


        string _goods_amount = string.Empty;
        /// <summary>
        /// 注释自行添加
        /// </summary>
        public string Goods_Amount
        {
            get { return _goods_amount; }
            set { _goods_amount = value; }
        }


        string _total_price = string.Empty;
        /// <summary>
        /// 注释自行添加
        /// </summary>
        public string Total_Price
        {
            get { return _total_price; }
            set { _total_price = value; }
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


        string _tax_amount = string.Empty;
        /// <summary>
        /// 注释自行添加
        /// </summary>
        public string Tax_Amount
        {
            get { return _tax_amount; }
            set { _tax_amount = value; }
        }


        string _post_fee = string.Empty;
        /// <summary>
        /// 注释自行添加
        /// </summary>
        public string Post_Fee
        {
            get { return _post_fee; }
            set { _post_fee = value; }
        }


        string _other_fee = string.Empty;
        /// <summary>
        /// 注释自行添加
        /// </summary>
        public string Other_Fee
        {
            get { return _other_fee; }
            set { _other_fee = value; }
        }


        string _logistics_code = string.Empty;
        /// <summary>
        /// 注释自行添加
        /// </summary>
        public string Logistics_Code
        {
            get { return _logistics_code; }
            set { _logistics_code = value; }
        }


        string _logistics_name = string.Empty;
        /// <summary>
        /// 注释自行添加
        /// </summary>
        public string Logistics_Name
        {
            get { return _logistics_name; }
            set { _logistics_name = value; }
        }


        string _logistics_no = string.Empty;
        /// <summary>
        /// 注释自行添加
        /// </summary>
        public string Logistics_No
        {
            get { return _logistics_no; }
            set { _logistics_no = value; }
        }


        string _purchase_no = string.Empty;
        /// <summary>
        /// 注释自行添加
        /// </summary>
        public string Purchase_No
        {
            get { return _purchase_no; }
            set { _purchase_no = value; }
        }


        string _provider_no = string.Empty;
        /// <summary>
        /// 注释自行添加
        /// </summary>
        public string Provider_No
        {
            get { return _provider_no; }
            set { _provider_no = value; }
        }


        string _outer_no = string.Empty;
        /// <summary>
        /// 注释自行添加
        /// </summary>
        public string Outer_No
        {
            get { return _outer_no; }
            set { _outer_no = value; }
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


        string _right_nums = string.Empty;
        /// <summary>
        /// 注释自行添加
        /// </summary>
        public string Right_Nums
        {
            get { return _right_nums; }
            set { _right_nums = value; }
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


        string _stockin_no = string.Empty;
        /// <summary>
        /// 注释自行添加
        /// </summary>
        public string Stockin_No
        {
            get { return _stockin_no; }
            set { _stockin_no = value; }
        }


        string _wms_outer_no = string.Empty;
        /// <summary>
        /// 注释自行添加
        /// </summary>
        public string Wms_Outer_No
        {
            get { return _wms_outer_no; }
            set { _wms_outer_no = value; }
        }


        string _warehouse_id = string.Empty;
        /// <summary>
        /// 注释自行添加
        /// </summary>
        public string Warehouse_Id
        {
            get { return _warehouse_id; }
            set { _warehouse_id = value; }
        }


        string _src_order_id = string.Empty;
        /// <summary>
        /// 注释自行添加
        /// </summary>
        public string Src_Order_Id
        {
            get { return _src_order_id; }
            set { _src_order_id = value; }
        }


        string _reason_id = string.Empty;
        /// <summary>
        /// 注释自行添加
        /// </summary>
        public string Reason_Id
        {
            get { return _reason_id; }
            set { _reason_id = value; }
        }


        string _logistics_id = string.Empty;
        /// <summary>
        /// 注释自行添加
        /// </summary>
        public string Logistics_Id
        {
            get { return _logistics_id; }
            set { _logistics_id = value; }
        }


        string _post_share_type = string.Empty;
        /// <summary>
        /// 注释自行添加
        /// </summary>
        public string Post_Share_Type
        {
            get { return _post_share_type; }
            set { _post_share_type = value; }
        }


        string _goods_type_count = string.Empty;
        /// <summary>
        /// 注释自行添加
        /// </summary>
        public string Goods_Type_Count
        {
            get { return _goods_type_count; }
            set { _goods_type_count = value; }
        }


        string _note_count = string.Empty;
        /// <summary>
        /// 注释自行添加
        /// </summary>
        public string Note_Count
        {
            get { return _note_count; }
            set { _note_count = value; }
        }

    }
}
