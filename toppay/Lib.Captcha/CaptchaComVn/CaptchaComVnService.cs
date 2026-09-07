using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Lib.Captcha.CaptchaComVn.ApiResponse;
using Libs.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Lib.Captcha.CaptchaComVn
{
    public class CaptchaComVnService: ICaptchaFactory
    {
        private static string merchant_key = "cd8c72e5a1ac71802aa503ab9cdabbfc411fa7e83b34c56df648a1ab483d2e8a"; //BB2D
        private static string ServiceUrl = ConfigurationManager.AppSettings["Service_Cap_Url"] ?? "http://iken.vn";

        public string ImageToText(string imageBase64, int type)
        {
            try
            {
                //string url = "https://captcha.com.vn/services/request";
                string url = ServiceUrl + "/services/request";
                var result = string.Empty;

                var dataPost = new Dictionary<string, string>();
                dataPost.Add("merchant_key", merchant_key);
                dataPost.Add("image", imageBase64);
                dataPost.Add("type", type.ToString());

                NLogLogger.Info(new string[] { "CaptchaComVn", "DeCaptcha", "Request", JsonConvert.SerializeObject(dataPost) });
                result = Task.Run(() => PostTask(url, dataPost)).Result;
                NLogLogger.Info(new string[] { "CaptchaComVn", "DeCaptcha", "Response", result });

                if (!string.IsNullOrEmpty(result))
                {
                    //result = result.Remove(0, 1);
                    var status = (bool)JObject.Parse(result)["success"];
                    if (status)
                    {
                        var txtCaptcha = (string)JObject.Parse(result)["predictions"]["captcha"];
                        return string.Format("{0}|{1}", txtCaptcha, 0);
                    }
                }


            }
            catch (Exception ex)
            {
                NLogLogger.Info(new string[] { "CaptchaComVn", "Exception", ex.Message });
            }

            return null;

        }

        public string ReportIncorrectImageCaptcha(string imageBase64, int type, string resultCap)
        {
            try
            {

                string url = ServiceUrl + "/services/feedback";
                var result = string.Empty;

                var dataPost = new Dictionary<string, string>();
                dataPost.Add("merchant_key", merchant_key);
                dataPost.Add("result ", resultCap);
                dataPost.Add("image", imageBase64);
                dataPost.Add("type", type.ToString());

                NLogLogger.Info(new string[] { "CaptchaComVn", "ReportIncorrectImageCaptcha", "Request", JsonConvert.SerializeObject(dataPost) });
                result = Task.Run(() => PostTask(url, dataPost)).Result;
                NLogLogger.Info(new string[] { "CaptchaComVn", "ReportIncorrectImageCaptcha", "Response", Regex.Unescape(result) });

            }
            catch (Exception ex)
            {
                NLogLogger.Info(new string[] { "CaptchaComVn", "ReportIncorrectImageCaptcha", "Exception", ex.Message });
            }

            return null;

        }

        public static async Task<string> PostTask(string url, Dictionary<string, string> postData)
        {
            try
            {

                var uri = new Uri(url);
                var httpContent = new FormUrlEncodedContent(postData);
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Accept", "*/*");
                client.Timeout = TimeSpan.FromMinutes(1);
                var response = await client.PostAsync(uri, httpContent);
                var byteArray = await response.Content.ReadAsByteArrayAsync();
                var responseContent = Encoding.UTF8.GetString(byteArray);
                return responseContent;
            }

            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "CaptchaComVn", "PostTask", "Exception", e.Message });

            }
            return string.Empty;
        }

        public string NoCaptchaTaskProxyless(string websiteURL, string websiteKey)
        {
            throw new NotImplementedException();
        }
    }
}
