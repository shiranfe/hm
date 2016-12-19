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
    public class JobTaskEmployeeModule
    {
        private readonly IUnitOfWork _uow;
       
        private readonly IRepository<JobTaskEmployee> _entityDal;
      
        public JobTaskEmployeeModule([Dependency]IUnitOfWork uow)
        {
            _uow = uow;
      
            _entityDal = _uow.Repository<JobTaskEmployee>();
          
        }

      


        /***************************************************/



        private JobTaskEmployee GetSingle(int templateID)
        {
            return _entityDal.SingleOrDefault(x => x.JobTaskEmployeeID == templateID);
        }


        internal JobTaskEmployeeDM GetSingleDM(int templateID)
        {
            var entity  =  GetSingle(templateID);
            var model = new JobTaskEmployeeDM();
          
            EntityToModel(model, entity);

            return model;

        }

        internal IQueryable<JobTaskEmployee> GetQuer()
        {
            return _entityDal.GetQueryableFresh();
        }

        internal void GetList(JobTaskEmployeeFilterDm filter)
        {

            var quer = GetQuer();

            var list = GetListByFilter(quer, filter);

            filter.TableList = CreateList(list);

        }

        private List<JobTaskEmployeeDM> CreateList(List<JobTaskEmployee> list)
        {
            return (from x in list
                    select new JobTaskEmployeeDM
                    {
                        JobTaskEmployeeID = x.JobTaskEmployeeID
                    })
                    .OrderByDescending(x => x.JobTaskEmployeeID)
                    .ToList();
        }

        private List<JobTaskEmployee> GetListByFilter(IQueryable<JobTaskEmployee> quer, JobTaskEmployeeFilterDm filter)
        {
            if (filter == null)
                return quer.ToList();

            /** filter by status*/
            Expression<Func<JobTaskEmployee, bool>> condition = i=> i.JobTaskEmployeeID>0;

            ///** filter by creator*/
            //if (filter.CreatorID > -1)
            //    condition = condition.AndAlso(i => i.Job.CreatorID == filter.CreatorID);

            ///** filter by Srch*/
            //if (!string.IsNullOrEmpty(filter.Srch))
            //    condition = condition.AndAlso(i => i.SearchStr.Contains(filter.Srch));

            var list = quer.AsEnumerable().Where(condition.Compile())
                .OrderByDescending(x => x.JobTaskEmployeeID);

            return LinqHelpers.FilterByPage(filter, list);
        }


        internal List<JobTaskEmployeeDM> GetTaskEmps(int jobTaskID)
        {
          return  GetQuer().Where(x => x.JobTaskID == jobTaskID).AsEnumerable()
                .Select(x=> new JobTaskEmployeeDM {
                    JobTaskEmployeeID=x.JobTaskEmployeeID,
                    EmployeeID=x.EmployeeID,
                    JobTaskID=x.JobTaskID,
                    VisitEnd=x.VisitEnd,
                    VisitStart=x.VisitStart,
                    EmployeeName = x.Employee.FullName
                })
                .ToList();
        
           
        }

        /********************         CHANGE       **********************/



        public void Update(JobTaskEmployeeDM model)
        {
           
            if (model.JobTaskEmployeeID > 0)
                Edit(model);
            else
                Add(model);

        }

        private void Add(JobTaskEmployeeDM model)
        {
            JobTaskEmployee entity = new JobTaskEmployee();
            
            ModelToEntity(model, entity);

            _entityDal.Add(entity);

            _uow.SaveChanges();
             
            model.JobTaskEmployeeID = entity.JobTaskEmployeeID;
        }

        private void Edit(JobTaskEmployeeDM model)
        {
            JobTaskEmployee entity = GetSingle(model.JobTaskEmployeeID);
            
            ModelToEntity(model, entity);

            _uow.SaveChanges();
        }


        private void ModelToEntity(JobTaskEmployeeDM model, JobTaskEmployee entity)
        {        
            Mapper.Map(model, entity);
        }

        private void EntityToModel(JobTaskEmployeeDM model, JobTaskEmployee entity)
        {         
           Mapper.Map(entity, model);
        }


        internal void Delete(int templateID)
        {
            var entity = GetSingle(templateID);
            _entityDal.Remove(entity);
        }
    }
}
