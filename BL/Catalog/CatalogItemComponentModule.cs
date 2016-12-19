using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using BL.Moduls;
using Common;
using DAL;
using Microsoft.Practices.Unity;
using Repository;

namespace BL
{
    public class CatalogItemComponentModule
    {
        private readonly IUnitOfWork _uow;
        private readonly IRepository<CatalogItemComponent> _entityDal;
        private readonly IRepository<vwCatalogItemComponent> _vwCatalogItemComponentDal;
        private readonly SupplierProductModule _supplierProductModule;
        private EmployeeModule _employeeModule;
        private readonly RoleModule _roleModule;

        public CatalogItemComponentModule([Dependency]IUnitOfWork uow,
               [Dependency]EmployeeModule employeeModule,
            [Dependency]RoleModule roleModule,
            [Dependency]SupplierProductModule supplierProductModule)
        {
            _uow = uow;
            _entityDal = _uow.Repository<CatalogItemComponent>();
            _vwCatalogItemComponentDal = _uow.Repository<vwCatalogItemComponent>();
            _supplierProductModule = supplierProductModule;
            _employeeModule = employeeModule;
            _roleModule = roleModule;
        }


        /***************************************************/

        private CatalogItemComponent GetSingle(int catalogItemComponentID)
        {
            return _entityDal.SingleOrDefault(x => x.CatalogItemComponentID == catalogItemComponentID);
        }

        public CatalogItemComponentDM GetSingleItemDM(int id)
        {
            return GetListQuer()
                    .SingleOrDefault(x=> x.CatalogItemComponentID == id);   
        }

        internal IQueryable<CatalogItemComponentDM> GetListQuer()
        {
            return (from g in _vwCatalogItemComponentDal.GetQueryableFresh()
                    let model = new CatalogItemDM()

            select new CatalogItemComponentDM
                    {
                        CatalogItemComponentID = g.CatalogItemComponentID,
                        CatalogItemID = g.CatalogItemID,
                        ComponentPrice = g.ComponentPrice,
                        ComponentSrcID = g.ComponentSrcID,
                        ComponentTypeID = g.ComponentTypeID,
                        Quantity = g.Quantity,
                        ComponentCost = g.ComponentCost,
                        ComponentName = g.ComponentName
                        //CatalogItemDM = new CatalogItemDM {
                        //    CatalogItemID = g.CatalogItem.CatalogItemID,
                        //    ItemName = g.CatalogItem.ItemName,
                        //    ItemNotes = g.CatalogItem.ItemNotes,
                        //    Pid = g.CatalogItem.Pid,
                        //    LinkedId = g.CatalogItem.LinkedId,
                        //    SortId = g.CatalogItem.SortId,
                        //},
                        //ItemName = g.CatalogItem.ItemName,
                        //ItemNotes = g.CatalogItem.ItemNotes,
                        // g.CatalogItem.Pid
                    });
        }

        //private static CatalogItemDM GetCatatlogItemDm(vwCatalogItemComponent g)
        //{
        //    var model = new CatalogItemDM();
        //    Mapper.Map(g.CatalogItem, model);
        //    return model;
        //}

        internal List<CatalogItemComponentDM> GetList(int catalogItemID)
        {
            return (from g in GetListQuer()
                    where g.CatalogItemID == catalogItemID
                    select g
                    ).ToList();
        }


        /// <summary>
        /// get all srcs possible for component - materials, outsrcs and personal
        /// </summary>
        /// <returns></returns>
        internal List<ComponentTypeDM> GetAllSrcs()
        {
            List<ComponentTypeDM> allSrcs = new List<ComponentTypeDM>();

            allSrcs = (from x in _supplierProductModule.GetQuer()
                       select new ComponentTypeDM {
                        Id= x.SupplierProductID,
                        Text = x.ProductName,
                        Cost = x.ProductCost,
                        Price = x.ProductPrice,
                        ComponentTypeID = x.IsMaterial ? CatalogItemType.Material : CatalogItemType.Outsource
                       }).ToList();


            List<ComponentTypeDM> roles = _roleModule.GetComponents();
        
            allSrcs.AddRange(roles);

            return allSrcs;
        }

        internal decimal GetItemPrice(int catalogItemID)
        {
            var quer = _vwCatalogItemComponentDal.GetQueryableFresh()
                .Where(x => x.CatalogItemID == catalogItemID).ToList();

            if(!quer.Any())
                return 0;

            return quer.Sum(s => s.ComponentPrice * s.Quantity);
        
          
        }
        /********************         CHANGE       **********************/

        internal void UpdateEmployeeHour(RoleDM model)
        {
            var quer = _entityDal.Where(x => x.ComponentTypeID == CatalogItemType.Personnel && x.ComponentSrcID == model.RoleID).ToList();
            foreach(var component in quer)
            {
                component.ComponentPrice = model.RolePrice;
                
            }
        }

        public void Update(CatalogItemComponentDM model)
        {

            if (model.CatalogItemComponentID > 0)
                Edit(model);
            else
                Add(model);

            _uow.SaveChanges();

        }

        private void Add(CatalogItemComponentDM model)
        {
            CatalogItemComponent entity = new CatalogItemComponent();

            ModelToEntity(model, entity);

            _entityDal.Add(entity);

        }

        private void Edit(CatalogItemComponentDM model)
        {
            CatalogItemComponent entity = GetSingle(model.CatalogItemComponentID);
            ModelToEntity(model, entity);
        }


        private void ModelToEntity(CatalogItemComponentDM model, CatalogItemComponent entity)
        {
            Mapper.Map(model, entity);
        }

        private void EmtityToModel(CatalogItemComponentDM model, CatalogItemComponent entity)
        {
            Mapper.Map(entity, model);
        }



        internal void Delete(int id)
        {
            var entity = GetSingle(id);

         
            _entityDal.Remove(entity);
        }



        internal void DeleteByItemId(int catatlogItemID)
        {
            var compsToRemove = _entityDal.ToList(x => x.CatalogItemID == catatlogItemID);

            compsToRemove.ForEach(x => _entityDal.Remove(x));
        }

     
    }
}
