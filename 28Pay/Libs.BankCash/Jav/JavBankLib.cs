using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Libs.Utils;
using System.Net;
using System.IO;
using Libs.API;
using System.Text.RegularExpressions;

namespace Libs.BankCash.Jav
{
    public class JavBankLib
    {
        private const string AccessKey = "161e64fd87f42b74bed84780337b2c1b";
        public class CashRespone
        {
            public int stt { get; set; }
          
        }
        public class MobileRequest
        {
            public string requestTime { get; set; }
            public string userName { get; set; }
            public string accountCheck { get; set; }
            public string authKey { get; set; }
        }
        public class MobileRespone
        {
            public int errorCode { get; set; }
            public string errorDesc { get; set; }
            public MobileMsg msg { get; set; }
        }
        public class MobileMsg
        {

            public string name { get; set; }

        }
        public class Callback
        {
            public string requestId { get; set; }
            public string bank { get; set; }
            public string result { get; set; } // Là OrderNo
            public string chargeAmount { get; set; }
            public string signature { get; set; }
            public string status { get; set; }

            public string chargeType { get; set; }
            public string chargeId { get; set; }
            public string chargeCode { get; set; }// Là OrderNo
            public string momoTransId { get; set; }

        }
        public class DataCallback
        {
            public string RefCode { get; set; }
            //public int Status { get; set; }
            public string TransactionID { get; set; }
            public Decimal Amount { get; set; }
            public string Signature { get; set; }

        }
        public class CashBankRequest
        {
            public string requestTime { get; set; }
            public string transId { get; set; }
            public string userName { get; set; }
            public string accountId { get; set; }
            public string accountName { get; set; }
            public string shortBankName { get; set; }
            public int amount { get; set; }
            public string comment { get; set; }
            public string authKey { get; set; }
        }
        public class CashRequest
        {
            public string type { get; set; }
            public string stk { get; set; }
            public string bank_type { get; set; }
            public int amount { get; set; }
            public string message { get; set; }
            public string ref_id { get; set; }
            public string receiver { get; set; }

        }
        public class CashMsg
        {
            public string momoTransId { get; set; }
            public string finishTime { get; set; }
            public string userName { get; set; }
            public string accountReceive { get; set; }
            public string accountName { get; set; }
            public int amount { get; set; }
            public string comment { get; set; }
            public string authKey { get; set; }
        }
        public static bool CheckValidMobile(string mobile)
        {
            if (string.IsNullOrEmpty(mobile))
                return false;
           
            string patternDienThoai = @"^[0]\d{9}$";
            Regex myRegexDienThoai = new Regex(patternDienThoai);

            Match mDienThoai = myRegexDienThoai.Match(mobile);

            if (!mDienThoai.Success)
            {
                return false;
            }

            return true;
        }
       
        public class CashResponeApi
        {
            public string Status { get; set; }
            public string BankName { get; set; }
            public string BankAccountNumber { get; set; }
            public string BankAccountName { get; set; }
            public int Amount { get; set; }
            public string RefCode { get; set; }
            public string OrderNo { get; set; }
            public int Timeout { get; set; }
        }
        public static string GenOrderCode()
        {
            string[] pp = ("q,w,e,r,t,y,u,i,o,p,a,s,d,f,g,h,j,k,l,z,x,c,v,b,n,m").Split(',');
            string tmp = "";
            Random rd = new Random();
            for (int i = 1; i <= 8; i++)
            {
                tmp += pp[rd.Next(0, pp.Length - 1)];
            }
            tmp = DateTime.Now.ToString("yyMMddHH") + tmp;
            return tmp.ToUpper();
        }
        public static string PostJson(string uri, string postData)
        {
            var request = (HttpWebRequest)WebRequest.Create(uri);
            request.ContentType = "application/json";
            request.Method = "POST";//GET
                                    //request.Accept = "JSON";
            using (Stream requestStream = request.GetRequestStream())
            {
                byte[] postDatabytes = Encoding.UTF8.GetBytes(postData);
                requestStream.Write(postDatabytes, 0, postDatabytes.Length);
            }
            var webResponse = request.GetResponse();
            if (webResponse == null)
            {
                return "Unable to connect to the remote server";
            }
            var sr = new StreamReader(webResponse.GetResponseStream());
            return sr.ReadToEnd().Trim();
        }
        public static string PostJson(string uri, string postData,string sign)
        {
            var request = (HttpWebRequest)WebRequest.Create(uri);
            request.ContentType = "application/json";
            request.Method = "POST";//GET
                                    //request.Accept = "JSON";
            request.Headers.Add("X-Access-Key", AccessKey);
            request.Headers.Add("X-Signature", sign);

            using (Stream requestStream = request.GetRequestStream())
            {
                byte[] postDatabytes = Encoding.UTF8.GetBytes(postData);
                requestStream.Write(postDatabytes, 0, postDatabytes.Length);
            }
            var webResponse = request.GetResponse();
            if (webResponse == null)
            {
                return "Unable to connect to the remote server";
            }
            var sr = new StreamReader(webResponse.GetResponseStream());
            return sr.ReadToEnd().Trim();
        }
        public static async Task<string> CallbackJson(string url, string postData)
        {

            NLogLogger.Info(new string[] { "Jav", "Callback", "Partner", "Request", postData });
            var uri = new Uri(url);
            var httpContent = new StringContent(postData, Encoding.UTF8, "application/json");
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var client = new HttpClient(new WebRequestHandler() { UseCookies = false, ReadWriteTimeout = 60000 });
            client.Timeout = TimeSpan.FromSeconds(60);
            try
            {
                var response = await client.PostAsync(uri, httpContent).ConfigureAwait(false);
                if (response.Content != null)
                {
                    var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    NLogLogger.Info(new string[] { "Jav", "Callback", "Partner", "Response", responseContent });
                    client.Dispose();
                    return responseContent;
                }

            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "Jav", "Exeption Post", e.Message });
                return string.Empty;
            }
            client.Dispose();
            return string.Empty;
        }
        public static async Task<string> GetTask(string url)
        {
            var uri = new Uri(url);
            HttpClient client = new HttpClient(new WebRequestHandler() { UseCookies = false, ReadWriteTimeout = 60000 });
            client.Timeout = TimeSpan.FromSeconds(60);

            NLogLogger.Info(new string[] { "VNPAY", "GetTask", url });

            try
            {
                var response = await client.GetAsync(uri);
                if (response.Content != null)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    client.Dispose();
                    return responseContent;
                }
            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "VNPAY", "GetTask", "Exception", url, e.Message });
            }
            client.Dispose();
            return string.Empty;
        }
        public static string HmacSha256Digest(string message, string secretKey)
        {
            byte[] keyBytes = System.Text.Encoding.UTF8.GetBytes(secretKey);
            byte[] messageBytes = System.Text.Encoding.UTF8.GetBytes(message);
            System.Security.Cryptography.HMACSHA256 cryptographer = new System.Security.Cryptography.HMACSHA256(keyBytes);
            byte[] bytes = cryptographer.ComputeHash(messageBytes);
            string base64String = Convert.ToBase64String(bytes, 0, bytes.Length);
            return base64String;
        }
        public static int ConvertResponCode(int responseStatus)
        {
            switch (responseStatus)
            {

                case -1:
                    return (int)ResponseCode.TransactionFailed;
                case 1:
                    return (int)ResponseCode.ParameterInvalid;
                case 2:
                    return (int)ResponseCode.BankAccountInvalid;
                case 71:
                case 4010:
                case -999:
                    return (int)ResponseCode.BankAccountInvalid;
                //case -102:
                //    return (int)ResponseCode.AccountNotExists;
                //case -104:
                //    return (int)ResponseCode.LoginFail;
                //case -105:
                //    return (int)ResponseCode.BankCodeInvalid;
                //case -108:
                //    return (int)ResponseCode.BankAmountInvalid;
                //case -109:
                //    return (int)ResponseCode.BankCardInfoInvalid;
                //case -115:
                //    return (int)ResponseCode.TransactionFailed;
                default:
                    return (int)ResponseCode.UndefinedError;
            }
        }

    }
}
