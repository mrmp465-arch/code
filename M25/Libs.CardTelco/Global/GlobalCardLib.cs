using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;
using Libs.Utils;

namespace Libs.CardTelco.Global
{
    public class CardRequest
    {
        public string req { get; set; }
        // Loại thẻ cần sử dụng. Các giá trị hợp lệ gồm: MOBI, VINA, VT, VCOIN, GATE, GM, BIT, ZING
        public string serial { get; set; } // Serial thẻ sử dụng
        public string cardcode { get; set; } // Mã số bí mật thẻ sử dụng
        public string telco { get; set; } // Mã giao dịch trên hệ thống của Đối tác (duy nhất, tối đa 30 ký tự).
        public string amount { get; set; } // Mã đối tác trên hệ thống gạch thẻ
        public string cpcode { get; set; } // Mật khẩu của Đối tác trên hệ thống gạch thẻ.

    }


    public class CardResponse
    {
        public string req { get; set; } //Trạng thái xử lý : 01 là Thành công, còn lại xem bảng mã lỗi
        public string status { get; set; } //Diễn giải kết quả
        public int amount { get; set; } //Serial thẻ sử dụng
        public string sign { get; set; }
    }

    public class CardRequestResponse
    {
        public int code { get; set; }
        public string message { get; set; }
    }
    public class GlobalCardLib
    {

        public static string PostData(string uri, string postData)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(uri);
                request.ContentType = "application/json; charset=utf-8";
                request.Method = "POST"; //GET
                request.Timeout = 30000;
                byte[] postDatabytes = Encoding.UTF8.GetBytes(postData);
                request.ContentLength = postDatabytes.Length;
                Stream dataStream = request.GetRequestStream();
                dataStream.Write(postDatabytes, 0, postDatabytes.Length);

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

        public static string HttpPostRawData(string url, string data)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "application/x-www-form-urlencoded";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {


                streamWriter.Write(data);
                streamWriter.Flush();
                streamWriter.Close();

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    return result;
                }
            }


        }
    }
}
