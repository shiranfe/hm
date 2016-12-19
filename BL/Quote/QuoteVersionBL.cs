using System.Collections.Generic;
using System.Linq;
using Common;
using DAL;
using Microsoft.Practices.Unity;
using Repository;

namespace BL
{
    public class QuoteVersionBL
    {
         private readonly IUnitOfWork _uow;
         private readonly QuoteModule _quoteModule;
         private readonly QuoteVersionModule _module;
         private readonly QuoteVersionItemModule _quoteVersionItemModule;
         private readonly CatalogItemModule _catalogItemModule;

         public QuoteVersionBL([Dependency]IUnitOfWork uow,
          [Dependency]QuoteModule quoteModule,
             [Dependency]QuoteVersionModule module,
             [Dependency]QuoteVersionItemModule quoteVersionItemModule,
             [Dependency]CatalogItemModule catalogItemModule
         )
        {
            _uow = uow;
            _module = module;
            _quoteModule = quoteModule;
            _quoteVersionItemModule = quoteVersionItemModule;
            _catalogItemModule = catalogItemModule;
        }

     

        /***************************************************/


        public List<QuoteVersionDM> GetItemsList(int quoteID)
        {
            return _module.GetList(quoteID);
        }

        public QuoteVersionDM GetSingleItemDM(int id)
        {
            var model =  _module.GetSingleDM(id);

            model.QuoteDM = _quoteModule.GetSingleDM(model.QuoteID);
            model.Items = _quoteVersionItemModule.GetList(id);
            model.TotalSum = model.Items.Where(x=>!x.ItemParentID.HasValue).Sum(x => x.ItemTotalPrice);
            model.Vat = SiteGlobals.Vat;
          
            SetCatatlogItems(model);
        

            return model;
        }





        public QuoteDM GetQuoteDM(int id)
        {
            QuoteVersion quoteVersion = _module.GetSingle(id);
           return  _quoteModule.GetSingleDM(quoteVersion.Quote, false);
        }

        public int GetClientID(int id)
        {
            return _module.GetClientID(id);
        }


        private void SetCatatlogItems(QuoteVersionDM model)
        {

            var items = _catalogItemModule.GetList();

            //items.Add(new CatalogItemDM {
            //    CatalogItemID=0,
            //    ItemName = "הוסף כפריט חדש",
            //    ItemNotes="בכדי להוסיף את הפריט יש ליצור אותו תחילה בקטלוג"
            //});

            model.CatalogItems = items.Select(x =>
                _catalogItemModule.GetCatalogItemForAutoComplete(x));
        }

        

        public void Update(QuoteVersionDM model)
        {
            _module.Update(model);

            _uow.SaveChanges();
        }

        public int Copy(int srcVersionID)
        {
            QuoteVersion entity = _module.Copy(srcVersionID);

            entity.QuoteVersionItem = _quoteVersionItemModule.Copy(srcVersionID);

            _uow.SaveChanges();

            return entity.QuoteVersionID;
        }


        public void Delete(int id)
        {
            
            _module.Delete(id);

            _uow.SaveChanges();
        }

        public void ImportFromQuote(int srcVersionID, int destVersionID)
        { 
            QuoteVersion destVersion = _module.GetSingle(destVersionID);
            QuoteVersionDM srcVersion = GetSingleItemDM(srcVersionID);


            var items = destVersion.QuoteVersionItem.Concat(_quoteVersionItemModule.Copy(srcVersionID)).ToList();
            for (int i = 0; i < items.Count; i++)
            {
                items[i].ItemSort = i+1;
            }
            destVersion.QuoteVersionItem = items;

            if (!string.IsNullOrEmpty(srcVersion.Appendices))
            {
                destVersion.Appendices += "<p>" + srcVersion.Appendices + "</p>";
            }
         

            _module.Update(destVersion);

            _uow.SaveChanges();
        }
    }
}
