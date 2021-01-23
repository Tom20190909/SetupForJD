using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WdtSdk;

namespace PurchaseOrderQuery
{
    class PurchaseOrderQuery
    {
        static void Main(string[] args)
        {
            WdtClient client = new WdtClient();
            client.sid = "";
            client.appkey = "";
            client.appsecret = "";
            client.gatewayUrl = "http://121.41.177.115/openapi2/purchase_order_query.php";
            
            client.putParams("start_time", "2018-06-03 00:00:00");
            client.putParams("end_time", "2018-06-21 20:13:41");
            client.putParams("status", "40");
            client.putParams("page_no", "0");
            client.putParams("page_size", "10");

            string result = client.wdtOpenapi();
            Console.WriteLine(result);
            Console.ReadKey();
        }
    }
}