using Microsoft.VisualStudio.TestTools.UnitTesting;
using APIMyViettel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using APIMyViettel.Entity;

namespace UnitTest
{


    [TestClass()]
    public class UtilsTests
    {
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();

        [TestMethod()]
        public void TopupTest()
        {
            var cookie = SmasService.GetTopup("hni_mn_phuongcanh", "123456aA@");
            Console.WriteLine(serializer.Serialize(cookie));
        }

        [TestMethod()]
        public void GetTopup()
        {
            string baseUrl = "https://smas.edu.vn";

            var getLogin = Task.Run(() => UtilsSmas.GetTask("https://smas.edu.vn/Home/LogOn", new CookieContainer())).Result;
            var parameters = new Dictionary<string, string>();
            parameters.Add("__RequestVerificationToken", getLogin.RequestVerificationToken);
            parameters.Add("UserName", "cxn_thcscamtrung");
            parameters.Add("Password", "buiquanghuan");
            parameters.Add("tokenAuthen", string.Empty);
            parameters.Add("LoginVnStudy", "False");

            var loginResponse = Task.Run(() => UtilsSmas.PostTask("https://smas.edu.vn/Home/LogOn", parameters, getLogin.CookieContainer)).Result;

            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(loginResponse.HtmlContent);

            var detectSuccess = loginResponse.HtmlContent.Contains("T&#234;n đăng nhập hoặc mật khẩu kh&#244;ng ch&#237;nh x&#225;c") || loginResponse.HtmlContent.Contains("đăng nhập kh&#244;ng th&#224;nh c&#244;ng");
            if (detectSuccess) new SmasCookie() { IsTopup = false };

            loginResponse.IsTopup = true;

            var rdbSelectLevel = doc.DocumentNode.SelectSingleNode("//div[contains(@class,'level-box')]") != null ? doc.DocumentNode.SelectSingleNode("//div[contains(@class,'level-box')]").GetAttributeValue("value", "empty") : "empty";
            if (rdbSelectLevel != "empty")
            {
                parameters.Clear();
                parameters.Add("__RequestVerificationToken", loginResponse.RequestVerificationToken);
                parameters.Add("RdbSelectLevel", rdbSelectLevel);

                var loginAuthorize = Task.Run(() => UtilsSmas.PostTask("https://smas.edu.vn/Home/AuthorizeAdmin", parameters, loginResponse.CookieContainer)).Result;

                loginResponse = loginAuthorize;
            }

            var getTopup = Task.Run(() => UtilsSmas.GetTask("https://smas.edu.vn/SMSEDUArea/Topup", loginResponse.CookieContainer)).Result;

            if (getTopup == null) new SmasCookie() { IsTopup = false }; //Bi khoa 15P

            //getTopup.IsTopup = true;
            var getImageBase64 = Task.Run(() => UtilsSmas.GetImageBase64(baseUrl + getTopup.CaptChaLink, getTopup.CookieContainer)).Result;
            var captcha = new Lib.Captcha.Anticaptcha.AnticaptchaService().ImageToText(getImageBase64.CaptChaBase64, 0);

            //Libs.Utils.SharedCache.Add("tqg_thcs_lamxuyen", getTopup);

            //var smasCookie = (SmasCookie)Libs.Utils.SharedCache.Get("smas");


            parameters.Clear();
            parameters.Add("__RequestVerificationToken", getTopup.RequestVerificationToken);
            parameters.Add("PinCard", "110415481242064");
            parameters.Add("CardSerial", "10001784234066");
            parameters.Add("ConfirmationCode", captcha);
            //parameters.Add("ConfirmationCode", "bb2d");
            var postTopup = UtilsSmas.PostTask("https://smas.edu.vn/SMSEDUArea/Topup/Topup", parameters, getTopup.CookieContainer).Result;
            Console.WriteLine(postTopup.HtmlContent);


            ////HtmlAgilityPack.HtmlDocument doctopup = new HtmlAgilityPack.HtmlDocument();
            //doc.LoadHtml(postTopup.HtmlContent);
            //var messageSuccess = string.Empty;
            //var messageError = string.Empty;
            //var messageCaptcha = string.Empty;

            //messageSuccess = doc.DocumentNode.SelectSingleNode("//p[@class='message-of-success mg-t-5']") != null ? doc.DocumentNode.SelectSingleNode("//p[@class='message-of-success mg-t-5']").InnerText.Trim() : string.Empty;
            //messageError = doc.DocumentNode.SelectSingleNode("//p[@class='message-of-error mg-t-5']") != null ? doc.DocumentNode.SelectSingleNode("//p[@class='message-of-error mg-t-5']").InnerText.Trim() : string.Empty;
            //messageCaptcha = doc.DocumentNode.SelectSingleNode("//span[@class ='field-validation-error']") != null ? doc.DocumentNode.SelectSingleNode("//span[@class ='field-validation-error']").InnerText.Trim() : string.Empty;

            //Console.WriteLine(messageSuccess);
            //Console.WriteLine(messageError);
            //Console.WriteLine(messageCaptcha);

            //
            //doc.LoadHtml(postTopup.HtmlContent);
            //var messageSuccess = doc.DocumentNode.SelectSingleNode("//p[@class='message-of-success']").InnerText;
            //var messageError = doc.DocumentNode.SelectSingleNode("//p[@class='message-of-error']").InnerText;
            //var messageCaptcha = doc.DocumentNode.SelectSingleNode("//span[@class ='field-validation-error']") != null ? doc.DocumentNode.SelectSingleNode("//span[@class ='field-validation-error']").InnerText : string.Empty;

            //Console.WriteLine(messageSuccess.Trim()); //Thầy/cô đã nạp thành công 10.000 đ vào tài khoản.
            //Console.WriteLine(messageError.Trim());
            //Console.WriteLine(HttpUtility.HtmlDecode(messageCaptcha));



        }
    }
}