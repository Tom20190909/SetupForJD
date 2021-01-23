using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WdtSdk;

namespace GoodsPush
{
    class GoodsPush
    {
        static void Main(string[] args)
        {
            WdtClient client = new WdtClient();
            client.sid = "";
            client.appkey = "";
            client.appsecret = "";
            client.gatewayUrl = "http://121.41.177.115/openapi2/goods_push.php";

            var goods_list = new[]
			{
				new
				{
					goods_no="llxxtest",
					goods_type=1,
					goods_name="测试测试ceshi",
					spec_list=new[]
					{
						new
						{
							spec_no="myxx",
                            spec_code = "myxx",
                            spec_name = "满夜雪啸"
						},
                        new
						{
							spec_no="dshg",
                            spec_code = "dshg",
                            spec_name = "凋松鹤骨"
						},
                        new
						{
							spec_no="mhgs",
                            spec_code = "mhgs",
                            spec_name = "梦回姑苏"
						}

					}
				}
			};
            
            string json = goods_list.ToJsonString();
            client.putParams("goods_list", json);

            string result = client.wdtOpenapi();
            Console.WriteLine(result);
            Console.ReadKey();
        }
    }
}