using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using Libs.Utils;


namespace EmailService
{
    partial class RetrieveEmailProcess : ServiceBase
    {
        private RetrieveEmailService retrieveEmail = new RetrieveEmailService();

        public RetrieveEmailProcess()
        {
            InitializeComponent();

            retrieveEmail.StartProcessRetrieveEmail();
        }

        protected override void OnStart(string[] args)
        {
            // TODO: Add code here to start your service.
            NLogLogger.Info(new string[] { "Service Is Start !" });
        }

        protected override void OnStop()
        {
            // TODO: Add code here to perform any tear-down necessary to stop your service.
            NLogLogger.Info(new string[] { "Service Is Stop !" });
            //string mobile = "0912440644";
            //string content = "Sms Process service stop!";
            //Provider.SmsService.SendSmsAlias(mobile, content);
        }
    }
}
