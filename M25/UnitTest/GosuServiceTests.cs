using Microsoft.VisualStudio.TestTools.UnitTesting;
using APIGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using APIGame.Entity;

namespace UnitTest
{
    [TestClass()]
    public class GosuServiceTests
    {
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        [TestMethod()]
        public void GetTopupTest()
        {
            var res = GosuService.GetTopup("napmoc004", "quagaquangu123");
            Console.WriteLine(res.HtmlContent);
        }

        [TestMethod()]
        public void TopupCardTest()
        {
            var res = GosuService.TopupCard("CA00443441", "7787705216", "napmoc004", "fpt", "napmoc004", "quagaquangu123");
            Console.WriteLine(serializer.Serialize(res));
        }
    }
}