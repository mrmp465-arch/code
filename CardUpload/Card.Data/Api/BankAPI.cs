using Card.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Card.Data.Api
{
    public static class BankAPI
    {
        private static readonly string Url = ConfigurationManager.AppSettings["Api_url"];
        private static readonly string CallbakUrl = ConfigurationManager.AppSettings["Callback_url"];
        private static string partnerKey = "e1a8d8ca47e0d70990fcef27ee267ced";
        private static string partnerCode = "dcp";
        public static List<BankAccount> GetBankCache()
        {
            var keycache = "BankList";
           
            var cachedata = RedisCaching.GetData(keycache);
            if (cachedata == null)
            {
                var list = GetBank();
                RedisCaching.Add(keycache, JsonConvert.SerializeObject(list), Constants.OneMinuteExpire *2);
                return list;
            }
            else
            {
                return JsonConvert.DeserializeObject<List<BankAccount>>(cachedata.ToString());
            }
        }
        public static List<BankAccount> GetBank()
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            var requestTime = DateTime.Now.ToString("yyyyMMddHHmmss");
            var signature = Encrypt.MD5(partnerCode + requestTime + partnerKey);
            var requestData = new RequestDataGetBank()
            {
                PartnerCode = partnerCode,
                RequestTime = requestTime,

                Signature = signature
            };
            //NLogLogger.Info("Requst Core "+ Url + "GetBank.ashx "+ serializer.Serialize(requestData));
            var serviceResponse = PostJson(Url + "GetBank.ashx", serializer.Serialize(requestData));
            //NLogLogger.Info( "Bank Test Response Core "+ serviceResponse );
            var resObj = serializer.Deserialize<APIResponse>(serviceResponse);
            var listbank = serializer.Deserialize<List<BankAccount>>(resObj.ResponseContent);
            return listbank;
        }
        public static APIResponse OrderBank(string bankCode, int amount, string refcode)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            var request = new RequestDataOrderBank()
            {
                PartnerCode = partnerCode,
                AccountName = partnerCode,
                BankCode = bankCode,
                Amount = amount,
                RefCode = refcode,
                CallbackUrl = CallbakUrl + "Handler/CallbackBankGate.ashx"

            };
            request.Signature = Encrypt.MD5(partnerCode + request.BankCode + request.Amount + request.RefCode + partnerKey);
            //NLogLogger.Info(new string[] { "BankRequest Test", "Response Core", urlService2, serializer.Serialize(request) });
            var serviceResponse = PostJson(Url + "BankRequest.ashx", serializer.Serialize(request));
            var resObj = serializer.Deserialize<APIResponse>(serviceResponse);
           
           // var respone = serializer.Deserialize<OrderRespone>(resObj.ResponseContent);
            return resObj;
            // NLogLogger.Info(new string[] { "BankRequest Test", "Response Core", serviceResponse });
        }
        public static APIResponse Cash(string bankCode, int amount, string refcode, string accountNumber, string accountName)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            var request = new RequestDataCash()
            {
                PartnerCode = partnerCode,
                BankCode = bankCode,
                AccountNumber = accountNumber,
                AccountName = accountName,
                Amount = amount,
                RefCode = refcode,
                CallbackUrl = CallbakUrl + "Handler/CallbackBankCash.ashx"

            };
            request.Signature = Encrypt.MD5(partnerCode + request.AccountNumber + request.AccountName + request.BankCode + request.Amount + request.RefCode + partnerKey);
            //NLogLogger.Info(new string[] { "BankRequest Test", "Response Core", urlService2, serializer.Serialize(request) });
            var serviceResponse = PostJson(Url + "BankCashV2.ashx", serializer.Serialize(request));
            var resObj = serializer.Deserialize<APIResponse>(serviceResponse);
            //var respone = serializer.Deserialize<CashRespone>(resObj.ResponseContent);
            return resObj;
            // NLogLogger.Info(new string[] { "BankRequest Test", "Response Core", serviceResponse });
        }
        private static string PostJson(string uri, string postData)
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
    }
    public class BankAccount
    {

        //public string BankName { get; set; }
        public string BankCode { get; set; }
    }
    public class RequestDataGetBank
    {
        public string PartnerCode { get; set; }
        public string RequestTime { get; set; }

        public string Signature { get; set; }

    }
    public class RequestDataOrderBank
    {
        public string PartnerCode { get; set; }
        public string BankCode { get; set; }
        public string AccountName { get; set; }
        public int Amount { get; set; }
        public string RefCode { get; set; }
        public string CallbackUrl { get; set; }
        public string Signature { get; set; }

    }
    public class RequestDataCash
    {
        public string PartnerCode { get; set; }
        public string AccountNumber { get; set; }
        public string BankCode { get; set; }
        public string AccountName { get; set; }
        public int Amount { get; set; }
        public string RefCode { get; set; }
        public string Signature { get; set; }
        public string CallbackUrl { get; set; }

    }
    public class APIResponse
    {
        public int ResponseCode { get; set; }
        public string Description { get; set; }
        public string ResponseContent { get; set; }
        public string Signature { get; set; }
    }
    public class OrderRespone
    {
        public string Status { get; set; }
        public string BankName { get; set; }
        public string Url { get; set; }
        public string BankAccountNumber { get; set; }
        public string BankAccountName { get; set; }
        public int Amount { get; set; }
        public int RefCode { get; set; }
        public string OrderNo { get; set; }
        public int Timeout { get; set; }
    }
    public class CashRespone
    {
        public string Status { get; set; }
        public string BankName { get; set; }
        public string BankAccountNumber { get; set; }
        public string BankAccountName { get; set; }
        public int Amount { get; set; }
        public int RefCode { get; set; }
        public string OrderNo { get; set; }
        public int Timeout { get; set; }
    }
    public class DataCallbackCash
    {
        public string RefCode { get; set; }
        public int Status { get; set; }
        public string TransactionID { get; set; }
        public int Amount { get; set; }
        public string Desciption { get; set; }
        public string Signature { get; set; }

    }
    public class DataCallbackGate
    {
        public string TransId { get; set; }
        public int Amount { get; set; }
        public string Content { get; set; }
        public string Signature { get; set; }
        public string BankCode { get; set; }
        public string Mobile { get; set; }
        public string mTransId { get; set; }
        public string Result { get; set; }

    }
}
