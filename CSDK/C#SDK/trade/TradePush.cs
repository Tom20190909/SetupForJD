using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WdtSdk;

namespace TradePush
{
    class TradePush
    {
        static void Main(string[] args)
        {
            WdtClient client = new WdtClient();
            client.sid = "";
            client.appkey = "";
            client.appsecret = "";
            client.gatewayUrl = "http://121.41.177.115/openapi2/trade_push.php";

            var trade_list = new[]
            {
                new
                {
                    tid = "TestTid45235678987654",
		            trade_status = "30",
		            delivery_term    = "1",
		            trade_time       = "0000-00-00 00:00:00",
		            pay_time         = "0000-00-00 00:00:00",
		            buyer_nick       = "ceshi",
		            buyer_email      = "123456234533@mail.com",
		            receiver_mobile  = "13233456110",
		            receiver_telno   = "1234563567",
		            receiver_zip     = "0000000",
		            receiver_province= "北京",
		            receiver_name    = "亚历山大",
		            receiver_city    = "北京市",
		            receiver_district= "海淀区",
		            receiver_address = "乌鲁木齐市新疆乌鲁木齐市西山路82号009信箱",
		            logistics_type   = "4",
		            invoice_type     = "0",
		            invoice_title    = "",
		            invoice_content  = "发票内容+",
		            buyer_message    = "发最好&&&的+",
		            remark           = "测试专用",
		            remark_flag      = "1",
		            paid             = "399",
                    post_amount      = "0",
		            cod_amount       = "0",
		            ext_cod_fee      = "0",
                    order_list       = new[]
                    {
                        new{
                            oid            = "TestOid3456789876543",
				            status         = "30",
				            refund_status  = "0",
				            goods_id       = "E166D18BAAEA420CB132E105B3B6128A",
				            spec_id        = "567898765",
				            goods_no       = "",
				            spec_no        = "9787533951092",
				            goods_name     = "情商是什么？——关于生活智慧的44个故事",
				            spec_name      = "",
				            num            = "1",
				            price          = "399",
				            adjust_amount  = "0",
				            discount       = "0",
				            share_discount = "0",
				            cid            = "13",
				            remark		   = "ceshiceshiceshi"
                        }
                    }
                }
            };
            String json = trade_list.ToJsonString();
            client.putParams("trade_list", json);
            client.putParams("shop_no", "api_test");
            client.putParams("switch", "0");

            string result = client.wdtOpenapi();
            Console.WriteLine(result);
            Console.ReadKey();
        }
    }
}