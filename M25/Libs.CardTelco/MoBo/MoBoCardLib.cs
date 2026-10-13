using Libs.API;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using Libs.Utils;

namespace Libs.CardTelco.MoBo
{

    public class CardRequest
    {
        public string clientId { get; set; }
        public string customer { get; set; }
        public string codeType { get; set; }
        public string serial { get; set; }
        public string pinCode { get; set; }
        public string urlCallback { get; set; }
        public string sign { get; set; }
        public string clientAmount { get; set; }
        public string customerId { get; set; }
    }

    public class Request
    {
        public string clientid { get; set; }
        public string serial { get; set; }
        public string pin { get; set; }
        public string callback { get; set; }
    }

    public class Result
    {

        public int realAmount { get; set; }
        //public string serial { get; set; }
        //public string vendor_id { get; set; }
        //public int timestamp { get; set; }
        //public string sign { get; set; }
    }

    public class Data
    {
        //public Request request { get; set; }
        public Result cardInfo { get; set; }
        public int response_id { get; set; }
    }

    public class CardResult
    {
        public int code { get; set; }
        public string message { get; set; }
        public Data data { get; set; }
    }
    public class MoboCardLib
    {
        //public static string createChecksum(CardResult data, string key)
        //{
        //    return Encrypts.MD5(String.Format("{0}{1}{2}{3}{4}{5}", data.data.result.status, data.data.result.amount, data.data.result.serial, data.data.result.vendor_id, data.data.result.timestamp, key));
        //}

        public static string PostJson(string uri, string postData)
        {
            var request = (HttpWebRequest)WebRequest.Create(uri);
            request.ContentType = "application/json;charset=UTF-8";
            request.Method = "POST";//GET
            request.Timeout = 60000;                   //request.Accept = "JSON";
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
        public static string HttpPost(string url, string parameters)
        {
            try
            {
                System.Net.WebRequest req = System.Net.WebRequest.Create(url);
                req.ContentType = "application/json;charset=UTF-8";
                req.Method = "POST";
                req.Timeout = 60000;
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(parameters);
                req.ContentLength = bytes.Length;
                System.IO.Stream os = req.GetRequestStream();
                os.Write(bytes, 0, bytes.Length); //Push it out there
                os.Close();
                System.Net.WebResponse resp = req.GetResponse();
                if (resp == null) return null;
                System.IO.StreamReader sr = new System.IO.StreamReader(resp.GetResponseStream());
                return sr.ReadToEnd().Trim();
            }
            catch (WebException e)
            {
                if (e.Status == WebExceptionStatus.Timeout)
                {
                    //Handle timeout exception
                    //return "{\"status\":-5002,\"data\":{\"amount\": 0},\"amount\": 0,\"msg\": \"Ngắt kết nối đến nhà cung cấp Mobo. Timeout exception\"}";
                    NLogLogger.Info(new string[] { "MoBo", "MoBoRequest", parameters, "Timeout exception" });
                    return "{\"code\":-5002,\"desc\":\"Ngắt kết nối đến nhà cung cấp Mobo. Timeout exception\"}";
                }
                else
                {
                    throw;
                }
            }

        }

        public static int ConvertResponCode(int responseStatus)
        {
            switch (responseStatus)
            {

                case 31:
                case 15:
                    return (int)ResponseCode.CardUsed;
                case 16:
                    return (int)ResponseCode.CardNotActivated;
                case 17:
                    return (int)ResponseCode.CardCodeInvalid;
                case 18:
                case 28:
                    return (int)ResponseCode.CardSerialInvalid;
                case 19:
                    return (int)ResponseCode.CardHasExpired;
                case 21:
                    return (int)ResponseCode.CardIsLocked;
                case 22:
                    return (int)ResponseCode.AccountLocked;
                case 23:
                    return (int)ResponseCode.CardCodeInvalid;
                case 24:
                    return (int)ResponseCode.PartnerNotExistsNotActive;
                case 25:
                    return (int)ResponseCode.CardProviderInvalid;
                case 26:
                    return (int)ResponseCode.SystemMaintain;
                case 30:
                    return (int)ResponseCode.CardSerialInvalid;
                case -5002:
                    return (int)ResponseCode.TransactionTimeout;
                default:
                    return (int)ResponseCode.UndefinedError;
            }
        }

    }

}
