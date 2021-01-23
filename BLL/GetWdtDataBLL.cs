using SleckEc2KdInterface.OPENAPI;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SleckEc2KdInterface.BLL
{
    class GetWdtDataBLL
    {
        public static string GetDataFromApi(Dictionary<string, string> dt,string api)
        {
            WdtClient client = new WdtClient();
            client.sid = ConfigurationManager.AppSettings["sid"].ToString();
            client.appkey = ConfigurationManager.AppSettings["appkey"].ToString();
            client.appsecret = ConfigurationManager.AppSettings["appsecret"].ToString();
            client.gatewayUrl = ConfigurationManager.AppSettings["gatewayUrl"].ToString() +api;

            foreach (KeyValuePair<string, string> kvp in dt)
            {
                client.putParams(kvp.Key, kvp.Value);
            }
           
            string result = client.wdtOpenapi();
            return result;



        }
    }
}
