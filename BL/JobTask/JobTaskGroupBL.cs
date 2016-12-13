using Common;
using Microsoft.Practices.Unity;
using Repository;

namespace BL
{
    public class JobTaskGroupBL
    {
         private readonly IUnitOfWork _uow;

         private readonly JobTaskGroupModule _module;


         public JobTaskGroupBL([Dependency]IUnitOfWork uow,
          
             [Dependency]JobTaskGroupModule module
         )
        {
            _uow = uow;
            _module = module;
         
        }

        /***************************************************/


        public void GetItemsList(JobTaskGroupFilterDm filter)
        {
             _module.GetList(filter);
        }

        public JobTaskGroupDM GetSingleItemDM(int id)
        {
            return  _module.GetSingleDM(id);
        }


        public void Update(JobTaskGroupDM model)
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
