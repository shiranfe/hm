using Common;
using Microsoft.Practices.Unity;
using Repository;

namespace BL
{
    public class JobTaskGroupFieldBL
    {
         private readonly IUnitOfWork _uow;

         private readonly JobTaskGroupFieldModule _module;


         public JobTaskGroupFieldBL([Dependency]IUnitOfWork uow,
          
             [Dependency]JobTaskGroupFieldModule module
         )
        {
            _uow = uow;
            _module = module;
         
        }

        /***************************************************/


        public void GetItemsList(JobTaskGroupFieldFilterDm filter)
        {
             _module.GetList(filter);
        }

        public JobTaskGroupFieldDM GetSingleItemDM(int id)
        {
            return  _module.GetSingleDM(id);
        }


        public void Update(JobTaskGroupFieldDM model)
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
