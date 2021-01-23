using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WdtSdk;

namespace PurchaseReturnPush
{
    class PurchaseReturnPush
    {
        static void Main(string[] args)
        {
            WdtClient client = new WdtClient();
            client.sid = "";
            client.appkey = "";
            client.appsecret = "";
            client.gatewayUrl = "http://121.41.177.115/openapi2/purchase_return_push.php";

            var return_info = new
			{
                purchase_no = "34567876543234",
                provider_no = "wl00001",
                outer_no = "345678987654",
                warehouse_no = "api_test",
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

            string json = return_info.ToJsonString();
            client.putParams("return_info", json);

            string result = client.wdtOpenapi();
            Console.WriteLine(result);
            Console.ReadKey();
        }
    }
}