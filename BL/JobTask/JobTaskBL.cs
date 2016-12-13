using System;
using Common;
using Microsoft.Practices.Unity;
using Repository;

namespace BL
{
    public class JobTaskBL
    {
         private readonly IUnitOfWork _uow;

         private readonly JobTaskModule _module;


         public JobTaskBL([Dependency]IUnitOfWork uow,
          
             [Dependency]JobTaskModule module
         )
        {
            _uow = uow;
            _module = module;
         
        }

        /***************************************************/


        public void GetItemsList(JobTaskFilterDm filter)
        {
             _module.GetList(filter);
        }

        public JobTaskDM GetSingleItemDM(int id)
        {
            return  _module.GetSingleDM(id);
        }


        public void Update(JobTaskDM model)
        {
             _module.Update(model);

           
        }

        public void Delete(int id)
        {
            
            _module.Delete(id);

            _uow.SaveChanges();
        }

        public JobDM GetJobDetails(int jobID)
        {
            return _module.GetJobDetails(jobID);
        }
    }
}
