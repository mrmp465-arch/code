using Card.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace Card.Data.Api
{
    public class Utilities
    {
       
       

        public static string HttpRequestGet(string url,string accessToken="")
        {
            var result = string.Empty;
            try
            {
                ServicePointManager.Expect100Continue = false;
                ServicePointManager.DefaultConnectionLimit = 64;
                ServicePointManager.MaxServicePointIdleTime = 500;
                var request = (HttpWebRequest)WebRequest.Create(url);
               
                //request.Proxy = null;
                request.UseDefaultCredentials = true;
                request.CookieContainer = new CookieContainer();
                request.Method = "GET";
                request.ContentType = "application/x-www-form-urlencoded";
              
                //request.ContentType = "text/xml; encoding='utf-8'";
                request.KeepAlive = false;
                request.AllowAutoRedirect = true;
                request.Proxy = null;
                request.ServicePoint.Expect100Continue = false;
                request.Timeout = 6868;
                if (!string.IsNullOrEmpty(accessToken))
                {
                    request.Headers.Add("Authorization", "bearer " + accessToken);
                }
                var response = (HttpWebResponse)request.GetResponse();
                var stream = response.GetResponseStream();
                if (stream != null)
                {
                    result = new StreamReader(stream).ReadToEnd();
                }
            }
            catch (WebException exception)
            {
                var responseStream = exception.Response?.GetResponseStream();

                if (responseStream != null)
                {
                    using (var reader = new StreamReader(responseStream))
                    {
                        result = reader.ReadToEnd();
                        //return result;
                    }
                }
                NLogLogger.Info(result);
                NLogLogger.Info(url);
                result = null;
            }

            return result;
        }
        //byte[] array = Encoding.ASCII.GetBytes(input);


        public static string HttpRequestPostDataJson(string url, string content, string accessToken = "")
        {
            string result = string.Empty;

            //try
            //{
            ServicePointManager.Expect100Continue = false;
            ServicePointManager.DefaultConnectionLimit = 64;
            ServicePointManager.MaxServicePointIdleTime = 500;
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.UseDefaultCredentials = true;
            request.CookieContainer = new CookieContainer();
            request.Method = "POST";
            // request.ContentType = "application/x-www-form-urlencoded";
            request.Timeout = 26868;
            request.ContentType = "application/json;charset=UTF-8";
            request.AllowAutoRedirect = true;

            request.Proxy = null;
            request.ServicePoint.Expect100Continue = false;
            request.Timeout = 6868;
            //request.Accept = "JSON";
            StreamWriter requestWriter = new StreamWriter(request.GetRequestStream());
            requestWriter.Write(content);
            requestWriter.Close();

            if (!string.IsNullOrEmpty(accessToken))
            {
                request.Headers.Add("Authorization", "bearer " + accessToken);
            }
            var response = (HttpWebResponse)request.GetResponse();
            using (var stream = response.GetResponseStream())
            {
                if (stream != null)
                {
                    result = new StreamReader(stream).ReadToEnd();
                    stream.Close();
                    response.Close();
                }
            }
            //}
            //catch (Exception ex)
            //{
            //    NLogLogger.PublishException(ex);
            //    result = null;
            //}
            return result;
        }

        public static string HttpRequestPostData(string url, string content,string accessToken="")
        {
            string result = string.Empty;

            //try
            //{
                ServicePointManager.Expect100Continue = false;
                ServicePointManager.DefaultConnectionLimit = 64;
                ServicePointManager.MaxServicePointIdleTime = 500;
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.UseDefaultCredentials = true;
                request.CookieContainer = new CookieContainer();
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.AllowAutoRedirect = true;

                request.Proxy = null;
                request.ServicePoint.Expect100Continue = false;
                request.Timeout = 6868;
                //request.Accept = "JSON";
                StreamWriter requestWriter = new StreamWriter(request.GetRequestStream());
                requestWriter.Write(content);
                requestWriter.Close();

                if (!string.IsNullOrEmpty(accessToken))
                {
                    request.Headers.Add("Authorization", "bearer "+ accessToken);
                }
                var response = (HttpWebResponse)request.GetResponse();
                using (var stream = response.GetResponseStream())
                {
                    if (stream != null)
                    {
                        result = new StreamReader(stream).ReadToEnd();
                        stream.Close();
                        response.Close();
                    }
                }
            //}
            //catch (Exception ex)
            //{
            //    NLogLogger.PublishException(ex);
            //    result = null;
            //}
            return result;
        }

        public static string HttpRequestPost(string url, byte[] postData)
        {
            string result = string.Empty;
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.UseDefaultCredentials = true;
                request.CookieContainer = new CookieContainer();
                request.Method = "POST";
               // request.ContentType = "raw";
                request.ContentType = "application/x-www-form-urlencoded";
                //request.ContentType = "text/xml; encoding='utf-8'";
                request.ContentLength = postData.Length;
                //myRequest.KeepAlive = false;
                request.AllowAutoRedirect = true;

                using (var stream = request.GetRequestStream())
                {
                    stream.Write(postData, 0, postData.Length);
                    stream.Close();
                }
                var response = (HttpWebResponse)request.GetResponse();
                using (var stream = response.GetResponseStream())
                {
                    if (stream != null)
                    {
                        result = new StreamReader(stream).ReadToEnd();
                        stream.Close();
                        response.Close();
                    }
                }
            }
            catch (Exception)
            {
                result = null;
            }
            return result;
        }
    }
}
