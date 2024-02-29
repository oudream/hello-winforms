using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HelloWinForms
{
    internal static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new HelloDataGridView());
            //Application.Run(new HelloRoslyn());
            //Application.Run(new HelloSnowflake());
            //Application.Run(new HelloWaitHandle());
            //Application.Run(new HelloLog());
            //Application.Run(new HelloOOP());
            //Application.Run(new HelloAsync());
            //Application.Run(new HelloCustomBar());
            Application.Run(new HelloServiceController());
        }
    }
}
