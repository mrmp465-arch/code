using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Net;

namespace Libs.Utils
{
    public class IPAddress
    {
        public static string Get()
        {
            if (!string.IsNullOrEmpty(HttpContext.Current.Request.ServerVariables["HTTP_REFERER"]))
            {
                string s = HttpContext.Current.Request.ServerVariables["HTTP_REFERER"];
                if (Uri.IsWellFormedUriString(s, UriKind.Absolute))
                {
                    s = new Uri(s).Host;
                }
                IPHostEntry host = Dns.GetHostEntry(s);
                return host.AddressList[0].ToString();
            }

            string ip = null;
            ip = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (string.IsNullOrEmpty(ip))
            {
                ip = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }
            if (ip.Contains(","))
                return ip.Split(',')[0].Trim();
            return ip;
        }

        public static string GetNoReferer()
        {
            if (!string.IsNullOrEmpty(HttpContext.Current.Request.ServerVariables["HTTP_REFERER"]))
            {
                string s = HttpContext.Current.Request.ServerVariables["HTTP_REFERER"];
                if (Uri.IsWellFormedUriString(s, UriKind.Absolute))
                {
                    s = new Uri(s).Host;
                }
                IPHostEntry host = Dns.GetHostEntry(s);
                return host.AddressList[0].ToString();
            }

            string ip = null;
            ip = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (string.IsNullOrEmpty(ip))
            {
                ip = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }

            return ip;
        }
        
    }
}
