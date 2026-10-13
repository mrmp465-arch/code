using Microsoft.VisualStudio.TestTools.UnitTesting;
using APIMyViettel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace UnitTest
{
    
    [TestClass()]
    public class TcVncdcServiceTests
    {
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        [TestMethod()]
        public void TopupCardTest()
        {
            string serial = "10002090902906";
            string pin = "717865452852264";
            string accountName = "Pt_thixaphutho";
            string passWord = "Thixa@123";

            var res = TcVncdcService.TopupCard(serial, pin, accountName, 7, accountName, passWord);

            Console.WriteLine(serializer.Serialize(res));

        }
    }
}