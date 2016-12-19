using System.Collections.Generic;
using System.Linq;
using Common;
using DAL;
using Microsoft.Practices.Unity;
using Repository;

namespace BL.Moduls
{
    public class ImportVbModule
    {
        private readonly IUnitOfWork _uow;
        private readonly IRepository<C_JobVibrationHtml> _cJobVibrationHtml;
        private readonly IRepository<C_JobVibrationNewJob> _cJobVibrationNewJob;
        private readonly IRepository<C_JobVibrationNewMachine> _cJobVibrationNewMachine;
      

        //public ImportVBModule()
        //    : this(_uow.Repository<C_JobVibrationHtml>(), 
        //    _uow.Repository<C_JobVibrationNewJob>(),
        //    _uow.Repository<C_JobVibrationNewMachine>())
        //{

        //}

        public ImportVbModule([Dependency]IUnitOfWork uow)
        {
            _uow = uow;
            _cJobVibrationHtml = _uow.Repository<C_JobVibrationHtml>();
            _cJobVibrationNewJob = _uow.Repository<C_JobVibrationNewJob>();
            _cJobVibrationNewMachine = _uow.Repository<C_JobVibrationNewMachine>();
        }



        /***************************************************/


        


        public void InsertJobVibrationHtml(List<C_JobVibrationHtml> listToAdd)
        {
            foreach (var x in listToAdd)
            {
                _cJobVibrationHtml.Add(x);
            }
            
        }

        public void InsertJobVibrationHtmlMachine()
        {
           
            StoreProce.spJobVibrationNewMachine();
        }

        public List<MachineBasicDM> GetVbNewMachine()
        {
            return (from m in _cJobVibrationNewMachine.GetQueryable()
                    select new MachineBasicDM
                    {
                        MachineID = m.ID,
                        MachineName = m.MachineName
                    }).ToList();
        }

        public void RunspJobVibrationNewJob()
        {
            StoreProce.spJobVibrationNewJob();
        }

        public List<VbHtmlReportChooseJobDM> GetJobVibrationNewJob()
        {
            return (from m in _cJobVibrationNewJob.GetQueryable()
                    select new VbHtmlReportChooseJobDM
                    {
                        JobID = m.JobID,
                        JobDate = m.Date,
                        ID = m.ID
                    }).ToList();
        }

    

        public void ToggleVbHtmlIncludesDate(int id, bool include)
        {
            C_JobVibrationNewJob job = _cJobVibrationNewJob
                .SingleOrDefault(x => x.ID == id);
            job.IsInclude = include;
        }

        public void ToggleVbHtmlIncludesMachine(int id, bool include)
        {
            C_JobVibrationNewMachine mac = _cJobVibrationNewMachine
               .SingleOrDefault(x => x.ID == id);
            mac.IsInclude = include;
        }

        public void CreateNewJobByClient(int clientID, string startDate)
        {
            StoreProce.spJobVibrationNewJobInsert(clientID, startDate);
            
        }

        public void CommitImportHtmlReport(int clientID)
        {
            StoreProce.spJobVibrationNew(clientID);
        }

        public List<C_JobVibrationHtml> Get_JobVibrationHtml()
        {
            return _cJobVibrationHtml.ToList();
        }

      

        public List<vwJobVibrationMachinePointLastStatus> GetJobVibrationMachinePointLastStatus(int clientID)
        {
            var quer = _uow.Repository<vwJobVibrationMachinePointLastStatus>();
            return (from x in quer.GetQueryable()
                    where x.ClientID == clientID
                    select x).ToList();
        }

       

        public void Update_JobVibrationNewJob(int? jobID)
        {
            var dates = _cJobVibrationNewJob.ToList();
            dates.ForEach(x => x.JobID = jobID);
        }



        public string GetNewJobDate()
        {
            return _cJobVibrationNewJob
                .Where(x => x.IsInclude)
                .First().Date.ToString("yyMMdd");
        }


        public void DelteAll_TempTables()
        {
            StoreProce.spDeleteTempTables();
        }
    }
}
