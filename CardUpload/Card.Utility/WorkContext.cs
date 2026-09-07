using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace  Card.Utility
{
    public static class WorkContext
    {
       
        
        /// <summary>
        /// Lấy giá trị sesstion bằng key
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>Trả về dạng string</returns>
        public static object GetSessionKey(string key)
        {
            if (HttpContext.Current != null && HttpContext.Current.Session != null)
            {
                var sesstion = HttpContext.Current.Session[key];
                if (sesstion != null)
                    return sesstion;
            }
            return null;
        }
        /// <summary>
        /// Lấy giá trị sesstion bằng key
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>Trả về dạng string</returns>
        public static string GetSessionKeyToString(string key)
        {
            if (HttpContext.Current != null && HttpContext.Current.Session != null)
            {
                var sesstion = HttpContext.Current.Session[key];
                if (sesstion != null)
                    return sesstion as string;
            }
            return string.Empty;
        }
        /// <summary>
        /// Sét giá trị vào sesstion
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Giá trị</param>
        public static void SetSessionKey(string key, object value)
        {
            if (HttpContext.Current != null && HttpContext.Current.Session != null)
                HttpContext.Current.Session[key] = value;
        }
        /// <summary>
        /// Bỏ giá trị của sesstion
        /// </summary>
        /// <param name="key">Key</param>
        public static void RemoveSessionKey(string key)
        {
            if (HttpContext.Current != null && HttpContext.Current.Response != null && HttpContext.Current.Session[key] != null)
                HttpContext.Current.Session.Remove(key);
        }
    }
}
