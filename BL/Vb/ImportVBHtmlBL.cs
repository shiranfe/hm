using System;
using System.Collections.Generic;
using System.Linq;
using BL.Moduls;
using Common;
using DAL;
using Microsoft.Practices.Unity;
using Repository;

namespace BL
{
    public class ImportVbHtmlBL
    {
        private readonly IUnitOfWork _uow;
        private readonly ImportVbModule _importVbModule;
        private readonly LangModule _langModule;
        private VbModule _vbModule;
        private readonly VbmachineModule _vbmachineModule;
        private readonly MachineModule _machineModule;

        //public ImportVBHtmlBL()
        //    : this(new ImportVBModule(), new LangModule(), new VbModule(), new VbmachineModule(), new MachineModule())
        //{
            
        //}

        public ImportVbHtmlBL([Dependency]IUnitOfWork uow, 
            [Dependency]ImportVbModule importVbModule, 
            [Dependency]LangModule langModule, 
            [Dependency]VbModule vbModule, 
            [Dependency]VbmachineModule vbmachineModule, 
            [Dependency]MachineModule machineModule)
        {
            _uow = uow;
            _langModule = langModule;
            _vbModule = vbModule;
            _vbmachineModule = vbmachineModule;
            _importVbModule = importVbModule;
            _machineModule = machineModule;
        }

        /***************************************************/

        public List<MachineBasicDM> GetVbNewMachine()
        {
            return _importVbModule.GetVbNewMachine();
        }


        public List<VbHtmlReportChooseJobDM> GetVbHtmlReportChooseJobDM()
        {
            _importVbModule.RunspJobVibrationNewJob();
            var dates = _importVbModule.GetJobVibrationNewJob();

            return dates;
        }

        public void AddJobVibrationHtml(List<VbHtmlReportDM> vbHtmlReportDMList)
        {
            _importVbModule.DelteAll_TempTables();
         

            List<C_JobVibrationHtml> listToAdd = vbHtmlReportDMList
                .Select(x => new C_JobVibrationHtml
                {
                    Date = Convert.ToDateTime(x.Date),
                    MachineName = x.MachineName,
                    PointNumber = x.Location.Split('-')[0],
                    Direction = x.Location.Split('-').Last(),
                    ScheduleEntry = x.ScheduleEntry.Split().First(),//leave only Demoed/Vel
                    Value = Convert.ToDouble(x.Latest)

                }).ToList();

            _importVbModule.InsertJobVibrationHtml(listToAdd);
           
            _uow.SaveChanges();
           
            _importVbModule.InsertJobVibrationHtmlMachine();

            _uow.SaveChanges();
        }

        public int CommitImportHtmlReport(int clientID, int? jobID)
        {

            if (!jobID.HasValue)
            {
                string startDate =_importVbModule.GetNewJobDate();
                _importVbModule.CreateNewJobByClient(clientID, startDate);
                var job = _uow.Repository<Job>();
                jobID = job.GetQueryable().Max(x => x.JobID);
            }

           _importVbModule.Update_JobVibrationNewJob(jobID);
           _uow.SaveChanges();

           _importVbModule.CommitImportHtmlReport(clientID);
           _uow.SaveChanges();

            InsertVbHtmlPointResualt((int)jobID, clientID);

            _uow.SaveChanges();

            return (int)jobID;
        }

        public void InsertVbHtmlPointResualt(int jobID, int clientID)
        {

            List<JobVibrationMachinePointResult> rslts = new List<JobVibrationMachinePointResult>();

            var htmlReasult = _importVbModule.Get_JobVibrationHtml();
            List<PickList> picks = _langModule.GePickList();

            var vwMachinePoint = _machineModule.GetvwMachinePoint(clientID);
            var pointResLastStatus = _importVbModule.GetJobVibrationMachinePointLastStatus(clientID);

          
            foreach (var rsl in htmlReasult)
            { 
                var points = vwMachinePoint.Where(x =>
                     String.Equals(x.MachineName.Trim(), rsl.MachineName.Trim(), StringComparison.OrdinalIgnoreCase) 
                     && x.PointNumber == rsl.PointNumber);

                if (points.Count() != 1)
                    throw new Exception("Machine must have only 1 point per number. MachineName:" + rsl.MachineName.Trim() + ", PointNumber:" + rsl.PointNumber);

                int machinePointID = points.Single().MachinePointID;
                var newPointResualt = new JobVibrationMachinePointResult
                {
                    JobID = jobID,
                    MachinePointID = machinePointID,
                    Date = rsl.Date,

                    Direction = picks.Single(x => x.Key == rsl.Direction).PickListID,
                    Value = rsl.Value
                    
                };
                var m = picks.Single(x => x.Key == rsl.ScheduleEntry);
                newPointResualt.ScheduleEntry = m.PickListID;//m.vwVelDemod.First().VelDemond;

                int? statusID = GetPointLastStauts(newPointResualt, pointResLastStatus);

                newPointResualt.StatusID = statusID.HasValue ?
                    statusID :
                    GetPointDefualtStatus(picks);

                rslts.Add(newPointResualt);
            }

            HideDuplicateResualts(rslts);

            _vbmachineModule.AddMulty(rslts);

            _uow.SaveChanges();
        }

        private void HideDuplicateResualts(List<JobVibrationMachinePointResult> rslts)
        {
            rslts.ForEach(x => x.Hide = true);
            var groupBy = rslts.GroupBy(x => new { x.MachinePointID, x.Direction, x.ScheduleEntry });
            var maxResutls = groupBy.SelectMany(a => a.Where(b => b.Value == a.Max(c => c.Value))).ToList();

            maxResutls.ForEach(x => x.Hide = false);

            _uow.SaveChanges();
        }

        private int GetPointDefualtStatus(List<PickList> picks)
        {
            return Convert.ToInt32(
                picks
                .Single(x => x.Entity == "Setting" && x.Key == "JobStatus_Default")
                .Value);
            //ISNULL(dbo.vwJobVibrationMachinePointLastStatus.StatusID,
            //(SELECT Value FROM dbo.PickList AS PickList_3 WHERE (Entity = 'Setting')
            //  AND ([Key] = 'JobStatus_Default')))
        }

        private int? GetPointLastStauts(JobVibrationMachinePointResult newPointResualt, List<vwJobVibrationMachinePointLastStatus> pointResLastStatus)
        {

            var  pointWithLastSts= pointResLastStatus
                .FirstOrDefault(m =>
                    m.MachinePointID == newPointResualt.MachinePointID &&
                    m.Direction == newPointResualt.Direction &&
                    m.VelDemond == newPointResualt.ScheduleEntry
                    );

        
            return (pointWithLastSts != null) ?
                  pointWithLastSts.StatusID :
                  (int?)null;
        }



        //private List<vwMachinePointDm> GetPointAndlastStats(int ClientID, List<C_JobVibrationHtml> htmlReasualt)
        //{
        //    var vwMachinePoint =_importVBModule.GetvwMachinePoint(ClientID);

        //    int DefualtStatus = GetPointDefualtStatus();

        //    foreach (var point in vwMachinePoint)
        //    {

        //        PointLastStatusDm PointWithLastSts =PointResLastStatus
        //            .SingleOrDefault(m => m.MachinePointID == point.MachinePointID);

        //        point.StatusID = (PointWithLastSts == null) ? 
        //            PointWithLastSts.StatusID : 
        //            DefualtStatus;

        //    }

        //    return vwMachinePoint;
        //}




        //private int GetMachinePointID(C_JobVibrationHtml rsl, List<vwMachinePointDm> vwMachinePoint)
        //{
        //    return vwMachinePoint
        //        .Single(x => x.MachineName == rsl.MachineName && x.PointNumber == rsl.PointNumber)
        //        .MachinePointID;
        //}

        public void ToggleVbHtmlIncludes(int id, bool include, string entity)
        {
            switch (entity)
            {
                case "Date":
                   _importVbModule.ToggleVbHtmlIncludesDate(id, include);
                    break;
                case "Machine":
                   _importVbModule.ToggleVbHtmlIncludesMachine(id, include);
                    break;
                default:
                    break;
            }
            _uow.SaveChanges();
        }

        
    }
}
