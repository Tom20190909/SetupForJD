using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WdtSdk;

namespace StockTransferQuery
{
    class StockTransferQuery
    {
        static void Main(string[] args)
        {
            WdtClient client = new WdtClient();
            client.sid = "";
            client.appkey = "";
            client.appsecret = "";
            client.gatewayUrl = "http://121.41.177.115/openapi2/stock_transfer_query.php";

            client.putParams("start_time", "2017-04-05 00:00:00");
            client.putParams("end_time", "2017-04-05 23:59:59");

            string result = client.wdtOpenapi();
            Console.WriteLine(result);
            Console.ReadKey();
        }
    }
}