using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace SleckEc2KdInterface.Entity 
 { 
 public class T_Sale_StockOutInfo
{
string  _order_no=string.Empty;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string Order_No 
 {  get { return _order_no;}
  set { _order_no = value; } 
 } 


string  _src_order_no=string.Empty;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string Src_Order_No 
 {  get { return _src_order_no;}
  set { _src_order_no = value; } 
 } 


string  _warehouse_no=string.Empty;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string Warehouse_No 
 {  get { return _warehouse_no;}
  set { _warehouse_no = value; } 
 } 


DateTime  _consign_time=DateTime.Now;
 /// <summary>
/// 注释自行添加
/// </summary>
 public DateTime Consign_Time 
 {  get { return _consign_time;}
  set { _consign_time = value; } 
 } 


string  _trade_type=string.Empty;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string Trade_Type 
 {  get { return _trade_type;}
  set { _trade_type = value; } 
 } 


string  _subtype=string.Empty;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string Subtype 
 {  get { return _subtype;}
  set { _subtype = value; } 
 } 


string  _goods_count=string.Empty;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string Goods_Count 
 {  get { return _goods_count;}
  set { _goods_count = value; } 
 }


 string  _goods_total_amount=string.Empty;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string Goods_Total_Amount 
 {  get { return _goods_total_amount;}
  set { _goods_total_amount = value; } 
 } 


string  _goods_total_cost=string.Empty;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string Goods_Total_Cost 
 {  get { return _goods_total_cost;}
  set { _goods_total_cost = value; } 
 } 


string  _post_fee=string.Empty;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string Post_Fee 
 {  get { return _post_fee;}
  set { _post_fee = value; } 
 } 


string  _logistics_no=string.Empty;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string Logistics_No 
 {  get { return _logistics_no;}
  set { _logistics_no = value; } 
 } 


string  _package_fee=string.Empty;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string Package_Fee 
 {  get { return _package_fee;}
  set { _package_fee = value; } 
 } 


string  _receiver_name=string.Empty;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string Receiver_Name 
 {  get { return _receiver_name;}
  set { _receiver_name = value; } 
 } 


string  _receiver_country=string.Empty;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string Receiver_Country 
 {  get { return _receiver_country;}
  set { _receiver_country = value; } 
 } 


string  _receiver_province=string.Empty;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string Receiver_Province 
 {  get { return _receiver_province;}
  set { _receiver_province = value; } 
 } 


string  _receiver_city=string.Empty;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string Receiver_City 
 {  get { return _receiver_city;}
  set { _receiver_city = value; } 
 } 


string  _receiver_district=string.Empty;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string Receiver_District 
 {  get { return _receiver_district;}
  set { _receiver_district = value; } 
 } 


string  _receiver_address=string.Empty;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string Receiver_Address 
 {  get { return _receiver_address;}
  set { _receiver_address = value; } 
 } 


string  _receiver_mobile=string.Empty;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string Receiver_Mobile 
 {  get { return _receiver_mobile;}
  set { _receiver_mobile = value; } 
 } 


string  _receiver_telno=string.Empty;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string Receiver_Telno 
 {  get { return _receiver_telno;}
  set { _receiver_telno = value; } 
 } 


string  _receiver_zip=string.Empty;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string Receiver_Zip 
 {  get { return _receiver_zip;}
  set { _receiver_zip = value; } 
 } 


string  _weight=string.Empty;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string Weight 
 {  get { return _weight;}
  set { _weight = value; } 
 } 


string  _logistics_code=string.Empty;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string Logistics_Code 
 {  get { return _logistics_code;}
  set { _logistics_code = value; } 
 } 


string  _print_remark=string.Empty;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string Print_Remark 
 {  get { return _print_remark;}
  set { _print_remark = value; } 
 } 


string  _paid=string.Empty;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string Paid 
 {  get { return _paid;}
  set { _paid = value; } 
 } 


string  _refund_status=string.Empty;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string Refund_Status 
 {  get { return _refund_status;}
  set { _refund_status = value; } 
 } 


string  _salesman_no=string.Empty;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string Salesman_No 
 {  get { return _salesman_no;}
  set { _salesman_no = value; } 
 } 


string  _remark=string.Empty;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string Remark 
 {  get { return _remark;}
  set { _remark = value; } 
 } 


string  _stockout_reason=string.Empty;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string Stockout_Reason 
 {  get { return _stockout_reason;}
  set { _stockout_reason = value; } 
 } 


string  _outer_no=string.Empty;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string Outer_No 
 {  get { return _outer_no;}
  set { _outer_no = value; } 
 } 


string  _trade_no=string.Empty;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string Trade_No 
 {  get { return _trade_no;}
  set { _trade_no = value; } 
 } 


string  _src_trade_no=string.Empty;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string Src_Trade_No 
 {  get { return _src_trade_no;}
  set { _src_trade_no = value; } 
 } 


string  _customer_no=string.Empty;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string Customer_No 
 {  get { return _customer_no;}
  set { _customer_no = value; } 
 } 


string  _trade_time;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string Trade_Time 
 {  get { return _trade_time;}
  set { _trade_time = value; } 
 } 


string  _pay_time;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string Pay_Time 
 {  get { return _pay_time;}
  set { _pay_time = value; } 
 } 


string  _shop_no=string.Empty;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string Shop_No 
 {  get { return _shop_no;}
  set { _shop_no = value; } 
 } 


string  _buyer_message=string.Empty;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string Buyer_Message 
 {  get { return _buyer_message;}
  set { _buyer_message = value; } 
 } 


string  _cs_remark=string.Empty;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string Cs_Remark 
 {  get { return _cs_remark;}
  set { _cs_remark = value; } 
 } 


string  _flag_name=string.Empty;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string Flag_Name 
 {  get { return _flag_name;}
  set { _flag_name = value; } 
 } 


string  _post_amount=string.Empty;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string Post_Amount 
 {  get { return _post_amount;}
  set { _post_amount = value; } 
 } 


string  _block_reason=string.Empty;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string Block_Reason 
 {  get { return _block_reason;}
  set { _block_reason = value; } 
 } 


string  _invoice_type=string.Empty;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string Invoice_Type 
 {  get { return _invoice_type;}
  set { _invoice_type = value; } 
 } 


string  _invoice_title=string.Empty;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string Invoice_Title 
 {  get { return _invoice_title;}
  set { _invoice_title = value; } 
 } 


string  _invoice_content=string.Empty;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string Invoice_Content 
 {  get { return _invoice_content;}
  set { _invoice_content = value; } 
 } 


string  _invoice_id=string.Empty;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string Invoice_Id 
 {  get { return _invoice_id;}
  set { _invoice_id = value; } 
 } 


string  _delivery_term=string.Empty;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string Delivery_Term 
 {  get { return _delivery_term;}
  set { _delivery_term = value; } 
 } 


string  _fenxiao_type=string.Empty;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string Fenxiao_Type 
 {  get { return _fenxiao_type;}
  set { _fenxiao_type = value; } 
 } 


string  _cod_amount=string.Empty;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string Cod_Amount 
 {  get { return _cod_amount;}
  set { _cod_amount = value; } 
 } 


string  _id_card_type=string.Empty;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string Id_Card_Type 
 {  get { return _id_card_type;}
  set { _id_card_type = value; } 
 } 


string  _receiver_area=string.Empty;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string Receiver_Area 
 {  get { return _receiver_area;}
  set { _receiver_area = value; } 
 } 


string  _shop_remark=string.Empty;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string Shop_Remark 
 {  get { return _shop_remark;}
  set { _shop_remark = value; } 
 } 


string  _platform_id=string.Empty;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string Platform_Id 
 {  get { return _platform_id;}
  set { _platform_id = value; } 
 } 


string  _trade_id=string.Empty;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string Trade_Id 
 {  get { return _trade_id;}
  set { _trade_id = value; } 
 } 


string  _checker_name=string.Empty;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string Checker_Name 
 {  get { return _checker_name;}
  set { _checker_name = value; } 
 } 


string  _operator_name=string.Empty;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string Operator_Name 
 {  get { return _operator_name;}
  set { _operator_name = value; } 
 } 


string  _stockout_no=string.Empty;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string Stockout_No 
 {  get { return _stockout_no;}
  set { _stockout_no = value; } 
 } 


string  _wms_status=string.Empty;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string Wms_Status 
 {  get { return _wms_status;}
  set { _wms_status = value; } 
 } 


string  _warehouse_type=string.Empty;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string Warehouse_Type 
 {  get { return _warehouse_type;}
  set { _warehouse_type = value; } 
 } 


string  _goods_type_count=string.Empty;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string Goods_Type_Count 
 {  get { return _goods_type_count;}
  set { _goods_type_count = value; } 
 } 


string  _raw_goods_count=string.Empty;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string Raw_Goods_Count 
 {  get { return _raw_goods_count;}
  set { _raw_goods_count = value; } 
 } 


string  _receiver_ring=string.Empty;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string Receiver_Ring 
 {  get { return _receiver_ring;}
  set { _receiver_ring = value; } 
 } 


string  _to_deliver_time=string.Empty;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string To_Deliver_Time 
 {  get { return _to_deliver_time;}
  set { _to_deliver_time = value; } 
 } 


string  _pre_charge_time=string.Empty;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string Pre_Charge_Time 
 {  get { return _pre_charge_time;}
  set { _pre_charge_time = value; } 
 } 


string  _logistics_id=string.Empty;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string Logistics_Id 
 {  get { return _logistics_id;}
  set { _logistics_id = value; } 
 } 


string  _unknown_goods_amount=string.Empty;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string Unknown_Goods_Amount 
 {  get { return _unknown_goods_amount;}
  set { _unknown_goods_amount = value; } 
 } 


string  _calc_post_cost=string.Empty;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string Calc_Post_Cost 
 {  get { return _calc_post_cost;}
  set { _calc_post_cost = value; } 
 } 


string  _post_cost=string.Empty;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string Post_Cost 
 {  get { return _post_cost;}
  set { _post_cost = value; } 
 } 


string  _calc_weight=string.Empty;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string Calc_Weight 
 {  get { return _calc_weight;}
  set { _calc_weight = value; } 
 } 


string  _post_weight=string.Empty;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string Post_Weight 
 {  get { return _post_weight;}
  set { _post_weight = value; } 
 } 


string  _package_cost=string.Empty;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string Package_Cost 
 {  get { return _package_cost;}
  set { _package_cost = value; } 
 } 


string  _has_invoice=string.Empty;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string Has_Invoice 
 {  get { return _has_invoice;}
  set { _has_invoice = value; } 
 } 


string  _picklist_no=string.Empty;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string Picklist_No 
 {  get { return _picklist_no;}
  set { _picklist_no = value; } 
 } 


string  _flag_id=string.Empty;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string Flag_Id 
 {  get { return _flag_id;}
  set { _flag_id = value; } 
 } 


string  _pos_allocate_mode=string.Empty;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string Pos_Allocate_Mode 
 {  get { return _pos_allocate_mode;}
  set { _pos_allocate_mode = value; } 
 } 


string  _note_count=string.Empty;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string Note_Count 
 {  get { return _note_count;}
  set { _note_count = value; } 
 } 


string  _reason_id=string.Empty;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string Reason_Id 
 {  get { return _reason_id;}
  set { _reason_id = value; } 
 } 


string  _reserve=string.Empty;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string Reserve 
 {  get { return _reserve;}
  set { _reserve = value; } 
 } 


string  _discount=string.Empty;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string Discount 
 {  get { return _discount;}
  set { _discount = value; } 
 } 


string  _src_tids=string.Empty;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string Src_Tids 
 {  get { return _src_tids;}
  set { _src_tids = value; } 
 } 


string  _tax=string.Empty;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string Tax 
 {  get { return _tax;}
  set { _tax = value; } 
 } 


string  _tax_rate=string.Empty;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string Tax_Rate 
 {  get { return _tax_rate;}
  set { _tax_rate = value; } 
 } 


string  _currency=string.Empty;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string Currency 
 {  get { return _currency;}
  set { _currency = value; } 
 } 


string  _id_card=string.Empty;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string Id_Card 
 {  get { return _id_card;}
  set { _id_card = value; } 
 } 


DateTime  _stock_check_time=DateTime.Now;
 /// <summary>
/// 注释自行添加
/// </summary>
 public DateTime Stock_Check_Time 
 {  get { return _stock_check_time;}
  set { _stock_check_time = value; } 
 } 

} 
}
