using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using BL.Moduls;
using Common;
using DAL;
using Microsoft.Practices.Unity;
using Repository;

namespace BL
{
    public class SupplierBL
    {
        private readonly IUnitOfWork _uow;
        private readonly ClientModule _clientModule;
        private readonly SupplierModule _supplierModule;
        private readonly SupplierProductModule _supplierProductModule;
        private readonly LangModule _langModule;
       

        //public SupplierBL()
        //    : this(new SupplierModule(), new SupplierCache())
        //{
            
        //}

        public SupplierBL([Dependency]IUnitOfWork uow,
            [Dependency]SupplierModule supplierModule,
             [Dependency]ClientModule clientModule, [Dependency]LangModule langModule,
            [Dependency]SupplierProductModule supplierProductModule)
        {
            _uow = uow;
            _supplierModule = supplierModule;
            _clientModule = clientModule;
            _supplierProductModule = supplierProductModule;
            _langModule = langModule;
        }

        /***************************************************/


        //public List<ClientDM> GetSelectList()
        //{
        //    return _supplierModule.GetSelectList();
        //}

        public List<ClientDM> GetIndex()
        {
            return _supplierModule.GetIndex();
        }

        public List<KeyValueDM> GetSupplierList()
        {
            return _supplierModule.GetPickQuer().ToList();
        }


        public ClientDM GetSingleDM(int clientID)
        {
            return _clientModule.GetClientDM(clientID);
        }

        public SupplierProductDM GetSingleProductDM(int supplierProductID)
        {
            return _supplierProductModule.GetSingleDM(supplierProductID);
        }



        public List<SupplierProductDM> GetPriceListProducts()
        {
            return _supplierProductModule.GetList(true);
        }

        public object GetPriceListOutsource()
        {
            return _supplierProductModule.GetList(false);
        }


        public List<KeyValueDM> GetProductLisType()
        {
            Expression<Func<PickList, bool>> cond = x =>
                x.Entity == "MaterialType" || x.Entity == "ServiceType";
             
            return _langModule.GePickListDM(cond).ToList();
        }

          

        /*************************      UPDATE      **************************/

        public void Update(ClientDM model)
        {
            model.IsSupplier = true;
            model.ShowInRefubrish = true;

            _clientModule.Update(model);

        }


        public void UpdateSupplierProduct(SupplierProductDM model)
        {
            _supplierProductModule.Update(model);

            _uow.SaveChanges();
           
        }


        public void Delete(int clientID)
        {

            _clientModule.Delete(clientID);

        }


        public void DeleteSupplierProduct(int supplierProductID)
        {
            _supplierProductModule.Delete(supplierProductID);
            _uow.SaveChanges();
        }
    }
}
