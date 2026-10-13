using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Text;
using System.Web.Configuration;

namespace CMS.Utility
{
    public class Config
    {
        #region[ConnectionString]
        public static string MainConnectionString
        {
            get
            {

                return GetConnStr("MainConnectionString");


            }
        }

        public static string GetConnStr(string name)
        {
            try
            {
                var rijndaelKey = new RijndaelEnhanced(GetAppsetting("SiteName"), "@1B2c3D4e5F6g7H8");
                return rijndaelKey.Decrypt(ConfigurationManager.ConnectionStrings[name].ConnectionString);
            }
            catch
            {
                return string.Empty;
            }
        }

        #endregion



        #region[Configuration]





        public static string GetAppsetting(string appSettingName)
        {
            return ConfigurationManager.AppSettings[appSettingName] ?? string.Empty;
        }
        #endregion






        #region Utility

        public static string GetIP()
        {
            string IP = "";
            //return IP;
            if (HttpContext.Current.Request.ServerVariables["HTTP_CLIENT_IP"] != null)
            {
                IP = HttpContext.Current.Request.ServerVariables["HTTP_CLIENT_IP"];
                return IP;
            }

            if (HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != null)
            {
                IP = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                return IP;
            }

            if (HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED"] != null)
            {
                IP = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED"];
                return IP;
            }

            if (HttpContext.Current.Request.ServerVariables["HTTP_X_CLUSTER_CLIENT_IP"] != null)
            {
                IP = HttpContext.Current.Request.ServerVariables["HTTP_X_CLUSTER_CLIENT_IP"];
                return IP;
            }

            if (HttpContext.Current.Request.ServerVariables["HTTP_FORWARDED_FOR"] != null)
            {
                IP = HttpContext.Current.Request.ServerVariables["HTTP_FORWARDED_FOR"];
                return IP;
            }

            if (HttpContext.Current.Request.ServerVariables["HTTP_FORWARDED"] != null)
            {
                IP = HttpContext.Current.Request.ServerVariables["HTTP_FORWARDED"];
                return IP;
            }

            if (IP == "")
            {
                IP = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }
            return IP;
        }

        public static bool CheckXSSInput(string input)
        {
            try
            {
                var listdangerousString = new List<string> { "<applet", "<body", "<embed", "<frame", "<script", "<frameset", "<html", "<iframe", "<img", "<style", "<layer", "<link", "<ilayer", "<meta", "<object" };
                if (string.IsNullOrEmpty(input)) return false;
                foreach (var dangerous in listdangerousString)
                {
                    if (input.Trim().ToLower().IndexOf(dangerous) >= 0) return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                //NLogLogger.PublishException(ex);
                return false;
            }
        }
        #endregion Utility


    }
}
