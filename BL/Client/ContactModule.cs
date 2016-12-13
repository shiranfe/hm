using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;
using Common;
using Repository;
using Microsoft.Practices.Unity;
using AutoMapper;

namespace BL.Moduls
{
    public class ContactModule
    {
        private readonly IUnitOfWork _uow; 
        private readonly IRepository<User> _userDal;
        private readonly IRepository<ContactInfo> _contactInfoDal;

        private readonly IRepository<vwClientTree> _vwClientTreeDal;
        private readonly ContactInfoModule _contactInfoModule;
        //public UserModule()
        //    : this(_uow.Repository<User>(), _uow.Repository<vwClientTree>())
        //{

        //}

        public ContactModule([Dependency]IUnitOfWork uow,
            [Dependency]ContactInfoModule contactInfoModule)
        {
            _uow = uow;
            _userDal = _uow.Repository<User>();
            _contactInfoDal= _uow.Repository<ContactInfo>();
            _vwClientTreeDal = _uow.Repository<vwClientTree>();
            _contactInfoModule = contactInfoModule;
        }

      


        /***************************************************/



        public User GetSingle(int userID)
        {
            return _userDal.SingleOrDefault(x => x.UserID == userID);
        }

        public UserDetailsDM GetSingleDM(int userID)
        {


            var model = _userDal.Where(x => x.UserID == userID)
                .Select(i => new UserDetailsDM
                {
                    UserID = i.UserID,
                    FirstName = i.FirstName,
                    LastName = i.LastName,
                    ClientName = i.Client.ClientName,
                    ClientID = i.ClientID,
                    Active = i.Active,
                }).Single();

            model.ContactInfoDM = _contactInfoModule.GetSingleInfo(userID, ObjType.User);

            return model;
        }


        internal List<ContactDM> GetList(int clientId)
        {
            var quer = (from contact in _userDal.GetQueryable()
                        join info in _contactInfoDal.Where(x => x.ObjType == ObjType.User)
                        on contact.UserID equals info.ObjID
                        select new { contact,info });

            if (clientId > 0)
                quer = quer.Where(x => x.contact.ClientID == clientId);

            return (from x in quer
                
                    select new ContactDM
                    {
                        UserID = x.contact.UserID,
                        FullName = x.contact.FullName,
                        ClientID = x.contact.ClientID,
                        ClientName = x.contact.Client.ClientName,
                        Mobile = x.info.Mobile,
                        Email = x.info.Email
                    }).OrderByDescending(x => x.UserID).ToList();
        }

   
        public List<ContactDM> GetClientAndChildsContacts(int[] clientAndChilds)
        {
            return (from u in _userDal.GetQueryable()
                    join v in _vwClientTreeDal.GetQueryable()
                    on u.ClientID equals v.ClientChildID
                    where clientAndChilds.Contains(u.ClientID)
                    select new ContactDM
                    {

                        UserID = u.UserID,
                        ClientID = u.ClientID,
                        ClientName = v.ClientChildName,
                        FullName = u.FullName,
                    }).Distinct().ToList();
        }


        /***************************************************/

        public void Update(UserDetailsDM user)
        {


            if (user.UserID > 0)
                Edit(user);
            else
                Add(user);

        }

        private void Edit(UserDetailsDM model)
        {

            
            User entity = GetSingle(model.UserID);

            ModelToEntity(entity, model);

            _userDal.Update(entity);

            _contactInfoModule.Update(model.ContactInfoDM);


        }


        private void Add(UserDetailsDM model)
        {
            
            User entity = new User
            {              
                PermissionID = 8, // Permission_Admin
                Active = true,
            };

            ModelToEntity(entity, model);

            _userDal.Add(entity);

            _uow.SaveChanges();

            model.ContactInfoDM.ObjType = ObjType.User;
            model.ContactInfoDM.ObjID = entity.UserID;
            _contactInfoModule.Add(model.ContactInfoDM);
        }

       

       
        /***************************************************/
        public int Delete(int userID)
        {
            User user = GetSingle(userID);

            if (user.JobContact.Any())
                throw new Exception("contact is linked to jobs");

            _userDal.Remove(user);

            return user.ClientID;
        }


 
        private void EntityToModel(User entity, UserDetailsDM model)
        {
            Mapper.DynamicMap(entity, model);
        }

        private void ModelToEntity(User entity, UserDetailsDM model)
        {
            Mapper.DynamicMap(model, entity);
        }

    }
}
