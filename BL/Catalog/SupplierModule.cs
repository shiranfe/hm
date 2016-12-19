using System.Collections.Generic;
using System.Linq;
using Common;
using DAL;
using Microsoft.Practices.Unity;
using Repository;

namespace BL.Moduls
{
    public class SupplierModule
    {
        private readonly IUnitOfWork _uow;
        private readonly IRepository<Client> _supplierDal;
        private IRepository<SupplierProduct> _supplierProductDal;

        public SupplierModule([Dependency]IUnitOfWork uow,
            [Dependency]ContactInfoModule contactInfoModule)
        {
            _uow = uow;
            _supplierDal = _uow.Repository<Client>();
            _supplierProductDal = _uow.Repository<SupplierProduct>();
        }


        /***************************************************/



        private IQueryable<Client> GetQuer()
        {
            return _supplierDal.Where(x => x.IsSupplier).OrderBy(x => x.ClientName);
        }

        internal IQueryable<KeyValueDM> GetPickQuer()
        {
            return GetQuer()
                .Select(x => new KeyValueDM
                {
                    PickListID = x.ClientID,
                    Key = x.ClientName
                });
        }

        public List<ClientDM> GetIndex()
        {

            var quer = GetQuer();
            return (from x in quer
                    select new ClientDM
                    {
                        ClientID = x.ClientID,
                        ClientName = x.ClientName
                       
                    }).OrderBy(x => x.ClientName).ToList();

        }

    




    }
}
