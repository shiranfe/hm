using Common;
using System.Collections.Generic;
using DAL;
using System.Linq;
using BL.Moduls;
using Repository;
using Microsoft.Practices.Unity;
using System;
using AutoMapper;
using System.Linq.Expressions;

namespace BL
{
    public class CrudBL<TModel, TEntity> where TEntity : class
    {
        private readonly IUnitOfWork _uow;
        private IRepository<TEntity> _entityDal;
       

        //public SupplierBL()
        //    : this(new SupplierModule(), new SupplierCache())
        //{
            
        //}

        public CrudBL([Dependency]IUnitOfWork uow)
        {
            _uow = uow;
            _entityDal = _uow.Repository<TEntity>();
        }

        /***************************************************/


        private TEntity GetSingle(int id)
        {
            return _entityDal.FindById( id);
        }


        internal TModel GetSingleDM(int id)
        {
            var entity = GetSingle(id);
            var model = new TModel();

            EmtityToModel(model, entity);

            return model;

        }


        internal List<TModel> GetList()
        {
            

        }




        /********************         CHANGE       **********************/

        public void Update(TModel model)
        {
              TEntity entity  = GetSingle
            if (model.CatalogItemID > 0)
                Edit(model);
            else
                Add(model);

            _uow.SaveChanges();
        }

        private void Add(TModel model)
        {
            TEntity entity = new TEntity();

            ModelToEntity(model, entity);

            _entityDal.Add(entity);



        }

        private void Edit(TModel model)
        {
            TEntity entity = GetSingle(model.CatalogItemID);
            ModelToEntity(model, entity);



            //_uow.SaveChanges();


        }


        private void ModelToEntity(TModel model, TEntity entity)
        {
            Mapper.DynamicMap<TModel, TEntity>(model, entity);
        }

        private void EmtityToModel(TModel model, TEntity entity)
        {
            Mapper.DynamicMap<TEntity, TModel>(entity, model);
        }





        internal void Delete(int id)
        {
            var entity = GetSingle(id);
            _entityDal.Remove(entity);
        }


    }
}
