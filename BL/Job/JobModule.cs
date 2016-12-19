using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Common;
using DAL;
using Microsoft.Practices.Unity;
using Repository;

namespace BL.Moduls
{
    public class JobModule
    {
        private readonly IUnitOfWork _uow;
        private readonly IRepository<Job> _jobDal;
        private readonly EmployeeModule _employeeModule;
        private readonly LangModule _langModule;
        private readonly VbModule _vbModule;
        private readonly JobAlignmentModule _alignmentModule;
        private readonly JobOutsideModule _jobOutsideModule;
        private readonly RefubrishModule _refubrishModule;
        private readonly MachineModule _machineModule;

        public JobModule([Dependency]IUnitOfWork uow,

            [Dependency] VbModule vbModule,
            [Dependency]RefubrishModule refubrishModule, 
            [Dependency]EmployeeModule employeeModule,
            [Dependency]JobAlignmentModule alignmentModule,
            [Dependency]JobOutsideModule jobOutsideModule,
            [Dependency]MachineModule machineModule,
           [Dependency]LangModule langModule)
        {
            _uow = uow;
            _jobDal = _uow.Repository<Job>();
            _employeeModule = employeeModule;
            _langModule = langModule;
            _refubrishModule = refubrishModule;
            _machineModule = machineModule;
            _vbModule = vbModule;
            _alignmentModule = alignmentModule;
            _jobOutsideModule = jobOutsideModule;
        }



        /**********************     SELECT      *****************************/

        internal Job GetSingle(int jobID)
        {
            return _jobDal.SingleOrDefault(x => x.JobID == jobID);
        }

        internal Job GetSingleFresh(int jobID)
        {
            return _jobDal.GetQueryableFresh().Where(x => x.JobID == jobID).SingleOrDefault();
        }

        internal JobRequestDM InitiateJobRequest(int empID)
        {
            return new JobRequestDM(true)
            {
                CreatorID = empID,
                StartDate = DateTime.Now,
                IsPosted = true,
                RefubrishDetailsDM = new RefubrishDetailsDM
                {
                    BranchID = GetEmployeeBranch(empID),
                    RefubrishStatusID = GetIntialStatus()
                }
            };
        }

        internal JobDM GetJobDM(int jobID)
        {
            var job = GetSingleFresh(jobID);

            var jobDM= SetJobDM(job);

            if (job.JobVibration != null)
                SetJobVb(job, jobDM);

            if (job.JobRefubrish != null)
                SetJobRefubrish(job, jobDM);

            if (job.JobAlignment != null)
                SetJobAlignment(job, jobDM);

            if (job.JobOutside != null)
                SetJobOutside(job, jobDM);

            return jobDM;
        }

        private void SetJobOutside(Job job, JobDM jobDM)
        {
            var v = job.JobOutside;
            jobDM.JobOutsideDM = new JobOutsideDM();

            Mapper.Map(v, jobDM.RefubrishDetailsDM);

            jobDM.JobOutsideDM.Address = v.Address;
            jobDM.JobOutsideDM.Zone = v.Zone;

            SetJobEquipments(job,jobDM);
            ///** didnt choose macine yet, show form to choose it*/ 
            //if (jobDM.RefubrishDetailsDM == null)
            //{
            //    IntiateNewRefubrishJob( jobDM);
            //}
            //jobDM.JobOutsideDM.MachineName = v.Machine.MachineName;
            //jobDM.JobOutsideDM.Parts = _machineModule.GetJobAndMachinePartsByType(jobDM.JobID, jobDM.RefubrishDetailsDM.MachineID, JobType.Refubrish);

        }

        private static void SetJobEquipments(Job job,JobDM jobDM)
        {
            //Mapper.CreateMap<JobEquipment, JobEquipmentDM>();
            //jobDM.Equipments = Mapper.Map<List<JobEquipmentDM>>(job.JobEquipment);

            jobDM.Equipments = job.JobEquipment.Select(x => new JobEquipmentDM {
                JobEquipmentID=x.JobEquipmentID,
                EquipmentDM = new EquipmentDM
                {
                    EquipmentTitle=x.Equipment.EquipmentTitle,
                    EquipmentID = x.Equipment.EquipmentID,
                    MachineType = x.Equipment.MachineType.Key,
                    MachineTypeName = x.Equipment.MachineType.Additional
                }
            })
                .ToList();

        }

        private JobDM SetJobDM(Job job)
        {
            JobDM jobDM = new JobDM();

            Mapper.Map(job, jobDM);

            jobDM.ClientName = job.Client.ClientName;
            jobDM.CreatorName = job.Employee != null ? job.Employee.FullName : string.Empty;

            return jobDM;
        }


        private void SetJobRefubrish(Job job, JobDM jobDM)
        {
            var v = job.JobRefubrish;
            jobDM.RefubrishDetailsDM = new RefubrishDetailsDM();

            Mapper.Map(v, jobDM.RefubrishDetailsDM);

            jobDM.RefubrishDetailsDM.BranchName = v.Branch.BranceName;
            jobDM.RefubrishDetailsDM.MachineName = v.Machine.MachineName;
            jobDM.RefubrishDetailsDM.Parts = _machineModule.GetJobAndMachinePartsByType(jobDM.JobID, jobDM.RefubrishDetailsDM.MachineID, JobType.Refubrish);
        }

        private void SetJobAlignment(Job job, JobDM jobDM)
        {


            jobDM.JobAlignmentDM = new JobAlignmentDM();
            Mapper.Map(job.JobAlignment, jobDM.JobAlignmentDM);

            jobDM.JobAlignmentDM.Parts = _machineModule.GetJobAndMachinePartsByType(jobDM.JobID, jobDM.JobAlignmentDM.MachineID, JobType.Alignment);
        }


        private void SetJobVb(Job job, JobDM jobDM)
        {
            var v = job.JobVibration;
            jobDM.VbReportDM = new VbReportDM {
               
                AnalyzerName = v.Analayzer.FullName,
                TesterName = v.Tester.FullName,
                InviterName = v.Job.Client.ClientName
            };
        }

        internal int GetJobRefubrishMachineID(int jobID)
        {
            return _refubrishModule.GetMachineID(jobID);
        }


        internal List<JobDM> JobsUnQuoted(int clientID)
        {
            var quer = _jobDal.Where(x => x.ClientID == clientID && !x.QuoteID.HasValue);
            return JobsList(quer);
        }

        private  List<JobDM> JobsList(IQueryable<Job> quer)
        {
            return (from x in quer
                    select new JobDM
                    {
                        ClientName = x.Client.ClientName,
                        JobID = x.JobID,
                        JobName = x.JobName,
                        StartDate = x.StartDate,
                        /** to know job types*/
                        RefubrishDetailsDM = x.JobRefubrish != null ? new RefubrishDetailsDM() : null,
                        VbReportDM = x.JobVibration != null ? new VbReportDM() : null,
                        JobAlignmentDM = x.JobAlignment != null ? new JobAlignmentDM() : null
                    })
                    .OrderByDescending(x=>x.StartDate)
                    .ToList();
        }

        internal List<JobDM> GetQuoteJobs(int quoteID)
        {
            var quer = _jobDal.Where(x => x.QuoteID == quoteID);
            return JobsList(quer);
        }

        #region /**********************     IntiateNewJob      *****************************/
        
        public JobDM IntiateNewJob(int empID, int adminClientID, JobType jobType)
        {
            var jobDM = new JobDM(true)
            {
                ClientID = adminClientID,
                CreatorID = empID,
                StartDate = DateTime.Now,
                IsPosted = true
            };

            switch (jobType)
            {
                case JobType.Refubrish:
                    IntiateNewRefubrishJob(jobDM);
                    break;
                case JobType.Vb:
                    IntiateNewVbJob(jobDM);
                    break;
                case JobType.Alignment:
                    IntiateNewAlignmentJob(jobDM);
                    break;
                case JobType.DynamicBalance:
                    IntiateNewDynamicBalanceJob(jobDM);
                    break;
                case JobType.Outside:
                    IntiateNewOutsideJob(jobDM);
                    break;
            }

            return jobDM;

        }

        private void IntiateNewOutsideJob(JobDM jobDM)
        {
            jobDM.JobOutsideDM = new JobOutsideDM();
        }

        private void IntiateNewDynamicBalanceJob(JobDM jobDM)
        {
            jobDM.VbReportDM = new VbReportDM();
        }


        public void IntiateNewRefubrishJob(JobDM jobDM)
        {
            jobDM.RefubrishDetailsDM = new RefubrishDetailsDM
            {
                BranchID = GetEmployeeBranch((int)jobDM.CreatorID),
                RefubrishStatusID = GetIntialStatus()       
            };

            jobDM.RefubrishDetailsDM.Parts = _machineModule.GetJobAndMachinePartsByType(jobDM.JobID, jobDM.RefubrishDetailsDM.MachineID, JobType.Refubrish);

        }

       

        private int GetEmployeeBranch(int empID)
        {
            return _employeeModule.GetEmployeeBranch(empID);
        }

        private int GetIntialStatus()
        {
            return _langModule.GePickList("JobRefubrishStatus").Single(x => x.Value == "1").PickListID;
        }




        private void IntiateNewAlignmentJob(JobDM jobDM)
        {
            jobDM.JobAlignmentDM = new JobAlignmentDM
            {
                TesterID = (int)jobDM.CreatorID,
                ShowTollerance=true      
            };

            jobDM.JobAlignmentDM.Parts = _machineModule.GetJobAndMachinePartsByType(jobDM.JobID, jobDM.JobAlignmentDM.MachineID, JobType.Alignment);
           
            ///initiate all parts as selected
            jobDM.JobAlignmentDM.Parts.ForEach(x => x.Selected = true);
        }

        private void IntiateNewVbJob(JobDM jobDM)
        {
            throw new NotImplementedException();
        }


         
         
        #endregion

       /**********************     UPDATE      *****************************/

        internal void Change(JobDM model)
        {   
            Job entity = (model.JobID > 0) ?
                EditJob(model) : AddJob(model);
             
             
            if (model.IsRefubrish)
                _refubrishModule.Change(entity, model);

            else if (model.IsVB)
                _vbModule.Change(entity, model);

            else if (model.IsAlignment)
                _alignmentModule.Change(entity, model);

            else if (model.IsOutside)
                _jobOutsideModule.Change(entity, model);

            else
                throw new Exception("no jobe type was found");

            _uow.SaveChanges();

            /** job created*/
            if (model.JobID == 0)
            {
                model.JobID = entity.JobID;
                if (model.IsRefubrish)
                    model.RefubrishDetailsDM.FirstPartID = entity.JobRefubrish.JobRefubrish_Part.First().JobRefubrishPartID;

                else if (model.IsAlignment)
                    model.JobAlignmentDM.FirstPartID = entity.JobAlignment.JobAlignmentPart.First().JobAlignmentPartID;
            }
        }



        private Job EditJob(JobDM jobDM)
        {
            Job job = GetSingle(jobDM.JobID);

            JobFromDM(jobDM, job);

            return job;
        }


        /**********************     ADD      *****************************/


        private Job AddJob(JobDM jobDM)
        {
            Job job = new Job();

            JobFromDM(jobDM, job);

            _jobDal.Add(job);

            return job;

        }

        private  void JobFromDM(JobDM jobDM, Job job)
        {
            Mapper.Map(jobDM, job);
        }






        internal void UpdateJobQuote(int jobID, int? quoteID)
        {
            var job = GetSingle(jobID);
            job.QuoteID = quoteID;

            _jobDal.Update(job);
        }

        /**********************     DELETE      *****************************/

        public void Delete(int jobId)
        {
            /** has store procedure*/
        }

       
    }
}
