using Common;
using System.Collections.Generic;
using DAL;
using System.Linq;
using Repository;
using System;
using BL.Moduls;
using Microsoft.Practices.Unity;

namespace BL
{
    public class VbBL
    {
        private readonly IUnitOfWork _uow;
        private readonly IRepository<Job> _jobDal;

        private readonly VbModule _vbModule;
        private readonly LangModule _langModule;
        private readonly VbCache _vBRepository;
        private readonly VbmachineModule _vbMachineModule;
        //public VbBL()
        //    : this(new VbModule(), new LangModule(), new VBCache(), _uow.Repository<Job>(), new VbmachineModule())
        //{

        //}

        public VbBL([Dependency]IUnitOfWork uow,
            [Dependency] VbModule vbModule,
            [Dependency] LangModule langModule,
            [Dependency]VbCache vBRepository,
            [Dependency]VbmachineModule vbMachineModule)
        {
            _uow = uow;
            _jobDal = _uow.Repository<Job>();
            _vbModule = vbModule;
            _langModule = langModule;
            _vBRepository = vBRepository;
            _vbMachineModule = vbMachineModule;
        }

        /***************************************************/


        public int GetPageCount(int clientID)
        {
            return _vBRepository.GetPageCount(clientID);
        }


        public List<VbReportDM> GetClientHistory(int clientID)
        {
            return _vbModule.GetMultiVbReports(clientID)
                 .OrderByDescending(x => x.JobDM.StartDate).ToList();
        }


        public void GetClientReports(VbFilterDm filter)
        {
            var quer = filter.ClientID > 0 ?
                 _vbModule.GetMultiVbReports(filter.ClientID): _vbModule.GetAllVbReports();

            filter.TableList = FilterIndex(filter, quer.AsEnumerable());

           
        }

        private List<VbReportDM> FilterIndex(VbFilterDm filter, IEnumerable<VbReportDM> quer)
        {

            /** filter by Srch*/
            if (!string.IsNullOrEmpty(filter.Srch))
                quer = quer.Where(i => i.SearchStr.Contains(filter.Srch));

            /** filter by IsPosted*/
            if (filter.IsPosted.HasValue)
                quer = quer.Where(i => i.JobDM.IsPosted==filter.IsPosted);

            var list =quer.OrderByDescending(x => x.JobDM.StartDate).ToList();

            return LinqHelpers.FilterByPage(filter, list);

        }

        public List<VbCurentMachineStsDM> GetCurrentMachineSts(int clientID, FilterSortPageDM filterSortDM)
        {
            var m = _vBRepository.GetCurrentMachineSts(clientID);


            if (!string.IsNullOrEmpty(filterSortDM.Filter))
            {
                string flt = filterSortDM.Filter.ToUpper();
                m = m.Where(x =>
                    x.MachineName.ToUpper().Contains(flt)
                    || x.ClientName.ToUpper().Contains(flt)
                    ).ToList();
            }
            switch (filterSortDM.Sort)
            {
                case "Job_TestDate":
                    m = m.OrderByDescending(s => s.LastDate).ToList();
                    break;
                case "User_Locations":
                    m = m.OrderBy(s => s.ClientName).ToList();
                    break;
                case "Global_ReportStatus":
                    m = m.OrderBy(s => s.StatusID).ToList();
                    break;
                default:
                    m = m.OrderBy(s => s.MachineName).ToList();
                    break;
            }
            return m;
        }

    

        //public List<VbCurentMachineStsDM> GetCurrentMachineSts(string MachineName)
        //{
        //    return _vbModule.GetCurrentMachineSts(MachineName);
        //}

        public VbReportDM GetVbReport(int jobID, bool isEnglish)
        {

            var rprt = _vbModule.GetVbReport(jobID, isEnglish);
            if (rprt != null)
                rprt.VbReportMachineDMs = _vbModule.GetMultiReportMachine(jobID, isEnglish);
            return rprt;

        }

        public VbReportEditDM GetVbReportEdit(int jobID)
        {

            var rprt = _vbModule.GetVbReportEdit(jobID);
            if (rprt != null)
            {
                rprt.VbEditMachineDMs = _vbMachineModule.GetMultyVbEditMachineDMs(jobID);
                int selMacID = rprt.VbEditMachineDMs.First().MachineID;
                rprt.SelectedVbEditMachine = GetSelectedVbEditMachine(selMacID, jobID);

                var quer = _vbModule.GetJobTemplateNotesQuer();
                rprt.JobTemplateNotesEN = quer.Select(x => x.EN).Where(x => !string.IsNullOrEmpty(x)).ToArray();
                rprt.JobTemplateNotesIL = quer.Select(x => x.IL).Where(x => !string.IsNullOrEmpty(x)).ToArray();
            }
            return rprt;

        }

        public VbMachineDM GetSelectedVbEditMachine(int machineID, int jobID)
        {
            var mac = _vbMachineModule.GetVbMachineDM(machineID, jobID);

            mac.NotesHisory = _vbMachineModule.GetNotesHisory(machineID, jobID);
            mac.VbPointResultDMs = _vbMachineModule.VbPointResultDMs(machineID, jobID);
            mac.VbStatusDMs = _langModule.GetVbStatusDM();
            return mac;
        }


        public List<ClientTreeDM> GetImportVbHtmlReportClients()
        {
            //TryFindMacInDb
            var quer = GetvwClientMachineCountHtmlImport();
            var clnts = (from m in quer
                         select new ClientTreeDM
                         {
                             ClientID = m.ClientID,
                             ClientName = m.vwParentsName2,
                         }).ToList();

            return clnts;
        }

        private List<vwClientMachineCountHtmlImport> GetvwClientMachineCountHtmlImport()
        {
            var sp = _uow.Repository<vwClientMachineCountHtmlImport>();
            return sp.GetQueryable().OrderByDescending(x => x.MachineCount)
                .ThenBy(x=>x.vwParentsName2)
                .ToList();
        }


        public List<JobDatesDM> GetOpenJobs()
        {
            var quer = _vbModule.GetVbJobQuer().ToList();
            return (from j in quer
                    where !j.Job.IsPosted
                    select new JobDatesDM
                    {
                        JobID = j.JobID,
                        JobName = (j.Job.StartDate).ToString("dd/MM/yy") + " - " + j.Job.Client.ClientName,
                        StartDate =j.Job.StartDate
                    }).OrderByDescending(x => x.StartDate).ToList();
        }

        private Job GetJobVibration(int jobID)
        {
            return _jobDal.SingleOrDefault(x => x.JobID == jobID);
        }

        public List<LangStringDM> GetJobTemplateNotes()
        {
            var quer = _vbModule.GetJobTemplateNotesQuer().ToList();
            var ans = (from q in quer
                       select new LangStringDM
                       {
                           EN = q.EN,
                           IL = q.IL,
                           Key = q.ID.ToString()
                       }).ToList();

            return ans;
        }


        public SpectrumDM GetPointResualt(int jobID)
        {
            var ans = new SpectrumDM
            {
                PointResualts = _vbMachineModule.GetPointResualt(jobID),
            };

            if (ans.PointResualts.Any())
            {
                ans.SelectedPointResualt = ans.PointResualts.First();
            }
            else
            {
                ans.SelectedPointResualt = new AdminPointResualtDM();
            }

            return ans;
        }

        public AdminPointResualtDM GetSelectedPointResualt(int pointResualtID)
        {
            var ans = _vbMachineModule.GetSinglePointResualt(pointResualtID);
            return ans ?? new AdminPointResualtDM();
        }




        public List<NotesHisory> GetMachineJobsHistory(int machineID)
        {

            return _vbMachineModule.GetNotesHisory(machineID, 0);
            //SelectedMachine sel = _machineModule.GetSelectedMachine(MachineID);
            //sel.PointDMs = _machineModule.GetMachinePoints(MachineID);
          
            //return new MachineVbDetailsDM
            //{
               
            //    History =  _vbMachineModule.GetNotesHisory(MachineID, 0),
            //    Machine = sel
            //};



        }


        /**********     ADD    ***********/
        public void ImportVbHtmlReport(List<VbHtmlReportDM> vbHtmlReportDMList)
        {
            throw new System.NotImplementedException();
        }




        /**********     CHANGE    ***********/

        public void UpdateHiddenResualt(int pointResultID, bool hidden)
        {
            _vbMachineModule.UpdateHiddenResualt(pointResultID, hidden);
            _uow.SaveChanges();
        }

        public void UpdateResualtStatus(int pointResultID, int statusID)
        {
            int pickListID = _langModule.GetPickListIDForVbStatus(statusID.ToString());
            _vbMachineModule.UpdateResualtStatus(pointResultID, pickListID);
            _uow.SaveChanges();
        }

        public void UpdateMultyResualtStatus(int machineID, int jobID, int statusID)
        {
            int pickListID = _langModule.GetPickListIDForVbStatus(statusID.ToString());
            _vbMachineModule.UpdateePointResultByMacAndJob(machineID, jobID, pickListID);
            _uow.SaveChanges();

        }

        public void UpdateVbGeneralNote(VBNotes vbNotes)
        {

            _vbMachineModule.UpdateVbGeneralNote(vbNotes);
            _uow.SaveChanges();
        }

        public void VbUpdateIsPosted(int jobID, bool isPosted)
        {
            var job = GetJobVibration(jobID);
            job.IsPosted = isPosted;
            _uow.SaveChanges();

        }

        public void UpdateJobTemplateNote(int id, string lang, string word)
        {
            var quer = _vbModule.GetJobTemplateNotesQuer();

            var rw = quer.Single(x => x.ID == id);
            switch (lang)
            {
                case "il":
                    rw.IL = word;
                    break;
                case "en":
                    rw.EN = word;
                    break;
                default:
                    break;
            }


            _uow.SaveChanges();
        }


        /**********     DELETE      ***********/

        public void DeleteJobVb(int jobID)
        {
            StoreProce.spJobVibrationDelete(jobID);
        }


        
    }
}
