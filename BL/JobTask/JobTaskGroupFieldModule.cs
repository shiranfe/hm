using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using Common;
using DAL;
using Microsoft.Practices.Unity;
using Repository;

namespace BL
{
    public class JobTaskGroupFieldModule
    {
        private readonly IUnitOfWork _uow;
       
        private readonly IRepository<JobTaskGroupField> _entityDal;
      
        public JobTaskGroupFieldModule([Dependency]IUnitOfWork uow)
        {
            _uow = uow;
      
            _entityDal = _uow.Repository<JobTaskGroupField>();
          
        }


        /***************************************************/

       

        private JobTaskGroupField GetSingle(int TemplateID)
        {
            return _entityDal.SingleOrDefault(x => x.JobTaskGroupFieldID == TemplateID);
        }


        internal JobTaskGroupFieldDM GetSingleDM(int TemplateID)
        {
            var entity  =  GetSingle(TemplateID);
            var model = new JobTaskGroupFieldDM();
          
            EntityToModel(model, entity);

            return model;

        }

        internal IQueryable<JobTaskGroupField> GetQuer()
        {
            return _entityDal.GetQueryableFresh();
        }

        internal void GetList(JobTaskGroupFieldFilterDm filter)
        {

            var quer = GetQuer();

            var list = GetListByFilter(quer, filter);

            filter.TableList = CreateList(list);

        }

        private List<JobTaskGroupFieldDM> CreateList(List<JobTaskGroupField> list)
        {
            return (from x in GetQuer()
                    select new JobTaskGroupFieldDM
                    {
                        Id = x.JobTaskGroupFieldID
                    })
                    .OrderByDescending(x => x.Id)
                    .ToList();
        }

        private List<JobTaskGroupField> GetListByFilter(IQueryable<JobTaskGroupField> quer, JobTaskGroupFieldFilterDm filter)
        {
            if (filter == null)
                return quer.ToList();

            /** filter by status*/
            Expression<Func<JobTaskGroupField, bool>> condition = i=> i.JobTaskGroupFieldID>0;

            ///** filter by creator*/
            //if (filter.CreatorID > -1)
            //    condition = condition.AndAlso(i => i.Job.CreatorID == filter.CreatorID);

            ///** filter by Srch*/
            //if (!string.IsNullOrEmpty(filter.Srch))
            //    condition = condition.AndAlso(i => i.SearchStr.Contains(filter.Srch));

            var list = quer.AsEnumerable().Where(condition.Compile())
                .OrderByDescending(x => x.JobTaskGroupFieldID);

            return LinqHelpers.FilterByPage(filter, list);
        }


        /********************         CHANGE       **********************/



        public void Update(JobTaskGroupFieldDM model)
        {
           
            if (model.Id > 0)
                Edit(model);
            else
                Add(model);

        }

        private void Add(JobTaskGroupFieldDM model)
        {
            JobTaskGroupField entity = new JobTaskGroupField();
            
            ModelToEntity(model, entity);

            _entityDal.Add(entity);    
        }

        private void Edit(JobTaskGroupFieldDM model)
        {
            JobTaskGroupField entity = GetSingle(model.Id);
            
            ModelToEntity(model, entity);
        }


        private void ModelToEntity(JobTaskGroupFieldDM model, JobTaskGroupField entity)
        {        
            Mapper.Map(model, entity);
        }

        private void EntityToModel(JobTaskGroupFieldDM model, JobTaskGroupField entity)
        {         
           Mapper.Map(entity, model);
        }


        internal void Delete(int TemplateID)
        {
            var entity = GetSingle(TemplateID);
            _entityDal.Remove(entity);
        }
    }
}
