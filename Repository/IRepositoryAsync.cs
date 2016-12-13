using System.Threading;
using System.Threading.Tasks;
using DAL;

namespace Repository
{
    public interface IRepositoryAsync<TEntity> : IRepository<TEntity> where TEntity : Entity
    {
        Task<TEntity> FindAsync(params object[] keyValues);
        Task<TEntity> FindAsync(CancellationToken cancellationToken, params object[] keyValues);
        Task<bool> DeleteAsync(params object[] keyValues);
        Task<bool> DeleteAsync(CancellationToken cancellationToken, params object[] keyValues);
    }
}
