using BL.Moduls;
using Common;
using DAL;
using Microsoft.Practices.Unity;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BL
{
    public class JobBL
    {
        private readonly IUnitOfWork _uow;
        public static IRepository<Job> JobDal;
        private readonly LangModule _langModuleDal;
        private readonly JobModule _jobModule;
        private readonly ClientMove _clientMove;
        /* 
           public JobBL()
               : this(new LangModule(), _uow.Repository<Job>())
           {

           }*/

        public JobBL([Dependency]IUnitOfWork uow,
            [Dependency]LangModule langModuleDal,
            [Dependency]ClientMove clientMove,
            [Dependency]JobModule jobModule)
        {
            _uow = uow; 
            _langModuleDal = langModuleDal;
            JobDal = _uow.Repository<Job>();
            _jobModule = jobModule;
            _clientMove = clientMove;
        }

        /***************************************************/


        public List<KeyValueDM> GetUrgency()
        {

            return _langModuleDal.GePickListDM("Urgency");
        }

        public dynamic GetTesters()
        {
            throw new NotImplementedException();
        }

        public  int GetClientID(int jobID)
        {
            return JobDal.Where(x => x.JobID == jobID)
                .Select(x => x.ClientID).Single() ;
        }



        public JobDM IntiateNewJob(int empID, int adminClientID, JobType jobType)
        {

            return _jobModule.IntiateNewJob(empID, adminClientID, jobType);

        }

        public JobRequestDM InitiateJobRequest(int empID)
        {
            return _jobModule.InitiateJobRequest(empID);
           
        }

        public JobDM GetJobDM(int jobID)
        {
            return _jobModule.GetJobDM(jobID);
        }


        public List<JobDM> JobsUnQuoted(int clientID)
        {
            return _jobModule.JobsUnQuoted(clientID);
        }

        public List<JobDM> GetQuoteJobs(int quoteID)
        {
            return _jobModule.GetQuoteJobs(quoteID);
        }

        /**********************/

        public void Change(JobDM jobDM)
        {
            _jobModule.Change(jobDM);    
           
        }

        public void UpdateJobQuote(int jobID, int? quoteID)
        {
            if (quoteID == 0)
                throw new Exception("clientid is not valid");

            _jobModule.UpdateJobQuote(jobID, quoteID);
            _uow.SaveChanges();

        }

        public void ChangeJobClient(int jobID, int clientID)
        {
            _clientMove.ChangeJobClient( jobID,  clientID);
            
        }

       
    }
}
