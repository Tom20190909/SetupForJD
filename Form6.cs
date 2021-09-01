using Sunny.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Configuration;

namespace SleckEc2KdInterface
{
    public partial class wflogin : UIForm
    {
        public static string fcustId;
        public static string fuserId;
        public static string fcustName;
        public static bool clientUp;
        public static string FUserType;
        public wflogin()
        {
            InitializeComponent();
        }


        private void  uiButton1_Click(object sender, EventArgs e)
        {

            getInfo();
        }
        public async void getInfo()
        {
            try
            {
                if(uiCheckBox1.Checked)
                {
                    Configuration config = System.Configuration.ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    config.AppSettings.Settings["userName"].Value = uitxtLoginId.Text;
                    config.AppSettings.Settings["userPwd"].Value = uitxtLoginIPwd.Text;
                    config.Save(ConfigurationSaveMode.Modified);
                    
                    System.Configuration.ConfigurationManager.RefreshSection("appSettings");

                }
                else
                {
                    Configuration config = System.Configuration.ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    config.AppSettings.Settings["userName"].Value ="";
                    config.AppSettings.Settings["userPwd"].Value = "";
                    config.Save(ConfigurationSaveMode.Modified);

                    System.Configuration.ConfigurationManager.RefreshSection("appSettings");
                }
                
                using (HttpClient client = new HttpClient())
                {

                    var user = new { FUserId = uitxtLoginId.Text, FUserPwd = uitxtLoginIPwd.Text };
                    var str = JsonConvert.SerializeObject(user);

                    HttpContent content = new StringContent(str);
                    content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                    HttpResponseMessage response =await client.PostAsync("http://120.78.210.159:3304/Order/VerifyLogin", content);
                    response.EnsureSuccessStatusCode();//用来抛异常的
                    string resultStr =await response.Content.ReadAsStringAsync();
                    var result = JObject.Parse(resultStr);
                    if (result.ContainsKey("IsSucc"))
                    {
                        if (Convert.ToBoolean(result["IsSucc"]))
                        {
                            fcustId = result["FCusCode"].ToString();
                            fuserId = result["Uid"].ToString();
                            fcustName= result["FUserName"].ToString();
                            FUserType = result["FUserType"].ToString();
                            this.Hide();
                            Form5 fm5 = new Form5();
                            fm5.ShowDialog();
                        }
                        else
                        {
                            UIMessageBox.Show(result["strMess"].ToString());
                        }
                    }
                    else
                    {
                        UIMessageBox.Show("无法访问系统,请关注是否网络问题!");
                    }
                }
            }
            catch (Exception ex)
            {
                UIMessageBox.Show(ex.Message.ToString());

            }
        }
    }
}
