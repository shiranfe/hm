namespace Repository
{
    public interface IUnitOfWork
    {
        void Dispose();
        void SaveChanges();
        void Dispose(bool disposing);
        IRepository<T> Repository<T>() where T : class;
    }
}