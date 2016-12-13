using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace DAL.Interfaces
{
    public interface IBasicDAL<T>
    {
     
        /// <summary>
        /// Get a selected extiry by the object primary key ID
        /// </summary>
        /// <param name="id">Primary key ID</param>
        T SingleOrDefault(Expression<Func<T, bool>> whereCondition);

        /// <summary> 
        /// Add entity to the repository 
        /// </summary> 
        /// <param name="entity">the entity to add</param> 
        /// <returns>The added entity</returns> 
        void Add(T entity);

        /// <summary>
        /// Save changes to update evntity
        /// </summary>
        /// <param name="entity"></param>
        void Update(T entity);
        /// <summary> 
        /// Mark entity to be deleted within the repository 
        /// </summary> 
        /// <param name="entity">The entity to delete</param> 
        void Remove(T entity);

      

        /// <summary> 
        /// Load the DAL using a linq expression filter
        /// </summary> 
        /// <typeparam name="E">the entity type to load</typeparam> 
        /// <param name="where">where condition</param> 
        /// <returns>the loaded entity</returns> 
        List<T> ToList(Expression<Func<T, bool>> whereCondition);

        /// <summary>
        /// Get all the element of this repository
        /// </summary>
        /// <returns></returns>
        List<T> ToList();

        /// <summary> 
        /// Query DAL from the repository that match the linq expression selection criteria
        /// </summary> 
        /// <typeparam name="E">the entity type to load</typeparam> 
        /// <param name="where">where condition</param> 
        /// <returns>the loaded entity</returns> 
        IQueryable<T> GetQueryable();

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <returns></returns>
        //object GetSingleValue(Expression<Func<T, bool>> whereCondition, Expression<Func<T, object>> val);
        
        
        void ExecuteSqlCommand(string sqlString);
    }

}
