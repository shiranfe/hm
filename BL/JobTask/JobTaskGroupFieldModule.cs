using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using Common;
using DAL;
using Microsoft.Practices.Unity;
using Repository;
using System.Data.Entity;

namespace BL
{
    public class JobTaskFieldModule
    {
        private readonly IUnitOfWork _uow;
        private readonly FieldPoolModule _fieldPoolModule;
        private readonly IRepository<JobTaskField> _entityDal;
      
        public JobTaskFieldModule([Dependency]IUnitOfWork uow,
            [Dependency]FieldPoolModule fieldPoolModule
            )
        {
            _uow = uow;
            _fieldPoolModule = fieldPoolModule;
            _entityDal = _uow.Repository<JobTaskField>();
          
        }


        /***************************************************/

       

        private JobTaskField GetSingle(int TemplateID)
        {
            return _entityDal.SingleOrDefault(x => x.JobTaskFieldID == TemplateID);
        }


        internal JobTaskFieldDM GetSingleDM(int TemplateID)
        {
            var entity  =  GetSingle(TemplateID);
            var model = new JobTaskFieldDM();
          
            EntityToModel(model, entity);

            return model;

        }

        private List<JobTaskField> GetMulty(int[] ids)
        {
            return _entityDal.ToList(x => ids.Contains(x.JobTaskFieldID));
        }

       

        internal IQueryable<JobTaskField> GetQuer()
        {
            return _entityDal.GetQueryableFresh();
        }

        internal void GetList(JobTaskFieldFilterDm filter)
        {

            var quer = GetQuer();

            var list = GetListByFilter(quer, filter);

            filter.TableList = CreateList(list);

        }

        private List<JobTaskFieldDM> CreateList(List<JobTaskField> list)
        {
            return (from x in GetQuer()
                    select new JobTaskFieldDM
                    {
                        Id = x.JobTaskFieldID
                    })
                    .OrderByDescending(x => x.Id)
                    .ToList();
        }

        private List<JobTaskField> GetListByFilter(IQueryable<JobTaskField> quer, JobTaskFieldFilterDm filter)
        {
            if (filter == null)
                return quer.ToList();

            /** filter by status*/
            Expression<Func<JobTaskField, bool>> condition = i=> i.JobTaskFieldID>0;

            ///** filter by creator*/
            //if (filter.CreatorID > -1)
            //    condition = condition.AndAlso(i => i.Job.CreatorID == filter.CreatorID);

            ///** filter by Srch*/
            //if (!string.IsNullOrEmpty(filter.Srch))
            //    condition = condition.AndAlso(i => i.SearchStr.Contains(filter.Srch));

            var list = quer.AsEnumerable().Where(condition.Compile())
                .OrderByDescending(x => x.JobTaskFieldID);

            return LinqHelpers.FilterByPage(filter, list);
        }


        public JobTaskFieldDM GetTaskField(JobTaskField enity)
        {
            JobTaskFieldDM res = TaskFieldToDm(enity);

            _fieldPoolModule.SetFieldsPickList(new List<StepGroupFieldDM> { res });

            return res;
        }

        private static JobTaskFieldDM TaskFieldToDm(JobTaskField x)
        {
            BankField flds = x.BankField;

            if (flds.FieldPool == null)
                throw new Exception("flds.FieldPool empty");

            return new JobTaskFieldDM
            {
                //FieldValue = x.FieldValue,
                Id = x.JobTaskFieldID,
                OrderVal = x.OrderVal,
                IsRequired = x.IsRequired,

                // JobRefubrishStepGroupID = flds.DynamicGroup.DynamicGroupID,
                //SubGroupID = not availble becuase fields attaced only to parent group anyway
                // DynamicGroupFieldID = flds.DynamicGroupFieldID,
                // FieldName = flds.DynamicGroupFieldID + "_" + flds.,
                FieldNameStr = flds.FieldNameStr,
                FieldTypeID = flds.FieldPool.FieldTypeID,
                FieldUnit = flds.FieldPool.FieldUnit,

                // OrderVal = flds.OrderVal,
                PickListEntity = flds.FieldPool.PickListEntity,
                PickListFromTable = flds.FieldPool.PickListFromTable
            };
        }

        /********************         CHANGE       **********************/

        internal void Sort(int[] ids)
        {
            var entites = GetMulty(ids);

            foreach (var field in entites)
            {
                field.OrderVal = Array.IndexOf(ids, field.JobTaskFieldID) + 1;
                _entityDal.Update(field);
            }
        }

        public JobTaskField Update(JobTaskFieldDM model)
        {

            var entity = model.Id > 0 ?
             Edit(model) : Add(model);


            _uow.SaveChanges();

            model.Id = entity.JobTaskFieldID;
            /** need to load bankField*/
            return _entityDal.Where(x => x.JobTaskFieldID == entity.JobTaskFieldID)
               .Include("BankField.FieldPool").Single();

        }

        private JobTaskField Add(JobTaskFieldDM model)
        {
            JobTaskField entity = new JobTaskField();
            
            ModelToEntity(model, entity);

            _entityDal.Add(entity);

            return entity;
        }

        private JobTaskField Edit(JobTaskFieldDM model)
        {
            JobTaskField entity = GetSingle(model.Id);
            
            ModelToEntity(model, entity);

            return entity;
        }


        private void ModelToEntity(JobTaskFieldDM model, JobTaskField entity)
        {        
            Mapper.Map(model, entity);
        }

        private void EntityToModel(JobTaskFieldDM model, JobTaskField entity)
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
