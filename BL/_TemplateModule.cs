using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using Common;
using Microsoft.Practices.Unity;
using Repository;

namespace BL
{
    public class _TemplateModule
    {
        private readonly IUnitOfWork _uow;
       
        private readonly IRepository<_Template> _entityDal;
      
        public _TemplateModule([Dependency]IUnitOfWork uow)
        {
            _uow = uow;
      
            _entityDal = _uow.Repository<_Template>();
          
        }


        /***************************************************/

       

        private _Template GetSingle(int id)
        {
            return _entityDal.SingleOrDefault(x => x._TemplateID == id);
        }


        internal _TemplateDM GetSingleDM(int id)
        {
            var entity  =  GetSingle(id);
            var model = new _TemplateDM();
          
            EntityToModel(model, entity);

            return model;

        }

        internal IQueryable<_Template> GetQuer()
        {
            return _entityDal.GetQueryableFresh();
        }

        internal void GetList(_TemplateFilterDm filter)
        {

            var quer = GetQuer();

            var list = GetListByFilter(quer, filter);

            filter.TableList = CreateList(list);

        }

        private List<_TemplateDM> CreateList(List<_Template> list)
        {
            return (from x in list
                    select new _TemplateDM
                    {
                        _TemplateID = x._TemplateID
                    })
                    .OrderByDescending(x => x._TemplateID)
                    .ToList();
        }

        private List<_Template> GetListByFilter(IQueryable<_Template> quer, _TemplateFilterDm filter)
        {
            if (filter == null)
                return quer.ToList();

            /** filter by status*/
            Expression<Func<_Template, bool>> condition = i=> i._TemplateID>0;

            ///** filter by creator*/
            //if (filter.CreatorID > -1)
            //    condition = condition.AndAlso(i => i.Job.CreatorID == filter.CreatorID);

            ///** filter by Srch*/
            //if (!string.IsNullOrEmpty(filter.Srch))
            //    condition = condition.AndAlso(i => i.SearchStr.Contains(filter.Srch));

            var list = quer.AsEnumerable().Where(condition.Compile())
                .OrderByDescending(x => x._TemplateID);

            return LinqHelpers.FilterByPage(filter, list);
        }


        /********************         CHANGE       **********************/



        public void Update(_TemplateDM model)
        {

            var entity = (model._TemplateID > 0) ? Edit(model) : Add(model);

            _uow.SaveChanges();

            model._TemplateID = entity._TemplateID;
        }

        private _Template Add(_TemplateDM model)
        {
            _Template entity = new _Template();
            
            ModelToEntity(model, entity);

            _entityDal.Add(entity);

            return entity;
        }

        private _Template Edit(_TemplateDM model)
        {
            _Template entity = GetSingle(model._TemplateID);
            
            ModelToEntity(model, entity);

            return entity;
        }


        private void ModelToEntity(_TemplateDM model, _Template entity)
        {        
            Mapper.Map(model, entity);
        }

        private void EntityToModel(_TemplateDM model, _Template entity)
        {         
           Mapper.Map(entity, model);
        }


        internal void Delete(int id)
        {
            var entity = GetSingle(id);
            _entityDal.Remove(entity);
        }
    }
}
