using AutoMapper;
using Common;
using DAL;
using Microsoft.Practices.Unity;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Moduls
{
    public class SupplierProductModule
    {
        private readonly IUnitOfWork _uow;
       
        private readonly IRepository<SupplierProduct> _supplierProductDal;
      
        public SupplierProductModule([Dependency]IUnitOfWork uow,
            [Dependency]ContactInfoModule contactInfoModule)
        {
            _uow = uow;
      
            _supplierProductDal = _uow.Repository<SupplierProduct>();
          
        }


        /***************************************************/

       

        private SupplierProduct GetSingle(int supplierProductID)
        {
            return _supplierProductDal.SingleOrDefault(x => x.SupplierProductID == supplierProductID);
        }


        internal SupplierProductDM GetSingleDM(int supplierProductID)
        {
            var entity  =  GetSingle(supplierProductID);
            var model = new SupplierProductDM();

            EmtityToModel(model, entity);

            return model;

        }


        internal List<SupplierProductDM> GetList(bool isMaterial=true)
        {

            return (from x in GetQuer()
                   // where x.IsMaterial == IsMaterial
                    select new SupplierProductDM 
                    {
                        SupplierProductID = x.SupplierProductID,
                        ClientID = x.ClientID,
                        IsForClients = x.IsForClients,
                        ProductName = x.ProductName,
                        ProductTypeKey = x.ProductType.Key,
                        SupplierName = x.Supplier.ClientName,
                        ManufactureName = x.ManufactureID.HasValue ? x.Manufacture.ClientName : null,
                        ProductCost = x.ProductCost,
                        IsMaterial= x.IsMaterial,
                        ProfitPrec = x.ProfitPrec,
                        ProductPrice = x.ProductPrice
                    }).OrderByDescending(x=>x.SupplierProductID).ToList();

         
        }

        internal IQueryable<SupplierProduct> GetQuer()
        {
            return _supplierProductDal.GetQueryableFresh();
        }


        internal IQueryable<KeyValueDM> GetBearingsPickList(string key)
        {
            return _supplierProductDal.Where(x => x.ProductType.Key == key)
                .Select(x => new KeyValueDM
                {
                    PickListID = x.SupplierProductID,
                    Key = x.ProductName + " (" + x.Manufacture.ClientName + ")",
                });
        }

     

        internal List<KeyValueDM> GetProductLisType()
        {
            return _supplierProductDal.Where(x =>
                x.ProductType.Key == "MaterialType" || x.ProductType.Key == "ServiceType")
                .Select(x => new KeyValueDM
                {
                    PickListID = x.SupplierProductID,
                    Key = x.ProductName + " (" + x.Manufacture.ClientName + ")",
                }).ToList();
        }

        /********************         CHANGE       **********************/



        public void Update(SupplierProductDM model)
        {
           
            if (model.SupplierProductID > 0)
                Edit(model);
            else
                Add(model);

            _uow.SaveChanges();
        }

        private void Add(SupplierProductDM model)
        {
            SupplierProduct entity = new SupplierProduct();
            
            ModelToEntity(model, entity);

            _supplierProductDal.Add(entity);

           

        }

        private void Edit(SupplierProductDM model)
        {
            SupplierProduct entity = GetSingle(model.SupplierProductID);
            ModelToEntity(model, entity);

        

            //_uow.SaveChanges();


        }


        private void ModelToEntity(SupplierProductDM model, SupplierProduct entity)
        {
            Mapper.DynamicMap<SupplierProductDM, SupplierProduct>(model, entity);
        }

        private void EmtityToModel(SupplierProductDM model, SupplierProduct entity)
        {
            Mapper.DynamicMap<SupplierProduct, SupplierProductDM>(entity, model);
        }





        internal void Delete(int supplierProductID)
        {
            var entity = GetSingle(supplierProductID);
            _supplierProductDal.Remove(entity);
        }
    }
}
