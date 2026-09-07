using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedCache.WinServiceCommon.Provider.Cache;

namespace Libs.Utils
{
    public class SharedCache
    {
        public static void Add(string key, object obj)
        {
            IndexusDistributionCache.SharedCache.Add(key, obj);
        }

        public static object Get(string key)
        {
            return IndexusDistributionCache.SharedCache.Get(key);
        }

        public static void Remove(string key)
        {
            IndexusDistributionCache.SharedCache.Remove(key);
        }
    }
}
