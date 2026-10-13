using Libs.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest
{
    [TestClass()]
    public class MailServiceTest
    {
        protected string ListMailNotify = "bb2dpay@gmail.com";
        string serviceUrl = "http://35.240.137.60:1583/";
        [TestMethod()]
        public void SendMailTest()
        {
            //var sim = "842342355465";
            //var deviceName = "Samsung J7";
            //var slot = 0;
            //try
            //{
            //    var data = Encrypts.Encrypt("pay", 100.ToString());
            //    var urlUnlock = string.Format("{0}UnlockSim.ashx?data={1}", serviceUrl, data);
            //    var subject = string.Format("Số SIM {0} đang bị khóa nạp thẻ !", sim);
            //    var body = string.Format("Số SIM <b>{0}</b> trên thiết bị <b>{1}</b> được cắm tại Slot <b>{2}</b> đang bị khóa nạp thẻ, hệ thống đã tự động ngừng phân bổ nạp tiền vào SIM này cho đến khi mở khóa lại.<br/><br/><b>Các bước mở khóa:</b><br/> Bước 1: Thực hiện thao tác mở khóa với nhà mạng.<br/>Bước 2: Ấn vào <a href=\"{3}\"><b>URL UNLOCK</b></a> này hoặc thao tác trên <b>CLIENT MOBILE</b> để báo với hệ thống Payment", sim, deviceName, slot, urlUnlock);
            //    var status = EmailService.SendMail(subject, body, ListMailNotify);
            //    Console.WriteLine(status);
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine(e.Message);
            //    throw;
            //}
           
        }
    }
}


