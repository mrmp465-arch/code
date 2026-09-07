using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Newtonsoft.Json;

namespace SMS.Utility
{
    public class RedisCaching
    {
      

        public static void Add ( string key, string data,int exp )
        {
            try
            {
                CacheUtils.SetCacheObject( key, data,exp);
            }
            catch ( Exception e )
            {
                NLogLogger.PublishException(e);
            }

        }

      
        public static void Remove ( string key )
        {
            try
            {
                CacheUtils.RemoveCache( key );
            }
            catch ( Exception e )
            {
                NLogLogger.PublishException(e);
            }

        }
        public static void RemoveGroup(string key)
        {
            try
            {
                CacheUtils.RemoveCacheGroup(key);
            }
            catch (Exception e)
            {
                NLogLogger.PublishException(e);
            }

        }

        public static void Flush ()
        {
            try
            {
                CacheUtils.Flush();
            }
            catch ( Exception e )
            {
                NLogLogger.PublishException(e);
            }

        }

        public static object GetData ( string key )
        {
            try
            {
                return CacheUtils.GetCacheObject( key );
            }
            catch ( Exception e )
            {
                NLogLogger.PublishException(e);
                return null;
            }

        }

     

       
    }
}
