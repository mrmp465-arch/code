using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace Libs.Utils
{
    public static class LogCache
    {
        public static string RemoveAllHtmlTags(object input)
        {
            var output = string.Format("{0}", input);
            if (string.IsNullOrEmpty(output)) return string.Empty;

            var htmlRegex = new Regex("<.*?>", RegexOptions.Compiled);
            output = HttpUtility.HtmlDecode(output);
            output = htmlRegex.Replace(output, string.Empty);
            return output.Replace("&", "%26").Trim();
        }
        public static void LogBank(LogInfo log)
        {
            log.Respone = RemoveAllHtmlTags(log.Respone);
            string KeyCache = string.Format("{0}:{1}", "LogBank", log.TransactionID);
            var result = DataCaching.GetCache<List<LogInfo>>(KeyCache);
            if (result == null)
            {
                result = new List<LogInfo>();
                
            }
            result.Add(log);
            DataCaching.SetCache(KeyCache, result, 86400*3);
        }
        public static void LogBankCash(LogInfo log)
        {
            log.Respone = RemoveAllHtmlTags(log.Respone);
            string KeyCache = string.Format("{0}:{1}", "LogBankCash", log.TransactionID);
            var result = DataCaching.GetCache<List<LogInfo>>(KeyCache);
            if (result == null)
            {
                result = new List<LogInfo>();

            }
            result.Add(log);
            DataCaching.SetCache(KeyCache, result, 86400*3);
        }
    }

    public  class LogInfo
    {
       
        public long TransactionID { get; set; }
        public string Url { get; set; }
        public string Request { get; set; }
        public string Respone { get; set; }
        public DateTime LogTime { get; set; }
    }
}
