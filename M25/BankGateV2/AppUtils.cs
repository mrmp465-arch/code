using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using Libs.Utils;

namespace BankGateV2
{
    public class AppUtils
    {
        public AppUtils()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        public enum ParamMethod
        {
            Get = 1,
            Post = 2,
            Delete = 3,
            PostAndGet = 4
        }

        //public static int Request(string name)
        //{
        //    try
        //    {
        //        return Convert.ToInt32(HttpContext.Current.Request[name]);
        //    }
        //    catch
        //    {
        //        return 0;
        //    }
        //}

        public static int GetParam(string key, int defaultvalue = 0, ParamMethod method = ParamMethod.Get, bool useDefault = false)
        {
            if (HttpContext.Current == null)
                return defaultvalue;

            try
            {
                if (useDefault) return defaultvalue;

                int outValue;
                var value = string.Empty;
                switch (method)
                {
                    case ParamMethod.Get:
                        value = HttpContext.Current.Request.QueryString[key];
                        break;
                    case ParamMethod.Post:
                        value = HttpContext.Current.Request.Form[key];
                        break;
                }
                return int.TryParse(value, out outValue) && outValue > 0 ? outValue : defaultvalue;
            }
            catch (Exception ex)
            {
               NLogLogger.Info(new string[] { "BankGate", "GetParam", ex.Message });
            }
            return defaultvalue;
        }

        public static long GetParam(string key, long defaultvalue = 0, ParamMethod method = ParamMethod.Get, bool useDefault = false)
        {
            if (HttpContext.Current == null)
                return defaultvalue;

            try
            {
                if (useDefault) return defaultvalue;

                long outValue;
                var value = string.Empty;
                switch (method)
                {
                    case ParamMethod.Get:
                        value = HttpContext.Current.Request.QueryString[key];
                        break;
                    case ParamMethod.Post:
                        value = HttpContext.Current.Request.Form[key];
                        break;
                }
                return long.TryParse(value, out outValue) && outValue > 0 ? outValue : defaultvalue;
            }
            catch (Exception ex)
            {
                NLogLogger.Info(new string[] { "BankGate", "GetParam", ex.Message });
            }
            return defaultvalue;
        }

        public static double GetParam(string key, double defaultvalue = 0.00, ParamMethod method = ParamMethod.Get, bool useDefault = false)
        {
            if (HttpContext.Current == null)
                return defaultvalue;

            try
            {
                if (useDefault) return defaultvalue;

                double outValue;
                var value = string.Empty;
                switch (method)
                {
                    case ParamMethod.Get:
                        value = HttpContext.Current.Request.QueryString[key];
                        break;
                    case ParamMethod.Post:
                        value = HttpContext.Current.Request.Form[key];
                        break;
                }
                return double.TryParse(value, out outValue) && outValue > 0 ? outValue : defaultvalue;
            }
            catch (Exception ex)
            {
                NLogLogger.Info(new string[] { "BankGate", "GetParam", ex.Message });
            }
            return defaultvalue;
        }

        public static decimal GetParam(string key, decimal defaultvalue = 0, ParamMethod method = ParamMethod.Get, bool useDefault = false)
        {
            if (HttpContext.Current == null)
                return defaultvalue;

            try
            {
                if (useDefault) return defaultvalue;

                decimal outValue;
                var value = string.Empty;
                switch (method)
                {
                    case ParamMethod.Get:
                        value = HttpContext.Current.Request.QueryString[key];
                        break;
                    case ParamMethod.Post:
                        value = HttpContext.Current.Request.Form[key];
                        break;
                }
                return decimal.TryParse(value, out outValue) && outValue > 0 ? outValue : defaultvalue;
            }
            catch (Exception ex)
            {
                NLogLogger.Info(new string[] { "BankGate", "GetParam", ex.Message });
            }
            return defaultvalue;
        }

        public static bool GetParam(string key, bool defaultvalue = false, ParamMethod method = ParamMethod.Get, bool useDefault = false)
        {
            try
            {
                bool outValue;
                string value = string.Empty;
                switch (method)
                {
                    case ParamMethod.Get:
                        value = HttpContext.Current.Request.QueryString[key];
                        break;
                    case ParamMethod.Post:
                        value = HttpContext.Current.Request.Form[key];
                        break;
                }
                return !bool.TryParse(value, out outValue) ? outValue : defaultvalue;
            }
            catch (Exception ex)
            {
                //Provider.LogService.Error(typeof(Parameters), ex.StackTrace, ex);
            }
            return defaultvalue;
        }

        public static DateTime GetParam(string key, DateTime defaultvalue, ParamMethod method = ParamMethod.Get, string formatDatetime = "yyyyMMddHHmmss", bool useDefault = false)
        {
            try
            {
                DateTime outValue;
                var value = string.Empty;
                switch (method)
                {
                    case ParamMethod.Get:
                        value = HttpContext.Current.Request.QueryString[key];
                        break;
                    case ParamMethod.Post:
                        value = HttpContext.Current.Request.Form[key];
                        break;
                }

                return DateTime.TryParseExact(value, formatDatetime, CultureInfo.InvariantCulture, DateTimeStyles.None, out outValue)
                    ? outValue : defaultvalue;
            }
            catch (Exception ex)
            {
                NLogLogger.Info(new string[] { "BankGate", "GetParam", ex.Message });
            }
            return defaultvalue;
        }

        public static string GetParam(string key, string defaultvalue = default(string), ParamMethod method = ParamMethod.Get, bool useDefault = false, bool userDecode = false)
        {
            try
            {
                if (string.IsNullOrEmpty(key)) return defaultvalue;
                var value = string.Empty;
                switch (method)
                {
                    case ParamMethod.Get:
                        value = HttpContext.Current.Request.QueryString[key];
                        break;
                    case ParamMethod.Post:
                        value = HttpContext.Current.Request.Form[key];
                        break;
                    case ParamMethod.PostAndGet:
                        value = HttpContext.Current.Request[key];
                        break;
                }
                return !string.IsNullOrEmpty(value) ? userDecode ? HttpUtility.UrlDecode(value) : value : defaultvalue;
            }
            catch (Exception ex)
            {
                NLogLogger.Info(new string[] { "BankGate", "GetParam", ex.Message });
            }
            return defaultvalue;
        }

        // Thông báo
        public static void Alert(Page page, string message)
        {
            ScriptManager.RegisterStartupScript(page, typeof(Page), "scriptkey", "window.setTimeout(\"alert('" + message + "')\",100);", true);
        }

        public static bool CheckAZ09(string s, int min, int max)
        {
            s = s.ToLower();
            if (s.Length < min || s.Length > max) return false;

            string data = "abcdefghijklmnopqrstuvwxyz0123456789";

            for (int i = 0; i < s.Length; i++)
            {
                if (data.IndexOf(s[i]) < 0) return false;
            }
            return true;
        }

        public static bool CheckRegular(string data, string pattern)
        {
            return new Regex(pattern).Match(data).Success;
        }


    }
}