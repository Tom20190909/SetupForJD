using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WdtSdk;

namespace PurchaseProviderCreate
{
    class PurchaseProviderCreate
    {
        static void Main(string[] args)
        {
            WdtClient client = new WdtClient();
            client.sid = "";
            client.appkey = "";
            client.appsecret = "";
            client.gatewayUrl = "http://121.41.177.115/openapi2/purchase_provider_create.php";

            client.putParams("provider_no", "llxtest");
            client.putParams("provider_name", "ceshi测试");
            client.putParams("min_purchase_num", "1");
            client.putParams("purchase_cycle_days", "1");
            client.putParams("arrive_cycle_days", "1");
            client.putParams("last_purchase_time", "2018-08-01 00:00:00");

            string result = client.wdtOpenapi();
            Console.WriteLine(result);
            Console.ReadKey();
        }
    }
}