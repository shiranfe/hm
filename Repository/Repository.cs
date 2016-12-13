
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using DAL;


namespace Repository
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        internal IDbContext Context;
        internal IDbSet<TEntity> DbSet;

        public Repository(IDbContext context)
        {
            Context = context;
            DbSet = context.Set<TEntity>();
            /** if had error in save cahnges - will clear it here*/
           // DbSet.Local.Clear();
        }

         
        public TEntity SingleOrDefault(Expression<Func<TEntity, bool>> whereCondition)
        {
            return DbSet.Where(whereCondition).FirstOrDefault<TEntity>();
        }

        public void Add(TEntity entity)
        {
            DbSet.Add(entity);
            
        }

        public virtual void Insert(TEntity entity)
        {
            DbSet.Attach(entity);
        }

        
        public void UpdateMulty(IList<TEntity> list)
        {
            foreach (var item in list)
            {

                DbSet.Attach(item);
                //DbSet.Add(item);
            }
            
        }

        public List<TEntity> ToList()
        {
            return DbSet.ToList();
        }

        public List<TEntity> ToList(Expression<Func<TEntity, bool>> whereCondition)
        {
            return DbSet.Where(whereCondition).ToList();
        }

        public IQueryable<TEntity> GetQueryable()
        {
            return DbSet.AsQueryable<TEntity>();
        }

        public IQueryable<TEntity> GetQueryableFresh()
        {

            HMErpEntities context = new HMErpEntities();
           return context.Set<TEntity>().AsQueryable<TEntity>();
            
          
        }

        public IQueryable<TEntity> Where(Expression<Func<TEntity, bool>> whereCondition)
        {
            return DbSet.Where(whereCondition).AsQueryable<TEntity>();

        }



        public virtual void Update(TEntity entity)
        {
          
            var updated = DbSet.Attach(entity);
            Context.Entry(entity).State = EntityState.Modified;

            //Context.SaveChanges();
        }

        public virtual void Remove(TEntity entity)
        {
            DbSet.Attach(entity);
            DbSet.Remove(entity);

        }

        /////////////



        public virtual TEntity FindById(object id)
        {
            return DbSet.Find(id);
        }

        public virtual void InsertGraph(TEntity entity)
        {
            DbSet.Add(entity);
        }

      

        public virtual void Remove(object id)
        {
            var entity = DbSet.Find(id);
            //var objectState = entity as IObjectState;
            //if (objectState != null) 
            //    objectState.State = ObjectState.Deleted;
            Remove(entity);

           
        }

     

       
        public virtual RepositoryQuery<TEntity> Query()
        {
            var repositoryGetFluentHelper =
                new RepositoryQuery<TEntity>(this);

            return repositoryGetFluentHelper;
        }

        internal IQueryable<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>,
                IOrderedQueryable<TEntity>> orderBy = null,
            List<Expression<Func<TEntity, object>>>
                includeProperties = null,
            int? page = null,
            int? pageSize = null)
        {
            IQueryable<TEntity> query = DbSet;
            
            if (includeProperties != null)
                includeProperties.ForEach(i => { query = query.Include(i); });

            if (filter != null)
                query = query.Where(filter);

            if (orderBy != null)
                query = orderBy(query);

            if (page != null && pageSize != null)
                query = query
                    .Skip((page.Value - 1)*pageSize.Value)
                    .Take(pageSize.Value);

            return query;
        }

       
    }
}