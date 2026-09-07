using Newtonsoft.Json;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text.RegularExpressions;

namespace SMS.Utility
{
    /// <summary>
    /// dịch vụ cache của redis, có sử dụng dll ngoài vì add trên nug bị giới hạn số lần đọc gi cache
    /// </summary>
    public class RedisCacheService : ICacheService
    {
        private static PooledRedisClientManager _redisClientManager = null;
        private object lockObject = new object();
        protected PooledRedisClientManager RedisClientManager
        {
            get
            {
                lock (lockObject)
                {
                    if (_redisClientManager == null)
                    {
                        var rediscacheConnection = ConfigurationManager.AppSettings["rediscacheserver"] ?? "127.0.0.1|6379|0";
                        var dbConfigs = rediscacheConnection.Split('|');
                        var config = new RedisClientManagerConfig
                        {
                            AutoStart = true,
                            DefaultDb = int.Parse(dbConfigs[2]),
                            MaxReadPoolSize = 200,
                            MaxWritePoolSize = 200 / 4 + 1
                        };
                        var redisServer = $"{dbConfigs[0]}:{dbConfigs[1]}";
                        var readWriteHosts = new[] { redisServer };
                        var readOnlyHosts = new[] { redisServer };
                        _redisClientManager = new PooledRedisClientManager(readWriteHosts, readOnlyHosts, config);
                    }
                }
                return _redisClientManager;
            }
        }

        /// <summary>
        /// Lấy cache theo key
        /// </summary>
        /// <param name="key">Khóa cache</param>
        /// <returns>Giá trị cache</returns>
        public string Get(string key)
        {
            IRedisClient client = RedisClientManager.GetReadOnlyClient();
            var data = client.Get<string>(key);
            RedisClientManager.DisposeClient((RedisNativeClient)client);

            if (!string.IsNullOrEmpty(data))
            {
                //return JsonConvert.DeserializeObject<object>(data);
                return data;
            }
            return null;
        }

        /// <summary>
        /// Lấy cache theo key và ép giá trị về kiểu T nếu cache không null
        /// </summary>
        /// <typeparam name="T">Kiểu đối tượng</typeparam>
        /// <param name="key">Khóa cache</param>
        /// <returns>Trả về đối tượng kiểu T nếu cache không null, và trà về null nếu cache null</returns>
        public T Get<T>(string key)
        {
            IRedisClient client = RedisClientManager.GetReadOnlyClient();

            var data = client.Get<string>(key);
            RedisClientManager.DisposeClient((RedisNativeClient)client);

            if (!string.IsNullOrEmpty(data))
            {
                //return JsonConvert.DeserializeObject<T>(data);
                return JsonConvert.DeserializeObject<T>(data);
            }
            return default(T);
        }

        /// <summary>
        /// Lấy một danh sách các cache data bằng một mẫu khóa cache
        /// </summary>
        /// <param name="pattern">Mẫu khóa cache</param>
        /// <returns>Danh sách key phù hợp</returns>
        public List<string> GetListKeyByPattern(string pattern)
        {
            IRedisClient client = RedisClientManager.GetReadOnlyClient();

            var regex = new Regex(pattern, RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase);
            var listKey = new List<string>();
            var ce = client.GetAllKeys();
            foreach (var item in ce)
            {
                if (regex.IsMatch(item))
                    listKey.Add(item);
            }
            RedisClientManager.DisposeClient((RedisNativeClient)client);

            return listKey;
        }

        /// <summary>
        /// Lấy một danh sách key và giá trị của key
        /// </summary>
        /// <param name="pattern">Key Mẫu</param>
        /// <returns>Danh sách key và giá trị của key</returns>
        public Dictionary<string, object> GetKeyAndData(string pattern)
        {
            IRedisClient client = RedisClientManager.GetReadOnlyClient();
            var data = client.GetAll<string>(GetListKeyByPattern(pattern));
            Dictionary<string, object> dataReturn = new Dictionary<string, object>();
            if (data != null)
                foreach (var item in data)
                    dataReturn.Add(item.Key, JsonConvert.DeserializeObject<object>(item.Value));

            RedisClientManager.DisposeClient((RedisNativeClient)client);
            return dataReturn;
        }

        /// <summary>
        /// Thêm đối tượng vào cache theo khóa, với thời gian cache là mặc định
        /// </summary>
        /// <param name="key">Khóa cache</param>
        /// <param name="data">Giá trị</param>
        public void Set(string key, string data)
        {
            IRedisClient client = RedisClientManager.GetClient();

            //client.Set(key, JsonConvert.SerializeObject(data), new TimeSpan(0, 0, 60)); //Mặc định thời gian cache là 60s
            client.Set(key, data, new TimeSpan(0, 0, 60)); //Mặc định thời gian cache là 60s
            RedisClientManager.DisposeClient((RedisNativeClient)client);
        }

        /// <summary>
        /// Thêm đối tượng vào cache theo khóa, với thời gian cache được chỉ định
        /// </summary>
        /// <param name="key">Khóa cache</param>
        /// <param name="data">Giá trị</param>
        /// <param name="cacheTime">Thời gian cache tính theo giây</param>
        public void Set(string key, string data, int cacheTime)
        {
            if (data != null)
            {
                IRedisClient client = RedisClientManager.GetClient();
                client.Remove(key);
                //if(data.GetType().Equals(System.String))
                //client.Set(key, JsonConvert.SerializeObject(data), new TimeSpan(0, 0, cacheTime));
                client.Set(key, data, new TimeSpan(0, 0, cacheTime));
                RedisClientManager.DisposeClient((RedisNativeClient)client);
            }
        }

        /// <summary>
        /// Kiểm tra giá chị cache có được gán theo khóa cache
        /// </summary>
        /// <param name="key">Khóa cache</param>
        /// <returns>Giá trị kiểm tra</returns>
        public bool IsSet(string key)
        {
            IRedisClient client = RedisClientManager.GetClient();
            var isContainsKey = client.ContainsKey(key);
            RedisClientManager.DisposeClient((RedisNativeClient)client);
            return isContainsKey;
        }

        /// <summary>
        /// Gỡ bỏ cache theo khóa cache
        /// </summary>
        /// <param name="key">Khóa cache</param>
        public void Remove(string key)
        {
            IRedisClient client = RedisClientManager.GetClient();
            client.Remove(key);
            RedisClientManager.DisposeClient((RedisNativeClient)client);
        }

        /// <summary>
        /// Gõ bỏ cache theo mẫu khóa cache
        /// </summary>
        /// <param name="pattern">Mẫu khóa cache</param>
        public void RemoveByPattern(string pattern)
        {
            IRedisClient client = RedisClientManager.GetClient();
            var regex = new Regex(pattern, RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase);
            var listKey = new List<string>();
            var ce = client.GetAllKeys();
            foreach (var item in ce)
            {
                if (regex.IsMatch(item))
                    client.Remove(item);
            }
            RedisClientManager.DisposeClient((RedisNativeClient)client);
        }

        /// <summary>
        /// Xóa toàn bộ cache
        /// </summary>
        public void Clear()
        {
            IRedisClient client = RedisClientManager.GetClient();
            client.FlushAll();
            RedisClientManager.DisposeClient((RedisNativeClient)client);
        }
    }
}
