using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
//using System.Data.Objects;
using System.Web;
using System.Threading;
using DAL;

namespace DAL.Infrastructure
{
    /// <summary>
    /// Manages the lifecycle of the EF's object context
    /// </summary>
    /// <remarks>Uses a context per http request approach or one per thread in non web applications</remarks>
    public static class ContextManager
    {
        #region Private Members

        // accessed via lock(_threadObjectContexts), only required for multi threaded non web applications
        private static readonly Hashtable _threadObjectContexts = new Hashtable();

        #endregion

        //public static IObjectSet<T> GetObjectSet<T>(T entity, string contextKey) 
        //    where T : class
        //{
        //    //return GetObjectContext(contextKey).CreateObjectSet<T>();
        //}

        /// <summary>
        /// Returns the active object context
        /// </summary>
        public static HMErpEntities GetObjectContext(string contextKey)
        {
            HMErpEntities HMErpEntities = GetCurrentObjectContext(contextKey);
            if (HMErpEntities == null) // create and store the object context
            {
                HMErpEntities = new HMErpEntities();
                StoreCurrentObjectContext(HMErpEntities, contextKey);
            }
            return HMErpEntities;
        }

        /// <summary>
        /// Gets the repository context
        /// </summary>
        /// <returns>An object representing the repository context</returns>
        public static object GetRepositoryContext(string contextKey)
        {
            return GetObjectContext(contextKey);
        }

        /// <summary>
        /// Sets the repository context
        /// </summary>
        /// <param name="repositoryContext">An object representing the repository context</param>
        public static void SetRepositoryContext(object repositoryContext, string contextKey)
        {
            if (repositoryContext == null)
            {
                RemoveCurrentObjectContext(contextKey);
            }
            else if (repositoryContext is HMErpEntities)
            {
                StoreCurrentObjectContext((HMErpEntities)repositoryContext, contextKey);
            }
        }


        #region Object Context Lifecycle Management

        /// <summary>
        /// gets the current object context 		
        /// </summary>
        private static HMErpEntities GetCurrentObjectContext(string contextKey)
        {
            HMErpEntities HMErpEntities = null;
            if (HttpContext.Current == null)
                HMErpEntities = GetCurrentThreadObjectContext(contextKey);
            else
                HMErpEntities = GetCurrentHttpContextObjectContext(contextKey);
            return HMErpEntities;
        }

        /// <summary>
        /// sets the current session 		
        /// </summary>
        private static void StoreCurrentObjectContext(HMErpEntities HMErpEntities, string contextKey)
        {
            if (HttpContext.Current == null)
                StoreCurrentThreadObjectContext(HMErpEntities, contextKey);
            else
                StoreCurrentHttpContextObjectContext(HMErpEntities, contextKey);
        }

        /// <summary>
        /// remove current object context 		
        /// </summary>
        private static void RemoveCurrentObjectContext(string contextKey)
        {
            if (HttpContext.Current == null)
                RemoveCurrentThreadObjectContext(contextKey);
            else
                RemoveCurrentHttpContextObjectContext(contextKey);
        }

        #region private methods - HttpContext related

        /// <summary>
        /// gets the object context for the current thread
        /// </summary>
        private static HMErpEntities GetCurrentHttpContextObjectContext(string contextKey)
        {
            HMErpEntities HMErpEntities = null;
            if (HttpContext.Current.Items.Contains(contextKey))
                HMErpEntities = (HMErpEntities)HttpContext.Current.Items[contextKey];
            return HMErpEntities;
        }

        private static void StoreCurrentHttpContextObjectContext(HMErpEntities HMErpEntities, string contextKey)
        {
            if (HttpContext.Current.Items.Contains(contextKey))
                HttpContext.Current.Items[contextKey] = HMErpEntities;
            else
                HttpContext.Current.Items.Add(contextKey, HMErpEntities);
        }

        /// <summary>
        /// remove the session for the currennt HttpContext
        /// </summary>
        private static void RemoveCurrentHttpContextObjectContext(string contextKey)
        {
            HMErpEntities HMErpEntities = GetCurrentHttpContextObjectContext(contextKey);
            if (HMErpEntities != null)
            {
                HttpContext.Current.Items.Remove(contextKey);
                HMErpEntities.Dispose();
            }
        }

        #endregion

        #region private methods - ThreadContext related

        /// <summary>
        /// gets the session for the current thread
        /// </summary>
        private static HMErpEntities GetCurrentThreadObjectContext(string contextKey)
        {
            HMErpEntities HMErpEntities = null;
            Thread threadCurrent = Thread.CurrentThread;
            if (threadCurrent.Name == null)
                threadCurrent.Name = contextKey;
            else
            {
                object threadObjectContext = null;
                lock (_threadObjectContexts.SyncRoot)
                {
                    threadObjectContext = _threadObjectContexts[contextKey];
                }
                if (threadObjectContext != null)
                    HMErpEntities = (HMErpEntities)threadObjectContext;
            }
            return HMErpEntities;
        }

        private static void StoreCurrentThreadObjectContext(HMErpEntities HMErpEntities, string contextKey)
        {
            lock (_threadObjectContexts.SyncRoot)
            {
                if (_threadObjectContexts.Contains(contextKey))
                    _threadObjectContexts[contextKey] = HMErpEntities;
                else
                    _threadObjectContexts.Add(contextKey, HMErpEntities);
            }
        }

        private static void RemoveCurrentThreadObjectContext(string contextKey)
        {
            lock (_threadObjectContexts.SyncRoot)
            {
                if (_threadObjectContexts.Contains(contextKey))
                {
                    HMErpEntities HMErpEntities = (HMErpEntities)_threadObjectContexts[contextKey];
                    if (HMErpEntities != null)
                    {
                        HMErpEntities.Dispose();
                    }
                    _threadObjectContexts.Remove(contextKey);
                }
            }
        }

        /*
        private static string BuildContextThreadName()
        {
            return Thread.CurrentThread.Name;
        }

        private static string BuildHttpContextName()
        {
            return OBJECT_CONTEXT_KEY;
        }*/

        #endregion

        #endregion


    }
}
