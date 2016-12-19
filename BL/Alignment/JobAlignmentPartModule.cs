using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Common;
using DAL;
using Microsoft.Practices.Unity;
using Repository;

namespace BL
{
    public class JobAlignmentPartModule
    {
        private readonly IUnitOfWork _uow;
       
        private readonly IRepository<JobAlignmentPart> _entityDal;
      
        public JobAlignmentPartModule([Dependency]IUnitOfWork uow)
        {
            _uow = uow;
      
            _entityDal = _uow.Repository<JobAlignmentPart>();
          
        }


        /***************************************************/

       

        private JobAlignmentPart GetSingle(int jobAlignmentPartID)
        {
            return _entityDal.SingleOrDefault(x => x.JobAlignmentPartID == jobAlignmentPartID);
        }


        internal JobAlignmentPartDM GetSingleDM(int jobAlignmentPartID)
        {
            var entity  =  GetSingle(jobAlignmentPartID);

            return CreatePartDM(entity);

        }

        internal JobAlignmentPartDM CreatePartDM(JobAlignmentPart entity)
        {
            var model = new JobAlignmentPartDM();

            EntityToModel(model, entity);

            model.MachineTypeStr = entity.MachinePart.MachineType.Key;

            model.StartDate = entity.JobAlignment.Job.StartDate.ToShortDateString();

            return model;

        }

        internal List<JobAlignmentPartDM> GetList(int jobID)
        {
           // var quer = GetQuer().Where(x => x.JobID == JobID);
            //Mapper.Map<JobAlignmentPart, JobAlignmentPartDM>(entity, model);

            return (from x in GetQuer()
                    select new JobAlignmentPartDM 
                    {
                        JobAlignmentPartID = x.JobAlignmentPartID
                      
                       
                    })
                    .OrderByDescending(x=>x.JobAlignmentPartID)
                    .ToList();

         
        }

        internal IQueryable<JobAlignmentPart> GetQuer()
        {
            return _entityDal.GetQueryableFresh();
        }


        /********************         CHANGE       **********************/



        public void Update(JobAlignmentPartDM model)
        {
           
            if (model.JobAlignmentPartID > 0)
                Edit(model);
            else
                Add(model);

        }

        private void Add(JobAlignmentPartDM model)
        {
            JobAlignmentPart entity = new JobAlignmentPart();
            
            ModelToEntity(model, entity);

            _entityDal.Add(entity);    
        }

        private void Edit(JobAlignmentPartDM model)
        {
            JobAlignmentPart entity = GetSingle(model.JobAlignmentPartID);
            
            ModelToEntity(model, entity);
        }


        private void ModelToEntity(JobAlignmentPartDM model, JobAlignmentPart entity)
        {        
            Mapper.Map(model, entity);
        }

        private void EntityToModel(JobAlignmentPartDM model, JobAlignmentPart entity)
        {         
           Mapper.Map(entity, model);
        }


        internal void Delete(int jobAlignmentPartID)
        {
            var entity = GetSingle(jobAlignmentPartID);
            _entityDal.Remove(entity);
        }



       
    }
}
