using System.Collections.Generic;
using Common;
using Microsoft.Practices.Unity;
using Repository;

namespace BL
{
    public class JobAlignmentPartBL
    {
         private readonly IUnitOfWork _uow;

         private readonly JobAlignmentPartModule _module;


         public JobAlignmentPartBL([Dependency]IUnitOfWork uow,
          
             [Dependency]JobAlignmentPartModule module
         )
        {
            _uow = uow;
            _module = module;
         
        }

        /***************************************************/


         public List<JobAlignmentPartDM> GetItemsList(int jobID)
        {
            return _module.GetList(jobID);
        }

        public JobAlignmentPartDM GetSingleItemDM(int id)
        {
            return  _module.GetSingleDM(id);
        }


        public void Update(JobAlignmentPartDM model)
        {
            _module.Update(model);

            _uow.SaveChanges();
        }

        public void Delete(int id)
        {
            
            _module.Delete(id);

            _uow.SaveChanges();
        }



     
    }
}
