using System.Collections.Generic;
using Common;
using Microsoft.Practices.Unity;
using Repository;

namespace BL
{
    public class RoleBL
    {
         private readonly IUnitOfWork _uow;

         private readonly RoleModule _module;
        private readonly CatalogItemComponentModule _catalogItemComponentModule;

        public RoleBL([Dependency]IUnitOfWork uow,
          [Dependency]CatalogItemComponentModule catalogItemComponentModule,
             [Dependency]RoleModule module
         )
        {
            _uow = uow;
            _module = module;
            _catalogItemComponentModule = catalogItemComponentModule;
        }

        /***************************************************/


        public List<RoleDM> GetItemsList()
        {
            return  _module.GetList();
        }

        public RoleDM GetSingleItemDM(int id)
        {
            return  _module.GetSingleDM(id);
        }


        public void Update(RoleDM model)
        {
            _module.Update(model);
            _catalogItemComponentModule.UpdateEmployeeHour(model);
            _uow.SaveChanges();
        }

        public void Delete(int id)
        {
            
            _module.Delete(id);

            _uow.SaveChanges();
        }



     
    }
}
