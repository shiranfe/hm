using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Common;
using DAL;
using Microsoft.Practices.Unity;
using Repository;

namespace BL
{
    public class JobEquipmentModule
    {
        private readonly IUnitOfWork _uow;
       
        private readonly IRepository<JobEquipment> _entityDal;
      
        public JobEquipmentModule([Dependency]IUnitOfWork uow)
        {
            _uow = uow;
      
            _entityDal = _uow.Repository<JobEquipment>();
          
        }


        /***************************************************/

       

        private JobEquipment GetSingle(int TemplateID)
        {
            return _entityDal.SingleOrDefault(x => x.JobEquipmentID == TemplateID);
        }


        internal JobEquipmentDM GetSingleDM(int TemplateID)
        {
            var entity  =  GetSingle(TemplateID);
            var model = new JobEquipmentDM();
          
            EntityToModel(model, entity);

            return model;

        }

        internal IQueryable<JobEquipment> GetQuer()
        {
            return _entityDal.GetQueryableFresh();
        }

        //internal void GetList(JobEquipmentFilterDm filter)
        //{

        //    var quer = GetQuer();

        //    var list = GetListByFilter(quer, filter);

        //    filter.TableList = CreateList(list);

        //}

        private List<JobEquipmentDM> CreateList(List<JobEquipment> list)
        {
            return (from x in GetQuer()
                    select new JobEquipmentDM
                    {
                        JobEquipmentID = x.JobEquipmentID
                    })
                    .OrderByDescending(x => x.JobEquipmentID)
                    .ToList();
        }

        //private List<JobEquipment> GetListByFilter(IQueryable<JobEquipment> quer, JobEquipmentFilterDm filter)
        //{
        //    if (filter == null)
        //        return quer.ToList();

        //    /** filter by status*/
        //    Expression<Func<JobEquipment, bool>> condition = i=> i.JobEquipmentID>0;

        //    ///** filter by creator*/
        //    //if (filter.CreatorID > -1)
        //    //    condition = condition.AndAlso(i => i.Job.CreatorID == filter.CreatorID);

        //    ///** filter by Srch*/
        //    //if (!string.IsNullOrEmpty(filter.Srch))
        //    //    condition = condition.AndAlso(i => i.SearchStr.Contains(filter.Srch));

        //    var list = quer.AsEnumerable().Where(condition.Compile())
        //        .OrderByDescending(x => x.JobEquipmentID);

        //    return LinqHelpers.FilterByPage(filter, list);
        //}


        /********************         CHANGE       **********************/



        public void Update(JobEquipmentDM model)
        {
           
            if (model.JobEquipmentID > 0)
                Edit(model);
            else
                Add(model);

        } 

        private void Add(JobEquipmentDM model)
        {
            JobEquipment entity = new JobEquipment();
            
            ModelToEntity(model, entity);

            _entityDal.Add(entity);

            _uow.SaveChanges();

            model.JobEquipmentID = entity.JobEquipmentID;
        }

        private void Edit(JobEquipmentDM model)
        {
            JobEquipment entity = GetSingle(model.JobEquipmentID);
            
            ModelToEntity(model, entity);

            _uow.SaveChanges();
        }


        private void ModelToEntity(JobEquipmentDM model, JobEquipment entity)
        {        
            Mapper.Map(model, entity);
        }

        private void EntityToModel(JobEquipmentDM model, JobEquipment entity)
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
