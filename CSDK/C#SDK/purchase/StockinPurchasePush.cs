using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WdtSdk;

namespace StockinPurchasePush
{
    class StockinPurchasePush
    {
        static void Main(string[] args)
        {
            WdtClient client = new WdtClient();
            client.sid = "";
            client.appkey = "";
            client.appsecret = "";
            client.gatewayUrl = "http://121.41.177.115/openapi2/stockin_purchase_push.php";

            var purchase_info = new
			{
                purchase_no = "34567876543234",
                warehouse_no = "api_test",
                outer_no = "4567765433",
                details_list = new[]
				{
					new
                    {
                        spec_no ="1001017308AYS2S",
		                stockin_price = "10",
			            tax_price = "5.5",
			            tax = "0.1",
			            stockin_num = "1",
                    },
                    new
                    {
                        spec_no ="sku002",
		                stockin_price = "10",
			            tax_price = "5.5",
			            tax = "0.1",
			            stockin_num = "1",
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