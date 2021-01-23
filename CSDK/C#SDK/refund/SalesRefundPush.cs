using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WdtSdk;

namespace SalesRefundPush
{
    class SalesRefundPush
    {
        static void Main(string[] args)
        {
            WdtClient client = new WdtClient();
            client.sid = "";
            client.appkey = "";
            client.appsecret = "";
            client.gatewayUrl = "http://121.41.177.115/openapi2/sales_refund_push.php";

            var api_refund_list = new[]
            {
                new
                {
                    tid = "45678987654323456",
                    shop_no = "api_test",
                    platform_id = "127",
                    refund_no = "4567899876543243",
                    type = "3",
                    status = "wait_seller_agree",
                    refund_fee = "23",
                    alipay_no = "4567887654",
                    buyer_nick = "ceshi",
                    refund_time = "2019-01-17 12:12:12",
                    reason = "ceshi",
                    desc = "ceshi",
                    order_list = new[]
                    {
                        new{
                            oid = "34567890987654",
                            num = "3"
                        }
                    },
                },
                new
                {
                    tid = "5678765432134",
                    shop_no = "api_test",
                    platform_id = "127",
                    refund_no = "765432345673",
                    type = "3",
                    status = "wait_seller_agree",
                    refund_fee = "23",
                    alipay_no = "4567887654",
                    buyer_nick = "ceshi",
                    refund_time = "2019-01-17 12:12:12",
                    reason = "ceshi",
                    desc = "ceshi",
                    order_list = new[]
                    {
                        new{
                            oid = "678876543234",
                            num = "3"
                        }
                    },
                }
            };

            string json = api_refund_list.ToJsonString();
            client.putParams("api_refund_list", json);

            string result = client.wdtOpenapi();
            Console.WriteLine(result);
            Console.ReadKey();
        }
    }
}