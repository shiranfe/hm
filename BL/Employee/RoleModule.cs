using AutoMapper;
using Common;
using Microsoft.Practices.Unity;
using Repository;
using System.Collections.Generic;
using System.Linq;
using DAL;
using System;
using BL.Moduls;

namespace BL
{
    public class RoleModule
    {
        private readonly IUnitOfWork _uow;

        private readonly IRepository<Role> _entityDal;
        private readonly LangModule _langModule;
       public RoleModule([Dependency]IUnitOfWork uow,
           [Dependency]LangModule langModule)
        {
            _uow = uow;

            _entityDal = _uow.Repository<Role>();
            _langModule = langModule;
        }


        /***************************************************/



        private Role GetSingle(int roleID)
        {
            return _entityDal.SingleOrDefault(x => x.RoleID == roleID);
        }


        internal RoleDM GetSingleDM(int roleID)
        {
            var entity = GetSingle(roleID);
            var model = new RoleDM();

            EntityToModel(model, entity);

            model.RoleHebrew = entity.LangString.IL;
            model.RoleEnglish = entity.LangString.EN;

            return model;

        }


        internal List<RoleDM> GetList()
        {

            return (from x in GetQuer()
                    select new RoleDM
                    {
                        RoleID = x.RoleID,
                        RoleCost = x.RoleCost,
                        RolePrice = x.RolePrice,
                        RoleHebrew = x.LangString.IL
                    })
                    .OrderByDescending(x => x.RoleID)
                    .ToList();


        }

        internal IQueryable<Role> GetQuer()
        {
            return _entityDal.GetQueryableFresh();
        }

        internal List<ComponentTypeDM> GetComponents()
        {
           
            return (from x in GetQuer()
                   // join l in _langString.GetQueryable() on x.RoleNameKey equals l.Key
                    select new ComponentTypeDM
                    {
                        Id = x.RoleID,
                        Text = x.LangString.IL,
                        Cost = x.RoleCost,
                        Price = x.RolePrice,
                        ComponentTypeID = CatalogItemType.Personnel
                    }).ToList();


        }


        /********************         CHANGE       **********************/



        public void Update(RoleDM model)
        {
            AddOrUpdateWord(model);

            if (model.RoleID > 0)
                Edit(model);
            else
                Add(model);

        }

        private void Add(RoleDM model)
        {
            Role entity = new Role();

         

            ModelToEntity(model, entity);

            _entityDal.Add(entity);

           
        }

        private void AddOrUpdateWord(RoleDM model)
        {
            if (string.IsNullOrEmpty(model.RoleEnglish))
                throw new Exception("must have engilsh name");

            model.RoleNameKey = "EmployeeRole_" + model.RoleEnglish;
           
            _langModule.AddOrUpdateWord(new LangString
            {
                Key = model.RoleNameKey,
                EN = model.RoleEnglish,
                IL = model.RoleHebrew
            });
        }

        private void Edit(RoleDM model)
        {
            Role entity = GetSingle(model.RoleID);

            ModelToEntity(model, entity);
        }


        private void ModelToEntity(RoleDM model, Role entity)
        {
            Mapper.DynamicMap<RoleDM, Role>(model, entity);
        }

        private void EntityToModel(RoleDM model, Role entity)
        {
            Mapper.DynamicMap<Role, RoleDM>(entity, model);
        }


        internal void Delete(int roleID)
        {
            var entity = GetSingle(roleID);
            _entityDal.Remove(entity);
        }
    }
}
