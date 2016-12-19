using System.Collections.Generic;
using System.Linq;
using Common;
using DAL;
using Microsoft.Practices.Unity;
using Repository;

namespace BL.Moduls
{
    public class EmployeeModule
    {
        private readonly IUnitOfWork _uow;
        private readonly IRepository<Employee> _employeeDal;
        private readonly IRepository<EmployeeRole> _employeeRoleDal;
        private IRepository<Role> _roleDal;
        //public EmployeeModule()
        //    : this(_uow.Repository<Employee>(), _uow.Repository<EmployeeRole>())
        //{

        //}

        public EmployeeModule([Dependency]IUnitOfWork uow)
        {
            _uow = uow;
            _employeeDal = _uow.Repository<Employee>();
            _employeeRoleDal = _uow.Repository<EmployeeRole>() ;
            _roleDal = _uow.Repository<Role>();
        }



        /***************************************************/


        public List<DropListDM> GetEmpByRole(string roleName)
        {
            return _employeeRoleDal
                .Where(x => x.PickList.Value == roleName)
                .Select(x => new DropListDM
                {
                    id = x.EmployeeID,
                    Text = x.Employee.FullName
                }).ToList();
        }



        public string GetEmpName(int empID)
        {
            return _employeeDal.SingleOrDefault(x => x.EmployeeID == empID).FullName;


        }




        internal int GetEmployeeBranch(int empID)
        {
            return _employeeDal.SingleOrDefault(x => x.EmployeeID == empID).BranchID;
        }


       
    }
}
