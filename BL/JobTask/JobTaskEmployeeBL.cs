using System.Collections.Generic;
using Common;
using Microsoft.Practices.Unity;
using Repository;

namespace BL
{
    public class JobTaskEmployeeBL
    {
         private readonly IUnitOfWork _uow;

         private readonly JobTaskEmployeeModule _module;


         public JobTaskEmployeeBL([Dependency]IUnitOfWork uow,
          
             [Dependency]JobTaskEmployeeModule module
         )
        {
            _uow = uow;
            _module = module;
         
        }

        public List<JobTaskEmployeeDM> GetTaskEmps(int jobTaskID)
        {
            return _module.GetTaskEmps(jobTaskID);
        }

        /***************************************************/


        public void GetItemsList(JobTaskEmployeeFilterDm filter)
        {
             _module.GetList(filter);
        }


        public JobTaskEmployeeDM GetSingleItemDM(int id)
        {
            return  _module.GetSingleDM(id);
        }


        public void Update(JobTaskEmployeeDM model)
        {
            _module.Update(model);

           
        }

        public void Delete(int id)
        {
            
            _module.Delete(id);

            _uow.SaveChanges();
        }



     
    }
}
