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
            if (log.TransactionID > 0)
            {
                log.Respone = RemoveAllHtmlTags(log.Respone);
                if (log.Respone.Length > 500)
                    log.Respone = log.Respone.Substring(0, 500);


                string KeyCache = string.Format("{0}:{1}", "LogBank", log.TransactionID);
                var result = DataCaching.GetCache<List<LogInfo>>(KeyCache);
                if (result == null)
                {
                    result = new List<LogInfo>();

                }
                result.Add(log);
                DataCaching.SetCache(KeyCache, result, 86400 * 3);
            }
        }
        public static void LogBankCash(LogInfo log)
        {
            if (log.TransactionID > 0)
            {
                log.Respone = RemoveAllHtmlTags(log.Respone);
                if (log.Respone.Length > 500)
                    log.Respone = log.Respone.Substring(0, 500);

                string KeyCache = string.Format("{0}:{1}", "LogBankCash", log.TransactionID);
                var result = DataCaching.GetCache<List<LogInfo>>(KeyCache);
                if (result == null)
                {
                    result = new List<LogInfo>();

                }
                result.Add(log);
                DataCaching.SetCache(KeyCache, result, 86400 * 3);
            }

        }
        public static List<LogInfo> GetLogBank(long Id)
        {

            string KeyCache = string.Format("{0}:{1}", "LogBank", Id);
            return DataCaching.GetCache<List<LogInfo>>(KeyCache);
        }
        public static List<LogInfo> GetLogBankCash(long Id)
        {

            string KeyCache = string.Format("{0}:{1}", "LogBankCash", Id);
            return DataCaching.GetCache<List<LogInfo>>(KeyCache);
        }
        public static void SetAmountLitmit(int Amount)
        {

            string KeyCache = string.Format("{0}:{1}", "AmountLitmit", DateTime.Now.ToString("yyddMM"));
            //var result = DataCaching.GetCache<string>(KeyCache);
            //if (result == null)
            //{
            //    result = "0";

            //}
            //var data = int.Parse(result) + Amount;
            DataCaching.SetCache(KeyCache, Amount.ToString(), 86400);
        }
        public static int GetAmountLitmit()
        {

            string KeyCache = string.Format("{0}:{1}", "AmountLitmit", DateTime.Now.ToString("yyddMM"));

            var result = DataCaching.GetCache<string>(KeyCache);
            if (result == null)
            {
                result = "0";

            }
            return int.Parse(result);
        }
    }

    public class LogInfo
    {

        public long TransactionID { get; set; }
        public string Url { get; set; }
        public string Request { get; set; }
        public string Respone { get; set; }
        public DateTime LogTime { get; set; }
    }
}
