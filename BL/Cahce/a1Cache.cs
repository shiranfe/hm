using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;
using Repository;
using Microsoft.Practices.Unity;

namespace BL
{
    public class A1Repository : Cache
   {
       private readonly IUnitOfWork _uow;
        
        //private CacheModule Rep;

       public A1Repository([Dependency]IUnitOfWork uow)
       {
           _uow = uow;
       }

        /*****************         Methods              ******************/



        /*****************         Dispose              ******************/

        private bool _disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    //Rep.Dispose();
                }
            }
            this._disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
