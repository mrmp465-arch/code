using Lib.Captcha;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web.Script.Serialization;
using System.Web;
using Libs.API;
using Libs.Utils;

namespace UnitTest
{
    [TestClass]
    public class WebVinaTest
    {

        [TestMethod]
        public void PostAppForm()
        {
            try
            {
                string ck = string.Empty;
                //string urlCap = "http://naptien.vinaphone.com.vn/Home/GenerateCaptcha";
                //var cap = GetForm(urlCap, ref ck);
                //Console.WriteLine(cap);

                ck = "wvim10nlvi1jkrfxsk4kok5z";

                string uri = "http://naptien.vinaphone.com.vn/Home/AddPrepaid";
                string parameters = "PhoneNum=0912440644&MaThe=18769219977993&Answer=Ht6mQW";
                var res = PostForm(uri, parameters, ck);
                Console.WriteLine(res);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }

        }

        private static string GetForm(string url, ref string aspck)
        {
            Uri uri = new Uri(url);
            var request = (HttpWebRequest)WebRequest.Create(uri);
            request.CookieContainer = new CookieContainer();
            var response = (HttpWebResponse)request.GetResponse();
            Stream data = response.GetResponseStream();
            string html = String.Empty;
            using (StreamReader sr = new StreamReader(data))
            {
                html = sr.ReadToEnd();
            }
            CookieCollection ckc = response.Cookies;
            Cookie ck = ckc["ASP.NET_SessionId"];
            if (null != ck)
            {
                aspck = ck.Value;
            }
            return html;

        }

        public static string PostForm(string url, string parameters, string cookie)
        {

            var uri = new Uri(url);
            var req = (HttpWebRequest)WebRequest.Create(uri);
            req.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
            req.Method = "POST";
            req.CookieContainer = new CookieContainer();
            req.CookieContainer.Add(new Cookie("ASP.NET_SessionId", cookie, "/", uri.Host));
            //req.CookieContainer.Add(new Cookie("TS012b5e7b", "014743ac897e51c60c57a0fb3f2128a55b81b75e77419aee9e1ab1a275d81d22dc8b93b1e2f93283f6c0aa353a9d4e0971e1d1c2c938d3d7ed1fee75135d78089e1ed051c2", "/", uri.Host));
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

        [TestMethod()]
        public void Topup()
        {
            var mobile = "0912440644";
            var pin = "18767163678480";
            var session = new DeCaptchaWebVina().DeCaptcha();
            string uri = "http://naptien.vinaphone.com.vn/Home/AddPrepaid";
            string parameters = string.Format("PhoneNum={0}&MaThe={1}&Answer={2}", mobile, pin, session.Value);
            var res = PostForm(uri, parameters, session.SessionId);
            Console.WriteLine(res);


            //JavaScriptSerializer serializer = new JavaScriptSerializer();
            //Console.WriteLine(serializer.Serialize(new DeCaptchaWebVina().DeCaptcha()));
        }

        [TestMethod()]
        public void ReportCaptChar()
        {
            var session = new Lib.Captcha.Anticaptcha.AnticaptchaService().ReportIncorrectImageCaptcha(251155145);
            Console.WriteLine(session);


            //JavaScriptSerializer serializer = new JavaScriptSerializer();
            //Console.WriteLine(serializer.Serialize(new DeCaptchaWebVina().DeCaptcha()));
        }
       
    }

}

