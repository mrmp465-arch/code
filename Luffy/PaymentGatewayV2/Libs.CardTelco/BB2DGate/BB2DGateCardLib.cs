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

namespace Libs.CardTelco.BB2DGate
{

    public class CardRequest
    {
        public string CardSerial { get; set; }
        public string CardCode { get; set; }
        public string CardType { get; set; }
        public string AccountName { get; set; }
        public string AppCode { get; set; }
        public string RefCode { get; set; }
        public int AmountUser { get; set; }
        public string CallBackUrl { get; set; }
    }

    public class RequestData
    {
        public string PartnerCode { get; set; }
        public string ServiceCode { get; set; }
        public string CommandCode { get; set; }
        public string RequestContent { get; set; }
        public string Signature { get; set; }

    }

    public class CardResponse
    {
        public int ResponseCode { get; set; }
        public string Description { get; set; }
        public string ResponseContent { get; set; }
        public string Signature { get; set; }
    }

    public class BB2DGateCardLib
    {
        public static string MD5(string data)
        {
            UTF8Encoding encoding1 = new UTF8Encoding();
            MD5CryptoServiceProvider provider1 = new MD5CryptoServiceProvider();
            byte[] buffer1 = encoding1.GetBytes(data);
            byte[] buffer2 = provider1.ComputeHash(buffer1);
            return BitConverter.ToString(buffer2).Replace("-", "").ToLower();
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

        public static string PostForm(string url, string parameters)
        {

            var req = (HttpWebRequest)WebRequest.Create(url);
            req.ContentType = "application/x-www-form-urlencoded";
            req.Method = "POST";
            //req.Timeout = 30000;
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

        public static int ConvertResponCode(int responseStatus)
        {
            switch (responseStatus)
            {

                case 15:
                    return (int)ResponseCode.CardUsed;
                case 16:
                    return (int)ResponseCode.CardNotActivated;
                case 17:
                    return (int)ResponseCode.CardCodeInvalid;
                case 18:
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
                default:
                    return (int)ResponseCode.UndefinedError;
            }
        }

    }

}
