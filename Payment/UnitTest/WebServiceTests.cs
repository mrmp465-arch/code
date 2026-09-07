using Microsoft.VisualStudio.TestTools.UnitTesting;
using APIMyViettel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using Newtonsoft.Json.Linq;

namespace UnitTest
{

    [TestClass()]
    public class WebServiceTests
    {
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        [TestMethod()]
        public void TopupCardTest()
        {
            string serial = "20000004542450";
            string pin = "125256651756091";
            string accountName = "0342787745";
            string passWord = "1234567a";


            var res = WebService.TopupCard(serial, pin, accountName, 1, accountName, passWord, true);

            Console.WriteLine(serializer.Serialize(res));
        }

        [TestMethod()]
        public void LoginTest()
        {
            var account = new Account() { AccountName = "395838388", Password = "Doanthanh123A@" };
            var res = WebService.Login(-1, ref account);
        }

        [TestMethod()]
        public void CheckCardTest_Seft()
        {
            var res = WebService.CheckCard("20000004542450");
            Console.WriteLine(serializer.Serialize(res));
        }

        [TestMethod()]
        public void TopupCardTest_ForFriend()
        {
            string serial = "20000011441398";
            string pin = "524021826675871";
            string mobile = "0342787745";
            string accountName = "981377989";
            string passWord = "Doanthanh123A@";

            
            var res = WebService.TopupCard(serial, pin, mobile, 0, 1, 20000, true);
            Console.WriteLine(serializer.Serialize(res));
            
        }
        [TestMethod()]
        public void CodeTest()
        {
            var res = "<script language=\"javascript\">alert(\"Tài khoản của quý khách đã đăng nhập nơi khác. Vui lòng đăng nhập lại để tiếp tục sử dụng.\");window.location = \"https://vietteltelecom.vn/api/thanh-toan-online-v2\";</script>{\"errorCode\":-2,\"message\":\"Tài khoản của quý khách đã đăng nhập nơi khác. Vui lòng đăng nhập lại để tiếp tục sử dụng.\",\"data\":[],\"loggedOut\":true}";
            if (res.Contains("</script>"))
            {
                res = "{" + res.Split('{')[1];
            }
            
            
            //var code = (int)JObject.Parse(res)["errorCode"];
            //var obj = serializer.Deserialize<APIMyViettel.Entity.TopupResponse>(res);
            //Console.WriteLine(serializer.Serialize(obj));

            dynamic obj = serializer.Deserialize(res, typeof(object));

            //string pattern = @"(?<=\"")([^\s,].*?)(?=\"")|null";
            //MatchCollection matches = Regex.Matches(res, pattern);

            Console.WriteLine(res);

        }

    }
}