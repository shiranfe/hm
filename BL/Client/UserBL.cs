using BL.Moduls;
using Common;
using DAL;
using System;
using System.Collections.Generic;
using Repository;
using Microsoft.Practices.Unity;

namespace BL
{
    public class UserBL
    {
        private readonly IUnitOfWork _uow;
        private readonly UserModule _userModule;
        private readonly ClientCache _clientCache;


        public UserBL([Dependency]IUnitOfWork uow, 
            [Dependency]UserModule userModule,
            [Dependency]ClientCache clientCache, 
            [Dependency]ClientModule clientModule)
        {
            _uow = uow;
            _userModule = userModule;
            _clientCache = clientCache;
        }

      
        /***************************************************/

        public string GetUserCurrentPassword(int userID)
        {
            return _userModule.SelectUserPassword(userID);
           
        }

        public string GetUserFullName(int userID)
        {
            return _userModule.SelectUserFullName(userID);
        }

        public int GetClientID(int userID)
        {
            return _userModule.GetClientID(userID);
        }
        
        public UserLoged IsLoginValid(UserAccountDM userAccountDM)
        {
            return _userModule.GetUserLoged(userAccountDM); 
        }


        public UserDetailsDM GetUserDetails(int userID)
        {
            return _userModule.SelectUserDetails(userID);
        }

        public UserLayoutDM GetUserLayout(int userID)
        {
            return _userModule.SelectUserLayout(userID);
        }

      

       

        public List<UserDM> GetUsersIndex(int adminClientID)
        {
            return adminClientID == 0 ?
                _userModule.GetAllUsers() :
                GetClientUsers(adminClientID);
        }

        public List<UserDM> GetClientUsers(int clientID)
        {
            if (clientID == 0)
                return new List<UserDM>();

            int[] clientAndChilds = _clientCache.GetClientAndChilds(clientID);

            return _userModule.GetAllClientUsers(clientAndChilds);
        }

  

        public void Update(UserDetailsDM userDetailsDM)
        {
            _userModule.Update(userDetailsDM);
            _uow.SaveChanges();
        }

        public void ChangePassword(UserAccountDM userAccountDM)
        {
            _userModule.UpdatePassword(userAccountDM);
            _uow.SaveChanges();
        }



        /********************         Admin       **********************/

        


        public int Delete(int userID)
        {
            User user = _userModule.GetUser(userID);
            int clientID = user.ClientID;

            _userModule.Delete(user);
            _uow.SaveChanges();

            return clientID;
        }
    }
}
