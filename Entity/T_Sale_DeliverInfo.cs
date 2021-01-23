using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
namespace SleckEc2KdInterface.Entity 
 { 
 public class T_Sale_DeliverInfo
{
string  _FID=string.Empty;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string FID 
 {  get { return _FID;}
  set { _FID = value; } 
 } 


string  _code=string.Empty;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string Code 
 {  get { return _code;}
  set { _code = value; } 
 } 


string  _platform_code=string.Empty;
        /// <summary>
        /// 注释自行添加
        /// </summary>
    [Display(Name = "平台交易号")]
        public string Platform_Code 
 {  get; set; } 


DateTime  _create_date=DateTime.Now;
 /// <summary>
/// 注释自行添加
/// </summary>
 public DateTime Create_Date 
 {  get { return _create_date;}
  set { _create_date = value; } 
 } 


string  _qty=string.Empty;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string Qty 
 {  get { return _qty;}
  set { _qty = value; } 
 } 


string  _amount=string.Empty;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string Amount 
 {  get { return _amount;}
  set { _amount = value; } 
 } 


string  _payment=string.Empty;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string Payment 
 {  get { return _payment;}
  set { _payment = value; } 
 } 


DateTime  _pay_time=DateTime.Now;
 /// <summary>
/// 注释自行添加
/// </summary>
 public DateTime Pay_Time 
 {  get { return _pay_time;}
  set { _pay_time = value; } 
 } 


string  _cod=string.Empty;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string Cod 
 {  get { return _cod;}
  set { _cod = value; } 
 } 


string  _refund=string.Empty;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string Refund 
 {  get { return _refund;}
  set { _refund = value; } 
 } 


DateTime  _invoiceDate=DateTime.Now;
 /// <summary>
/// 注释自行添加
/// </summary>
 public DateTime Invoicedate 
 {  get { return _invoiceDate;}
  set { _invoiceDate = value; } 
 } 


string  _unpaid_amount=string.Empty;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string Unpaid_Amount 
 {  get { return _unpaid_amount;}
  set { _unpaid_amount = value; } 
 } 


string  _post_fee=string.Empty;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string Post_Fee 
 {  get { return _post_fee;}
  set { _post_fee = value; } 
 } 


string  _cod_fee=string.Empty;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string Cod_Fee 
 {  get { return _cod_fee;}
  set { _cod_fee = value; } 
 } 


string  _discount_fee=string.Empty;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string Discount_Fee 
 {  get { return _discount_fee;}
  set { _discount_fee = value; } 
 } 


string  _post_cost=string.Empty;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string Post_Cost 
 {  get { return _post_cost;}
  set { _post_cost = value; } 
 } 


DateTime  _plan_delivery_date=DateTime.Now;
 /// <summary>
/// 注释自行添加
/// </summary>
 public DateTime Plan_Delivery_Date 
 {  get { return _plan_delivery_date;}
  set { _plan_delivery_date = value; } 
 } 


string  _buyer_memo=string.Empty;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string Buyer_Memo 
 {  get { return _buyer_memo;}
  set { _buyer_memo = value; } 
 } 


string  _seller_memo=string.Empty;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string Seller_Memo 
 {  get { return _seller_memo;}
  set { _seller_memo = value; } 
 } 


string  _receiver_name=string.Empty;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string Receiver_Name 
 {  get { return _receiver_name;}
  set { _receiver_name = value; } 
 } 


string  _receiver_phone=string.Empty;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string Receiver_Phone 
 {  get { return _receiver_phone;}
  set { _receiver_phone = value; } 
 } 


string  _receiver_mobile=string.Empty;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string Receiver_Mobile 
 {  get { return _receiver_mobile;}
  set { _receiver_mobile = value; } 
 } 


string  _receiver_zip=string.Empty;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string Receiver_Zip 
 {  get { return _receiver_zip;}
  set { _receiver_zip = value; } 
 } 


string  _receiver_address=string.Empty;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string Receiver_Address 
 {  get { return _receiver_address;}
  set { _receiver_address = value; } 
 } 


string  _create_name=string.Empty;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string Create_Name 
 {  get { return _create_name;}
  set { _create_name = value; } 
 } 


string  _express_no=string.Empty;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string Express_No 
 {  get { return _express_no;}
  set { _express_no = value; } 
 } 


string  _vip_name=string.Empty;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string Vip_Name 
 {  get { return _vip_name;}
  set { _vip_name = value; } 
 } 


string  _shop_name=string.Empty;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string Shop_Name 
 {  get { return _shop_name;}
  set { _shop_name = value; } 
 } 


string  _area_name=string.Empty;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string Area_Name 
 {  get { return _area_name;}
  set { _area_name = value; } 
 } 


string  _warehouse_name=string.Empty;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string Warehouse_Name 
 {  get { return _warehouse_name;}
  set { _warehouse_name = value; } 
 } 


string  _express_code=string.Empty;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string Express_Code 
 {  get { return _express_code;}
  set { _express_code = value; } 
 } 


string  _express_name=string.Empty;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string Express_Name 
 {  get { return _express_name;}
  set { _express_name = value; } 
 } 


string  _tag_name=string.Empty;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string Tag_Name 
 {  get { return _tag_name;}
  set { _tag_name = value; } 
 } 


string  _seller_memo_late=string.Empty;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string Seller_Memo_Late 
 {  get { return _seller_memo_late;}
  set { _seller_memo_late = value; } 
 } 


string  _shelf_no=string.Empty;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string Shelf_No 
 {  get { return _shelf_no;}
  set { _shelf_no = value; } 
 } 


string  _vip_code=string.Empty;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string Vip_Code 
 {  get { return _vip_code;}
  set { _vip_code = value; } 
 } 


string  _warehouse_code=string.Empty;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string Warehouse_Code 
 {  get { return _warehouse_code;}
  set { _warehouse_code = value; } 
 } 


string  _shop_code=string.Empty;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string Shop_Code 
 {  get { return _shop_code;}
  set { _shop_code = value; } 
 } 


string  _vip_real_name=string.Empty;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string Vip_Real_Name 
 {  get { return _vip_real_name;}
  set { _vip_real_name = value; } 
 } 


string  _vip_id_card=string.Empty;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string Vip_Id_Card 
 {  get { return _vip_id_card;}
  set { _vip_id_card = value; } 
 } 


string  _sync_memo=string.Empty;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string Sync_Memo 
 {  get { return _sync_memo;}
  set { _sync_memo = value; } 
 } 


string  _picture_bill=string.Empty;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string Picture_Bill 
 {  get { return _picture_bill;}
  set { _picture_bill = value; } 
 } 


string  _delivery_name=string.Empty;
 /// <summary>
/// 注释自行添加
/// </summary>
 public string Delivery_Name 
 {  get { return _delivery_name;}
  set { _delivery_name = value; } 
 } 


DateTime  _delivery_date=DateTime.Now;
 /// <summary>
/// 注释自行添加
/// </summary>
 public DateTime Delivery_Date 
 {  get { return _delivery_date;}
  set { _delivery_date = value; } 
 } 

} 
}
