using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Xml.Serialization;

namespace SMS.Utility
{
    public static class DataCaching
    {
        public static object GetCache(string key)
        {
            return HttpRuntime.Cache.Get(key);
        }


        public static object GetHashCache(string hashKey, object param)
        {
            Hashtable hashtable1 = (Hashtable)GetCache(hashKey);
            if (hashtable1 == null)
            {
                return null;
            }
            if (hashtable1[param] == null)
            {
                return null;
            }
            return hashtable1[param];
        }


        public static void InsertCache(string key, object data, double expireTime)
        {
            if (expireTime == 0)
            {
                expireTime = 3;
            }

            RemoveCache(key);
            HttpRuntime.Cache.Insert(key, data, null, DateTime.Now.AddMinutes(expireTime), Cache.NoSlidingExpiration);
        }


        public static void InsertCacheNoExpireTime(string key, object data)
        {
            if (data != null)
            {
                HttpRuntime.Cache.Insert(key, data);
            }
        }

        public static bool RemoveCache(string key)
        {
            if (string.IsNullOrEmpty(key)) return true;

            if (HttpContext.Current.Cache[key] != null)
            {
                HttpRuntime.Cache.Remove(key);
                return true;
            }
            return false;
        }


        public static void SetHashCache(string hashKey, object param, double expireTime, object data)
        {
            Hashtable hashtable1 = (Hashtable)GetCache(hashKey);
            if (hashtable1 == null)
            {
                hashtable1 = new Hashtable();
                hashtable1.Add(param, data);
                InsertCache(hashKey, hashtable1, expireTime);
            }
            else if ((hashtable1[param] == null) && !hashtable1.ContainsKey(param))
            {
                hashtable1.Add(param, data);
            }
        }
    }
}
