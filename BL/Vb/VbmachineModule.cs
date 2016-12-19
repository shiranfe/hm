using System.Collections.Generic;
using System.Linq;
using Common;
using DAL;
using Microsoft.Practices.Unity;
using Repository;

namespace BL.Moduls
{
    public class VbmachineModule
    {
        private readonly IUnitOfWork _uow;
        private readonly IRepository<JobVibrationMachine> _jobVibrationMachineDal;
        private readonly IRepository<vwJobVibrationMachine> _vwjobVibrationMachineDal;
        private readonly IRepository<JobVibrationMachinePointResult> _jobVibrationMachinePointResultDal;
        private readonly IRepository<Job> _jobDal;


        public VbmachineModule([Dependency]IUnitOfWork uow)
        {
            _uow = uow;
            _jobVibrationMachineDal = _uow.Repository<JobVibrationMachine>();
            _vwjobVibrationMachineDal = _uow.Repository<vwJobVibrationMachine>();
            _jobVibrationMachinePointResultDal = _uow.Repository<JobVibrationMachinePointResult>();
            _jobDal = _uow.Repository<Job>() ;
        }



        /***************************************************/



        public List<AdminPointResualtDM> GetPointResualt(int jobID)
        {

            var ans = (from p in _jobVibrationMachinePointResultDal.GetQueryable()
                       where !(bool)p.Hide && p.JobID == jobID
                       select new AdminPointResualtDM
                       {
                           PointResualtID = p.JobVibrationMachinePointResultID,
                           ClientName = p.MachinePoint.Machine.Client.ClientName,
                           MachineName = p.MachinePoint.Machine.MachineName,
                           PointName = p.MachinePoint.PointNumber,
                           Date = p.Date,
                           ScheduleEntryID = p.ScheduleEntry,
                           ScheduleEntryKey = p.PickList.Key,
                           DirectionID = (int)p.Direction,
                           DirectionStr = p.PickList1.Key,
                           Value = p.Value

                       }).OrderBy(x => x.MachineName).ThenBy(x => x.PointName).ToList();

            //GetPointResultStrings(ans);


            return ans;

        }

        public AdminPointResualtDM GetSinglePointResualt(int pointResualtID)
        {

            var ans = (from p in _jobVibrationMachinePointResultDal.GetQueryable()
                       where p.JobVibrationMachinePointResultID == pointResualtID
                       select new AdminPointResualtDM
                       {
                           PointResualtID = p.JobVibrationMachinePointResultID,
                           ClientName = p.MachinePoint.Machine.Client.ClientName,
                           MachineName = p.MachinePoint.Machine.MachineName,
                           PointName = p.MachinePoint.PointNumber,
                           Date = p.Date,
                           ScheduleEntryID = p.ScheduleEntry,
                           ScheduleEntryKey = p.PickList.Key,
                           DirectionID = (int)p.Direction,
                           DirectionStr = p.PickList1.Key,
                           Value = p.Value

                       }).Single();

            //GetPointResultStrings(ans);


            return ans;

        }

        public MachineVBDM GetMachineVb(int machineID, int jobID)
        {

            string defClientNotes = "הוסף הערה";
            return (from m in _jobVibrationMachineDal.GetQueryable()
                    where m.MachineID == machineID && m.JobID == jobID
                    select new MachineVBDM
                    {
                        MachineID = m.MachineID,
                        MachineName = m.Machine.MachineName,
                        ClientNotes = m.ClientNote != null ? m.ClientNote : defClientNotes,
                        NotesIL = m.GeneralNoteIL,
                        NotesEN = m.GeneralNoteEN,
                        StartDate =m.JobVibration.Job.StartDate,
                        MachineTypeID = m.Machine.MachineType.Value,
                        JobID = m.JobID
                    }).SingleOrDefault();

        }

        public List<JobDatesDM> GetPreviusMacVbJobs(MachineVBDM mac)
        {

            return (from m in _jobVibrationMachineDal.GetQueryable()
                    where m.MachineID == mac.MachineID
                    select new JobDatesDM
                    {
                        JobID = m.JobVibration.JobID,
                        JobName = m.JobVibration.Job.JobName,
                        StartDate = m.JobVibration.Job.StartDate
                    }).OrderByDescending(x => x.StartDate).ToList();

        }

        public List<MachinePointMinDM> GetMacPointList(MachineVBDM mac)
        {

            var quer = from p in _jobVibrationMachinePointResultDal.GetQueryable()
                       where p.MachinePoint.MachineID == mac.MachineID && p.JobID == mac.JobID && p.Hide == false
                       //  && p.MachinePoint.ShowPoint
                       group p by p.MachinePointID into grp
                       select grp.OrderBy(x => x.Status.Value).FirstOrDefault();

            var ans = (from p in quer

                       select new MachinePointMinDM
                       {
                           MachinePointID = p.MachinePointID,
                           PointNumber = p.MachinePoint.PointNumber,
                           StatusID = p.Status.Value,
                           HtmlX = p.MachinePoint.HtmlX,
                           HtmlY = p.MachinePoint.HtmlY,
                           LangStr = p.Status.Key,
                           LastDate = p.Date,
                           ShowPoint = p.MachinePoint.ShowPoint
                       }).OrderBy(x => x.PointNumber).ToList();

            return ans;

        }

       
        public VbMachineDM GetVbMachineDM(int machineID, int jobID)
        {

            return (from m in _jobVibrationMachineDal.GetQueryable()
                    where m.MachineID == machineID && m.JobID == jobID
                    select new VbMachineDM
                    {
                        MachineID = m.MachineID,
                        MachineName = m.Machine.MachineName,
                        ClientNote = m.ClientNote,
                        LastTipul = m.Machine.Comments,
                        GeneralNoteEN = m.GeneralNoteEN,
                        GeneralNoteIL = m.GeneralNoteIL

                    }).Single();


        }

        public JobVibrationMachine GetJobVibrationMachine(int machineID, int jobID)
        {
            return _jobVibrationMachineDal
                .SingleOrDefault(x => x.JobID == jobID && x.MachineID == machineID);
        }

        public List<NotesHisory> GetNotesHisory(int machineID, int jobID)
        {


            return (from m in _vwjobVibrationMachineDal.GetQueryable()
                    where m.MachineID == machineID && m.JobID != jobID
                    && (m.GeneralNoteEN ?? m.GeneralNoteIL) != null
                    select new NotesHisory
                    {
                        JobStartDate = m.JobStartDate,
                        GeneralNote = m.GeneralNoteEN ?? m.GeneralNoteIL
                    }).ToList();

        }





        public List<VbMachineDM> GetMultyVbEditMachineDMs(int jobID)
        {

            var vs = (from v in _jobVibrationMachineDal.GetQueryable()
                      where v.JobID == jobID
                      select new VbMachineDM
                      {
                          MachineID = v.MachineID,
                          MachineName = v.Machine.MachineName

                      }).OrderBy(x => x.MachineName)
                   .ToList();


            return vs;

        }

        public List<PointResultDM> GetPointResult(int machinePointID, int jobID)
        {

            var ans = (from p in _jobVibrationMachinePointResultDal.GetQueryable()
                       where p.MachinePointID == machinePointID && !(bool)p.Hide && p.JobID == jobID
                       select new PointResultDM
                       {
                           JobVibrationMachinePointResultID = p.JobVibrationMachinePointResultID,
                           JobID = p.JobID,
                           MachinePointID = p.MachinePointID,
                           Date = p.Date,
                           ScheduleEntryID = p.ScheduleEntry,
                           ScheduleEntryKey = p.PickList.Key,
                           DirectionID = (int)p.Direction,
                           DirectionStr = p.PickList1.Key,
                           Value = p.Value,
                           StatusID = p.Status.Value,
                           LangStr = p.Status.Key
                           // PrcntChange= p.pre,
                       }).OrderBy(x => x.StatusID).ToList();

            //GetPointResultStrings(ans);


            return ans;

        }

        public PointResultDM GetPointResultDM(int pointResultID)
        {

            var ans = (from p in _jobVibrationMachinePointResultDal.GetQueryable()
                       where p.JobVibrationMachinePointResultID == pointResultID
                       select new PointResultDM
                       {
                           JobVibrationMachinePointResultID = p.JobVibrationMachinePointResultID,
                           JobID = p.JobID,
                           MachinePointID = p.MachinePointID,
                           ScheduleEntryID = p.ScheduleEntry,
                           DirectionID = (int)p.Direction,
                           DirectionStr = p.PickList1.Key,
                           ScheduleEntryKey = p.PickList.Key
                       }).Single();


            return ans;

        }


        public List<VbPointResultDM> VbPointResultDMs(int machineID, int jobID)
        {

            var ans = (from p in _jobVibrationMachinePointResultDal.GetQueryable()
                       where p.MachinePoint.MachineID == machineID && p.JobID == jobID
                       select new VbPointResultDM
                       {
                           JobVibrationMachinePointResultID = p.JobVibrationMachinePointResultID,

                           MachinePointID = p.MachinePointID,
                           PoineNumber = p.MachinePoint.PointNumber,
                           ScheduleEntryID = p.ScheduleEntry,
                           ScheduleEntryKey = p.PickList.Key,
                           DirectionID = (int)p.Direction,
                           DirectionKey = p.PickList1.Key,
                           Value = p.Value,
                           StatusID = p.Status.Value,
                           LangStr = p.Status.Key,
                           IsHidden = p.Hide ?? false
                       }).OrderBy(x => x.PoineNumber)
                       .ThenBy(x => x.ScheduleEntryID)
                       .ThenBy(x => x.DirectionID)
                       .ThenBy(x => x.Value);
            return ans.ToList();

        }


        public IQueryable<GraphPointDM> GetGraphPoint(PointResultDM g)
        {

            var ans = (from p in _jobVibrationMachinePointResultDal.GetQueryable()
                       join j in _jobDal.GetQueryable() on p.JobID equals j.JobID
                       where
                           p.MachinePointID == g.MachinePointID &&
                           p.ScheduleEntry == g.ScheduleEntryID &&
                           p.Direction == g.DirectionID &&
                           j.IsPosted && !(bool)p.Hide
                       select new GraphPointDM
                       {
                           Date = p.Date,
                           JobID = p.JobID,
                           Value = (double)p.Value
                       });
            return ans;

        }

        public IQueryable<GraphPointDM> GetGraphLegend(int machinePointID)
        {
            var ans = (from p in _jobVibrationMachinePointResultDal.GetQueryable()
                       join j in _jobDal.GetQueryable() on p.JobID equals j.JobID
                       where 
                       p.MachinePointID == machinePointID && 
                       j.IsPosted && !(bool)p.Hide
                       select new GraphPointDM
                       {
                           ScheduleEntryKey = p.PickList.Key,
                           DirectionStr = p.PickList1.Key,
                           Date = p.Date,
                           JobID = p.JobID,
                           Value = (double)p.Value
                       });
            return ans;

        }


        public int GetLastVbJobID(int machineID)
        {

            return _jobVibrationMachineDal.Where(x => x.MachineID == machineID)
                .Select(i => i.JobID).Max(x => x);

        }


        public List<JobVibrationMachinePointResult> GetPointResultByPointID(int pointID)
        {

            var quer = _jobVibrationMachinePointResultDal.Where(x => x.MachinePointID == pointID);
            return quer.ToList();
        }

        public List<JobVibrationMachine> GetJobVibrationMachineList(int machineID)
        {
            return _jobVibrationMachineDal.Where(x => x.MachineID == machineID).ToList();
        }


       


        /*****************         UPDATE              ******************/

        public void AddMulty(List<JobVibrationMachinePointResult> rslts)
        {
            foreach (var x in rslts)
            {
                _jobVibrationMachinePointResultDal.Add(x);
            }

        }



        /*****************         UPDATE              ******************/


        internal void Update(MachineVBDM model)
        {
            JobVibrationMachine entity = _jobVibrationMachineDal.SingleOrDefault(x => x.MachineID == model.MachineID && x.JobID == model.JobID);
            entity.ClientNote = model.ClientNotes;
           
        }



        public void UpdateClientNotes(int machineID, int jobID, string clientNotes)
        {

            JobVibrationMachine entity = _jobVibrationMachineDal.SingleOrDefault(x => x.MachineID == machineID && x.JobID == jobID);
            entity.ClientNote = clientNotes;
            

        }

        public void UpdateVbGeneralNote(VBNotes vbNotes)
        {
            JobVibrationMachine entity = GetJobVibrationMachine(vbNotes.MachineID, vbNotes.JobID);

            if (vbNotes.Lang == "IL")
                entity.GeneralNoteIL = vbNotes.GeneralNote;
            else
                entity.GeneralNoteEN = vbNotes.GeneralNote;

        }

        public void UpdateHiddenResualt(int pointResultID, bool hidden)
        {

            JobVibrationMachinePointResult pointResult = _jobVibrationMachinePointResultDal
                .SingleOrDefault(x => x.JobVibrationMachinePointResultID == pointResultID);
            pointResult.Hide = hidden;

        }

        public void UpdateResualtStatus(int pointResultID, int pickListID)
        {

            JobVibrationMachinePointResult pointResult = _jobVibrationMachinePointResultDal
                .SingleOrDefault(x => x.JobVibrationMachinePointResultID == pointResultID);
            pointResult.StatusID = pickListID;

        }

        public void UpdateePointResultByMacAndJob(int machineID, int jobID, int pickListID)
        {

            List<JobVibrationMachinePointResult> rstls = _jobVibrationMachinePointResultDal
                .Where(x => x.JobID == jobID && x.MachinePoint.MachineID == machineID)
                .ToList();
            rstls.ForEach(x => x.StatusID = pickListID);

        }



    }
}
