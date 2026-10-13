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
    public class VTCServiceTests
    {
        static JavaScriptSerializer serializer = new JavaScriptSerializer();

        [TestMethod()]
        public void GetTopupTest()
        {
           // var vtcLogin = VTCService.Topup(1, "", "", "", "type", VTCService.GetTopup(1, "uron123", "123456", 1), "uron123", 1);
        }

        [TestMethod()]
        public void TopupCardTest()
        {
            var result = VTCService.TopupCard(1, "ID0363583440", "378457288205", "uron123", "uron123", "123456");
            Console.WriteLine(serializer.Serialize(result));
        }

        [TestMethod()]
        public void CodeTest()
        {
            var dis = "Thẻ Cào Seri: ID0364514765";
            var serial = "id0364514765";
            if (dis.Contains(serial.ToUpper()))
            {
                Console.WriteLine(((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds());
            }

           
        }
    }
}