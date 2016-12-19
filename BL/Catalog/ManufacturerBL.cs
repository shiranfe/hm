using System.Collections.Generic;
using System.Linq;
using BL.Moduls;
using Common;
using Microsoft.Practices.Unity;
using Repository;

namespace BL
{
    public class ManufacturerBL
    {
        private readonly IUnitOfWork _uow;
        
        private readonly ClientModule _clientModule;
        private readonly ManufacturerModule _module;
       

        //public ManufacturerBL()
        //    : this(new ManufacturerModule(), new ManufacturerCache())
        //{
            
        //}

        public ManufacturerBL([Dependency]IUnitOfWork uow,
            [Dependency]ManufacturerModule manufacturerModule,
             [Dependency]ClientModule clientModule
           )
        {
            _uow = uow;
            _module = manufacturerModule;
          
            _clientModule = clientModule;
        }

        /***************************************************/



        public List<ClientDM> GetIndex()
        {
            return _module.GetIndex();
        }

        public List<KeyValueDM> GetList()
        {
            return _module.GetPickQuer().ToList();
        }


        public ClientDM GetSingleDM(int clientID)
        {
            return _clientModule.GetClientDM(clientID);
        }


        //public List<KeyValueDM> GetProductLisType()
        //{
        //    Expression<Func<PickList, bool>> cond = x =>
        //        x.Entity == "MaterialType" || x.Entity == "ServiceType";
             
        //    return _langModule.GePickListDM(cond).ToList();
        //}

          

        /*************************      UPDATE      **************************/

        public void Update(ClientDM model)
        {
            model.IsManufacture = true;

            _clientModule.Update(model);

        }


        public void Delete(int clientID)
        {

            _clientModule.Delete(clientID);

        }

    }
}
