using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Common;
using DAL;
using Microsoft.Practices.Unity;
using Repository;

namespace BL
{
    public class JobOutsideModule
    {
        private readonly IUnitOfWork _uow;

        private readonly IRepository<JobOutside> _entityDal;


        public JobOutsideModule([Dependency]IUnitOfWork uow, 
            [Dependency]MachinePartModule machinePartModule)
        {
            _uow = uow;
            _entityDal = _uow.Repository<JobOutside>();
        }


        /***************************************************/

        private JobOutside GetSingleFresh(int jobId)
        {
            return _entityDal.GetQueryableFresh().SingleOrDefault(x => x.JobID == jobId);
        }

        private JobOutside GetSingle(int jobId)
        {
            return _entityDal.SingleOrDefault(x => x.JobID == jobId);
        }


        internal JobOutsideDM GetSingleDm(int jobId)
        {
            var entity = GetSingle(jobId);

            var model =  CreateSingleDm(entity);


            if (entity.Machine == null)
                entity = GetSingleFresh(entity.JobID);

           // model.MachineTypeID = entity.Machine.MachineTypeID;
         
         
            return model;

        }

        internal void Update(JobOutsideDM model)
        {
            var entity = GetSingle(model.JobID);

          //  entity.OpenNotes = model.OpenNotes;

        }



        private JobOutsideDM CreateSingleDm(JobOutside entity)
        {
            var model = new JobOutsideDM();

            EntityToModel(model, entity);

            if (entity.Machine == null)
                entity = GetSingleFresh(entity.JobID);

            model.MachineName = entity.Machine.MachineName;
            model.JobDM = CreateJobDm(entity.Job);

            return model;
        }

        internal IQueryable<JobOutside> GetQuer()
        {
            return  _entityDal.GetQueryableFresh();
        }

        internal List<JobOutsideDM> GetListByClient(int clientId)
        {
            var quer = _entityDal.GetQueryableFresh().Where(x => x.Job.ClientID == clientId);

            return CreateList(quer);
        }

        private List<JobOutsideDM> CreateList(IQueryable<JobOutside> quer)
        {
            return (from v in quer.ToList()

                    select new JobOutsideDM
                    { 
                        JobID = v.JobID,
                        Address = v.Address,
                        Zone= v.Zone,
                        JobDM = CreateJobDm(v.Job)
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

        internal List<JobOutsideDM> GetList()
        {

            return (from x in GetQuer()
                    select new JobOutsideDM
                    {
                        JobID = x.JobID


                    })
                    .OrderByDescending(x => x.JobID)
                    .ToList();


        }

        internal int GetClientId(int jobId)
        {
            return GetSingle(jobId).Job.ClientID;
        }

        public List<JobOutsideDM> GetListByMachine(int machineId)
        {
            var quer = _entityDal.GetQueryableFresh().Where(x => x.MachineID == machineId);

            return CreateList(quer);

        }

        /********************         CHANGE       **********************/

        internal void Change(Job job, JobDM jobDm)
        {
            jobDm.JobOutsideDM.JobDM = jobDm;

            if (job.JobOutside != null)
                Edit(job, jobDm);
            else
                Add(job, jobDm);
        }


        private void Add(Job job, JobDM jobDm)
        {
            job.JobOutside = new JobOutside();

            OutsideFromDm(jobDm.JobOutsideDM, job.JobOutside);
        }

        private void Edit(Job job, JobDM jobDm)
        {
            OutsideFromDm(jobDm.JobOutsideDM, job.JobOutside);

        }


        /// <summary>
        /// change jobRefubrish and JobParts
        /// </summary>
        /// <param name="model"></param>
        /// <param name="entity"></param>
        private void OutsideFromDm(JobOutsideDM model, JobOutside entity)
        {
            model.JobID = entity.JobID;

            ModelToEntity(model, entity);
         
        }




        private void ModelToEntity(JobOutsideDM model, JobOutside entity)
        {
            Mapper.Map(model, entity);
        }

        private void EntityToModel(JobOutsideDM model, JobOutside entity)
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
