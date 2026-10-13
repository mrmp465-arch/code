using Microsoft.VisualStudio.TestTools.UnitTesting;
using APIGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace UnitTest
{
    [TestClass()]
    public class DzoServiceTests
    {
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        [TestMethod()]
        public void GetTopupTest()
        {
            var result = DzoService.GetTopup("socola130888@gmail.com", "Hahaha123", "", "");


        }

        [TestMethod()]
        public void TopupCardTest()
        {
            var topUp = DzoService.TopupCard("CB03297371", "5882777557", "socola130888@gmail.com", "", "",  "socola130888@gmail.com", "Hahaha123");
            Console.WriteLine(serializer.Serialize(topUp));
        }
    }
}