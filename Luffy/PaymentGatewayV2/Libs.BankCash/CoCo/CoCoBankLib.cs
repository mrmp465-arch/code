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

namespace Libs.BankCash.CoCo
{
    public class CoCoBankLib
    {
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
            public string requestTime { get; set; }
            public string transId { get; set; }
            public string userName { get; set; }
            public string accountReceive { get; set; }
            public int amount { get; set; }
            public string comment { get; set; }
            public string authKey { get; set; }
        }
        public class MobileRequest
        {
            public string requestTime { get; set; }
            public string userName { get; set; }
            public string accountCheck { get; set; }
            public string authKey { get; set; }
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
        public class CashRespone
        {
            public int errorCode { get; set; }
            public string errorDesc { get; set; }
            public string transId { get; set; }
            public CashMsg msg { get; set; }
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
        public class MoibleResponeApi
        {
            public string Status { get; set; }
            public string BankName { get; set; }
            public string BankAccountNumber { get; set; }
            public string BankAccountName { get; set; }
            
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
            request.Timeout = 100000;
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
        public static int ConvertResponCode(int responseStatus)
        {
            switch (responseStatus)
            {

                case -1:
                    return (int)ResponseCode.TransactionFailed;
                case -99:
                    return (int)ResponseCode.BankAccountInvalid;
                case -100:
                    return (int)ResponseCode.SystemError;
                case -101:
                    return (int)ResponseCode.IpInvalid;
                case -102:
                    return (int)ResponseCode.AccountNotExists;
                case -104:
                    return (int)ResponseCode.LoginFail;
                case -105:
                    return (int)ResponseCode.BankCodeInvalid;
                case -108:
                    return (int)ResponseCode.BankAmountInvalid;
                case -109:
                    return (int)ResponseCode.BankCardInfoInvalid;
                case -115:
                    return (int)ResponseCode.BankAmountLimit;
                case -10:
                    return (int)ResponseCode.BankAccountInvalid;
                default:
                    return (int)ResponseCode.UndefinedError;
            }
        }

    }
}
