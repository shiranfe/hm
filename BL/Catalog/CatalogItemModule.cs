using AutoMapper;
using Common;
using DAL;
using Microsoft.Practices.Unity;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BL
{
    public class CatalogItemModule
    {
        private readonly IUnitOfWork _uow;
        private readonly IRepository<CatalogItem> _entityDal;
        private readonly CatalogItemComponentModule _catalogItemComponentModule;

        public CatalogItemModule([Dependency]IUnitOfWork uow, 
            [Dependency]CatalogItemComponentModule catalogItemComponentModule)
        {
            _uow = uow;
            _entityDal = _uow.Repository<CatalogItem>();
            _catalogItemComponentModule = catalogItemComponentModule;
        }


        /***************************************************/

        private CatalogItem GetSingle(int id)
        {
            return _entityDal.SingleOrDefault(x => x.CatalogItemID == id);
        }


        internal CatalogItemDM GetSingleDM(int id)
        {
            var entity = GetSingle(id);
            var model = new CatalogItemDM();

            EntityToModel(model, entity);     
          
            return model;

        }

        internal void GetList(CatalogFilterDm filter)
        {
            var quer = ItemsQuer();

            filter.TableList = GetListByFilter(quer, filter);

        }

        private List<CatalogItemDM>  GetListByFilter(IEnumerable<CatalogItemDM> quer, CatalogFilterDm filter)
        {
            if (filter == null)
                return quer.ToList();

            /** filter by Srch*/
            if (!string.IsNullOrEmpty(filter.Srch))
                quer = quer.Where(i => i.SearchStr.Contains(filter.Srch));

            var list = quer.ToList();

            return LinqHelpers.FilterByPage(filter, list);
        }

        internal List<CatalogItemDM> GetList()
        {
            return ItemsQuer().OrderByDescending(x => x.CatalogItemID).ToList();

        }

        private IEnumerable<CatalogItemDM> ItemsQuer()
        {
            /** get item price and cost*/
            var grpby = (from x in _catalogItemComponentModule.GetListQuer().ToList()
                         group x by new
                         {
                             x.CatalogItemID// x.ItemName,x.CatalogItemID , x.ItemNotes
                         });

            var summedItems =
                 (from g in grpby
                  select new
                  {
                      CatalogItemID = g.Key.CatalogItemID,
                      CatalogItemPrice = g.Sum(s => s.ComponentPrice * s.Quantity),
                      CatalogItemCost = g.Sum(s => s.ComponentCost * s.Quantity)
                  }).ToList();


            return (from s in _entityDal.GetQueryableFresh().ToList()
                    join g in summedItems
                    on s.CatalogItemID equals g.CatalogItemID
                    select new CatalogItemDM
                    {
                        CatalogItemID = g.CatalogItemID,
                        CatalogItemPrice = g.CatalogItemPrice,
                        CatalogItemCost = g.CatalogItemCost,
                        FieldPoolID = s.FieldPoolID,
                        ItemNotes = s.ItemNotes,
                        ItemName = s.ItemName,
                        Pid = s.Pid,
                        LinkedId = s.LinkedId,
                        SortId = s.SortId,
                        IsGroup = s.ItemChilds.Any()
                    })
                    ;
        }

        internal List<CatalogItemDM> GetDropItems()
        {
            return _entityDal.GetQueryableFresh().AsEnumerable().Where(x=>x.CatalogItemComponent.Any())
                 .Select(x => new CatalogItemDM
                 {
                     CatalogItemID = x.CatalogItemID,
                     ItemName = x.ItemName + (x.ItemNotes!=null ? " - "  + x.ItemNotes : "")

                 }).ToList();
        }

        public object GetCatalogItemForAutoComplete(CatalogItemDM x)
        {
            return new
            {
                value = x.CatalogItemID,
                text = x.ItemName,
                label = x.ItemName +  " " + x.ItemNotes ,
                price = x.CatalogItemPrice,
                notes = x.ItemNotes ?? string.Empty,
                fieldPoolID =x.FieldPoolID
            };
        }
   

        /********************         CHANGE       **********************/

        public void Update(CatalogItemDM model)
        {

            if (model.CatalogItemID > 0)
                Edit(model);
            else
                Add(model);

          
        }

        private void Add(CatalogItemDM model)
        {
            CatalogItem entity = new CatalogItem();

            ModelToEntity(model, entity);

            _entityDal.Add(entity);
           
            _uow.SaveChanges();

            model.CatalogItemID = entity.CatalogItemID;

        }

        private void Edit(CatalogItemDM model)
        {
            CatalogItem entity = GetSingle(model.CatalogItemID);
            ModelToEntity(model, entity);

            _uow.SaveChanges();


        }


        private void ModelToEntity(CatalogItemDM model, CatalogItem entity)
        {
            Mapper.DynamicMap(model, entity);
        }

        private void EntityToModel(CatalogItemDM model, CatalogItem entity)
        {
            Mapper.DynamicMap(entity, model);
        }





        internal void Delete(int id)
        { 
            var entity = GetSingle(id);

            if (entity.QuoteVersionItem.Any())
                throw new Exception("Can delete item that is connected to offer");

            _entityDal.Remove(entity);
        }

    }
}
