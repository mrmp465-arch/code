using System;
using System.Web;
using System.Collections;
using ServiceStack.Redis;
using System.Collections.Generic;
using System.Configuration;
using Newtonsoft.Json;
using System.Threading;
using System.Text.RegularExpressions;

namespace Libs.Utils
{

    public class DataCaching
    {
        public static BaseDataCaching Instance = new RedisCaching();
        public static object GetCacheObject(string CacheKey)
        {
            try
            {
                return Instance.GetCache(CacheKey);
            }
            catch (Exception ex)
            {
                //FileExceptionHandler.Handle(ex, "CacheUtils", "GetCache:" + CacheKey);
                return null;
            }
        }
        public static void SetCacheObject(string CacheKey, object value, int? Expire = null)
        {
            if (!Expire.HasValue)
            {
                Expire = Instance.OneDayExpire;
            }
            Instance.InsertCache(CacheKey, value, Instance.FiveMinuteExpire);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="CacheKey"></param>
        /// <returns></returns>
        public static List<T> GetCacheList<T>(string CacheKey)
        {
            try
            {
                var data = Instance.GetCache(CacheKey);
                List<T> result = null;
                if (data != null)
                {
                    result = JsonConvert.DeserializeObject<List<T>>(data.ToString());
                }
                return result;
            }
            catch (Exception ex)
            {
                //FileExceptionHandler.Handle(ex, "CacheUtils", "GetCacheList:" + CacheKey);
                return null;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="CacheKey"></param>
        /// <param name="lstValue"></param>
        /// <param name="Expire">FiveMinuteExpire</param>
        /// <returns></returns>
        public static List<T> SetCacheList<T>(string CacheKey, List<T> lstValue, int? Expire = null)
        {
            try
            {
                if (lstValue != null && lstValue.Count > 0)
                {
                    if (!Expire.HasValue)
                    {
                        Expire = Instance.OneDayExpire;
                    }
                    Instance.InsertCache(CacheKey, lstValue, Instance.FiveMinuteExpire);
                }
            }
            catch (Exception ex)
            {
                //FileExceptionHandler.Handle(ex, "CacheUtils", "SetCacheList:" + CacheKey);
            }
            return lstValue;
        }
        public static T GetCache<T>(string CacheKey) where T : class
        {
            try
            {
                var data = Instance.GetCache(CacheKey);
                if (data != null)
                {
                    return JsonConvert.DeserializeObject<T>(data.ToString());
                }
                return default(T);
            }
            catch (Exception ex)
            {
                //FileExceptionHandler.Handle(ex, "CacheUtils", "GetCache:" + CacheKey);
                return null;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="CacheKey"></param>
        /// <param name="result"></param>
        /// <param name="Expire">FiveMinuteExpire</param>
        /// <returns></returns>
        public static T SetCache<T>(string CacheKey, T result, int? Expire = null) where T : class
        {
            try
            {
                if (result != null)
                {
                    if (!Expire.HasValue)
                    {
                        Expire = Instance.OneDayExpire;
                    }
                    Instance.InsertCache(CacheKey, result, (int)Expire);
                }

                return result;
            }
            catch (Exception ex)
            {
                //FileExceptionHandler.Handle(ex, "CacheUtils", "SetCache:" + CacheKey);
                return null;
            }
        }
        public static void RemoveCache(string CacheKey)
        {
            try
            {
                Instance.RemoveCache(CacheKey);
            }
            catch (Exception ex)
            {
                //FileExceptionHandler.Handle(ex, "CacheUtils", "RemoveCache:" + CacheKey);
            }
        }
        public static void RemoveByPattern(string pattern)
        {
            try
            {
                Instance.RemoveByPattern(pattern);
            }
            catch (Exception ex)
            {
                //FileExceptionHandler.Handle(ex, "CacheUtils", "RemoveCache:" + CacheKey);
            }
           
        }
    }

        public class RedisCaching : BaseDataCaching
    {
       
        #region Properties and Contrur
        private PooledRedisClientManager _redisClientManager;
        public RedisCaching()
        {

            var config = new RedisClientManagerConfig
            {
                AutoStart = true,
                DefaultDb = Config.RedisDb,
                MaxReadPoolSize = 200,
                MaxWritePoolSize = 200 / 4 + 1
            };

            var readWriteHosts = new[] { string.Format("{0}:{1}", Config.RedisServer, Config.RedisPort) };
            var readOnlyHosts = new[] { string.Format("{0}:{1}", Config.RedisServer, Config.RedisPort) };
            _redisClientManager = new PooledRedisClientManager(readWriteHosts, readOnlyHosts, config)
            {
              //  ConnectTimeout = 100
            };
            //_redisClientManager.PoolTimeout = 100;
        }
        #endregion

        public override bool NotInCache(string cacheKey)
        {
            if (!Config.EnableDataCaching)
                return false;

            var cacheData = GetCache(cacheKey);
            if (cacheData != null)
                return false;
            return true;
        }

        public override object GetCache(string cacheKey)
        {
            if (!Config.EnableDataCaching) return null;
            IRedisClient client = null;
            try
            {
                client = _redisClientManager.GetReadOnlyClient();

                return (client != null) ? client.Get<object>(cacheKey) : null;
            }
            catch (Exception ex)
            {
                ////FileExceptionHandler.Handle(ex, "RedisCaching", "GetCache:Cachekey " + cacheKey);
                return null;
            }
            finally
            {
                if (client != null) _redisClientManager.DisposeClient((RedisNativeClient)client);
            }
        }

        public override Dictionary<string, object> GetCaches(List<string> cacheKeys)
        {
            var dic = new Dictionary<string, object>();
            foreach (string key in cacheKeys)
            {
                var value = GetCache(key);
                if (value != null)
                {
                    if (dic.ContainsKey(key)) dic.Remove(key);
                    dic.Add(key, value);
                }
            }
            return dic;
        }

        public override int GetExpireTime(string cacheKey)
        {
            try
            {
                int expireTime = MinDataCacheTime;
                return expireTime;
            }
            catch (Exception ex)
            {
                ////FileExceptionHandler.Handle(ex, "LocalDataCaching", "GetExpireTime:Cachekey" + cacheKey);
                return -1;
            }

        }

        public override bool InsertCache(string cacheKey, object data, int expireTime)
        {
            if (expireTime < 0)
            {
                expireTime = 60;
            }
            IRedisClient client = null;
            try
            {
                if (Config.EnableDataCaching)
                {
                    client = _redisClientManager.GetClient();
                    return (client != null) && client.Set(cacheKey, data, new TimeSpan(0, 0, expireTime));
                }
                return false;
            }
            catch (Exception ex)
            {
                ////FileExceptionHandler.Handle(ex, "LocalDataCaching", "InsertCache:Cachekey" + cacheKey);
                return false;
            }
            finally
            {
                if (client != null) _redisClientManager.DisposeClient((RedisNativeClient)client);
            }
        }
       
    

        public override bool RemoveCache(string cacheKey)
        {
            IRedisClient client = null;
            try
            {
                client = _redisClientManager.GetClient();
                return (client != null) && client.Remove(cacheKey);
            }
            catch (Exception ex)
            {
                ////FileExceptionHandler.Handle(ex, "LocalDataCaching", "RemoveCache:Cachekey" + cacheKey);
                return false;
            }
            finally
            {
                if (client != null) _redisClientManager.DisposeClient((RedisNativeClient)client);
            }
        }

        public override bool RemoveAllCache()
        {
            IRedisClient client = null;
            try
            {
                client = _redisClientManager.GetClient();
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                if (client != null) _redisClientManager.DisposeClient((RedisNativeClient)client);
            }
        }
        /// <summary>
        /// Gõ bỏ cache theo mẫu khóa cache
        /// </summary>
        /// <param name="pattern">Mẫu khóa cache</param>
        public override void RemoveByPattern(string pattern)
        {
            var listKey = GetListKeyByPattern(pattern);
            foreach (var key in listKey)
                RemoveCache(key);
        }
        /// <summary>
        /// Lấy một danh sách các cache data bằng một mẫu khóa cache
        /// </summary>
        /// <param name="pattern">Mẫu khóa cache</param>
        /// <returns>Danh sách key phù hợp</returns>
        public List<string> GetListKeyByPattern(string pattern)
        {
            IRedisClient client = null;
            try
            {
                client = _redisClientManager.GetReadOnlyClient();

                var regex = new Regex(pattern, RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase);
                var listKey = new List<string>();
                var ce = client.GetAllKeys();
                foreach (var item in ce)
                {
                    if (regex.IsMatch(item))
                        listKey.Add(item);
                }
                return listKey;
            }
            catch (Exception ex)
            {
                // FileExceptionHandler.Handle(ex);
                return new List<string>();
            }
            finally
            {
                if (client != null) _redisClientManager.DisposeClient((RedisNativeClient)client);
            }
        }
       
    } 
    public abstract class BaseDataCaching
    {
        public int DefaultExpire = -1;
        public int NeverExpire = 0;
        public int TenSecondsDataCacheTime = 10;
        public int MinDataCacheTime = 60 / 2;
        public int OneMinuteExpire = 60 * 2;
        public int FiveMinuteExpire = 300;
        public int OneHourExpire = 3600;
        public int OneDayExpire = 86400;
        public static string SiteId = "";
        public static string DebugCache = "false";


        public abstract bool NotInCache(string cacheKey);
        public abstract object GetCache(string cacheKey);
        public abstract Dictionary<string, object> GetCaches(List<string> cacheKeys);
        public abstract int GetExpireTime(string cacheKey);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <param name="data"></param>
        /// <param name="expireTime">Thời gian hết hạn tính theo giây</param>
        /// <returns></returns>
        public abstract bool InsertCache(string cacheKey, object data, int expireTime);
        public abstract bool RemoveCache(string cacheKey); 
        public abstract bool RemoveAllCache();
        public abstract void RemoveByPattern(string pattern);
    }
    public static class Config
    {
        #region Redis Cache
        public static string RedisServer
        {
            get
            {
                var str = ConfigurationManager.AppSettings["RedisServer"];
                return string.IsNullOrEmpty(str) ? "" : str;
            }
        }
        public static int RedisPort
        {
            get
            {
                var str = ConfigurationManager.AppSettings["RedisPort"];
                return string.IsNullOrEmpty(str) ? 0 : int.Parse(str);
            }
        }

        public static int RedisDb
        {
            get
            {
                var str = ConfigurationManager.AppSettings["RedisDB"];
                return string.IsNullOrEmpty(str) ? 0 : int.Parse(str);
            }
        }

        public static bool EnableDataCaching
        {
            get
            {
                var str = ConfigurationManager.AppSettings["EnableDataCaching"];
                bool result;
                if (bool.TryParse(str, out result)) return result;
                return false;
            }
        }
        #endregion
    }


    public class OTPRedisCaching : BaseDataCaching
    {

        #region Properties and Contrur
        private PooledRedisClientManager _redisClientManager;
        public OTPRedisCaching()
        {

            var config = new RedisClientManagerConfig
            {
                AutoStart = true,
                DefaultDb = 2,
                MaxReadPoolSize = 200,
                MaxWritePoolSize = 200 / 4 + 1
            };

            var readWriteHosts = new[] { string.Format("{0}:{1}", Config.RedisServer, Config.RedisPort) };
            var readOnlyHosts = new[] { string.Format("{0}:{1}", Config.RedisServer, Config.RedisPort) };
            _redisClientManager = new PooledRedisClientManager(readWriteHosts, readOnlyHosts, config)
            {
                //  ConnectTimeout = 100
            };
            //_redisClientManager.PoolTimeout = 100;
        }
        #endregion

        public override bool NotInCache(string cacheKey)
        {
            if (!Config.EnableDataCaching)
                return false;

            var cacheData = GetCache(cacheKey);
            if (cacheData != null)
                return false;
            return true;
        }

        public override object GetCache(string cacheKey)
        {
            if (!Config.EnableDataCaching) return null;
            IRedisClient client = null;
            try
            {
                client = _redisClientManager.GetReadOnlyClient();

                return (client != null) ? client.Get<object>(cacheKey) : null;
            }
            catch (Exception ex)
            {
                ////FileExceptionHandler.Handle(ex, "RedisCaching", "GetCache:Cachekey " + cacheKey);
                return null;
            }
            finally
            {
                if (client != null) _redisClientManager.DisposeClient((RedisNativeClient)client);
            }
        }

        public override Dictionary<string, object> GetCaches(List<string> cacheKeys)
        {
            var dic = new Dictionary<string, object>();
            foreach (string key in cacheKeys)
            {
                var value = GetCache(key);
                if (value != null)
                {
                    if (dic.ContainsKey(key)) dic.Remove(key);
                    dic.Add(key, value);
                }
            }
            return dic;
        }

        public override int GetExpireTime(string cacheKey)
        {
            try
            {
                int expireTime = MinDataCacheTime;
                return expireTime;
            }
            catch (Exception ex)
            {
                ////FileExceptionHandler.Handle(ex, "LocalDataCaching", "GetExpireTime:Cachekey" + cacheKey);
                return -1;
            }

        }

        public override bool InsertCache(string cacheKey, object data, int expireTime)
        {
            if (expireTime < 0)
            {
                expireTime = 60;
            }
            IRedisClient client = null;
            try
            {
                if (Config.EnableDataCaching)
                {
                    client = _redisClientManager.GetClient();
                    return (client != null) && client.Set(cacheKey, data, new TimeSpan(0, 0, expireTime));
                }
                return false;
            }
            catch (Exception ex)
            {
                ////FileExceptionHandler.Handle(ex, "LocalDataCaching", "InsertCache:Cachekey" + cacheKey);
                return false;
            }
            finally
            {
                if (client != null) _redisClientManager.DisposeClient((RedisNativeClient)client);
            }
        }



        public override bool RemoveCache(string cacheKey)
        {
            IRedisClient client = null;
            try
            {
                client = _redisClientManager.GetClient();
                return (client != null) && client.Remove(cacheKey);
            }
            catch (Exception ex)
            {
                ////FileExceptionHandler.Handle(ex, "LocalDataCaching", "RemoveCache:Cachekey" + cacheKey);
                return false;
            }
            finally
            {
                if (client != null) _redisClientManager.DisposeClient((RedisNativeClient)client);
            }
        }

        public override bool RemoveAllCache()
        {
            IRedisClient client = null;
            try
            {
                client = _redisClientManager.GetClient();
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                if (client != null) _redisClientManager.DisposeClient((RedisNativeClient)client);
            }
        }
        /// <summary>
        /// Gõ bỏ cache theo mẫu khóa cache
        /// </summary>
        /// <param name="pattern">Mẫu khóa cache</param>
        public override void RemoveByPattern(string pattern)
        {
            var listKey = GetListKeyByPattern(pattern);
            foreach (var key in listKey)
                RemoveCache(key);
        }
        /// <summary>
        /// Lấy một danh sách các cache data bằng một mẫu khóa cache
        /// </summary>
        /// <param name="pattern">Mẫu khóa cache</param>
        /// <returns>Danh sách key phù hợp</returns>
        public List<string> GetListKeyByPattern(string pattern)
        {
            IRedisClient client = null;
            try
            {
                client = _redisClientManager.GetReadOnlyClient();

                var regex = new Regex(pattern, RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase);
                var listKey = new List<string>();
                var ce = client.GetAllKeys();
                foreach (var item in ce)
                {
                    if (regex.IsMatch(item))
                        listKey.Add(item);
                }
                return listKey;
            }
            catch (Exception ex)
            {
                // FileExceptionHandler.Handle(ex);
                return new List<string>();
            }
            finally
            {
                if (client != null) _redisClientManager.DisposeClient((RedisNativeClient)client);
            }
        }

    }
    public class OTPDataCaching
    {
        public static BaseDataCaching Instance = new OTPRedisCaching();
        public static object GetCacheObject(string CacheKey)
        {
            try
            {
                return Instance.GetCache(CacheKey);
            }
            catch (Exception ex)
            {
                //FileExceptionHandler.Handle(ex, "CacheUtils", "GetCache:" + CacheKey);
                return null;
            }
        }
        public static void SetCacheObject(string CacheKey, object value, int? Expire = null)
        {
            if (!Expire.HasValue)
            {
                Expire = Instance.OneDayExpire;
            }
            Instance.InsertCache(CacheKey, value, Instance.FiveMinuteExpire);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="CacheKey"></param>
        /// <returns></returns>
        public static List<T> GetCacheList<T>(string CacheKey)
        {
            try
            {
                var data = Instance.GetCache(CacheKey);
                List<T> result = null;
                if (data != null)
                {
                    result = JsonConvert.DeserializeObject<List<T>>(data.ToString());
                }
                return result;
            }
            catch (Exception ex)
            {
                //FileExceptionHandler.Handle(ex, "CacheUtils", "GetCacheList:" + CacheKey);
                return null;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="CacheKey"></param>
        /// <param name="lstValue"></param>
        /// <param name="Expire">FiveMinuteExpire</param>
        /// <returns></returns>
        public static List<T> SetCacheList<T>(string CacheKey, List<T> lstValue, int? Expire = null)
        {
            try
            {
                if (lstValue != null && lstValue.Count > 0)
                {
                    if (!Expire.HasValue)
                    {
                        Expire = Instance.OneDayExpire;
                    }
                    Instance.InsertCache(CacheKey, lstValue, Instance.FiveMinuteExpire);
                }
            }
            catch (Exception ex)
            {
                //FileExceptionHandler.Handle(ex, "CacheUtils", "SetCacheList:" + CacheKey);
            }
            return lstValue;
        }
        public static T GetCache<T>(string CacheKey) where T : class
        {
            try
            {
                var data = Instance.GetCache(CacheKey);
                if (data != null)
                {
                    return JsonConvert.DeserializeObject<T>(data.ToString());
                }
                return default(T);
            }
            catch (Exception ex)
            {
                //FileExceptionHandler.Handle(ex, "CacheUtils", "GetCache:" + CacheKey);
                return null;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="CacheKey"></param>
        /// <param name="result"></param>
        /// <param name="Expire">FiveMinuteExpire</param>
        /// <returns></returns>
        public static T SetCache<T>(string CacheKey, T result, int? Expire = null) where T : class
        {
            try
            {
                if (result != null)
                {
                    if (!Expire.HasValue)
                    {
                        Expire = Instance.OneDayExpire;
                    }
                    Instance.InsertCache(CacheKey, result, (int)Expire);
                }

                return result;
            }
            catch (Exception ex)
            {
                //FileExceptionHandler.Handle(ex, "CacheUtils", "SetCache:" + CacheKey);
                return null;
            }
        }
        public static void RemoveCache(string CacheKey)
        {
            try
            {
                Instance.RemoveCache(CacheKey);
            }
            catch (Exception ex)
            {
                //FileExceptionHandler.Handle(ex, "CacheUtils", "RemoveCache:" + CacheKey);
            }
        }
        public static void RemoveByPattern(string pattern)
        {
            try
            {
                Instance.RemoveByPattern(pattern);
            }
            catch (Exception ex)
            {
                //FileExceptionHandler.Handle(ex, "CacheUtils", "RemoveCache:" + CacheKey);
            }

        }
    }
}
