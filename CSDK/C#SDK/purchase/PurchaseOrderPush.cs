using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WdtSdk;

namespace PurchaseOrderPush
{
    class PurchaseOrderPush
    {
        static void Main(string[] args)
        {
            WdtClient client = new WdtClient();
            client.sid = "";
            client.appkey = "";
            client.appsecret = "";
            client.gatewayUrl = "http://121.41.177.115/openapi2/purchase_order_push.php";

            var purchase_info = new
			{
                provider_no = "wl00001",
                warehouse_no = "001",
                outer_no = "34567876543234",
                is_use_outer_no = "0",
                is_check = "1",
                contact = "旺旺",
                telno = "1333333333",
                receive_address = "天博中润",
                logistics_type = "4",
                other_fee = "100.01",
                post_fee = "100.02",
                remark = "测试",
                details_list = new[]
				{
					new
                    {
                        spec_no ="1001017308AYS2S",
		                num ="1",
		                price ="10.11",
		                discount ="10.22",
		                tax ="0.2",
		                remark ="API测试",
		                prop1 ="011",
		                prop2 ="022"
                    },
                    new
                    {
                        spec_no ="sku002",
		                num ="1",
		                price ="10.11",
		                discount ="10.22",
		                tax ="0.2",
		                remark ="API测试",
		                prop1 ="011",
		                prop2 ="022"
                    },

				}
				
			};

            string json = purchase_info.ToJsonString();
            client.putParams("purchase_info", json);

            string result = client.wdtOpenapi();
            Console.WriteLine(result);
            Console.ReadKey();
        }
    }
}