using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Web;

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
        public static string GetV4()
        {
            var request = HttpContext.Current?.Request;
            if (request == null)
                return string.Empty;

            // IP kết nối trực tiếp đến IIS
            string ip = request.ServerVariables["REMOTE_ADDR"];

            if (string.IsNullOrWhiteSpace(ip))
                return string.Empty;

            System.Net.IPAddress address;

            if (System.Net.IPAddress.TryParse(ip, out address))
            {
                // IPv4-mapped IPv6
                if (address.IsIPv4MappedToIPv6)
                    return address.MapToIPv4().ToString();

                // IPv4 hoặc IPv6 thực
                return address.ToString();
            }

            return string.Empty;
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
