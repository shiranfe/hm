using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using BL.Moduls;
using Common;
using DAL;
using Microsoft.Practices.Unity;
using Repository;

namespace BL
{
    public class EmployeeBL
    {

        private readonly IUnitOfWork _uow;
        private readonly IRepository<Employee> _employeeDal;
        private readonly IRepository<EmployeeRole> _employeeRoleDal ;
        private readonly ContactInfoModule _contactInfoModule;
        private readonly EmployeeCache _employeeCache;


        //public EmployeeBL()
        //    : this(_uow.Repository<Employee>(),_uow.Repository<EmployeeRole>())
        //{
            
        //}

        public EmployeeBL([Dependency]IUnitOfWork uow, 
            [Dependency]EmployeeCache employeeCache,
            [Dependency]ContactInfoModule contactInfoModule)
        {
            _uow = uow;
            _employeeDal = _uow.Repository<Employee>();
            _employeeRoleDal = _uow.Repository<EmployeeRole>();
            _contactInfoModule = contactInfoModule;
            _employeeCache = employeeCache;
        }

        readonly string _userNameExistError = "שם המשתמש קיים במערכת. בחר שם משתמש ייחודי";


        /***************************************************/
        private Employee GetSingle(int empID)
        {
            Employee employee = _employeeDal.SingleOrDefault(x => x.EmployeeID == empID);
            return employee;
        }


        public List<DropListDM> GetEmpByRole(string role)
        {
            var roles = _employeeRoleDal
                .Where(x => x.PickList.Key == role).ToList();
            
            return roles
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

  

        public UserLoged EmpIsLoginValid(UserAccountDM userAccountDM)
        {
            return (from e in _employeeDal
                    where e.Username == userAccountDM.Username &&
                           e.Password == userAccountDM.Password
                    select new UserLoged
                    {
                        UserID = e.EmployeeID

                    }).SingleOrDefault();

        }

        public EmployeeDM GetEmployeeDetails(int empID)
        {
            var model = _employeeDal.Where(x=>x.EmployeeID==empID)
                .Select(e=> new EmployeeDM
                    {
                        EmployeeID = e.EmployeeID,
                        Active = e.Active,
                        FirstName = e.FirstName,
                        LastName = e.LastName,
                       // FullName= e.FirstName + " " + e.LastName,
                     
                        Password= e.Password,
                        Username= e.Username,
                        BranchID=e.BranchID
                    }).Single();

            model.ContactInfoDM = _contactInfoModule.GetSingleInfo(empID, ObjType.Employee);

            return model;
        }
    
        public EmployeeDM GetEmployeePermision(int employeeID)
        {

            return GetEmployeeFromCache(employeeID) ?? LoadEmployee(employeeID);
        }

        private EmployeeDM GetEmployeeFromCache(int employeeID)
        {
            return _employeeCache.GetSingle(employeeID);
        }

        private EmployeeDM LoadEmployee(int employeeID)
        {
            var q = _employeeDal.GetQueryableFresh()
                .Where(e=> e.EmployeeID == employeeID).SingleOrDefault();

            var emp =  new EmployeeDM
                    {
                        EmployeeID = q.EmployeeID,
                        FullName = q.FullName,
                        Perrmisions = new UserPerrmisionDM
                        {
                            ShowSettings = q.ShowSettings,
                            ShowVB = q.ShowVB,
                            ShowClientSettings = q.ShowClientSettings,
                            ShowRefubrish = q.ShowRefubrish,
                            ShowCatalog = q.ShowCatalog,
                            ShowQuote = q.ShowQuote,
                            ShowAlignment = q.ShowAlignment,
                            ShowBalancing = q.ShowBalancing,
                            ShowManagReports = q.ShowManagReports,
                            ShowFieldsEdit = q.ShowFieldsEdit
                        }

                    };
          

             
            _employeeCache.Add(emp);

            return emp;

        }

        public List<EmployeeDM> GetEmployeesList(bool withAll = false)
        {

            var list = (from e in _employeeDal.ToList()
                        select new EmployeeDM
                        {
                            EmployeeID = e.EmployeeID,
                            FullName = e.FullName,
                            Active = e.Active,
                            BranchID = e.BranchID,
                            BranchName = e.Branch?.BranceName ?? "not set"
                        }).ToList();

            if (withAll)
                list.Add(new EmployeeDM { FullName = " - כולם - ", EmployeeID = -1 });

            return list.OrderBy(x => x.FullName).ToList();
        }


        /***************************************************/

        public void Update(EmployeeDM model)
        {        
            if (model.EmployeeID > 0)
                Edit(model);
            else
                Add(model);

           
        }

        private void Edit(EmployeeDM model)
        {
           
            IsUserExist(model.Username, model.EmployeeID);


            Employee entity = GetSingle(model.EmployeeID);

            ModelToEntity(entity, model);

            _employeeDal.Update(entity);

            _contactInfoModule.Update(model.ContactInfoDM);

            _uow.SaveChanges();

            _employeeCache.Remove(model.EmployeeID);
        }

        private void IsUserExist(string userName)
        {
            bool userExist = _employeeDal.Where(x => x.Username == userName).Any();
            if (userExist)
                throw new Exception(_userNameExistError);
        }

        private void IsUserExist(string userName, int employeeID)
        {
            bool userExist = _employeeDal.Where(x => x.Username == userName && x.EmployeeID != employeeID).Any();
            if (userExist)
                throw new Exception(_userNameExistError);
          
        }

      


        private void Add(EmployeeDM model)
        {
            IsUserExist(model.Username);

            Employee entity = new Employee
            {                             
                Active = true,
                ShowRefubrish = true
            };

            ModelToEntity(entity, model);

            _employeeDal.Add(entity);

            _uow.SaveChanges();


            model.ContactInfoDM.ObjType = ObjType.Employee;
            model.ContactInfoDM.ObjID = entity.EmployeeID;
            _contactInfoModule.Add(model.ContactInfoDM);

            _uow.SaveChanges();
        }

  

        public void Delete(int employeeID)
        {
            Employee user = GetSingle(employeeID);
            _employeeDal.Remove(user);

            _contactInfoModule.Delete(employeeID, ObjType.Employee);

            _uow.SaveChanges();
        }



        public void UpdatePermision(EmployeeDM model)
        {
            Employee entity = GetSingle(model.EmployeeID);

            Mapper.Map(model.Perrmisions, entity);

            _uow.SaveChanges();

            _employeeCache.Remove(model.EmployeeID);
        }



        private void EntityToModel(Employee entity, EmployeeDM model)
        {
            Mapper.Map(entity, model);
        }

        private void ModelToEntity(Employee entity, EmployeeDM model)
        {
            Mapper.Map(model, entity);
        }

       
    }
}
