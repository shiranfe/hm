using System;
using Common;
using Microsoft.Practices.Unity;
using Repository;

namespace BL
{
    public class JobTaskFieldBL
    {
         private readonly IUnitOfWork _uow;

         private readonly JobTaskFieldModule _module;


         public JobTaskFieldBL([Dependency]IUnitOfWork uow,
          
             [Dependency]JobTaskFieldModule module
         )
        {
            _uow = uow;
            _module = module;
         
        }

        /***************************************************/


        public void GetItemsList(JobTaskFieldFilterDm filter)
        {
             _module.GetList(filter);
        }

        public JobTaskFieldDM GetSingleItemDM(int id)
        {
            return  _module.GetSingleDM(id);
        }


        public JobTaskFieldDM Update(JobTaskFieldDM model)
        {
            var isAdded = model.Id == 0;

            var entity = _module.Update(model);

            return isAdded ? _module.GetTaskField(entity) : null;
        }

        public void Delete(int id)
        {
            
            _module.Delete(id);

            _uow.SaveChanges();
        }

        public void Delete(int[] ids)
        {
            foreach (var id in ids)
            {
                _module.Delete(id);
            }

            _uow.SaveChanges();

        }

        public void Sort(int[] ids)
        {
            _module.Sort(ids);
            _uow.SaveChanges();
        }
    }
}
