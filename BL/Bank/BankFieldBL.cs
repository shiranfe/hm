using Common;
using Microsoft.Practices.Unity;
using Repository;
using System.Collections.Generic;
using System;

namespace BL
{
    public class BankFieldBL
    {
         private readonly IUnitOfWork _uow;

         private readonly BankFieldModule _module;
        private readonly FieldPoolModule _fieldPoolModule;

        public BankFieldBL([Dependency]IUnitOfWork uow,
           [Dependency]FieldPoolModule fieldPoolModule,
             [Dependency]BankFieldModule module
         )
        { 
            _uow = uow;
            _module = module;
            _fieldPoolModule = fieldPoolModule;
        }

        /***************************************************/


        public void GetItemsList(BankFieldFilterDm filter)
        {
             _module.GetList(filter);
        }

        public BankFieldDM GetSingleItemDM(int id)
        {
            return  _module.GetSingleDM(id);
        }

        public List<FieldPoolDM> GetPoolFields()
        {
            return _fieldPoolModule.GetPoolFields();
        }


        public void Update(BankFieldDM model)
        {
            _module.Update(model);

           
        }

        public void Delete(int id)
        {
            
            _module.Delete(id);

            _uow.SaveChanges();
        }

        public List<DropListDM> GetDrop()
        {
            return _module.GetDrop();
        }
    }
}
