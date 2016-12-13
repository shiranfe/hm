using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EFCachingProvider.Caching;

namespace DAL
{
    public class EntityCache
    {
        private static InMemoryCache cacheInstance;
        private static object lockObject = new object();

        public static InMemoryCache Instance
        {
            get
            {
                if (cacheInstance == null)
                {
                    lock (lockObject)
                    {
                        if (cacheInstance == null)
                            cacheInstance = new InMemoryCache();
                    }
                }

                return cacheInstance;
            }
        }
    }
}
