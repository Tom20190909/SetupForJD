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
        //  private static ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        [STAThread]
        static void Main()
        {
            try
            {
               

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
               
                Application.Run(new Form2());
            }
            catch(Exception ex)
            {
                Console.Write(ex.Message);
            }
        }



          
        }    
}
