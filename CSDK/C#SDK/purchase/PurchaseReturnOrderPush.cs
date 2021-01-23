using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WdtSdk;

namespace PurchaseReturnOrderPush
{
    class PurchaseReturnOrderPush
    {
        static void Main(string[] args)
        {
            WdtClient client = new WdtClient();
            client.sid = "";
            client.appkey = "";
            client.appsecret = "";
            client.gatewayUrl = "http://121.41.177.115/openapi2/purchase_return_order_push.php";

            var purchase_return_info = new
			{
                return_no = "CR201901160001",
                outer_no = "456789876543",
                warehouse_no = "api_test",
                logistics_code = "219",
                logistics_no = "3456789876543",
                post_cost = "0.01",
                detail_list = new[]
				{
					new
                    {
                        spec_no ="1001017308AYS2S",
		                num = "1"
                    },
                    new
                    {
                        spec_no ="sku002",
                        num = "1"
                    }
				}
				
			};

            string json = purchase_return_info.ToJsonString();
            client.putParams("purchase_return_info", json);

            string result = client.wdtOpenapi();
            Console.WriteLine(result);
            Console.ReadKey();
        }
    }
}