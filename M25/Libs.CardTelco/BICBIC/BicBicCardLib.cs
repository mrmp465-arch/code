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

namespace Libs.CardTelco.BicBic
{

    public class CardRequest
    {
        public string username { get; set; }

        public string password { get; set; }
        public string serial { get; set; }
        public string pincode { get; set; }
        public string telco { get; set; }
        public string amount { get; set; }
        public string requestid { get; set; }
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
        public int status { get; set; }
        public int amount { get; set; }
        public string serial { get; set; }
        public string vendor_id { get; set; }
        public int timestamp { get; set; }
        public string sign { get; set; }
    }

    public class Data
    {
        public Request request { get; set; }
        public Result result { get; set; }
    }

    public class CardResult
    {
        public string errorcode { get; set; }
        public string description { get; set; }
        public string requestid { get; set; }
        public int amount { get; set; }

    }

    public class BicBicCardLib
    {

        public static string PostJson(string uri, string postData)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(uri);
                request.ContentType = "application/json";
                request.Method = "POST";//GET
                request.Timeout = 60000;
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
            catch (WebException e)
            {
                if (e.Status == WebExceptionStatus.Timeout)
                {
                    //Handle timeout exception
                    //return "{\"status\":-5002,\"data\":{\"amount\": 0},\"amount\": 0,\"msg\": \"Ngắt kết nối đến nhà cung cấp Mobo. Timeout exception\"}";
                    NLogLogger.Info(new string[] { "BicBic", "BicBicRequest", postData, "Timeout exception" });
                    return "{\"code\":-5002,\"desc\":\"Ngắt kết nối đến nhà cung cấp BicBIC. Timeout exception\"}";
                }
                else
                {
                    throw;
                }
            }
        }
        public static string HttpPost(string url, string parameters)
        {
            try
            {
                System.Net.WebRequest req = System.Net.WebRequest.Create(url);
                req.ContentType = "application/x-www-form-urlencoded";
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
                    NLogLogger.Info(new string[] { "BicBic", "BicBicRequest", parameters, "Timeout exception" });
                    return "{\"code\":-5002,\"desc\":\"Ngắt kết nối đến nhà cung cấp Mobo. Timeout exception\"}";
                }
                else
                {
                    throw;
                }
            }

        }

        public static int ConvertResponCode(string status, string description)
        {
            var responseStatus = int.Parse(status);
            switch (responseStatus)
            {

                case -1:
                    if (description.Contains("Thông tin thẻ không đúng"))
                        return (int)ResponseCode.CardSerialInvalid;
                    if (description.Contains("Thẻ đã được sử dụng"))
                        return (int)ResponseCode.CardUsed;

                    return (int)ResponseCode.CardCodeInvalid;

                case 3:
                    return (int)ResponseCode.ParameterInvalid;
                case 4:
                    return (int)ResponseCode.CardUsed;
                case 2:
                    return (int)ResponseCode.CardProviderInvalid;
                case 12:
                    return (int)ResponseCode.CardAmountInvalid;
                case 13:
                    return (int)ResponseCode.CardUsed;
                case 30:
                    return (int)ResponseCode.SystemMaintain;
                default:
                    return (int)ResponseCode.UndefinedError;
            }
        }

    }
    public class TopupRequest
    {
        public string cardSerial { get; set; }
        public string cardCode { get; set; }
        public string telco { get; set; }
        public string partnerCode { get; set; }
        public string providerCode { get; set; }
        public long transactionId { get; set; }
        public int amount { get; set; }
        public string sim { get; set; }
        public string simTarget { get; set; }
        public string clientId { get; set; }
        public int slot { get; set; }
    }

    public class TopupResponse
    {
        public int code { get; set; }
        public string message { get; set; }
        public int amount { get; set; }
    }
}
