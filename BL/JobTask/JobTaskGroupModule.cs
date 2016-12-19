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
    public class JobTaskGroupModule
    {
        private readonly IUnitOfWork _uow;
       
        private readonly IRepository<JobTaskGroup> _entityDal;
      
        public JobTaskGroupModule([Dependency]IUnitOfWork uow)
        {
            _uow = uow;
      
            _entityDal = _uow.Repository<JobTaskGroup>();
          
        }


        /***************************************************/

       

        private JobTaskGroup GetSingle(int TemplateID)
        {
            return _entityDal.SingleOrDefault(x => x.JobTaskGroupID == TemplateID);
        }


        internal JobTaskGroupDM GetSingleDM(int TemplateID)
        {
            var entity  =  GetSingle(TemplateID);
            var model = new JobTaskGroupDM();
          
            EntityToModel(model, entity);

            return model;

        }

        internal IQueryable<JobTaskGroup> GetQuer()
        {
            return _entityDal.GetQueryableFresh();
        }

        internal void GetList(JobTaskGroupFilterDm filter)
        {

            var quer = GetQuer();

            var list = GetListByFilter(quer, filter);

            filter.TableList = CreateList(list);

        }

        private List<JobTaskGroupDM> CreateList(List<JobTaskGroup> list)
        {
            return (from x in GetQuer()
                    select new JobTaskGroupDM
                    {
                        JobTaskGroupID = x.JobTaskGroupID
                    })
                    .OrderByDescending(x => x.JobTaskGroupID)
                    .ToList();
        }

        private List<JobTaskGroup> GetListByFilter(IQueryable<JobTaskGroup> quer, JobTaskGroupFilterDm filter)
        {
            if (filter == null)
                return quer.ToList();

            /** filter by status*/
            Expression<Func<JobTaskGroup, bool>> condition = i=> i.JobTaskGroupID>0;

            ///** filter by creator*/
            //if (filter.CreatorID > -1)
            //    condition = condition.AndAlso(i => i.Job.CreatorID == filter.CreatorID);

            ///** filter by Srch*/
            //if (!string.IsNullOrEmpty(filter.Srch))
            //    condition = condition.AndAlso(i => i.SearchStr.Contains(filter.Srch));

            var list = quer.AsEnumerable().Where(condition.Compile())
                .OrderByDescending(x => x.JobTaskGroupID);

            return LinqHelpers.FilterByPage(filter, list);
        }


        /********************         CHANGE       **********************/



        public void Update(JobTaskGroupDM model)
        {
           
            if (model.JobTaskGroupID > 0)
                Edit(model);
            else
                Add(model);

        }

        private void Add(JobTaskGroupDM model)
        {
            JobTaskGroup entity = new JobTaskGroup();
            
            ModelToEntity(model, entity);

            _entityDal.Add(entity);    
        }

        private void Edit(JobTaskGroupDM model)
        {
            JobTaskGroup entity = GetSingle(model.JobTaskGroupID);
            
            ModelToEntity(model, entity);
        }


        private void ModelToEntity(JobTaskGroupDM model, JobTaskGroup entity)
        {        
            Mapper.Map(model, entity);
        }

        private void EntityToModel(JobTaskGroupDM model, JobTaskGroup entity)
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
