using log4net;
using SleckEc2KdInterface.SERVICE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Topshelf;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]
namespace SleckEc2KdInterface
{

    static class Program
    {
        private static ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        static void Main()
        {
            try
            {
                HostFactory.Run(o =>
                {

                    o.Service<EcServiceControl>();

                    o.RunAsLocalSystem();

                    o.SetServiceName("AutoGetECData");

                    o.SetDisplayName("自动获取旺店通数据及转存至金蝶云");

                    o.SetDescription("自动获取旺店通数据及转存至金蝶云");

                });
            }
            catch(Exception ex)
            {
                Console.Write(ex.Message);
            }
        }



          
        }    
}
