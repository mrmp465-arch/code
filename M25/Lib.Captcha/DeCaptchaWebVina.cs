using Libs.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Lib.Captcha
{
    public class DeCaptchaWebVina
    {
        string urlCap = "http://naptien.vinaphone.com.vn/Home/GenerateCaptcha";
        public Captcha DeCaptcha()
        {
            try
            {
                //Get CaptCha in DB if null Call Captcha Service
                string ck = string.Empty;
                var capImageBase64 = GetForm(urlCap, ref ck);
                var captcha = new Lib.Captcha.Anticaptcha.AnticaptchaService().ImageToText(capImageBase64, 0);
                var cap = captcha.Split('|');
                return new Captcha()
                {
                    SessionId = ck,
                    Value = cap[0],
                    TaskId = Convert.ToInt32(cap[1])
                };
            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "DeCaptchaWebVina", "DeCaptcha", e.Message });
                return new Captcha();
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

    }

    //public class DeCaptcha
    //{
    //    public long Id { get; set; }
    //    public string SessionId { get; set; }
    //    public string Captcha { get; set; }
    //}
}
