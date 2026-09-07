using Microsoft.VisualStudio.TestTools.UnitTesting;
using APIMyViettel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using APIMyViettel.Entity;

namespace UnitTest
{
    [TestClass()]
    public class UtilsGppTests
    {
        JavaScriptSerializer serializer = new JavaScriptSerializer();

        [TestMethod()]
        public void GetTaskTest()
        {
            var getLogin = Task.Run(() => UtilsGpp.GetTask("https://gpp.com.vn", new GppCookie() { CookieContainer = new CookieContainer(), X_XSRF_TOKEN = string.Empty, SessionId = string.Empty, IsLogin = false, ResponseMsg = string.Empty })).Result;
            Console.WriteLine(serializer.Serialize(getLogin));

            var param = new Dictionary<string, string>();
            param.Add("returnUrlHash", string.Empty);
            param.Add("tenancyName", "hdg99");
            param.Add("usernameOrEmailAddress", "hdg99_admin");
            param.Add("password", "123456aA@");
            param.Add("g-recaptcha-response", string.Empty);
            param.Add("loginAttemptCount", "0");

            var postTaskLogin = Task.Run(() => UtilsGpp.PostTaskLogin("https://gpp.com.vn/?returnUrl=/Application/Index", param, getLogin)).Result;
            Console.WriteLine(serializer.Serialize(postTaskLogin));

            var getApplication = Task.Run(() => UtilsGpp.GetTask("https://gpp.com.vn/Application/Index", postTaskLogin)).Result;
            Console.WriteLine(serializer.Serialize(getApplication));

            var requestCard = new GppTopupRequest()
            {
                maKhachHang = string.Empty,
                maTheCao = "810772059515719",
                serial = "10000146308252"
            };

            var topupResult = Task.Run(() => UtilsGpp.PostTaskTopup("https://gpp.com.vn/api/services/app/lichSuGiaoDich/NapTien", serializer.Serialize(requestCard), getApplication)).Result;
            Console.WriteLine(topupResult);

            if (!string.IsNullOrEmpty(topupResult))
            {
                var resObj = serializer.Deserialize<GppTopupResponse>(topupResult);
                switch (resObj.result.code)
                {
                    case 1:
                        Console.WriteLine(resObj.result.returnValue.menh_gia);
                        break;
                }
            }
        }

        [TestMethod()]
        public void GetTopupTest()
        {
            var accountName = "bdg1759_admin";
            var passWord = "123456a";

            var res = GppService.GetTopup(accountName, passWord);

            //var res = new GppCookie();
            //res.X_XSRF_TOKEN = "6FjneUyPB97B7CcjTf_FmPCow65H4Wxcw3ekn3apbyKF-0FzFcmafer8WBeeZWGZ9yCBqwglLmeWIph8BaYcLXPpos0p3qRpoZF1mUWAo7_kdlNko7gbf3eoCBbAXdiUR5N_wKTy87WOrGZha6TkfA2";

            var requestCard = new GppTopupRequest()
            {
                maKhachHang = string.Empty,
                maTheCao = "314263201124190",
                serial = "10001361415792"
            };

            var resTopup = GppService.Topup(requestCard.serial, requestCard.maTheCao, res);

            Console.WriteLine(resTopup);
        }

        [TestMethod()]
        public void GetResultTest()
        {
            var res =
                "{\"result\":{\"code\":0,\"errorCode\":\"\",\"message\":\"Nạp thẻ lỗi!\",\"returnValue\":null},\"targetUrl\":null,\"success\":true,\"error\":null,\"unAuthorizedRequest\":false,\"__abp\":true}";
            var resObj = serializer.Deserialize<GppTopupResponse>(res);
            Console.WriteLine(resObj.result.code);
        }
    }
}