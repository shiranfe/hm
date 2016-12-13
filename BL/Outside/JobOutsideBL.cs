using BL.Moduls;
using Common;
using DAL;
using Microsoft.Practices.Unity;
using Repository;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Linq.Expressions;
using System.Collections;

namespace BL
{
    public class JobOutsideBL
    {
         private readonly IUnitOfWork _uow;

         private readonly JobOutsideModule _module;
         private readonly MachineModule _machineModule;
        private readonly JobTaskModule _jobTaskModule;
        private readonly JobModule _jobModule;

        public JobOutsideBL([Dependency]IUnitOfWork uow,
           [Dependency]MachineModule machineModule,
           [Dependency]JobModule jobModule,
           [Dependency]JobTaskModule jobTaskModule,
             [Dependency]JobOutsideModule module
         )
        {
            _uow = uow;
            _module = module;
            _machineModule = machineModule;
            _jobTaskModule = jobTaskModule;
            _jobModule = jobModule;
        }

        /***************************************************/


         public void GetItemsList(OutsideFilterDm filter)
        {
            IQueryable<JobOutside> quer = _module.GetQuer();

            List<JobOutside> list = GetListByFilter(quer, filter);

            filter.TableList = CreateList(list);


        }

        public List<PickListDM> GetZones()
        {
            var list= _module.GetQuer().GroupBy(x => x.Zone)
                .Select(x => new PickListDM { Value=x.Key, Text=x.Key }).ToList();

            list.Insert(0, new PickListDM { Value="-1", Text = "- הכל -" });

            return list;
        }

        private List<JobOutsideDM> CreateList(List<JobOutside> list)
        {
            return list.Select(v => new JobOutsideDM
            {
                JobID = v.JobID,
                Address=v.Address,
                Zone = v.Zone,
                QuoteID = v.Job.QuoteID,
                FirstPartID = v.MachineID.HasValue ? v.Job.JobRefubrish.JobRefubrish_Part.First().JobRefubrishPartID : (int?)null,
                JobDM = new JobDM
                {
                   
                    ClientID = v.Job.ClientID,
                    ClientName = v.Job.Client.ClientName,
                    JobName = v.Job.JobName,
                    CreatorID = v.Job.CreatorID,
                    Creator = v.Job.Employee.FullName,
                    Comments = v.Job.Comments,
                    //ReturningJobParentID = v.ReturningJobParentID,
                    ////ClinetNotes = v.Job.JobRefubrish.ClinetNotes,
                    //BranchName = v.Branch.BranceName,
                    //StatusStr = v.RefubrishStatus.Key,
                    StartDate = v.Job.StartDate,
                    EndDate = v.Job.EndDate,
                    DueDate = v.Job.DueDate,
                    //Parts = v.JobRefubrish_Part.Where(x => x.MachinePart.MachineID == v.MachineID).Select(x => new MachinePartDM
                    //{
                    //    id = x.JobRefubrishPartID,
                    //    MachinePartID = x.MachinePartID,
                    //    MachineTypeID = x.MachinePart.MachineTypeID,
                    //    PartName = x.MachinePart.PartName,
                    //    MachineTypeStr = x.MachinePart.MachineType.Key
                    //}).ToList()
                    JobTasks = GetOutsideTasks(v.Job)
                }
              
                //MachineName = v.Machine.MachineName + (v.Machine.Kw != null ? " | " + v.Machine.Kw + "kw" : "") + (v.Machine.Rpm != null ? " | " + v.Machine.Rpm + "rpm" : ""),
                //MachineAddress = v.Machine.Address,
                //MachineID = v.MachineID,
            }).ToList();
        }

        private List<JobTaskDM> GetOutsideTasks(Job job)
        {
            if (job.JobTask == null)
                return new List<JobTaskDM>();


            return job.JobTask.Where(x=>x.JobTaskEmployee.Any()).Select(x => new JobTaskDM
            {
                ManagerNotes = x.ManagerNotes,
                TaskName = x.TaskName,
                JobTaskID = x.JobTaskID,
                TaskTime = x.JobTaskEmployee.LastOrDefault().VisitStart,
                EmployeeStr = x.JobTaskEmployee.Select(e => e.Employee.FirstName).ToArray()
            }).ToList();
        }

        private List<JobOutside> GetListByFilter(IQueryable<JobOutside> quer, OutsideFilterDm filter)
        { 
            if (filter == null)
                return quer.ToList();

            /** filter by status*/
            var condition = ZoneCondtion(filter);

            /** filter by creator*/
            if (filter.CreatorID > -1)
                condition = condition.AndAlso(i => i.Job.CreatorID == filter.CreatorID);

            /** filter by Srch*/
            if (!string.IsNullOrEmpty(filter.Srch))
                condition = condition.AndAlso(i => i.SearchStr.Contains(filter.Srch));

            var list = quer.AsEnumerable().Where(condition.Compile()).OrderByDescending(x => x.Job.StartDate);

            return LinqHelpers.FilterByPage(filter, list);
        }

        private static Expression<Func<JobOutside, bool>> ZoneCondtion(OutsideFilterDm filter)
        {
            /** show all zones*/
            if (filter.Zone=="-1")
                return i => i.JobID>0;

            return i => i.Zone ==filter.Zone;
        }

        public JobDM GetSingleItemDM(int id)
        {
            var  model = _jobModule.GetJobDM(id);
            _jobModule.IntiateNewRefubrishJob(model);
            return model;
        }

        public void Update(JobOutsideDM model)
        {
            _module.Update( model);
            _uow.SaveChanges();
        }

        public int GetClientID(int jobID)
        {
            return _module.GetClientId(jobID);
        }

        public List<JobOutsideDM> GetMachineJobsHistory(int id)
        {
            return _module.GetListByMachine(id);
        }


        public void Delete(int id)
        {
            
            _module.Delete(id);

            _uow.SaveChanges();
        }







       
    }
}
