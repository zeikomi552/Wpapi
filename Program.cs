using log4net.Config;
using Markdig;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WordPressPCL;
using WordPressPCL.Models;
using Wpapi.Models;
using Wpapi.Models.Wordpress;
using System.Linq;

namespace Wpapi
{
    class Program : WpapiBase
    {
        static int Main(string[] args)
        {
            // log4netの設定ファイル読み込み処理
            XmlConfigurator.Configure(new System.IO.FileInfo("log4net.config"));

            try
            {
                WpapiCommand.ExecuteCommand(args);
                return 0;
            }
            catch (Exception e)
            {
                StringBuilder msg = new StringBuilder();
                foreach (var arg in args) msg.Append(arg + " ");

                Logger.Error("wpapi " + msg.ToString());
                Logger.Error(e.Message);

                Console.WriteLine("wpapi " + msg.ToString());
                Console.WriteLine(e.Message);
                return -1;
            }
        }


    }
}
