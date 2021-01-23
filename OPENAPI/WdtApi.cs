using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SleckEc2KdInterface.OPENAPI
{
    class WdtClient
    {
        public string sid;
        public string appkey;
        public string appsecret;
        public string gatewayUrl;
        private Dictionary<string, string> param;

        public WdtClient()
        {
            param = new Dictionary<string, string>();
        }

        public void putParams(string key, string value)
        {
            if (key.Length == 0 || value.Length == 0)
                throw new Exception("传入参数或者值为空");
            param.Add(key, value);
        }

        public string wdtOpenapi()
        {
            HttpWebRequest request = null;
            HttpWebResponse response = null;
            Stream serviceRequestBodyStream = null;
            try
            {
                request = (HttpWebRequest)WebRequest.Create(gatewayUrl);
                request.Credentials = CredentialCache.DefaultCredentials;
                request.KeepAlive = false;
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";

                UTF8Encoding encoding = new UTF8Encoding();

                double epoch = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;

                param.Add("sid", sid);
                param.Add("appkey", appkey);
                param.Add("timestamp", epoch.ToString("f0"));
                string postData = CreateParam(true);
                byte[] bodyBytes = encoding.GetBytes(postData);
                request.ContentLength = bodyBytes.Length;
                using (serviceRequestBodyStream = request.GetRequestStream())
                {
                    serviceRequestBodyStream.Write(bodyBytes, 0, bodyBytes.Length);
                    serviceRequestBodyStream.Close();
                    using (response = (HttpWebResponse)request.GetResponse())
                    {
                        using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                        {
                            string result = reader.ReadToEnd();
                            reader.Close();
                            return result;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //LogManager.WriteError(ex, "Post");
                throw ;
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                }
                if (request != null)
                {
                    request.Abort();
                }
            }
        }

        private string CreateParam(bool isLower = false)
        {
            //排序
            param = param.OrderBy(r => r.Key).ToDictionary(r => r.Key, r => r.Value);

            StringBuilder sb = new StringBuilder();
            int i = 0;
            foreach (var item in param)
            {
                if (item.Key == "sign")
                    continue;
                if (i > 0)
                {
                    sb.Append(";");
                }
                i++;
                sb.Append(item.Key.Length.ToString("00"));
                sb.Append("-");
                sb.Append(item.Key);
                sb.Append(":");

                sb.Append(item.Value.Length.ToString("0000"));
                sb.Append("-");
                sb.Append(item.Value);
            }
            if (isLower)
                param.Add("sign", MD5Encrypt(sb + appsecret).ToLower());
            else
            {
                param.Add("sign", MD5Encrypt(sb + appsecret));
            }
            sb = new StringBuilder();
            i = 0;
            foreach (var item in param)
            {
                if (i == 0)
                    sb.Append(string.Format("{0}={1}", item.Key, HttpUtility.UrlEncode(item.Value, Encoding.UTF8)));
                else
                    sb.Append(string.Format("&{0}={1}", item.Key, HttpUtility.UrlEncode(item.Value, Encoding.UTF8)));
                i++;
            }

            return sb.ToString();
        }

        private string MD5Encrypt(string strText)
        {
            MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] result = md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(strText));
            string strMd5 = BitConverter.ToString(result);
            strMd5 = strMd5.Replace("-", "");
            return strMd5;// System.Text.Encoding.Default.GetString(result);
        }


    }

    public static class Utils
    {
        public static string ToJsonString(this object obj)
        {
            if (obj == null)
                return null;
            return JsonConvert.SerializeObject(obj);
        }

        public static object ToJsonString(this string json)
        {
            return json == null ? null : JsonConvert.DeserializeObject(json);
        }
    }
}
