using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WdtSdk;

namespace StockoutOrderPush
{
    class StockoutOrderPush
    {
        static void Main(string[] args)
        {
            WdtClient client = new WdtClient();
            client.sid = "";
            client.appkey = "";
            client.appsecret = "";
            client.gatewayUrl = "http://121.41.177.115/openapi2/stockout_order_push.php";

            var stockout_info = new
            {
                outer_no = "dd456789876543",
                warehouse_no = "api_test",
                detail_list = new[]
                {
                    new{
                        spec_no = "test-ptsd-00002",
                        num = "1",
                        price = "10"
                    },
                    new{
                        spec_no = "test-ptsd-00001",
                        num = "1",
                        price = "10"
                    }
                }
            };

            string json = stockout_info.ToJsonString();
            client.putParams("stockout_info", json);

            string result = client.wdtOpenapi();
            Console.WriteLine(result);
            Console.ReadKey();
        }
    }
}