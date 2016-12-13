using AutoMapper;
using DAL;
using Common;
using Microsoft.Practices.Unity;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace BL
{
    public class BankTaskModule
    {
        private readonly IUnitOfWork _uow;
       
        private readonly IRepository<BankTask> _entityDal;

      
        private readonly BankTask_FieldModule _bankTask_FieldModule;

        public BankTaskModule([Dependency]IUnitOfWork uow,
            [Dependency]BankTask_FieldModule bankTask_FieldModule)
        {
            _uow = uow;
      
            _entityDal = _uow.Repository<BankTask>();
     
            _bankTask_FieldModule = bankTask_FieldModule;
        }


        /***************************************************/

       

        private BankTask GetSingle(int id)
        {
            return _entityDal.SingleOrDefault(x => x.BankTaskID == id);
        }


        internal BankTaskDM GetSingleDM(int id)
        {
            var entity  =  GetSingle(id);
            var model = new BankTaskDM();
          
            EntityToModel(model, entity);

            model.TaskFields = _bankTask_FieldModule.GetTaskFields(entity.BankTask_Field);

            return model;

        }

        internal IQueryable<BankTask> GetQuer()
        {
            return _entityDal.GetQueryableFresh();
        }

        internal void GetList(BankTaskFilterDm filter)
        {

            var quer = GetQuer();

            var list = GetListByFilter(quer, filter);
             
            filter.TableList = CreateList(list);

        }

        private List<BankTaskDM> CreateList(List<BankTask> list)
        {
            return (from x in list
                    select new BankTaskDM
                    {
                        BankTaskID = x.BankTaskID,
                        TaskName = x.TaskName,
                        ManagerNotes=x.ManagerNotes,
                        //TaskFields = x.BankTask_Field
                        //    .Select(y => TaskFieldToDm(y)).ToList()
                    })
                    .OrderByDescending(x => x.BankTaskID)
                    .ToList();
        }

       

        private List<BankTask> GetListByFilter(IQueryable<BankTask> quer, BankTaskFilterDm filter)
        {
            if (filter == null)
                return quer.ToList();

            /** filter by status*/
            Expression<Func<BankTask, bool>> condition = i=> i.BankTaskID>0;

            ///** filter by creator*/
            //if (filter.CreatorID > -1)
            //    condition = condition.AndAlso(i => i.Job.CreatorID == filter.CreatorID);

            ///** filter by Srch*/
            if (!string.IsNullOrEmpty(filter.Srch))
                condition = condition.AndAlso(i => i.SearchStr.Contains(filter.Srch));

            var list = quer.AsEnumerable().Where(condition.Compile())
                .OrderByDescending(x => x.BankTaskID);

            return LinqHelpers.FilterByPage(filter, list);
        }


        /********************         CHANGE       **********************/



        public void Update(BankTaskDM model)
        {
            var entity = (model.BankTaskID > 0) ? Edit(model) : Add(model);
   
            _uow.SaveChanges();

            model.BankTaskID = entity.BankTaskID;
        }

        private BankTask Add(BankTaskDM model)
        {
            BankTask entity = new BankTask();
            
            ModelToEntity(model, entity);

            _entityDal.Add(entity);

            return entity;
        }

        private BankTask Edit(BankTaskDM model)
        {
            BankTask entity = GetSingle(model.BankTaskID);
            
            ModelToEntity(model, entity);

            return entity;
        }


        private void ModelToEntity(BankTaskDM model, BankTask entity)
        {        
            Mapper.DynamicMap(model, entity);
        }

        private void EntityToModel(BankTaskDM model, BankTask entity)
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
