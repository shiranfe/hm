using Common;
using DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
//using System.Data.Objects;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace DAL
{ 
   

    public class BasicDAL<T> : IBasicDAL<T>
        where T : class 
    {
        //internal DbContext Context;
        internal DbSet<T> DbSet;

        //protected IQueryable<T> quer;
        private static HMErpEntities db;

        public BasicDAL()
        {
            db = db ?? new HMErpEntities();   
            DbSet = DbSet ?? db.Set<T>();
            
        }



        #region BasicDAL Members

        public void Add(T entity)
        {
            DbSet.Add(entity);
            
        }

        public void AddMulty(IList<T> list)
        {
            foreach (var item in list)
            {
                DbSet.Add(item);
            }

            
        }

        public void Update(T entity)
        {
            DbSet.Attach(entity);
        }

      

        public void Remove(T entity)
        {
            DbSet.Remove(entity);
            
        }



        public List<T> ToList()
        {
            return DbSet.ToList();
        }

        public List<T> ToList(Expression<Func<T, bool>> whereCondition)
        {
            return DbSet.Where(whereCondition).ToList();
        }

        public T SingleOrDefault(Expression<Func<T, bool>> whereCondition)
        {
            
            return DbSet.Where(whereCondition).FirstOrDefault<T>();
        }

        //public object GetSingleValue(Expression<Func<T, bool>> whereCondition, Expression<Func<T, object>> val)
        //{

        //    return DbSet.Where(whereCondition).Select(val).Single();
        //}



   

        public IQueryable<T> GetQueryable()
        {
            return DbSet.AsQueryable<T>();
        }

        public IQueryable<T> Where(Expression<Func<T, bool>> whereCondition)
        {
            var q =  DbSet.Where(whereCondition).AsQueryable<T>();
            return q;
        }

        public void ExecuteSqlCommand(string sqlString)
        {
            db.Database.ExecuteSqlCommand(sqlString);
        }


        public void SaveChanges()
        {
            db.SaveChanges();
        }
        #endregion





    }

    
}
