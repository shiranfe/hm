using System;
using Common;
using Microsoft.Practices.Unity;
using Repository;

namespace BL
{
    public class EquipmentBL
    {
         private readonly IUnitOfWork _uow;

         private readonly EquipmentModule _module;


         public EquipmentBL([Dependency]IUnitOfWork uow,
          
             [Dependency]EquipmentModule module
         )
        {
            _uow = uow;
            _module = module;
         
        }

        /***************************************************/


        public void GetItemsList(EquipmentFilterDm filter)
        {
             _module.GetList(filter);
        }

        public EquipmentDM GetSingleItemDM(int id)
        {
            return  _module.GetSingleDM(id);
        }


        public void Update(EquipmentDM model)
        {
            _module.Update(model);

      
        }

        public void Delete(int id)
        {
            
            _module.Delete(id);

            _uow.SaveChanges();
        }

        public void QuickAdd(EquipmentDM model)
        {
            throw new NotImplementedException();
        }
    }
}
