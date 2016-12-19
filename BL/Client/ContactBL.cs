using System.Collections.Generic;
using BL.Moduls;
using Common;
using Microsoft.Practices.Unity;
using Repository;

namespace BL
{
    public class ContactBL
    {
        private readonly IUnitOfWork _uow;
        private readonly ContactModule _module;
        private readonly ClientCache _clientCache;


        public ContactBL([Dependency]IUnitOfWork uow, 
            [Dependency]ContactModule module,
            [Dependency]ClientCache clientCache)
        {
            _uow = uow;
            _module = module;
            _clientCache = clientCache;
        }

      
        /***************************************************/


        public UserDetailsDM Get(int userID)
        {
            return _module.GetSingleDM(userID);
        }

        public List<ContactDM> GetIndex(int adminClientID)
        {
            return _module.GetList(adminClientID);
            //return adminClientID == 0 ?
            //    _module.GetAll() :
            //    GetClientContacts(adminClientID);
        }

        //public List<ContactDM> GetClientAndChildsContacts(int clientID)
        //{
        //    if (clientID == 0)
        //        return new List<ContactDM>();

        //    int[] clientAndChilds = _clientCache.GetClientAndChilds(clientID);

        //    return _module.GetClientAndChildsContacts(clientAndChilds);
        //}

        public void Update(UserDetailsDM userDetailsDM)
        {
            _module.Update(userDetailsDM);
            _uow.SaveChanges();
        }


        /********************         Admin       **********************/


        public int Delete(int userID)
        {
           int clientID= _module.Delete(userID);
            _uow.SaveChanges();

            return clientID;
        }
    }
}
