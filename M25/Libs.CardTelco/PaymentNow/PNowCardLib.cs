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

namespace Libs.CardTelco.PaymentNow
{
    public class CardResult
    {
        public int status { get; set; }
        public data data { get; set; }
        public int amount { get; set; }
        public string msg { get; set; }
    }

    public class CardRequest
    {
        public int project_id { get; set; }
        public int user_id { get; set; }
        public string trans_id { get; set; }
        public string card_id { get; set; }
        public string pin_field { get; set; }
        public string seri_field { get; set; }
        public int time { get; set; }
        public string sign { get; set; }
    }

    public class data
    {
        public int amount { get; set; }
    }

    public class PNowCardLib
    {
        public static string createChecksum(CardRequest data, string key)
        {
            return Encrypts.MD5(String.Format("{0}{1}{2}{3}{4}{5}{6}{7}", data.user_id, data.project_id, data.trans_id, data.card_id, data.pin_field, data.seri_field, data.time, key));
        }

        public static string PostData(string uri, string postData)
        {

            try
            {
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
                if (webResponse == null)
                {
                    return "{\"status\":-5001,\"data\":{\"amount\": 0},\"amount\": 0,\"msg\": \"Ngắt kết nối đến nhà cung cấp PaymentNow. Timeout exception\"}";
                }
                var sr = new StreamReader(webResponse.GetResponseStream());
                return sr.ReadToEnd().Trim();
            }
            catch (WebException e)
            {
                if (e.Status == WebExceptionStatus.Timeout)
                {
                    //Handle timeout exception
                    return "{\"status\":-5002,\"data\":{\"amount\": 0},\"amount\": 0,\"msg\": \"Ngắt kết nối đến nhà cung cấp PaymentNow. Timeout exception\"}";
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
                req.Timeout = 30000;
                byte[] bytes = System.Text.Encoding.ASCII.GetBytes(parameters);
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
                    return "{\"status\":-5002,\"data\":{\"amount\": 0},\"amount\": 0,\"msg\": \"Ngắt kết nối đến nhà cung cấp PaymentNow. Timeout exception\"}";
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
                case 0:
                    //Thẻ đã được sử dụng
                    return (int)ResponseCode.TransactionFailed;
                case -33:
                    //Product ID not exists or blocked
                    return (int)ResponseCode.PartnerNotExistsNotActive;
                case -44:
                //Telco (provider) not exists or not active
                case -9:
                //Provider invalid
                case 70:
                //Provider system busy
                case 97:
                    //Provider system is not support
                    return (int)ResponseCode.ProviderNotFound;
                case 98:
                    //Provider system closed temporary
                    return (int)ResponseCode.SystemMaintain;
                case -55:
                //Signature wrong
                case 140:
                    //Invalid signature
                    return (int)ResponseCode.SignatureInvalid;
                case -66:
                    //Transaction id of partner duplicate
                    return (int)ResponseCode.TransactionDuplicate;
                case -6:
                    //Product ID invalid
                    return (int)ResponseCode.PartnerNotExistsNotActive;
                case -7:
                    //Card pin invalid
                    return (int)ResponseCode.CardCodeInvalid;
                case -8:
                //Card serial invalid
                case 51:
                    //Card serial is invalid
                    return (int)ResponseCode.CardSerialInvalid;
                case -10:
                    //Transaction ID of partner invalid
                    return (int)ResponseCode.TransactionInvalid;
                case 24:
                //Bad card data
                case 52:
                //Card serial and pin is not match
                case 53:
                    //Card serial or pin is incorrect
                    return (int)ResponseCode.CardFormatInvalid;
                case 2:
                    //Transaction timeout
                    return (int)ResponseCode.TransactionTimeout;
                case 3:
                //System error
                case 7:
                case 8:
                case 10:
                case 12:
                case 56:
                case 57:
                case 58:
                case 60:
                case 61:
                case 62:
                case 63:
                case 64:
                case 65:
                case 130:
                    //Process error
                    return (int)ResponseCode.SystemError;
                case 4:
                    //Card is incorrect
                    return (int)ResponseCode.CardFormatInvalid;
                case 5:
                //Wrong card input exceed.
                case 55:
                    //Card is block for 24 hours
                    return (int)ResponseCode.CardIsLocked;
                case 9:
                //charging chanel overload
                case 11:
                //Telco connector corrupted
                case 13:
                    //System busy
                    return (int)ResponseCode.PaymentConnectionFailed;
                case -2:
                    //Card is locked
                    return (int)ResponseCode.CardIsLocked;
                case -3:
                    //Card is expired
                    return (int)ResponseCode.CardHasExpired;
                case 50:
                    //Card is used or card do not exist
                    return (int)ResponseCode.CardSerialInvalid;
                case 59:
                    //Card is not activate
                    return (int)ResponseCode.CardNotActivated;
                case 99:
                    //Transaction pending
                    return (int)ResponseCode.TransactionReview;
                case 100:
                    //Params incorrect
                    return (int)ResponseCode.ParameterInvalid;
                case 110:
                    //User id not found
                    return (int)ResponseCode.AccountNotExists;
                case 120:
                    //Project id not found
                    return (int)ResponseCode.ProviderNotFound;
                case 150:
                    //Time error
                    return (int)ResponseCode.SystemError;
                case 160:
                    //Card id not found
                    return (int)ResponseCode.CardProviderInvalid;
                case 170:
                    //Balance not enough
                    return (int)ResponseCode.BalanceNotEnough;
                case -5001:
                    return (int)ResponseCode.PaymentConnectionFailed;
                case -5002:
                    return (int)ResponseCode.TransactionTimeout;
                default:
                    return (int)ResponseCode.UndefinedError;
            }
        }

    }

}
