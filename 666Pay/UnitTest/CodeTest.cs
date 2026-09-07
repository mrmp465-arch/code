using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web.Script.Serialization;
using System.Web;
using APIMyViettel.Entity;
using Libs.API;
using Libs.BankDirect.TraoDoiUSDT;
using Libs.Utils;
using Newtonsoft.Json.Linq;

namespace UnitTest
{
    [TestClass]
    public class CodeTest
    {
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        public class UseCardRequest
        {
            public string CardSerial { get; set; }
            public string CardCode { get; set; }
            public string CardType { get; set; }
            public string AccountName { get; set; }
            public string ReferCode { get; set; }
        }

        public class CardData
        {
            public long requestId { get; set; }
            public string accountNo { get; set; }
            public string price { get; set; }
            public float amount { get; set; }
            public string rate { get; set; }
        }


        //public class CardResponse
        //{
        //    public string code { get; set; } //Trạng thái xử lý : 01 là Thành công, còn lại xem bảng mã lỗi
        //    public string message { get; set; } //Diễn giải kết quả
        //    public CardData data { get; set; } //Serial thẻ sử dụng
        //}

        public class CardResponse
        {
            //{"success":true,"ErrMsg":"Bạn đã nạp thẻ thành công!"}
            public bool success { get; set; }
            public string ErrMsg { get; set; }
            public int Amount { get; set; }
        }

        public class APIResponseString
        {
            public int code { get; set; }
            public string message { get; set; }
            public string data { get; set; }
            public string response_time { get; set; }
            public string sign { get; set; }
        }

        public class APIResponceObject
        {
            public string code { get; set; }
            public string message { get; set; }
            public APIResponseData data { get; set; }
            public string response_time { get; set; }
            public string sign { get; set; }
        }

        public class APIResponseData
        {
            public int cob { get; set; }
            public int cob_payment { get; set; }
            public int total { get; set; }
        }

        public class CardResult
        {
            public int status { get; set; }
            public string data { get; set; }
            public int amount { get; set; }
            public string msg { get; set; }
        }

        //public class ResultData
        //{
        //    public int status { get; set; }
        //    public string data { get; set; }
        //    public string msg { get; set; }

        //}

        public class CardResponseA
        {

            public string error_code { get; set; }
            public string message { get; set; }
            public string result { get; set; }
            public string debug { get; set; }
        }

        [TestMethod]
        public void PushNotify()
        {
            // ... Target page.
            string page = string.Format("http://api.gamevuiviet.club/GameServer/api/CardMobiConnect/SetSimNumBlock?nsptype=3&simblockNum={0}", "84936402600");
            var client = new WebClient();
            var result = client.DownloadString(page);
            Console.WriteLine(result);
        }

        [TestMethod]
        public void JSON_Test()
        {

            //int ussdValue = 0;
            //int.TryParse("1", out ussdValue);
            var JSONStr = "{\"code\":-373,\"message\":\"Card provider invalid\",\"content\":\"100\"}";
            var objJson = serializer.Deserialize<TopupResponse>(JSONStr);
            Console.WriteLine(serializer.Serialize(objJson));
        }

        public class TopupResponse
        {
            public int code { get; set; }
            public string message { get; set; }
            public int content { get; set; }
        }

        public class DataResponse
        {
            public string resultCode { set; get; }
            public RawResponse data { get; set; }
        }


        public class RawResponse
        {
            public string processorResponseCode { get; set; }
            public int amount { get; set; }
            public string currency { get; set; }
            public string processorID { get; set; }
        }
        [TestMethod]
        public void Reg_Test()
        {
            //string input = "{\"currency_pair\": 1, \"status\": \"processing\", \"method\": \"buy\", \"amount\": \"1.27659574\", \"rate\": \"23500.00000000\", \"call_back\": \"https://bankgate.drumpal.info/callback/usdtcalback.ashx?signature=5032384d3e04f54268456e256e485f19\", \"real_payment_received\": true, \"code\": \"TEE9AFCH\", \"timeout\": 120, \"note\": {\"banks\": [{\"bank_name\": \"Vietcombank\", \"master_bank_name\": \"NGUYEN QUANG VIET\", \"master_bank_account\": \"0491000173233\"}], \"target_amount\": 30000, \"receive_usdt_address\": \"0x86bC1A495F8f47e9128f9C64e25A7d6e3D003c6C\"}, \"email\": \"toitest@gmail.com\", \"date_added\": \"2020-05-11T12:51:23.694159+07:00\", \"remaining\": 7071.812113, \"amount_vcc\": 30000, \"amount_usdt\": \"1.27659574\", \"input_transaction\": {\"id\": 111522, \"date_added\": \"2020-05-11T12:53:31.849597+07:00\", \"date_modified\": \"2020-05-11T12:53:31.867357+07:00\", \"amount\": \"30000.00000000\", \"status\": \"success\", \"hash\": \"102733.110520.125247.TEE9AFCH\", \"note\": {\"sms_info\": {\"ref\": \"102733.110520.125247.TEE9AFCH\", \"amount\": 30000.0, \"context\": \"TEE9AFCH\", \"sms_type\": 0, \"bank_name\": \"Vietcombank\"}}, \"is_manual_deposit\": false, \"currency\": 2, \"order\": 95182}}";
            //string input = "{\"currency_pair\": 1, \"status\": \"completed\", \"method\": \"buy\", \"amount\": \"1.27659574\", \"rate\": \"23500.00000000\", \"call_back\": \"https://bankgate.drumpal.info/callback/usdtcalback.ashx?signature=5032384d3e04f54268456e256e485f19\", \"real_payment_received\": true, \"code\": \"TEE9AFCH\", \"timeout\": 120, \"note\": {\"banks\": [{\"bank_name\": \"Vietcombank\", \"master_bank_name\": \"NGUYEN QUANG VIET\", \"master_bank_account\": \"0491000173233\"}], \"target_amount\": 30000, \"receive_usdt_address\": \"0x86bC1A495F8f47e9128f9C64e25A7d6e3D003c6C\"}, \"email\": \"toitest@gmail.com\", \"date_added\": \"2020-05-11T12:51:23.694159+07:00\", \"remaining\": 7071.118339, \"amount_vcc\": 30000, \"amount_usdt\": \"1.27659574\", \"input_transaction\": {\"id\": 111522, \"date_added\": \"2020-05-11T12:53:31.849597+07:00\", \"date_modified\": \"2020-05-11T12:53:31.867357+07:00\", \"amount\": \"30000.00000000\", \"status\": \"success\", \"hash\": \"102733.110520.125247.TEE9AFCH\", \"note\": {\"sms_info\": {\"ref\": \"102733.110520.125247.TEE9AFCH\", \"amount\": 30000.0, \"context\": \"TEE9AFCH\", \"sms_type\": 0, \"bank_name\": \"Vietcombank\"}}, \"is_manual_deposit\": false, \"currency\": 2, \"order\": 95182}, \"output_transaction\": {\"id\": 111523, \"date_added\": \"2020-05-11T12:53:31.901363+07:00\", \"date_modified\": \"2020-05-11T12:53:32.541713+07:00\", \"amount\": \"1.27659574\", \"status\": \"success\", \"hash\": \"012FADDD49964CDD8475235F9FCC3F6E\", \"note\": {\"send_amount\": \"1.27659574\", \"receive_usdt_address\": \"0x86bC1A495F8f47e9128f9C64e25A7d6e3D003c6C\"}, \"is_manual_deposit\": false, \"currency\": 1, \"order\": 95182}}";
            //string input =
            //    "{\"currency_pair\": 1, \"status\": \"canceled\", \"method\": \"buy\", \"amount\": \"2.97872340\", \"rate\": \"23500.00000000\", \"call_back\": \"https://bankgate.drumpal.info/callback/usdtcalback.ashx?signature=7a7abc6efe93e06d4377b58e472816c1\", \"real_payment_received\": true, \"code\": \"TEECD1CH\", \"timeout\": 120, \"note\": {\"banks\": [{\"bank_name\": \"Vietcombank\", \"master_bank_name\": \"NGUYEN QUANG VIET\", \"master_bank_account\": \"0491000173233\"}], \"target_amount\": 70000, \"receive_usdt_address\": \"0x86bC1A495F8f47e9128f9C64e25A7d6e3D003c6C\"}, \"email\": \"fb332650211045220@gmail.com\", \"date_added\": \"2020-05-11T10:56:09.048066+07:00\", \"remaining\": 0, \"amount_vcc\": 70000, \"amount_usdt\": \"2.97872340\"}";
            //Console.WriteLine(serializer.Serialize(serializer.Deserialize<TraoDoiUSDTBankLib.OrderResponse>(input))); // >> 12053302342



            //var regex = new Regex(@"^(id)\d{10}$", RegexOptions.IgnoreCase);
            //Console.WriteLine(regex.IsMatch("iD1111111111"));

            //var amount = Convert.ToInt64(Regex.Match("Ban dang o MSHN02H. So may cua ban la: 84705249260", @"\d+$").Value.Trim());
            //Console.WriteLine(amount);

            //var query = "l?p d?";
            //string q = Regex.Replace(query, @"&quot;|['"",&?%\.*:#/\\-]", "").Trim();
            //Console.WriteLine(q);
            //var resultString = Regex.Match("Thầy/cô đã nạp thành công 50.000 đ vào tài khoản.", @"(\d+.)+").Value;
            //Console.WriteLine(resultString);
            //if (!new Regex(@"^[a-zA-Z0-9]{4,30}$").Match("298412").Success)
            //{
            //    Console.WriteLine("Failed");
            //}
            //else
            //{
            //    long minTime = Convert.ToInt64(DateTime.UtcNow.AddMinutes(-10).ToString("yyyyMMddHHmmss"));
            //    long maxTime = Convert.ToInt64(DateTime.UtcNow.AddMinutes(10).ToString("yyyyMMddHHmmss"));

            //    var url = HttpUtility.UrlEncode("Báo-5472ac38f5620f75523488a2-23-500,000 đ - Nhận 2,500 KC + tặng thêm 450 KC");

            //    Console.WriteLine(url);
            //}

            //var accountName = "hni03_2594_admin";
            //int index = accountName.LastIndexOf("_");
            //var tenancyName = accountName.Substring(0,index);
            //Console.WriteLine(tenancyName);

            var str = Regex.Replace("3''()*&@!#$%^|><?~[B]qwse2432", @"[^0-9a-zA-Z:,]+", "");
            Console.WriteLine(str);


        }

        [TestMethod]
        public void Code_Test()
        {

            //var res = "{\"result\":null,\"targetUrl\":null,\"success\":false,\"error\":{\"code\":0,\"message\":\"Sai tên đăng nhập hoặc mật khẩu\",\"details\":null,\"validationErrors\":null},\"unAuthorizedRequest\":false,\"__abp\":true}";
            //var message = (string)JObject.Parse(res)["error"]["message"];
            //Console.WriteLine(message);



            //var HtmlContent = "T&#234;n đăng nhập hoặc mật khẩu kh&#244;ng ch&#237;nh x&#225;c || đăng nhập kh&#244;ng th&#224;nh c&#244;ng";
            //Console.WriteLine(HttpUtility.HtmlDecode(HtmlContent));


            //DateTime lastTime = DateTime.ParseExact("2/12/2019 12:55:16 AM", "M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture);
            //Console.WriteLine(lastTime.ToString());
            //DateTime lastTime = DateTime.ParseExact("10/8/2018 12:39:57 AM", "MM/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture);
            //DateTime dateUsed = DateTime.ParseExact("21/10/2018 18:02:48", "dd/MM/yyyy H:mm:ss", CultureInfo.InvariantCulture);
            //int totalsec = (dateUsed - lastTime).Seconds;

            //Console.WriteLine(totalsec);

            //var obj = new RawResponse()
            //{
            //    processorResponseCode = "0",
            //    amount = 10000,
            //    currency = "VND",
            //    processorID = "345345"
            //};

            //string[] arr = new string[]
            //{
            //    obj.processorResponseCode,
            //    obj.amount.ToString(),
            //    obj.currency,
            //    obj.processorID
            //};

            //Console.WriteLine(serializer.Serialize(arr));
            //var bank = Libs.BankGate.MegaBank.BankCode.Bank()["vcb"];
            //Console.WriteLine(bank);
            //int[] listValue = { 10000,20000,30000,50000,100000,200000,300000,500000 };
            //if (listValue.Contains(10000))
            //Console.WriteLine("TRUE");
            //else Console.WriteLine("FALSE");

            //var i = (int)Math.Ceiling(1.05);
            //List<String> Items = "viettel|vms|vnp".Split('|').Select(i => i.Trim()).Where(i => i != string.Empty).ToList(); //Split them all and remove spaces
            //Items.Remove("viettel"); //or whichever you want
            //string newProductCode = String.Join("|", Items.ToArray());
            //Console.WriteLine(newProductCode);
            //Console.WriteLine(i);

            //var connStr = "4545576558084";
            //connStr = Regex.Replace(connStr, "[^;]", "*");
            //Console.WriteLine(connStr);
            //string dt = "00:00:00 31/12/2022";
            //var datetime = DateTime.ParseExact(dt, "HH:mm:ss dd/MM/yyyy", CultureInfo.InvariantCulture);
            //Console.WriteLine(datetime.ToString());

            //string[] pp = ("nut1,nut2").Split(',');
            //Random rd = new Random();
            //var r = rd.Next(0, pp.Length);
            //var partnerSplit = pp[r];
            //Console.WriteLine(r + ": " + partnerSplit);

            //var timeSpan1 = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds; // 1591702754
            //var timeSpan2 = DateTime.Now.Subtract(new DateTime(1970, 1, 9, 0, 0, 00)).TotalSeconds; // 1591036754.0874
            //var timeSpan3 = (long)(DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds; // 1591702754087
            //Console.WriteLine(timeSpan1 + " | " + timeSpan2 + " | " + timeSpan3);

            decimal a = Convert.ToDecimal(127000.27);
            var b = Convert.ToInt64(a);
            Console.WriteLine(b);
        }





        [TestMethod]
        public void RoundRobin_Test()
        {


            Queue<String> q = new Queue<String>();
            var s = q.Dequeue();
            q.Enqueue(s);

            Console.WriteLine(q);

        }
        [TestMethod]
        public void PostJSON()
        {
            string uri = "http://naptien.vinaphone.com.vn/Home/AddPrepaid";
            string postData = "{PhoneNum:0912440644, MaThe:435436543242, Answer:\"2J1SF\"}";

            var request = (HttpWebRequest)WebRequest.Create(uri);
            request.Timeout = 30000;
            request.ContentType = "application/json";
            request.Method = "POST"; //GET
                                     //request.Accept = "JSON";
            using (Stream requestStream = request.GetRequestStream())
            {
                byte[] postDatabytes = Encoding.UTF8.GetBytes(postData);
                requestStream.Write(postDatabytes, 0, postDatabytes.Length);
            }
            var webResponse = request.GetResponse();
            var sr = new StreamReader(webResponse.GetResponseStream());
            Console.WriteLine(sr.ReadToEnd().Trim());


        }



        [TestMethod]
        public void PostAppForm()
        {
            string uri = "http://naptien.vinaphone.com.vn/Home/AddPrepaid";
            string parameters = "PhoneNum=091024406&MaThe=1254364564&Answer=NEpI3E";
            System.Net.WebRequest req = System.Net.WebRequest.Create(uri);
            req.ContentType = "application/x-www-form-urlencoded";
            req.Method = "POST";
            req.Timeout = 30000;
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(parameters);
            req.ContentLength = bytes.Length;
            System.IO.Stream os = req.GetRequestStream();
            os.Write(bytes, 0, bytes.Length); //Push it out there
            os.Close();
            System.Net.WebResponse resp = req.GetResponse();
            System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream());
            Console.WriteLine(sr.ReadToEnd().Trim());


        }
        [TestMethod]
        public void MD5()
        {
            var signature = "123456789012345678912345675,00";
            signature = Encrypts.MD5(signature);
            Console.WriteLine(signature);
        }

        [TestMethod]
        public void Time()
        {
            //Console.WriteLine(DateTime.Now.ToString("o"));

            //var jsonPost = string.Format("{{orderid:{0},status={1},amount={2}}}", 1111, 1, 1000);
            //Console.WriteLine(jsonPost);

            string timeCheck = "2020-06-02T12:53";

            if (timeCheck.Count(f => f == ':') == 1)
            {
                timeCheck = timeCheck + ":00";
            }
            DateTime dateUsed = DateTime.ParseExact(timeCheck, "yyyy-MM-dd'T'HH:mm:ss", CultureInfo.InvariantCulture);
            Console.WriteLine(dateUsed.ToString());
        }

        [TestMethod]
        public void CodeSample()
        {
            //var response = "1000000";
            //var amount = !string.IsNullOrEmpty(response) ? Convert.ToInt32(response) : 0;
            //Console.WriteLine(amount);

            var strAmount = Regex.Match("Đã nạp thành công với mệnh giá 10,000", @"(\d+.)+").Value.Trim().Replace(",", "");
            Console.WriteLine(strAmount);
        }

        [TestMethod()]
        public void GetFromQueryStringTest()
        {
            var a = new PostGetHelper().GetFromQueryString<aObj>();
        }

        public class aObj
        {
            private string x { get; set; }
            private string y { get; set; }
        }
    }

}

