using Libs.API;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace Libs.CardTelco.ABTPay
{
    public class CardResult
    {
        public string code { get; set; }
        public string message { get; set; }
        public string txn_id { get; set; }
        public int card_amount { get; set; }
        public int net_amount { get; set; }
    }

    public class CardRequest
    {
        public int merchant_id { get; set; }
        public string merchant_txn_id { get; set; }
        public string pin { get; set; }
        public string seri { get; set; }
        public string card_type { get; set; }
        public string checksum { get; set; }
    }

    public class RecheckRequest
    {
        public string merchant_txn_id { get; set; }
        public int merchant_id { get; set; }
        public string checksum { get; set; }
    }

    public class ABTCardLib
    {
        public static string createChecksum(ArrayList args, string secrectKey)
        {
            byte[] key = Encoding.ASCII.GetBytes(secrectKey);
            string tmp = "";
            foreach (string s in args)
            {
                tmp = tmp + "|" + s;
            }

            tmp = tmp.Substring(1);
            return hmacsha1(tmp, key);

        }

        public static string hmacsha1(string input, byte[] key)
        {
            HMACSHA1 myhmacsha1 = new HMACSHA1(key);
            byte[] byteArray = Encoding.ASCII.GetBytes(input);
            MemoryStream stream = new MemoryStream(byteArray);
            return myhmacsha1.ComputeHash(stream).Aggregate("", (s, e) => s + String.Format("{0:x2}", e), s => s);
        }

        public static string PostData(string uri, string postData)
        {

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                request.Timeout = 30000;
                request.ContentType = "application/json";
                request.Method = "POST"; //GET
                //request.Accept = "JSON";
                byte[] postDatabytes = Encoding.UTF8.GetBytes(postData);
                request.ContentLength = postDatabytes.Length;
                Stream dataStream = request.GetRequestStream();
                dataStream.Write(postDatabytes, 0, postDatabytes.Length);

                //using (Stream requestStream = request.GetRequestStream())
                //{
                //    byte[] postDatabytes = Encoding.UTF8.GetBytes(postData);
                //    requestStream.Write(postDatabytes, 0, postDatabytes.Length);
                //}

                var webResponse = request.GetResponse();
                dataStream = webResponse.GetResponseStream();
                if (dataStream == null)
                {
                    webResponse.Close();
                    return "{\"card_amount\":0,\"net_amount\":0,\"code\":\"-5001\",\"message\":\"không kết nối được nhà cung cấp ABTPay\",\"txn_id\":\"0\"}";
                }

                var sr = new StreamReader(dataStream);
                var response = sr.ReadToEnd().Trim();

                sr.Close();
                dataStream.Close();
                webResponse.Close();

                return response;
            }
            catch (WebException e)
            {
                if (e.Status == WebExceptionStatus.Timeout)
                {
                    //Handle timeout exception
                    return "{\"card_amount\":0,\"net_amount\":0,\"code\":\"-5002\",\"message\":\"Ngắt kết nối đến nhà cung cấp ABTPay. Timeout exception \",\"txn_id\":\"0\"}";
                }
                else
                {
                    throw;
                }
            }

        }

        public static int ConvertResponCode(string responseStatus)
        {
            switch (responseStatus)
            {
                case "2":
                    //Thẻ đã được sử dụng
                    return (int)ResponseCode.CardUsed;
                case "3":
                    //Thẻ đã bị khóa
                    return (int)ResponseCode.CardIsLocked;
                case "4":
                    //Thực hiện sai quá số lần cho phép
                    return (int)ResponseCode.TransactionLimit;
                case "5":
                    //Merchant không tồn tại
                    return (int)ResponseCode.ServiceNotExists;
                case "6":
                    //Mã giao dịch không tồn tại
                    return (int)ResponseCode.TransactionInvalid;
                case "7":
                    //Sai định dạng thông tin truyền vào
                    return (int)ResponseCode.ParameterInvalid;
                case "8":
                    //Đơn vị phát hành thẻ không tồn tại
                    return (int)ResponseCode.ServiceNotExists;
                case "9":
                    //Hệ thống của Đơn vị phát hành thẻ đang bận Session Timeout
                    return (int)ResponseCode.PaymentConnectionFailed;
                case "10":
                    //Hệ thống của Đơn vị phát hành thẻ đang bận Session Timeout
                    return (int)ResponseCode.PaymentConnectionFailed;
                case "11":
                    //Lỗi khi Đơn vị phát hành thẻ xử lý giao dịch (Lỗi phát sinh khi đơn vị phát hành thẻ đang xử lý giao dịch)
                    return (int)ResponseCode.SystemError;
                case "12":
                    //Merchant bị khóa
                    return (int)ResponseCode.ServiceIsLocked;
                case "13":
                    //Sai địa chỉ IP
                    return (int)ResponseCode.ParameterInvalid;
                case "14":
                    //Trùng mã giao dịch (transRef)
                    return (int)ResponseCode.TransactionDuplicate;
                case "15":
                    //Serial thẻ không hợp lệ
                    return (int)ResponseCode.CardSerialInvalid;
                case "16":
                    //Thẻ đã bị khóa
                    return (int)ResponseCode.CardIsLocked;
                case "17":
                    //Thẻ đang xử lý
                    return (int)ResponseCode.TransactionSuspicious;
                case "95":
                    //Lỗi database hệ thống của Telco
                    return (int)ResponseCode.TransactionFailed;
                case "96":
                    //Transaction lỗi
                    return (int)ResponseCode.TransactionFailed;
                case "98":
                    //Chờ xử lý
                    return (int)ResponseCode.TransactionFailed;
                case "-1":
                    //Nạp qua nhà cung cấp khác
                    return (int)ResponseCode.TransactionFailed;
                case "00":
                    //Không inset được vào DB
                    return (int)ResponseCode.TransactionFailed;
                case "99":
                    //Không inset được vào DB
                    return (int)ResponseCode.UndefinedError;
                case "-5001":
                    return (int)ResponseCode.PaymentConnectionFailed;
                case "-5002":
                    return (int)ResponseCode.TransactionTimeout;
                default:
                    return (int)ResponseCode.UndefinedError;
            }
        }

    }

}
