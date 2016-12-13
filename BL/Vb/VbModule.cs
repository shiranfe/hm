using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;
using Common;
using Repository;
using Microsoft.Practices.Unity;

namespace BL.Moduls
{
    public class VbModule
    {
        private readonly IUnitOfWork _uow;
       // private IRepository<VbModule> _vbModuleDal;
        private readonly IRepository<vwJobVibration> _vwJobVibration;
        private readonly IRepository<JobVibration> _jobVibration;
        private readonly IRepository<vwJobVibrationIndex> _vwJobVibrationIndex;
        private readonly IRepository<vwJobVibrationReport> _vwJobVibrationReport;
        private readonly IRepository<JobTemplateNotes> _jobTemplateNotes;

        public VbModule([Dependency]IUnitOfWork uow)
        {
            _uow = uow;
           // _vbModuleDal = _uow.Repository<VbModule>();
            _vwJobVibration =_uow.Repository<vwJobVibration>() ;
            _vwJobVibrationIndex = _uow.Repository<vwJobVibrationIndex>();
            _vwJobVibrationReport = _uow.Repository<vwJobVibrationReport>();
            _jobVibration = _uow.Repository<JobVibration>();
            _jobTemplateNotes = _uow.Repository<JobTemplateNotes>();
        }



        /***************************************************/


        public VbReportDM GetVbReport(int jobID,bool isEnglish)
        {
           
       
            return (from v in _vwJobVibration.GetQueryable()
                    where v.JobID == jobID
                    select new VbReportDM
                    {
                        JobDM = new JobDM
                        {
                            JobID = v.JobID,
                            ClientID = v.ClientID,
                            ClientName = isEnglish ? (v.ClientFullNameEnglish ?? v.vwParentsName2) : v.vwParentsName2,
                            StartDate = (DateTime)v.StartDate,
                        },                     
                        Areas = v.vwParentsName2,                    
                        AnalyzerName = v.AnalyzerName,
                        TesterName = v.TesterName,
                        //Urgency = undefined,
                        InviterName = v.ClientName,

                    }).SingleOrDefault();

        }

        public VbReportEditDM GetVbReportEdit(int jobID)
        {

            string undefined = "Undefined";//GlobalDM.GetTransStr("Undefined");
            return (from v in _vwJobVibration.GetQueryable()
                    where v.JobID == jobID
                    select new VbReportEditDM
                    {
                        JobDM = new JobDM
                        {
                            JobID = v.JobID,
                            ClientID = v.ClientID,
                            ClientName = v.vwParentsName2 ?? undefined,
                            StartDate = (DateTime)v.StartDate,
                            JobName = v.JobName,
                            IsPosted = v.IsPosted,
                            //Urgency = undefined,
                        },   
                       
                        Areas = v.vwParentsName2,
                        AnalyzerName = v.AnalyzerName,
                        TesterName = v.TesterName,                      
                        InviterName = v.ClientName,
                       
                    }).SingleOrDefault();

        }

        public IQueryable<VbReportDM> GetMultiVbReports(int clientID)
        {
            var ans = (from v in _vwJobVibrationIndex.GetQueryable()
                       where v.MainClientID == clientID
                       select new VbReportDM
                       {
                           JobDM = new JobDM
                           {
                               JobID = v.JobID,
                               JobName = v.JobName,
                               StartDate = (DateTime)v.StartDate,
                               IsPosted = v.IsPosted,
                               MainClientID = v.MainClientID,
                               ClientName = v.ClientName
                           },   
                        
                           Areas = v.vwParentsName2,
                         
                       });
            return ans;

        }

        public IQueryable<VbReportDM> GetAllVbReports()
        {

        //    DateTime lstyear = DateTime.Now.AddMonths(-3);
            return (from v in _vwJobVibration.GetQueryable()
                      // where v.StartDate > lstyear
                       select new VbReportDM
                       {
                           JobDM = new JobDM
                           {
                               JobID = v.JobID,
                               JobName = v.JobName,
                               StartDate = (DateTime)v.StartDate,
                               IsPosted = v.IsPosted,
                               MainClientID = v.ClientID,
                               ClientName = v.ClientName
                           },                            
                           Areas = v.vwParentsName2,                        
                       });


        }
       

        public List<VbReportMachineDM> GetMultiReportMachine(int jobID, bool isEnglish)
        {
            try
            {
                var quer = (from v in _vwJobVibrationReport.GetQueryableFresh()
                            where v.JobID == jobID && v.StatusValue != null && v.StatusValue != 6
                            select v).AsEnumerable();
                var vs = (from v in quer
                          select new VbReportMachineDM
                          {
                              MachineID = v.MachineID,
                              MachineName = v.MachineName,
                              MaxValue = v.MaxValue,
                             // Areas = v.ClientName,
                               Areas = isEnglish ? (v.ClientNameEnglish ?? v.ClientName) : v.ClientName,
                              StatusID = v.StatusValue.ToString(),
                              LangStr = v.StatusLangStr,
                              NotesEN = v.GeneralNoteEN,
                              NotesIL = v.GeneralNoteIL,
                              Details = v.Details,
                          }).OrderBy(x => x.StatusID)
                       .ToList();

                //vs.ForEach(x => x.Status = _langModule.GeTransStr(x.Status));
                return vs;
            }
            catch (Exception)
            {
                
                throw;
            }
           
          
        }

        public IQueryable<JobVibration> GetVbJobQuer()
        {
            return _jobVibration.GetQueryable();
        }

      

        public IQueryable<JobTemplateNotes> GetJobTemplateNotesQuer()
        {
            return _jobTemplateNotes.Where(x=>x.JobType==TemplateType.JobVibraton);
        }

     

        /*****************         INSERT              ******************/




        

       

       
        /*****************         DELETE              ******************/










        internal void Change(Job job, JobDM jobDM)
        {
            throw new NotImplementedException();
        }
    }
}
