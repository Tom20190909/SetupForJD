using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WdtSdk;

namespace ApiGoodsStockChangeAck
{
    class ApiGoodsStockChangeAck
    {
        static void Main(string[] args)
        {
            WdtClient client = new WdtClient();
            client.sid = "";
            client.appkey = "";
            client.appsecret = "";
            client.gatewayUrl = "http://121.41.177.115/openapi2/api_goods_stock_change_ack.php";

            var stock_sync_list = new[]
            {
                new
                {
                    rec_id = "908",
                    sync_stock = "100",
                    stock_change_count = "338"
                },
                new
                {
                    rec_id = "912",
                    sync_stock = "100",
                    stock_change_count = "338"
                },
                new
                {
                    rec_id = "913",
                    sync_stock = "100",
                    stock_change_count = "338"
                }
            };

            string json = stock_sync_list.ToJsonString();
            client.putParams("stock_sync_list", json);

            string result = client.wdtOpenapi();
            Console.WriteLine(result);
            Console.ReadKey();
        }
    }
}