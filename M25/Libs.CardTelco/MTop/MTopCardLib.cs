using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;
using Libs.Utils;

namespace Libs.CardTelco.MTop
{
    public class CardRequest
    {
        public string issuer { get; set; }
        // Loại thẻ cần sử dụng. Các giá trị hợp lệ gồm: MOBI, VINA, VT, VCOIN, GATE, GM, BIT, ZING
        public string cardSerial { get; set; } // Serial thẻ sử dụng
        public string cardCode { get; set; } // Mã số bí mật thẻ sử dụng
        public string accountNo { get; set; } // Mã giao dịch trên hệ thống của Đối tác (duy nhất, tối đa 30 ký tự).
        public string walletType { get; set; } // Mã đối tác trên hệ thống gạch thẻ
        public string serviceId { get; set; } // Mật khẩu của Đối tác trên hệ thống gạch thẻ.

    }


    public class CardResponse
    {
        public string code { get; set; } //Trạng thái xử lý : 01 là Thành công, còn lại xem bảng mã lỗi
        public string message { get; set; } //Diễn giải kết quả
        public CardData data { get; set; } //Serial thẻ sử dụng
    }

    public class CardData
    {
        public long requestId { get; set; }
        public string accountNo { get; set; }
        public string price { get; set; }
        public float amount { get; set; }
        public string rate { get; set; }
    }



    public class MTopCardLib
    {

        public static CardResponse ProcessCard(CardRequest cardRequest, string serviceURL)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string responseData = PostData(serviceURL, serializer.Serialize(cardRequest));
            NLogLogger.Info(new string[] { "MTop", "MTopResponse raw", responseData });
            return serializer.Deserialize<CardResponse>(responseData);
        }

        public static string login(string url)
        {
            string responseData = GetData(url);
            return responseData;
        }

        private static string PostData(string uri, string postData)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(uri);
                request.ContentType = "application/json; charset=utf-8";
                request.Method = "POST"; //GET
                request.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 5.1; en-US; rv:1.8.1.7) Gecko/20070914 Firefox/2.0.0.7";
                request.Timeout = 30000;
                System.Net.ServicePointManager.ServerCertificateValidationCallback += delegate { return true; };
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
                    return "{\"code\":\"-5001\",\"message\":\"không kết nối được nhà cung cấp.\",\"data\":{\"requestId\":0,\"accountNo\":\"0\",\"price\":\"0\",\"amount\":0,\"rate\":null}}";
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
                    return "{\"code\":\"-5002\",\"message\":\"Ngắt kết nối đến nhà cung cấp. Timeout exception\",\"data\":{\"requestId\":0,\"accountNo\":\"0\",\"price\":\"0\",\"amount\":0,\"rate\":null}}";
                }

                throw;

            }
        }

        private static string GetData(string url)
        {
            try
            {
                string html = string.Empty;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                request.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 5.1; en-US; rv:1.8.1.7) Gecko/20070914 Firefox/2.0.0.7";
                System.Net.ServicePointManager.ServerCertificateValidationCallback += delegate { return true; };
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    using (Stream stream = response.GetResponseStream())
                    using (StreamReader sr = new StreamReader(stream))
                    {
                        html = sr.ReadToEnd();
                    }
                }
                return html;
            }
            catch (Exception)
            {
                NLogLogger.Info(new string[] { "MTop", "MTop URLRequest", url });
                throw;
            }

        }
    }
}
