using DAL;
using Microsoft.Practices.Unity;
using Repository;
using System;

namespace BL
{
    public class EmployeeLogBL
    {

        private readonly IUnitOfWork _uow;

        private readonly IRepository<EmployeeLog> _employeeLogDal;

        public EmployeeLogBL([Dependency]IUnitOfWork uow)
        {
            _uow = uow;
            _employeeLogDal = _uow.Repository<EmployeeLog>() ;

        }

        public void Add(int empId, string entityName, string action, string param)
        {
            var MaxParamLen = Math.Min(param.Length, 500);
          
            var entity = new EmployeeLog {
                EmployeeID=empId,
                Entity = entityName,
                Action =action,
                Param = param.Substring(0, MaxParamLen),
                CreationTime= DateTime.Now,  
            };
           
            _employeeLogDal.Add(entity);

            _uow.SaveChanges();
        }

    }


}
