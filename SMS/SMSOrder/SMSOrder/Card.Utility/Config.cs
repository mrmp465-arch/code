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

namespace SMS.Utility
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

        public static int VTTMax
        {
            get
            {

                return int.Parse(GetAppsetting("VTTMax"));


            }
        }
        public static int VMSMax
        {
            get
            {

                return int.Parse(GetAppsetting("VMSMax"));


            }
        }
        public static int MaxVNM
        {
            get
            {

                return int.Parse(GetAppsetting("MaxVNM"));


            }
        }
        public static int VNPBoundary
        {
            get
            {

                return int.Parse(GetAppsetting("VNPBoundary"));


            }
        }
        public static int PercentUsePort
        {
            get
            {

                return int.Parse(GetAppsetting("PercentUsePort"));


            }
        }
        public static int TimePortHold
        {
            get
            {

                return int.Parse(GetAppsetting("TimePortHold"));


            }
        }
        public static int MaxUsingVTT
        {
            get
            {

                return int.Parse(GetAppsetting("MaxUsingVTT"));


            }
        }
        public static int MaxUsingVMS
        {
            get
            {

                return int.Parse(GetAppsetting("MaxUsingVMS"));


            }
        }
        public static int MaxUsingVNP
        {
            get
            {

                return int.Parse(GetAppsetting("MaxUsingVNP"));


            }
        }
        public static int MaxVTT
        {
            get
            {

                return int.Parse(GetAppsetting("MaxVTT"));


            }
        }
        public static int MaxVNP
        {
            get
            {

                return int.Parse(GetAppsetting("MaxVNP"));


            }
        }
        public static int MaxVMS
        {
            get
            {

                return int.Parse(GetAppsetting("MaxVMS"));


            }
        }
        public static string EndSMSText
        {
            get
            {
                return ConfigurationManager.AppSettings["EndSMSText"] ?? ".|;|!|@|*|'";
            }
        }
        public static string sn
        {
            get
            {
                return ConfigurationManager.AppSettings["sn"] ?? "";
            }
        }
        public static List<string> TestPort
        {
            get
            {
                var s= ConfigurationManager.AppSettings["TestPort"] ?? "";
                return s.Split(',').ToList();
            }
        }
        public static List<string> BlackKeys
        {
            get
            {
                var s = ConfigurationManager.AppSettings["BlackKeys"] ?? "";
                return s.Split(',').ToList();
            }
        }
        public static List<string> StudenVNP
        {
            get
            {
                var s = ConfigurationManager.AppSettings["StudenVNP"] ?? "";
                return s.Split(',').ToList();
            }
        }
        public static List<string> OnlyVNP
        {
            get
            {
                var s = ConfigurationManager.AppSettings["OnlyVNP"] ?? "";
                return s.Split(',').ToList();
            }
        }
        public static List<string> PriorityVNP
        {
            get
            {
                var s = ConfigurationManager.AppSettings["PriorityVNP"] ?? "";
                return s.Split(',').ToList();
            }
        }
        public static string GetAppsetting(string appSettingName)
        {
            return ConfigurationManager.AppSettings[appSettingName] ?? string.Empty;
        }
        #endregion






        #region Utility
        public static bool SetAppSettingValue(string key, string value, string path)
        {
            try
            {

                // Open App.Config of executable
                System.Configuration.Configuration config =

                  WebConfigurationManager.OpenWebConfiguration(path);

                AppSettingsSection appSettings = (AppSettingsSection)config.GetSection("appSettings");

                // Add an Application Setting.
                appSettings.Settings.Remove(key);
                appSettings.Settings.Add(key, value);

                // Save the configuration file.
                config.Save(ConfigurationSaveMode.Full);

                // Force a reload of a changed section.
                ConfigurationManager.RefreshSection("appSettings");

                return true;
            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                return false;
            }


        }
        public static string GetIP()
        {
            string IP = "";
            return IP;
            //if (HttpContext.Current.Request.ServerVariables["HTTP_CLIENT_IP"] != null)
            //{
            //    IP = HttpContext.Current.Request.ServerVariables["HTTP_CLIENT_IP"];
            //    return IP;
            //}

            //if (HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != null)
            //{
            //    IP = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            //    return IP;
            //}

            //if (HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED"] != null)
            //{
            //    IP = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED"];
            //    return IP;
            //}

            //if (HttpContext.Current.Request.ServerVariables["HTTP_X_CLUSTER_CLIENT_IP"] != null)
            //{
            //    IP = HttpContext.Current.Request.ServerVariables["HTTP_X_CLUSTER_CLIENT_IP"];
            //    return IP;
            //}

            //if (HttpContext.Current.Request.ServerVariables["HTTP_FORWARDED_FOR"] != null)
            //{
            //    IP = HttpContext.Current.Request.ServerVariables["HTTP_FORWARDED_FOR"];
            //    return IP;
            //}

            //if (HttpContext.Current.Request.ServerVariables["HTTP_FORWARDED"] != null)
            //{
            //    IP = HttpContext.Current.Request.ServerVariables["HTTP_FORWARDED"];
            //    return IP;
            //}

            //if (IP == "")
            //{
            //    IP = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            //}
            //return IP;
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
                NLogLogger.PublishException(ex);
                return false;
            }
        }
        #endregion Utility


    }
}
