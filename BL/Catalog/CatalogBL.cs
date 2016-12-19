using System.Collections.Generic;
using Common;
using Microsoft.Practices.Unity;
using Repository;

namespace BL
{
    public class CatalogBL
    {
         private readonly IUnitOfWork _uow;

         private readonly CatalogItemModule _catalogItemModule;
         private readonly CatalogItemComponentModule _catalogItemComponentModule;


        public CatalogBL([Dependency]IUnitOfWork uow,
            [Dependency]CatalogItemComponentModule catalogItemComponentModule,
             [Dependency]CatalogItemModule catalogItemModule
         )
        {
            _uow = uow;
            _catalogItemComponentModule = catalogItemComponentModule;
            _catalogItemModule = catalogItemModule;
         
        }

        /***************************************************/




        public void GetItemsList(CatalogFilterDm filter)
        {
             _catalogItemModule.GetList( filter);
        }

        public CatalogItemDM GetSingleItemDM(int id)
        {
            return  _catalogItemModule.GetSingleDM(id);

            //SetItemsComponents(model);

            //return model;
        }


        public List<CatalogItemDM> GetDropItems()
        {
            var list =  _catalogItemModule.GetDropItems();
            list.Insert(0, new CatalogItemDM { ItemName=" - לא מקושר -" });
            return list;
        }


        //private void SetItemsComponents(CatalogItemDM model)
        //{
        //    model.Components = _catalogItemComponentModule.GetList(model.CatalogItemID);

        //    model.CatalogItemPrice = model.Components.Sum(x => x.ComponentPrice*x.Quantity);

        //    model.AllSrcs = GetAllSrcs();


        //}

        public object Update(CatalogItemDM model)
        {
            _catalogItemModule.Update(model);

            _uow.SaveChanges();

            //will allow to send new details of the created item
         //   model = GetSingleItemDM(model.CatalogItemID);

            model.CatalogItemPrice = _catalogItemComponentModule.GetItemPrice(model.CatalogItemID);

            return _catalogItemModule.GetCatalogItemForAutoComplete(model);
        }

        public void Delete(int id)
        {

            _catalogItemComponentModule.DeleteByItemId(id);

            _catalogItemModule.Delete(id);

            _uow.SaveChanges();
        }

      
    }
}
