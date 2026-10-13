using Microsoft.VisualStudio.TestTools.UnitTesting;
using APIMyViettel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using Newtonsoft.Json.Linq;

namespace UnitTest
{
    [TestClass()]
    public class ShopOneServiceTests
    {
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        [TestMethod()]
        public void TopupCardTest()
        {
            string serial = "10001784247311";
            string pin = "712283363516394";
            string accountName = "0963074569";
            string passWord = "833087";

            var res = ShopOneService.TopupCard(serial, pin, accountName, 8, accountName, passWord);

            Console.WriteLine(serializer.Serialize(res));
        }

        [TestMethod()]
        public void TopupCardCodeTest()
        {
            string success = "{\"isLite\":false,\"token\":\"bc46a8a9-323c-48d1-b2f1-01fc38aa2014?t=1541259758000\",\"money\":10000.0,\"errorMsg\":null,\"type\":\"NORMAL\",\"totalMoney\":100000}";
            string faile = "{\"isLite\":false,\"token\":\"9302e7a3-d8da-4b36-9d41-b339bf2ce23f?t=1541260028000\",\"errorMsg\":\"Mã thẻ hoặc số serial thẻ không đúng.Bạn đã nhập sai 1 lần, vui lòng thử lại sau 3 phút\",\"type\":\"NORMAL\",\"totalMoney\":100000}";
            var res = (string)JObject.Parse(success)["money"]; 
            Console.WriteLine(serializer.Serialize(res));
        }
    }
}