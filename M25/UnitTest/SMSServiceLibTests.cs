using Microsoft.VisualStudio.TestTools.UnitTesting;
using Libs.SMS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Libs.API;

namespace UnitTest
{
    [TestClass()]
    public class SMSServiceLibTests
    {
        [TestMethod()]
        public void HttpGetTest()
        {
           var rp = SMSServiceLib.HttpGet("http://jmpk.coby345.club/api/pk/getccu");
           Console.WriteLine(rp);
            
        }
        [TestMethod()]
        public void PatternTest()
        {
            var rt = string.Empty;
            var pattern = string.Format(@"^CH +NAP([0-9])+ +{0}_([0-9])+$", "PC");
            var regex = new Regex(pattern,RegexOptions.IgnoreCase);
            if (regex.IsMatch("CH NAP10 PC _6890501"))
            {
                rt = "{\"status\":1,\"sms\":\"Kiem tra thanh cong\" ,\"type\" :\"text\"}";
            }
            else
            {
                rt = "{\"status\":0,\"sms\":\"Kiem tra that bai\" ,\"type\" :\"text\"}";
            }
            Console.WriteLine(rt);
        }

        
    }
}