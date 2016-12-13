using System.Data.Entity;

namespace Entities
{
    public interface IDataContext 
    {
        DbSet<T> Set<T>() where T : Entity;
        
        int SaveChanges();
        void SyncObjectState(object entity);

        void Dispose();
    }
}