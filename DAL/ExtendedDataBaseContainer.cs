using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EFCachingProvider;
using EFCachingProvider.Caching;
using EFProviderWrapperToolkit;

using System.Data.Common;
using System.Data.Entity.Validation;
//using System.Data.EntityClient;
//using System.Data.Objects;

namespace DAL
{
    //[DbConfigurationType(typeof(DbConfiguration))]
    public class ExtendedDataBaseContainer : HMErpEntities, IDbContext
    {
        
        public new IDbSet<T> Set<T>() where T : class
        {
            return base.Set<T>();
        } 

        public override int SaveChanges()
        {
            return base.SaveChanges();
             
          
        }

    }
}
