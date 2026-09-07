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
    public class MyViettelServiceTests
    {
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        [TestMethod()]
        public void CheckCardApiTest()
        {
            var card = MyViettelService.CheckCardApi("10005411946232");
            Console.WriteLine(serializer.Serialize(card));
            
        }
    }
}