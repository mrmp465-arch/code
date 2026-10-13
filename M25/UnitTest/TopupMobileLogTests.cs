using Microsoft.VisualStudio.TestTools.UnitTesting;
using Libs.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace UnitTest
{
    [TestClass()]
    public class TopupMobileLogTests
    {
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        [TestMethod()]
        public void GetProcess_TestTest()
        {
           var topupProcess = new TopupMobileLog().GetProcess_Test("zing", 0, string.Empty, String.Empty);
           Console.WriteLine(serializer.Serialize(topupProcess));
        }
    }
}