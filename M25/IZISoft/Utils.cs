using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using Libs.Utils;

namespace IZISoft
{
    public class Utils
    {
        public static string GetCache(string key)
        {
            string KeyCache = string.Format("{0}:{1}", "izi", key);
            try
            {
                var result = DataCaching.GetCache<string>(KeyCache);
                return result;
            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "IZISoft", "GetCache", "Exception", KeyCache, e.Message });
                return null;
            }
        }

        public static string SetCache(string key, string data)
        {
            string KeyCache = string.Format("{0}:{1}", "izi", key);
            try
            {

                var result = DataCaching.SetCache(KeyCache, data);
                return result;

            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "IZISoft", "SetCache", "Exception", e.Message });
                return null;
            }
        }

        public static void RemoveCache(string key)
        {
            string KeyCache = string.Format("{0}:{1}", "izi", key);
            try
            {
                DataCaching.RemoveCache(KeyCache);
            }
            catch (Exception e)
            {
                NLogLogger.Info(new string[] { "IZISoft", "RemoveCache", "Exception", e.Message });
            }
        }
    }
}