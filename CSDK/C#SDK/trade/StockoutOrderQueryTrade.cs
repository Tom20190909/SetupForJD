using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WdtSdk;

namespace StockoutOrderQueryTrade
{
    class StockoutOrderQueryTrade
    {
        static void Main(string[] args)
        {
            WdtClient client = new WdtClient();
            client.sid = "";
            client.appkey = "";
            client.appsecret = "";
            client.gatewayUrl = "http://121.41.177.115/openapi2/stockout_order_query_trade.php";

            client.putParams("start_time", "2018-10-24 12:30:00");
            client.putParams("end_time", "2018-10-24 13:10:00");

            string result = client.wdtOpenapi();
            Console.WriteLine(result);
            Console.ReadKey();
        }
    }
}