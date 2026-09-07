using System;
using System.Configuration;
using System.Diagnostics;
using System.Net;
using System.Net.Mail;
using System.Text;
using NLog;

namespace Libs.Utils
{
    public static class NLogLogger
    {
        static NLogLogger()
        {
            Logger = LogManager.GetCurrentClassLogger();

        }

        public static Logger Logger { get; set; }

        public static void Info(string message)
        {
            Logger.Info(message);
        }

        public static void Info(string[] list)
        {
            string message = "";
            for (int i = 0; i < list.Length; i++)
            {
                message = message + "\t" + list[i];
            }
            Info(message);
        }
    }
}
