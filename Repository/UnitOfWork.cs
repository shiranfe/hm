using System;
using System.Collections;
using DAL;
using System.Data.Entity;
using Microsoft.Practices.Unity;

namespace Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private  IDbContext _context;

        private bool _disposed;
        private Hashtable _repositories;

        public UnitOfWork([Dependency] IDbContext dbContext)
        {
            _context = dbContext; 
        }
         

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void SaveChanges()
        {
            //try
            //{
                _context.SaveChanges();
           
            //}
            //catch (Exception e)
            //{
            //    /** if had error in save cahnges -clear all repositoris for reload.
            //        when will reload deffective DbSet in Repository(IDbContext context)
            //            will DbSet.Local.Clear();
            //    DID PROBLEMS - REMOVE IT FOR NOW
            //    */
            //  //  _repositories = new Hashtable();
            //    throw e;
            //}


        } 

        public virtual void Dispose(bool disposing)
        {
            if (!_disposed)
                if (disposing)
                    _context.Dispose();

            _disposed = true;
        }




        public IRepository<T> Repository<T>() where T : class
        {
            if (_repositories == null)
                _repositories = new Hashtable();

            var type = typeof(T).Name;

            if (!_repositories.ContainsKey(type))
            {
                var repositoryType = typeof(Repository<>);

                var repositoryInstance =
                    Activator.CreateInstance(repositoryType
                            .MakeGenericType(typeof(T)), _context);

                _repositories.Add(type, repositoryInstance);
            }

            return (IRepository<T>)_repositories[type];
        }
    }
}