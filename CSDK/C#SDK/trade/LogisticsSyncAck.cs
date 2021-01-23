using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WdtSdk;

namespace LogisticsSyncAck
{
    class LogisticsSyncAck
    {
        static void Main(string[] args)
        {
            WdtClient client = new WdtClient();
            client.sid = "";
            client.appkey = "";
            client.appsecret = "";
            client.gatewayUrl = "http://121.41.177.115/openapi2/logistics_sync_ack.php";

            var logistics_list = new[]
            {
                new
                {
                    rec_id = "5496",
                    status = "0",
                    message = "同步成功"
                },
                new
                {
                    rec_id = "5557",
                    status = "0",
                    message = "同步成功"
                },
                new
                {
                    rec_id = "5558",
                    status = "0",
                    message = "同步成功"
                }
            };

            String json = logistics_list.ToJsonString();

            client.putParams("logistics_list", json);

            string result = client.wdtOpenapi();
            Console.WriteLine(result);
            Console.ReadKey();
        }
    }
}