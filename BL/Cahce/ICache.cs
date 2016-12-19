using System;

namespace BL
{
   public class Cache : IDisposable
   {
      

        /*****************         Methods              ******************/



        /*****************         Dispose              ******************/

        private bool _disposed;

        protected virtual void Dispose(bool disposing)
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

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
