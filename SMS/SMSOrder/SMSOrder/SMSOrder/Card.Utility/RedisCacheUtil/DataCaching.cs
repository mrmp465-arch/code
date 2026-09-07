using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMS.Utility
{
    public class DataCaching
    {
        public static ICacheService Instance;
        static DataCaching()
        {
            Instance = new RedisCacheService();
        }
    }
    public class CacheUtils
    {
        public static string GetCacheObject(string cacheKey)
        {
            try
            {
                return DataCaching.Instance.Get(cacheKey);
            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
                return null;
            }
        }
        public static void SetCacheObject(string CacheKey, string value, int Expire = 0)
        {
            if (Expire==0)
            {
                Expire = Constants.OneDayExpire*3;
            }
            DataCaching.Instance.Set(CacheKey, value, Expire);
        }



      
        
        public static void RemoveCache(string CacheKey)
        {
            try
            {
                DataCaching.Instance.Remove(CacheKey);
            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
            }
        }
        public static void RemoveCacheGroup(string CacheKey)
        {
            try
            {
                DataCaching.Instance.RemoveByPattern(CacheKey);
            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
            }
        }
        public static void Flush()
        {
            try
            {
                DataCaching.Instance.Clear();
            }
            catch (Exception ex)
            {
                NLogLogger.PublishException(ex);
            }
        }

    }
}
