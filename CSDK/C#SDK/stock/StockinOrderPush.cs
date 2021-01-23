using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WdtSdk;

namespace StockinOrderPush
{
    class StockinOrderPush
    {
        static void Main(string[] args)
        {
            WdtClient client = new WdtClient();
            client.sid = "";
            client.appkey = "";
            client.appsecret = "";
            client.gatewayUrl = "http://121.41.177.115/openapi2/stockin_order_push.php";

            var stockin_info = new
            {
                outer_no = "34567898765432",
                warehouse_no = "api_test",
                post_fee = "30",
                other_fee = "40",
                discount = "10",
                goods_list = new[]
                {
                    new{
                        spec_no = "6521478635",
                        stockin_num = "1",
                        stockin_price = "11.11",
                    },
                    new{
                        spec_no = "test-ptsd-00001",
                        stockin_num = "1",
                        stockin_price = "11.11",
                    }
                }
            };

            string json = stockin_info.ToJsonString();
            client.putParams("stockin_info", json);

            string result = client.wdtOpenapi();
            Console.WriteLine(result);
            Console.ReadKey();
        }
    }
}