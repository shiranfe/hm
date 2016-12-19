using System.Collections.Generic;
using System.Linq;
using Common;
using Microsoft.Practices.Unity;
using Repository;

namespace BL
{
    public class EmployeeCache : Cache
    {
        private readonly IUnitOfWork _uow;

        //private CacheModule Rep;

        public EmployeeCache([Dependency]IUnitOfWork uow)
        {
            _uow = uow;
        }

        /*****************         Methods              ******************/

        private static readonly object Lock = new object();



        internal EmployeeDM GetSingle(int employeeId)
        {
            lock (Lock)
            {
                return CacheModule.EmployeeDM.SingleOrDefault(x => x.EmployeeID == employeeId);
            }

        }

        internal void Add(EmployeeDM emp)
        {
            lock (Lock)
            {
                if (GetSingle(emp.EmployeeID) == null)
                {
                    CacheModule.EmployeeDM.Add(emp);
                }
             
            }

        }

        internal void Remove(int employeeId)
        {
            lock (Lock)
            {
                CacheModule.EmployeeDM.RemoveAll(x => x.EmployeeID == employeeId);
            }

        }

        internal void RemoveAll(EmployeeDM emp)
        {
            lock (Lock)
            {
                CacheModule.EmployeeDM = new List<EmployeeDM>();
            }

        }
    }
}
