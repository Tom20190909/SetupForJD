using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WdtSdk;

namespace StockTransferPush
{
    class StockTransferPush
    {
        static void Main(string[] args)
        {
            WdtClient client = new WdtClient();
            client.sid = "";
            client.appkey = "";
            client.appsecret = "";
            client.gatewayUrl = "http://121.41.177.115/openapi2/stock_transfer_push.php";

            var transfer_info = new
            {
                outer_no = "5678765432345",
                from_warehouse_no = "api_test",
                to_warehouse_no = "wh002",
                skus = new[]
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

            string json = transfer_info.ToJsonString();
            client.putParams("transfer_info", json);

            string result = client.wdtOpenapi();
            Console.WriteLine(result);
            Console.ReadKey();
        }
    }
}