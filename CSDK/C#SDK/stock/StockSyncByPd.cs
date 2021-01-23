using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WdtSdk;

namespace StockSyncByPd
{
    class StockSyncByPd
    {
        static void Main(string[] args)
        {
            WdtClient client = new WdtClient();
            client.sid = "";
            client.appkey = "";
            client.appsecret = "";
            client.gatewayUrl = "http://121.41.177.115/openapi2/stock_sync_by_pd.php";


            var goods_list = new[]
            {
                new{
                    spec_no = "test-ptsd-00002",
                    stock_num = "9999"
                },
                new{
                    spec_no = "test-ptsd-00001",
                    stock_num = "9999"
                }
            };

            string json = goods_list.ToJsonString();

            client.putParams("warehouse_no", "api_test");
            client.putParams("is_adjust_stock", "1");
            client.putParams("goods_list", json);

            string result = client.wdtOpenapi();
            Console.WriteLine(result);
            Console.ReadKey();
        }
    }
}