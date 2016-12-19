using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Common;
using DAL;
using Microsoft.Practices.Unity;
using Repository;

namespace BL.Moduls
{
    public class UserModule
    {
        private readonly IUnitOfWork _uow; 
        private readonly IRepository<User> _userDal;
        private readonly IRepository<vwClientTree> _vwClientTreeDal;
        private readonly ContactInfoModule _contactInfoModule;
        //public UserModule()
        //    : this(_uow.Repository<User>(), _uow.Repository<vwClientTree>())
        //{

        //}

        public UserModule([Dependency]IUnitOfWork uow,
            [Dependency]ContactInfoModule contactInfoModule)
        {
            _uow = uow;
            _userDal = _uow.Repository<User>();
            _vwClientTreeDal = _uow.Repository<vwClientTree>();
            _contactInfoModule = contactInfoModule;
        }

        readonly string _userNameExistError = "שם המשתמש קיים במערכת. בחר שם משתמש ייחודי";

        /***************************************************/


        public int GetClientID(int userID)
        {

            return _userDal
                .Where(x => x.UserID == userID)
                .Select(x => x.ClientID).Single();

        }

        public string SelectUserPassword(int userID)
        {

            User user = _userDal.SingleOrDefault(x => x.UserID == userID);
            return user.Password;

        }

        public string SelectUserFullName(int userID)
        {

            User user = _userDal.SingleOrDefault(x => x.UserID == userID);
            return user.FirstName + ' ' + user.LastName;

        }



        public User GetUser(int userID)
        {
            return _userDal.SingleOrDefault(x => x.UserID == userID);
        }

        public UserLoged GetUserLoged(UserAccountDM userAccountDM)
        {

            return (from user in _userDal
                    where user.Username == userAccountDM.Username &&
                           user.Password == userAccountDM.Password
                    select new UserLoged
                    {
                        UserID = user.UserID,
                        ClientID = user.ClientID
                    }).SingleOrDefault();

        }

        public UserDetailsDM SelectUserDetails(int userID)
        {


            var model = _userDal.Where(x => x.UserID == userID)
                .Select(i => new UserDetailsDM
                {
                    UserID = i.UserID,
                    FirstName = i.FirstName,
                    LastName = i.LastName,
                    ClientName = i.Client.ClientName,
                    Password = i.Password,
                    ClientID = i.ClientID,
                    Username = i.Username,
                    Active = i.Active
                }).Single();

            model.ContactInfoDM = _contactInfoModule.GetSingleInfo(userID, ObjType.User);

            return model;
        }

        public UserLayoutDM SelectUserLayout(int userID)
        {

            return _userDal.Where(x => x.UserID == userID).Select(i => new UserLayoutDM
            {
                ClientID = i.ClientID,
                ClientName = i.Client.ClientName,
                FullName = i.FullName
            }).Single();

        }
        
         

        public List<UserDM> GetAllUsers()
        {
            return (from u in _userDal.GetQueryable()
                    select new UserDM
                    {
                        UserID = u.UserID,
                        Username = u.Username,
                        ClientID = u.ClientID,
                        ClientName = u.Client.ClientName,
                        FullName = u.FullName
                        
                    })
                    .OrderByDescending(x=>x.Username!=null)
                    .ThenByDescending(x=>x.UserID).ToList();
        }

        public List<UserDM> GetAllClientUsers(int[] clientAndChilds)
        {
            return (from u in _userDal.GetQueryable()
                    join v in _vwClientTreeDal.GetQueryable()
                    on u.ClientID equals v.ClientChildID
                    where clientAndChilds.Contains(u.ClientID)
                    select new UserDM
                    {
                        UserID = u.UserID,
                        Username = u.Username,
                        ClientID = u.ClientID,
                        ClientName = v.ClientChildName,
                        FullName = u.FullName

                    }).Distinct().ToList();
        }


        /***************************************************/

        public void UpdatePassword(UserAccountDM userAccountDM)
        {

            User user = _userDal.SingleOrDefault(x => x.UserID == userAccountDM.UserID);
            user.Password = userAccountDM.Password;

        }

        public void Update(UserDetailsDM user)
        {


            if (user.UserID > 0)
                Edit(user);
            else
                Add(user);

        }

        private void Edit(UserDetailsDM model)
        {
          
            IsUserExist(model.Username, model.UserID);
            
            User entity = GetUser(model.UserID);

            ModelToEntity(entity, model);

            _userDal.Update(entity);

            _contactInfoModule.Update(model.ContactInfoDM);


        }


        private void Add(UserDetailsDM model)
        {
            IsUserExist(model.Username);
              

          
            User entity = new User
            {              
                PermissionID = 8, // Permission_Admin
                Active = true
            };

            ModelToEntity(entity, model);


            _userDal.Add(entity);

            _uow.SaveChanges();

            model.ContactInfoDM.ObjType = ObjType.User;
            model.ContactInfoDM.ObjID = entity.UserID;
            _contactInfoModule.Add(model.ContactInfoDM);
        }

       

       
        /***************************************************/
        public void Delete(User user)
        {
            _userDal.Remove(user);
        }



        private void IsUserExist(string userName)
        {
            if (!string.IsNullOrEmpty(userName) && _userDal.Where(x => x.Username == userName).Any())
                throw new Exception(_userNameExistError);
            
        }

        private void IsUserExist(string userName, int userID)
        {
            if (string.IsNullOrEmpty(userName))
                return;

            if (_userDal.Where(x => x.Username == userName && x.UserID != userID).Any())
                throw new Exception(_userNameExistError);
           
        }


        private void EntityToModel(User entity, UserDetailsDM model)
        {
            Mapper.Map(entity, model);
        }

        private void ModelToEntity(User entity, UserDetailsDM model)
        {
            Mapper.Map(model, entity);
        }

    }
}
