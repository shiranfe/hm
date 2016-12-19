using Common;
using Microsoft.Practices.Unity;
using Repository;

namespace BL
{
    public class BankTask_FieldBL
    {
         private readonly IUnitOfWork _uow;

         private readonly BankTask_FieldModule _module;
     
        public BankTask_FieldBL([Dependency]IUnitOfWork uow,
          
             [Dependency]BankTask_FieldModule module
         )
        {
            _uow = uow;
            _module = module;
         
        }

        /***************************************************/


        public void GetItemsList(BankTask_FieldFilterDm filter)
        {
             _module.GetList(filter);
        }

        public BankTask_FieldDM GetSingleItemDM(int id)
        {
            return  _module.GetSingleDM(id);
        }


        public JobTaskFieldDM Update(BankTask_FieldDM model)
        {
            var isAdded = model.BankTask_FieldID == 0;
        
            var entity  = _module.Update(model);

            return isAdded ? _module.GetTaskField(entity) : null;

        }

        public void Delete(int id)
        {
            
            _module.Delete(id);

            _uow.SaveChanges();
        }

        public void Sort(int[] ids)
        {
            _module.Sort(ids);
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
    }
}
