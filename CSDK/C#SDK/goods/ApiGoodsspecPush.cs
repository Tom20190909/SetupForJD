using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WdtSdk;

namespace ApiGoodsspecPush
{
    class ApiGoodsspecPush
    {
        static void Main(string[] args)
        {
            WdtClient client = new WdtClient();
            client.sid = "";
            client.appkey = "";
            client.appsecret = "";
            client.gatewayUrl = "http://121.41.177.115/openapi2/api_goodsspec_push.php";

            var api_goods_info = new
			{
                platform_id = "127",
                shop_no = "octmami",
                goods_list = new[]
				{
					new
					{
						goods_id="6543217612345",
                        spec_id = "732176432",
                        goods_no = "myxx",
                        spec_no = "myxx",
                        status = "1"
					},
                    new
					{
						goods_id="87654123787",
                        spec_id = "45687654",
                        goods_no = "dshg",
                        spec_no = "dshg",
                        status = "1"
					},
                    new
					{
						goods_id="87654322355",
                        spec_id = "234567876",
                        goods_no = "mhgs",
                        spec_no = "mhgs",
                        status = "1"
					}

				}
				
			};

            string json = api_goods_info.ToJsonString();
            client.putParams("api_goods_info", json);

            string result = client.wdtOpenapi();
            Console.WriteLine(result);
            Console.ReadKey();
        }
    }
}