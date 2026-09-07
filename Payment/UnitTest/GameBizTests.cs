using Microsoft.VisualStudio.TestTools.UnitTesting;
using APIGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace APIGame.Tests
{
    [TestClass()]
    public class GameBizTests
    {
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        [TestMethod()]
        public void SendWebTranferBalanceTest()
        {
            var resule = new GameBiz().SendWebTranferBalance("111111", "vnp", 10000, "0912440644", "pp", "b2btranfer", "127.0.0.1");
            Console.WriteLine(serializer.Serialize(resule));
        }

        [TestMethod()]
        public void SendWebTranferAccountBalanceTest()
        {
            var resule = new GameBiz().SendWebTranferAccountBalance("1", "gate", 1, "127.0.0.1");
            Console.WriteLine(serializer.Serialize(resule));
        }
    }
}
