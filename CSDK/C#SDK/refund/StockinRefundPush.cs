using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WdtSdk;

namespace StockinRefundPush
{
    class StockinRefundPush
    {
        static void Main(string[] args)
        {
            WdtClient client = new WdtClient();
            client.sid = "";
            client.appkey = "";
            client.appsecret = "";
            client.gatewayUrl = "http://121.41.177.115/openapi2/stockin_refund_push.php";

            var stockin_refund_info = new
            {
                refund_no = "TK1901090001",
                outer_no = "234567898765432",
                warehouse_no = "api_test",
                logistics_code = "SMWL001",
                logistics_no = "6787654321234",
                detail_list = new[]
                {
                    new{
                        spec_no = "6521478635",
                        stockin_num = "1",
                        stockin_price = "11.11",
                    }
                }
            };

            string json = stockin_refund_info.ToJsonString();
            client.putParams("stockin_refund_info", json);

            string result = client.wdtOpenapi();
            Console.WriteLine(result);
            Console.ReadKey();
        }
    }
}