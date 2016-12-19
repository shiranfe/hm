using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Common;
using DAL;
using Microsoft.Practices.Unity;
using Repository;

namespace BL
{
    public class JobAlignmentModule
    {
        private readonly IUnitOfWork _uow;

        private readonly IRepository<JobAlignmentPart> _jobAlignmentPartDal;
        private readonly IRepository<JobAlignment> _entityDal;
        private readonly MachinePartModule _machinePartModule;
        private readonly JobAlignmentPartModule _jobAlignmentPartModule;


        public JobAlignmentModule([Dependency]IUnitOfWork uow, 
            [Dependency] JobAlignmentPartModule jobAlignmentPartModule,
            [Dependency]MachinePartModule machinePartModule)
        {
            _uow = uow;
            _jobAlignmentPartDal = _uow.Repository<JobAlignmentPart>();
            _entityDal = _uow.Repository<JobAlignment>();
            _machinePartModule = machinePartModule;
            _jobAlignmentPartModule = jobAlignmentPartModule;
        }


        /***************************************************/

        private JobAlignment GetSingleFresh(int jobId)
        {
            return _entityDal.GetQueryableFresh().SingleOrDefault(x => x.JobID == jobId);
        }

        private JobAlignment GetSingle(int jobId)
        {
            return _entityDal.SingleOrDefault(x => x.JobID == jobId);
        }

        private JobAlignment GetSingleByPart(int jobAlignmentPartId)
        {
            var entity = _jobAlignmentPartDal
                .Where(x => x.JobAlignmentPartID == jobAlignmentPartId)
                .Select(x => x.JobAlignment)
                .SingleOrDefault()
                ;
            return entity;
        }

        internal JobAlignmentDM GetSingleDm(int jobId)
        {
            var entity = GetSingle(jobId);

            var model =  CreateSingleDm(entity);


            if (entity.Machine == null)
                entity = GetSingleFresh(entity.JobID);

            model.MachineTypeID = entity.Machine.MachineTypeID;
            model.JobParts = new List<JobAlignmentPartDM>();

            foreach(var part in entity.JobAlignmentPart){
            
                model.JobParts.Add(_jobAlignmentPartModule.CreatePartDM(part));
            }
         
            return model;

        }

        internal void Update(JobAlignmentDM model)
        {
            var entity = GetSingle(model.JobID);

            entity.OpenNotes = model.OpenNotes;

        }

        internal JobAlignmentDM GetSingleDmByPart(int jobAlignmentPartId)
        {
            var entity = GetSingleByPart(jobAlignmentPartId);

            return CreateSingleDm(entity);
        }


        private JobAlignmentDM CreateSingleDm(JobAlignment entity)
        {
            var model = new JobAlignmentDM();

            EntityToModel(model, entity);

            if (entity.Machine == null)
                entity = GetSingleFresh(entity.JobID);

            model.MachineName = entity.Machine.MachineName;
            
            model.JobDM = CreateJobDm(entity.Job);

            return model;
        }

        internal List<JobAlignmentDM> GetListAll()
        {
            var quer = _entityDal.GetQueryableFresh();
            return CreateList(quer);
        }

        internal List<JobAlignmentDM> GetListByClient(int clientId)
        {
            var quer = _entityDal.GetQueryableFresh().Where(x => x.Job.ClientID == clientId);

            return CreateList(quer);
        }

        private List<JobAlignmentDM> CreateList(IQueryable<JobAlignment> quer)
        {
            return (from v in quer.ToList()

                    select new JobAlignmentDM
                    { 
                        JobID = v.JobID,
                        JobDM = CreateJobDm(v.Job),
                        FirstPartID = v.JobAlignmentPart.First().JobAlignmentPartID
                    }).OrderByDescending(x => x.JobID).ToList();
        }

        private JobDM CreateJobDm(Job v)
        {
            return new JobDM
            {
                ClientID = v.ClientID,
                CreatorID= v.CreatorID,
                Creator = v.Employee.FullName,
                ClientName = v.Client.ClientName,
                CreatorName = v.Employee.FullName,
                JobName = v.JobName,
                StartDate = v.StartDate,
                EndDate = v.EndDate,
                ContactName = v.ContactID.HasValue ? v.Contact.FullName : string.Empty
            };
        }

        internal List<JobAlignmentDM> GetList()
        {

            return (from x in GetQuer()
                    select new JobAlignmentDM
                    {
                        JobID = x.JobID


                    })
                    .OrderByDescending(x => x.JobID)
                    .ToList();


        }

        internal IQueryable<JobAlignment> GetQuer()
        {
            return _entityDal.GetQueryableFresh();
        }

        internal int GetClientId(int jobId)
        {
            return GetSingle(jobId).Job.ClientID;
        }

        public List<JobAlignmentDM> GetListByMachine(int machineId)
        {
            var quer = _entityDal.GetQueryableFresh().Where(x => x.MachineID == machineId);

            return CreateList(quer);

        }

        /********************         CHANGE       **********************/

        internal void Change(Job job, JobDM jobDm)
        {
            jobDm.JobAlignmentDM.JobDM = jobDm;

            if (job.JobAlignment != null)
                Edit(job, jobDm);
            else
                Add(job, jobDm);
        }


        private void Add(Job job, JobDM jobDm)
        {
            job.JobAlignment = new JobAlignment();

            AlignmentFromDm(jobDm.JobAlignmentDM, job.JobAlignment);
        }

        private void Edit(Job job, JobDM jobDm)
        {
            AlignmentFromDm(jobDm.JobAlignmentDM, job.JobAlignment);

        }


        /// <summary>
        /// change jobRefubrish and JobParts
        /// </summary>
        /// <param name="model"></param>
        /// <param name="entity"></param>
        private void AlignmentFromDm(JobAlignmentDM model, JobAlignment entity)
        {
            model.JobID = entity.JobID;

            ModelToEntity(model, entity);
         
            var partsSelected = model.MachinePartID;
             

            //RemovePartsFromJob(entity, partsSelected);
            if (partsSelected!=null)
                AddPartsToJob(entity, partsSelected, (int)model.JobDM.CreatorID);
        }




        private void AddPartsToJob(JobAlignment entity, int[] partsSelected, int creatorId)
        {
            var macParts = _machinePartModule.GetList(entity.MachineID);

            foreach (var partId in partsSelected)
            {
                var partInJob = entity.JobAlignmentPart.SingleOrDefault(x => x.MachinePartID == partId);

                if (partInJob == null)
                {

                    var macPart = macParts.Single(x => x.MachinePartID == partId);

                    //eneinge is always part of the test, no need to add it
                    if (macPart.MachineTypeID != (int)MachineType.EngineAC &&
                        macPart.MachineTypeID != (int)MachineType.EngineDC)
                    {
                        var jobPart = new JobAlignmentPart
                        {
                            MachinePartID = partId
                            //JobID = entity.JobID,
                        };

                        entity.JobAlignmentPart.Add(jobPart);
                    }


                }
            } 

            if (!entity.JobAlignmentPart.Any())
            {
                throw new Exception("Alignment must have atleast one part that is not Engeine");
            }
        }



        private void ModelToEntity(JobAlignmentDM model, JobAlignment entity)
        {
            Mapper.Map(model, entity);
        }

        private void EntityToModel(JobAlignmentDM model, JobAlignment entity)
        {
            Mapper.Map(entity, model);
        }


        internal void Delete(int jobId)
        {
            var entity = GetSingle(jobId);
            _entityDal.Remove(entity);
        }


      



    }
}
