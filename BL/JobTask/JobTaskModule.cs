using AutoMapper;
using BL.Moduls;
using Common;
using DAL;
using Microsoft.Practices.Unity;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace BL
{
    public class JobTaskModule
    {
        private readonly IUnitOfWork _uow;
       
        private readonly IRepository<JobTask> _entityDal;
        private readonly IRepository<Job> _jobDal;
        private readonly FieldPoolModule _fieldPoolModule;

        public JobTaskModule([Dependency]IUnitOfWork uow,
                  [Dependency]FieldPoolModule fieldPoolModule)
        {
            _uow = uow;
      
            _entityDal = _uow.Repository<JobTask>();
            _jobDal = _uow.Repository<Job>();

            _fieldPoolModule = fieldPoolModule;
        }


        /***************************************************/

       

        private JobTask GetSingle(int jobTaskID)
        {
            return _entityDal.GetQueryableFresh().SingleOrDefault(x => x.JobTaskID == jobTaskID);
        }


        internal JobTaskDM GetSingleDM(int jobTaskID)
        {
            var entity  =  GetSingle(jobTaskID);
            var model = new JobTaskDM();
          
            EntityToModel(model, entity);

            model.JobDM = CreateJobDm(entity.Job);

            //Mapper.CreateMap<JobTask, JobTaskDM>();
            //model.TaskGroups = Mapper.Map<List<JobTaskGroupDM>>(entity.JobTaskGroup);
            model.TaskGroups = entity.JobTaskGroup.Select(x => new JobTaskGroupDM
            {
                GroupNameStr = x.GroupNameStr,
                LinkedGroupID = x.LinkedGroupID,
                JobTaskGroupID = x.JobTaskGroupID,
                JobTaskGroupFieldDMs = x.JobTaskGroupField
                    .Select(y => GroupFieldToDm(y)).ToList()
            }).ToList();

            List<JobTaskGroupFieldDM> allFields = model.TaskGroups.SelectMany(x => x.JobTaskGroupFieldDMs).ToList();
            _fieldPoolModule.SetFieldsPickList(allFields.Select(x=> (StepGroupFieldDM)x).ToList());

            return model;

        }

        private static JobTaskGroupFieldDM GroupFieldToDm(JobTaskGroupField x)
        {
            DynamicGroupField flds = x.DynamicGroupField;

            if (flds.DynamicGroup == null)
                throw new Exception("flds.DynamicGroup empty");

            if (flds.FieldPool == null)
                throw new Exception("flds.FieldPool empty");

            return new JobTaskGroupFieldDM
            {
               
                FieldValue = x.FieldValue,
                Id = x.JobTaskGroupFieldID,
                OrderVal = x.OrderVal,
                JobRefubrishStepGroupID = flds.DynamicGroup.DynamicGroupID,
                //SubGroupID = not availble becuase fields attaced only to parent group anyway
                DynamicGroupFieldID = flds.DynamicGroupFieldID,
                // FieldName = flds.DynamicGroupFieldID + "_" + flds.,
                FieldNameStr = flds.FieldNameStr,
                FieldTypeID = flds.FieldPool.FieldTypeID,
                FieldUnit = flds.FieldPool.FieldUnit,
                IsRequired = flds.IsRequired,

               // OrderVal = flds.OrderVal,
                PickListEntity = flds.FieldPool.PickListEntity,
                PickListFromTable = flds.FieldPool.PickListFromTable,
            };
        }
     

        private JobDM CreateJobDm(Job v)
        {
            return new JobDM
            {
                ClientID = v.ClientID,
                CreatorID = v.CreatorID,
                Creator = v.Employee.FullName,
                ClientName = v.Client.ClientName,
                CreatorName = v.Employee.FullName,
                JobName = v.JobName,
                StartDate = v.StartDate,
                EndDate = v.EndDate,
                ContactName = v.ContactID.HasValue ? v.Contact.FullName : string.Empty,
                JobOutsideDM = GetJobOutside(v.JobOutside)
            };
        }

        private static JobOutsideDM GetJobOutside(JobOutside j)
        {
            if (j == null)
                return null;

            return new JobOutsideDM
            {
                Address = j.Address,
                Zone=j.Zone,    
            };
        }

        internal JobDM GetJobDetails(int jobID)
        {
            return CreateJobDm(_jobDal.GetQueryableFresh().SingleOrDefault(x => x.JobID == jobID));
        }

        internal IQueryable<JobTask> GetQuer()
        {
            return _entityDal.GetQueryableFresh();
        }

        internal void GetList(JobTaskFilterDm filter)
        {

            var quer = GetQuer();

            var list = GetListByFilter(quer, filter);

            filter.TableList = CreateList(list);

        }

        private List<JobTaskDM> CreateList(List<JobTask> list)
        {
            return (from x in GetQuer()
                    select new JobTaskDM
                    {
                        JobTaskID = x.JobTaskID,
                        EmpNotes=x.EmpNotes,
                        JobID=x.JobID,
                        JobRefubrishStepID = x.JobRefubrishStepID,
                        ManagerNotes=x.ManagerNotes,
                        TaskName=x.TaskName,
                        
                    })
                    .OrderByDescending(x => x.JobTaskID)
                    .ToList();
        }

        private List<JobTask> GetListByFilter(IQueryable<JobTask> quer, JobTaskFilterDm filter)
        {
            if (filter == null)
                return quer.ToList();

            /** filter by status*/
            Expression<Func<JobTask, bool>> condition = i=> i.JobTaskID>0;

            ///** filter by creator*/
            //if (filter.CreatorID > -1)
            //    condition = condition.AndAlso(i => i.Job.CreatorID == filter.CreatorID);

            ///** filter by Srch*/
            //if (!string.IsNullOrEmpty(filter.Srch))
            //    condition = condition.AndAlso(i => i.SearchStr.Contains(filter.Srch));

            var list = quer.AsEnumerable().Where(condition.Compile())
                .OrderByDescending(x => x.JobTaskID);

            return LinqHelpers.FilterByPage(filter, list);
        }


        /********************         CHANGE       **********************/



        public void Update(JobTaskDM model)
        {
           
            if (model.JobTaskID > 0)
                Edit(model);
            else
                Add(model);

        }

        private void Add(JobTaskDM model)
        {
            JobTask entity = new JobTask();
            
            ModelToEntity(model, entity);

            _entityDal.Add(entity);

            _uow.SaveChanges();

            model.JobTaskID = entity.JobTaskID;
        }

        private void Edit(JobTaskDM model)
        {
            JobTask entity = GetSingle(model.JobTaskID);
            
            ModelToEntity(model, entity);

            _uow.SaveChanges();
        }


        private void ModelToEntity(JobTaskDM model, JobTask entity)
        {        
            Mapper.DynamicMap(model, entity);
        }

        private void EntityToModel(JobTaskDM model, JobTask entity)
        {         
           Mapper.DynamicMap(entity, model);
        }


        internal void Delete(int jobTaskID)
        {
            var entity = GetSingle(jobTaskID);
            _entityDal.Remove(entity);
        }
    }
}
