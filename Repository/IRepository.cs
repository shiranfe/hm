using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using DAL;

namespace Repository
{
    public interface IRepository<TEntity> where TEntity : class
    {
        TEntity FindById(object id);
        void InsertGraph(TEntity entity);
        void Update(TEntity entity);
        void Remove(object id);
        void Remove(TEntity entity);
        void Insert(TEntity entity);
        RepositoryQuery<TEntity> Query();

     
        TEntity SingleOrDefault(Expression<Func<TEntity, bool>> whereCondition);
        void Add(TEntity entity);
        void UpdateMulty(IList<TEntity> list);
        List<TEntity> ToList(Expression<Func<TEntity, bool>> whereCondition);  
        List<TEntity> ToList();
        IQueryable<TEntity> GetQueryable();
        IQueryable<TEntity> Where(Expression<Func<TEntity, bool>> whereCondition);

        IQueryable<TEntity> GetQueryableFresh();
       
    }
}
