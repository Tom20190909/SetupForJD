using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WdtSdk;

namespace StockQuery
{
    class StockQuery
    {
        static void Main(string[] args)
        {
            WdtClient client = new WdtClient();
            client.sid = "";
            client.appkey = "";
            client.appsecret = "";
            client.gatewayUrl = "http://121.41.177.115/openapi2/stock_query.php";

            client.putParams("start_time", "2019-01-16 00:00:00");
            client.putParams("end_time", "2019-01-17 12:00:00");

            string result = client.wdtOpenapi();
            Console.WriteLine(result);
            Console.ReadKey();
        }
    }
}