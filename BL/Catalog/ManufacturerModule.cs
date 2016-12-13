using Common;
using DAL;
using Microsoft.Practices.Unity;
using Repository;
using System.Collections.Generic;
using System.Linq;

namespace BL.Moduls
{
    public class ManufacturerModule
    {
   
        private readonly IRepository<Client> _manufacturerDal;

        public ManufacturerModule([Dependency]IUnitOfWork uow)
        {
            _manufacturerDal = uow.Repository<Client>();
        }


        /***************************************************/



        private IQueryable<Client> GetAllManufacturerQuer()
        {
            return _manufacturerDal.Where(x => x.IsManufacture).OrderBy(x => x.ClientName);
        }

        public List<ClientDM> GetIndex()
        {

            var quer = GetAllManufacturerQuer();
            return (from x in quer
                    select new ClientDM
                    {
                        ClientID = x.ClientID,
                        ClientName = x.ClientName,
                       
                    }).OrderBy(x => x.ClientName).ToList();

        }


        internal IQueryable<KeyValueDM> GetPickQuer()
        {
            return _manufacturerDal.Where(x => x.IsManufacture)
                .Select(x => new KeyValueDM
                {
                    PickListID = x.ClientID,
                    Key = x.ClientName,
                });
        }






    }
}
