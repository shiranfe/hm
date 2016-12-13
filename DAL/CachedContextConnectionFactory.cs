using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EFCachingProvider;
using EFCachingProvider.Caching;

namespace DAL
{
    public class CachedContextConnectionFactory : System.Data.Entity.Infrastructure.IDbConnectionFactory
    {
        public DbConnection CreateConnection(string nameOrConnectionString)
        {
            var providerInvariantName = "System.Data.SqlClient";

            var wrappedConnectionString = "wrappedProvider=" +
                providerInvariantName + ";" +
                nameOrConnectionString;

            return new EFCachingConnection
            {
                ConnectionString = wrappedConnectionString,
                CachingPolicy = CachingPolicy.CacheAll,
                Cache = EntityCache.Instance
            };
        }
    }
}
