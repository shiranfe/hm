using System;
using Microsoft.Practices.Unity;
using Repository;

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

        private bool _disposed;

        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    //Rep.Dispose();
                }
            }
            _disposed = true;
        }

        public new void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
