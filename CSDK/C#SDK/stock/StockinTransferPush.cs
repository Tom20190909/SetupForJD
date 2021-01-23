using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WdtSdk;

namespace StockinTransferPush
{
    class StockinTransferPush
    {
        static void Main(string[] args)
        {
            WdtClient client = new WdtClient();
            client.sid = "";
            client.appkey = "";
            client.appsecret = "";
            client.gatewayUrl = "http://121.41.177.115/openapi2/stockin_transfer_push.php";

            var stockin_info = new
            {
                outer_no = "7898765432432",
                src_order_type = "2",
                src_order_no = "TF201901170004",
                warehouse_no = "wh002",
                goods_list = new[]
                {
                    new{
                        spec_no = "test-ptsd-00002",
                        num = "1"
                    },
                    new{
                        spec_no = "test-ptsd-00001",
                        num = "1"
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