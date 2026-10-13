using Microsoft.VisualStudio.TestTools.UnitTesting;
using APIGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using APIGame;
using APIGame.Entity;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace UnitTest
{
    [TestClass()]
    public class GarenaServiceTests
    {
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        [TestMethod()]
        public void TopupCardTest()
        {
            var name = "ppteam00";
            var pass = "Gteam@123";
            //var passEnd = "6c316c5c3382dc639e42492d311235c0";
            //var GarenaCookie = new GameCookie();
            //GarenaCookie.CookieContainer = new CookieContainer();
            //Prelogin

            var urlpreLogin = string.Format("https://auth.garena.com/api/prelogin?account={0}&format=json&id={1}&app_id=10017", name, DateTime.Now.Subtract(new DateTime(1970, 1, 9, 0, 0, 00)).TotalSeconds);
            var preLoginResult = Task.Run(() => UtilsGarena.GetTask(urlpreLogin, new CookieContainer())).Result;
            var preLogin = serializer.Deserialize<PreLogin>(preLoginResult.HtmlContent);
            //var encryptedPassword = Task.Run(() => UtilsGarena.GetTask(string.Format(urlEncrypt,pass), new CookieContainer())).Result;
            //Console.WriteLine(encryptedPassword.HtmlContent);
            var s = ChromeDriverService.CreateDefaultService();
            s.HideCommandPromptWindow = true;
            ChromeDriver d = new ChromeDriver(s);
            try
            {
                //Ecrypt
                var urlEncrypt = "http://14.231.250.48:8080/Encrypt?password={0}&v1={1}&v2={2}";
                d.Navigate().GoToUrl(string.Format(urlEncrypt, pass, preLogin.v1, preLogin.v2));
                IJavaScriptExecutor e = (IJavaScriptExecutor)d;
                string encryptedPassword = (string)e.ExecuteScript("return document.title");
                s.Dispose();

                //Login
                var urlLogin = string.Format("https://auth.garena.com/api/login?account={0}&password={1}&format=json&id={2}&app_id=10017", name, encryptedPassword, DateTime.Now.Subtract(new DateTime(1970, 1, 9, 0, 0, 00)).TotalSeconds);
                var loginResult = Task.Run(() => UtilsGarena.GetTask(urlLogin, preLoginResult.CookieContainer)).Result;

                //Grant Token
                var urlGrantToken = "https://auth.garena.com/oauth/token/grant";
                var param = new Dictionary<string, string>();
                param.Add("client_id", "10017");
                param.Add("redirect_uri", "https://napthe.vn/app/10090");
                param.Add("response_type", "token");
                param.Add("platform", "1");
                param.Add("locale", "vi-VN");
                param.Add("theme", "white");
                param.Add("format", "json");
                param.Add("id", DateTime.Now.Subtract(new DateTime(1970, 1, 9, 0, 0, 00)).TotalSeconds.ToString());
                param.Add("app_id", "10017");

                var tokenReult = Task.Run(() => UtilsGarena.PostTask(urlGrantToken, param, loginResult.CookieContainer)).Result;
                var token = serializer.Deserialize<GrantToken>(tokenReult.HtmlContent);
                //Console.WriteLine(tokenReult.HtmlContent);

                //InspectToken --> SAVA Cookie đoạn này
                var urlInspectToken = "https://napthe.vn/api/auth/inspect_token";
                var inspectTokenRequest = new InspectToken()
                {
                    token = token.access_token
                };
                var inspectTokenResult = Task.Run(() => UtilsGarena.PostTask(urlInspectToken, serializer.Serialize(inspectTokenRequest), tokenReult.CookieContainer)).Result;
                //Console.WriteLine(inspectTokenResult.HtmlContent);



                //Charge
                var cardCode = "2822001815634124";
                var friend_username = "friend_username";
                var payRequest = new PayRequest()
                {
                    app_id = 10090,
                    service = "pc",
                    packed_role_id = 0,
                    channel_id = 205000,
                    channel_data = new ChannelData()
                    {
                        card_password = cardCode,
                        friend_username = friend_username
                    }

                };

                var urlPay = "https://napthe.vn/api/shop/pay/init?language=vi&region=VN";
                var payResult = Task.Run(() => UtilsGarena.PostTask(urlPay, serializer.Serialize(payRequest), inspectTokenResult.CookieContainer)).Result;
                var payResponse = serializer.Deserialize<PayResponse>(payResult.HtmlContent);
                if (payResponse.result == "success")
                {
                    //CheckCharge
                    var urlCheckPay = "https://napthe.vn/api/shop/pay/poll";
                    var checkPayRequest = new PayCheckRequest()
                    {
                        display_id = payResponse.display_id
                    };
                    var checkPayResult = Task.Run(() => UtilsGarena.PostTask(urlCheckPay, serializer.Serialize(checkPayRequest), payResult.CookieContainer)).Result;
                    var payCheckResponse = serializer.Deserialize<PayCheckResponse>(checkPayResult.HtmlContent);
                    Console.WriteLine("Success:" + payCheckResponse.point_amount / 2 * 1000);
                }
                else
                {
                    //{"display_id":"15807125233508078763","result":"error_invalid_card","exec":{"display_id":"15807125233508078763"}} --> CardCode Invalid
                    //{"display_id":"15807125233508078763","result":"error_used_card","exec":{"display_id":"15807125233508078763"}} --> CardUsed or Invalid
                    //{ "display_id":"13511475890774449061","result":"error_friend_username","exec":{ "display_id":"13511475890774449061"} } --> User ko Tồn Tại
                    //{"error":"error_require_captcha"}

                    Console.WriteLine(payResult.HtmlContent);
                }






            }
            catch
            {

            }


            //var passwordMd5 = APIGame.Sercurity.Encrypts.MD5(pass);
            //var passwordKey = APIGame.Sercurity.Encrypts.HashSHA256(APIGame.Sercurity.Encrypts.HashSHA256(passwordMd5 + preLogin.v1) + preLogin.v2);
            //var clsCrypto = new APIGame.Sercurity.ClsCrypto(passwordKey);
            //var encryptedPassword = clsCrypto.Encrypt(passwordMd5);
            //Console.WriteLine(encryptedPassword);
            //var encryptedPassword = APIGame.Sercurity.Encrypts.Encrypt(passwordMd5, passwordKey);
            //encryptedPassword = APIGame.Sercurity.Encrypts.Base64Encode(encryptedPassword.ToString());
            //byte[] bytes = System.Convert.FromBase64String(encryptedPassword);
            //Console.WriteLine(BitConverter.ToString(bytes));
        }

        [TestMethod()]
        public void GetTopupTest()
        {
            var result = GarenaService.GetTopup("trungdtdev", "Bin&Bon2d", "0", "0");
            var resultPay = GarenaService.Topup("12312312", "1470697913137802", "trungdtdev", result, "0", "0", true);
            
        }

        [TestMethod()]
        public void GetTopupOpenIdTest()
        {

            var loginOpenID = GarenaService.GetTopupOpenId("1857158667", "0", "0");
            var topup = GarenaService.TopupOpenId("1857158667", "9433195582335547", "193396021", loginOpenID, "0", "0");
            Console.WriteLine(topup);
        }

        [TestMethod()]
        public void TopupCardTest1()
        {
            var result1 = GarenaService.TopupCard("210712238", "9278773601063775", "trungdtdev", "1", "1", "trungdtdev", "Bin&Bon2d", 9);
            Console.WriteLine(serializer.Serialize(result1));

            //var result2 = GarenaService.TopupCard("193151542", "4790474070139946", "trungdtdev", "1", "1", "trungdtdev", "Bin&Bon2d", 9);
            //Console.WriteLine(serializer.Serialize(result2));

        }
    }
}