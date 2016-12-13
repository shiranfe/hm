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
    public class ContactInfoModule
    {
        private readonly IUnitOfWork _uow;
        private readonly IRepository<ContactInfo> _contactInfoDal;

        public ContactInfoModule([Dependency]IUnitOfWork uow)
        {
            _uow = uow;
            _contactInfoDal = _uow.Repository<ContactInfo>();
        }


        /***************************************************/



        internal ContactInfoDM GetSingleInfo(int objID, ObjType objType)
        {
            var entity = GetSingle(objID, objType);

            ContactInfoDM model = new ContactInfoDM();
            EntityToModel(entity, model);

            return model;
        }

        private ContactInfo GetSingle(int objID, ObjType objType)
        {
            var entity = _contactInfoDal.SingleOrDefault(x => x.ObjID == objID && x.ObjType == objType);

            return entity;
        }

        private void EntityToModel(ContactInfo entity, ContactInfoDM model)
        {
            Mapper.DynamicMap(entity, model);
        }

        private void ModelToEntity(ContactInfo entity, ContactInfoDM model)
        {
            Mapper.DynamicMap(model, entity);
        }


        internal void Update(ContactInfoDM model)
        {
            var entity = GetSingle(model.ObjID, model.ObjType);

             ModelToEntity(entity, model);
             _contactInfoDal.Update(entity);
             //_uow.SaveChanges();
        }

        internal void Add(ContactInfoDM model)
        {
            var entity = new ContactInfo();

            ModelToEntity(entity, model);

            _contactInfoDal.Add(entity);

            //_uow.SaveChanges();
        }

        internal void Delete(int objID, ObjType objType)
        {
            var entity = GetSingle(objID, objType);

            _contactInfoDal.Remove(entity);

        }

        public string GetEmail(int objID, ObjType objType)
        {
           return _contactInfoDal.SingleOrDefault(x => x.ObjID == objID && x.ObjType == objType)?.Email ?? "";

        }

        public ContactInfo GetSingleByEmail(string from)
        {
            return _contactInfoDal.Where(x => x.Email == from && x.ObjType == ObjType.Employee).SingleOrDefault();
        }


        public void SetSenderInfo(EmailDm model)
        {
            var sender = GetSingleByEmail(model.From);
            if(sender==null)
                throw new Exception("cant fin sender by email");

            model.EmailPassword = sender.EmailPassword;

            model.Creator = _uow.Repository<Employee>().SingleOrDefault(x => x.EmployeeID == sender.ObjID)?.FullName;

        }
    }
}
