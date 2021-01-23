using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WdtSdk;

namespace PurchaseProviderQuery
{
    class PurchaseProviderQuery
    {
        static void Main(string[] args)
        {
            WdtClient client = new WdtClient();
            client.sid = "";
            client.appkey = "";
            client.appsecret = "";
            client.gatewayUrl = "http://121.41.177.115/openapi2/purchase_provider_query.php";

            client.putParams("page_no", "0");
            client.putParams("page_size", "10");
            client.putParams("provider_no", "GYS01217");
            client.putParams("column", "provider_name,address,website,remark,is_disabled,deleted,modified,created");
            string result = client.wdtOpenapi();
            Console.WriteLine(result);
            Console.ReadKey();
        }
    }
}