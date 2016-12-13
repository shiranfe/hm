using Common;
using Microsoft.Practices.Unity;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public class CatalogItemComponentBL
    {
         private readonly IUnitOfWork _uow;

      
         private readonly CatalogItemComponentModule _module;



         public CatalogItemComponentBL([Dependency]IUnitOfWork uow,
          
            [Dependency]CatalogItemComponentModule catalogItemComponentModule)
        {
            _uow = uow;
            _module = catalogItemComponentModule;
        }

        /***************************************************/


         public CatalogItemComponentDM GetSingleItemDM(int id)
         {
            return _module.GetSingleItemDM(id);
         }

         public List<CatalogItemComponentDM> GetItemsList(int id)
        {          
            return _module.GetList(id);
        }

         public List<ComponentTypeDM> GetAllSrcs()
         {
             return _module.GetAllSrcs();
         }

         public void Update(CatalogItemComponentDM model)
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
