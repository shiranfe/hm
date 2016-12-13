using AutoMapper;
using DAL;
using Common;
using Microsoft.Practices.Unity;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Data.Entity;

namespace BL
{
    public class BankTask_FieldModule
    {
        private readonly IUnitOfWork _uow;
        private readonly FieldPoolModule _fieldPoolModule;
        private readonly IRepository<BankTask_Field> _entityDal;
      
        public BankTask_FieldModule([Dependency]IUnitOfWork uow,
            [Dependency]FieldPoolModule fieldPoolModule)
        {
            _uow = uow;
            _fieldPoolModule = fieldPoolModule;
            _entityDal = _uow.Repository<BankTask_Field>();
            _fieldPoolModule = fieldPoolModule;
        }


        /***************************************************/

       

        private BankTask_Field GetSingle(int id)
        {
            return _entityDal.SingleOrDefault(x => x.BankTask_FieldID == id);
        }


        internal BankTask_FieldDM GetSingleDM(int id)
        {
            var entity  =  GetSingle(id);
            var model = new BankTask_FieldDM();
          
            EntityToModel(model, entity);

            return model;

        }

        internal IQueryable<BankTask_Field> GetQuer()
        {
            return _entityDal.GetQueryableFresh();
        }

        internal void GetList(BankTask_FieldFilterDm filter)
        {

            var quer = GetQuer();

            var list = GetListByFilter(quer, filter);

            filter.TableList = CreateList(list);

        }

        private List<BankTask_FieldDM> CreateList(List<BankTask_Field> list)
        {
            return (from x in list
                    select new BankTask_FieldDM
                    {
                        BankTask_FieldID = x.BankTask_FieldID,
                    })
                    .OrderByDescending(x => x.BankTask_FieldID)
                    .ToList();
        }

        private List<BankTask_Field> GetListByFilter(IQueryable<BankTask_Field> quer, BankTask_FieldFilterDm filter)
        {
            if (filter == null)
                return quer.ToList();

            /** filter by status*/
            Expression<Func<BankTask_Field, bool>> condition = i=> i.BankTask_FieldID>0;

            ///** filter by creator*/
            //if (filter.CreatorID > -1)
            //    condition = condition.AndAlso(i => i.Job.CreatorID == filter.CreatorID);

            ///** filter by Srch*/
            //if (!string.IsNullOrEmpty(filter.Srch))
            //    condition = condition.AndAlso(i => i.SearchStr.Contains(filter.Srch));

            var list = quer.AsEnumerable().Where(condition.Compile())
                .OrderByDescending(x => x.BankTask_FieldID);

            return LinqHelpers.FilterByPage(filter, list);
        }


        public JobTaskGroupFieldDM GetTaskField(BankTask_Field enity)
        {
            var res = TaskFieldToDm(enity);

            _fieldPoolModule.SetFieldsPickList(new List<StepGroupFieldDM> { (StepGroupFieldDM)res });

            return res;
        }

        public List<JobTaskGroupFieldDM> GetTaskFields(ICollection<BankTask_Field> list)
        {
            var res = list.Select(y => TaskFieldToDm(y)).ToList();

            _fieldPoolModule.SetFieldsPickList(res.Select(x => (StepGroupFieldDM)x).ToList());

            return res;
        }

        private static JobTaskGroupFieldDM TaskFieldToDm(BankTask_Field x)
        {
            BankField flds = x.BankField;

            if (flds.FieldPool == null)
                throw new Exception("flds.FieldPool empty");

            return new JobTaskGroupFieldDM
            {

                //FieldValue = x.FieldValue,
                TaskFieldID = x.BankTask_FieldID,
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
                PickListFromTable = flds.FieldPool.PickListFromTable,
            };
        }

        /********************         CHANGE       **********************/



        public BankTask_Field Update(BankTask_FieldDM model)
        {
            BankTask_Field entity = model.BankTask_FieldID > 0 ?
               Edit(model) : Add(model);


            _uow.SaveChanges();

            model.BankTask_FieldID = entity.BankTask_FieldID;
            /** need to load bankField*/
            return _entityDal.Where(x => x.BankTask_FieldID == entity.BankTask_FieldID)
               .Include("BankField.FieldPool").Single();
        }

        private BankTask_Field Add(BankTask_FieldDM model)
        {
            BankTask_Field entity = new BankTask_Field();
            
            ModelToEntity(model, entity);

            _entityDal.Add(entity);

            return entity;
        }

        private BankTask_Field Edit(BankTask_FieldDM model)
        {
            BankTask_Field entity = GetSingle(model.BankTask_FieldID);
            
            ModelToEntity(model, entity);

            return entity;
        }


        private void ModelToEntity(BankTask_FieldDM model, BankTask_Field entity)
        {        
            Mapper.DynamicMap(model, entity);
        }

        private void EntityToModel(BankTask_FieldDM model, BankTask_Field entity)
        {         
           Mapper.DynamicMap(entity, model);
        }


        internal void Delete(int id)
        {
            var entity = GetSingle(id);
            _entityDal.Remove(entity);
        }
    }
}
