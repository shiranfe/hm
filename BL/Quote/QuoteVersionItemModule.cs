using AutoMapper;
using Common;
using DAL;
using Microsoft.Practices.Unity;
using Repository;
using System.Collections.Generic;
using System.Linq;
using System;

namespace BL
{


    public class QuoteVersionItemModule
    {
        private readonly IUnitOfWork _uow;
       
        private readonly IRepository<QuoteVersionItem> _entityDal;
      
        public QuoteVersionItemModule([Dependency]IUnitOfWork uow)
        {
            _uow = uow;
      
            _entityDal = _uow.Repository<QuoteVersionItem>();
          
        }


        /***************************************************/

       

        private QuoteVersionItem GetSingle(int quoteVersionItemID)
        {
            return _entityDal.SingleOrDefault(x => x.QuoteVersionItemID == quoteVersionItemID);
        }


        internal QuoteVersionItemDM GetSingleDM(int quoteVersionItemID)
        {
            var entity  =  GetSingle(quoteVersionItemID);
            var model = new QuoteVersionItemDM();
          
            EntityToModel(model, entity);

            return model;

        }


        internal List<QuoteVersionItemDM> GetList(int quoteVersionID)
        {

            return (from x in GetQuer()
                    where x.QuoteVersionID==quoteVersionID
                    select new QuoteVersionItemDM
                    {
                        QuoteVersionItemID = x.QuoteVersionItemID,
                        ItemNotes = x.ItemNotes,
                        ItemParentID = x.ItemParentID,
                        ItemPricePerUnit = x.ItemPricePerUnit,
                        ItemQuntity = x.ItemQuntity,
                        ItemTitle = x.ItemTitle,
                        QuoteVersionID = x.QuoteVersionID,
                        CatalogItemID = x.CatalogItemID,
                        ItemSort=x.ItemSort,
                        FieldPoolID=x.FieldPoolID,
                        FieldValue = x.FieldValue
                    })
                    .OrderBy(x => x.ItemSort)
                    .ThenBy(x=>x.QuoteVersionItemID)
                    .ToList();

         
        }

      

        internal IQueryable<QuoteVersionItem> GetQuer()
        {

            //var db = new List<QuoteVersionItem>();

            //db.Add(new QuoteVersionItem { QuoteVersionItemID = 1, QuoteVersionID = 4, ItemTitle = "פריט חדש", ItemPricePerUnit = 2680, ItemQuntity = 1, ItemNotes = "אחלה של פירוט על המוצר", CatalogItemID = 28 });
            //db.Add(new QuoteVersionItem { QuoteVersionItemID = 2, QuoteVersionID = 4, ItemTitle = "שיפוץ מנוע 400 כס", ItemPricePerUnit = 7860, ItemQuntity = 2, CatalogItemID = 1 });

            //return db.AsQueryable();
            return _entityDal.GetQueryableFresh();
        }


        /********************         CHANGE       **********************/



        public void Update(QuoteVersionItemDM model)
        {
           
            if (model.QuoteVersionItemID > 0)
                Edit(model);
            else
                Add(model);

          

          
        }

        private void Add(QuoteVersionItemDM model)
        {
            QuoteVersionItem entity = new QuoteVersionItem();
            
            ModelToEntity(model, entity);

            _entityDal.Add(entity);

            _uow.SaveChanges();

            model.QuoteVersionItemID = entity.QuoteVersionItemID;
        }



        private void Edit(QuoteVersionItemDM model)
        {
            QuoteVersionItem entity = GetSingle(model.QuoteVersionItemID);
            
            ModelToEntity(model, entity);

            _uow.SaveChanges();
        }

        internal ICollection<QuoteVersionItem> Copy(int srcVersionID)
        {

            var items = GetQuer().Where(x=> x.QuoteVersionID==srcVersionID).ToList();

            return DuplicateItems( items);
        }

        private ICollection<QuoteVersionItem> DuplicateItems(List<QuoteVersionItem> items)
        {
            var newItems = new List<QuoteVersionItem>();

            foreach (var item in items)
            {
                newItems.Add(new QuoteVersionItem
                {
                    CatalogItemID = item.CatalogItemID,
                    ItemNotes = item.ItemNotes,
                    ItemParentID = item.ItemParentID,
                    ItemQuntity = item.ItemQuntity,
                    ItemTitle = item.ItemTitle,
                    FieldPoolID = item.FieldPoolID,
                    FieldValue = item.FieldValue,
                    ItemPricePerUnit = item.ItemPricePerUnit,
                    ItemSort = item.ItemSort
                });
            }

            return newItems;
        }



        private void ModelToEntity(QuoteVersionItemDM model, QuoteVersionItem entity)
        {        
            Mapper.DynamicMap(model, entity);
        }

        private void EntityToModel(QuoteVersionItemDM model, QuoteVersionItem entity)
        {         
           Mapper.DynamicMap(entity, model);
        }
         

        internal void Delete(int quoteVersionItemID)
        {
            var entity = GetSingle(quoteVersionItemID);
            _entityDal.Remove(entity);
        }
         

        internal void DeleteByQuote(int id)
        {
            var items = _entityDal.Where(x => x.QuoteVersion.QuoteID == id).ToList();
            items.ForEach(x => _entityDal.Remove(x));
        }



    }
}
