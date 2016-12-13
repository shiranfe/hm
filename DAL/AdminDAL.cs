using Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DAL
{
    public class AdminDAL
    {
        private static HMErpEntities db = new HMErpEntities();

        public static List<MachineBasicDM> GetDefualtMachine()
        {
            return (from m in db.Machine
                    where m.ClientID == 230
                    select new MachineBasicDM
                    {
                        MachineID = m.MachineID,
                        MachineName = m.MachineName,
                    }).ToList();
        }

        public static SelectedMachine GetSelectedMachine(int MachineID)
        {
            var ans =  (from m in db.Machine
                    where m.MachineID == MachineID
                    select new SelectedMachine
                    {
                        MachineID = m.MachineID,
                        MachineName = m.MachineName,
                        MachineTypeID = m.MachineTypeID==null ? "1" : m.MachineType.Value ,
                        IsDefualtMac = m.ClientID == 230,
                       
                    }).Single();
            ans.ObjID = ans.IsDefualtMac ?Convert.ToInt32(ans.MachineTypeID) : ans.MachineID;
            return ans;
        }

        public static List<MachinePointMinDM> GetMachinePoints(int MachineID)
        {
            return (from p in db.MachinePoint
                    where p.MachineID == MachineID
                    select new MachinePointMinDM
                    {
                        MachinePointID = p.MachinePointID,
                        PointNumber = p.PointNumber,
                        HtmlX = p.HtmlX,
                        HtmlY = p.HtmlY,
                        ShowPoint=p.ShowPoint,
                    }).OrderBy(x => x.PointNumber).ToList();
        }

        public static void UpdatePointXY(int MachinePointID, int X, int Y)
        {
           
                var pnt = db.MachinePoint.Single(x => x.MachinePointID == MachinePointID);
                pnt.HtmlX = X;
                pnt.HtmlY = Y;
                db.SaveChanges();
          
        }



        public static void ChangePointShow(int MachinePointID, bool Show)
        {
           
                var pnt = db.MachinePoint.Single(x => x.MachinePointID == MachinePointID);
                pnt.ShowPoint = Show;
                db.SaveChanges();
          
        }

        public static List<AdminPointResualtDM> GetPointResualt(int JobID)
        {
           
                var ans= (from p in db.JobVibrationMachinePointResult
                          where  !(bool)p.Hide && p.JobID==JobID
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
                              Value = p.Value,
                             
                          }).OrderBy(x=>x.MachineName).ThenBy(x=>x.PointName).ToList();

                //GetPointResultStrings(ans);
    
               
                return ans;
           
        }

        public static AdminPointResualtDM GetSinglePointResualt(int PointResualtID)
        {
           
                var ans = (from p in db.JobVibrationMachinePointResult
                           where p.JobVibrationMachinePointResultID==PointResualtID
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
                               Value = p.Value,

                           }).Single();

                //GetPointResultStrings(ans);


                return ans;
           
        }

        public static List<JobTemplateNotes> GetJobTemplateNotes()
        {
            return db.JobTemplateNotes.ToList();
        }

        public static IQueryable <JobTemplateNotes> GetJobTemplateNotesQuer()
        {
            return db.JobTemplateNotes;
        }

        public static void UpdateJobTemplateNote(JobTemplateNotes rw)
        {
            db.SaveChanges();
        }





        public static void SaveChanges()
        {
            db.SaveChanges();
        }

        public static List<JobVibrationMachinePointResult> GetPointResultByPointID(int PointID)
        {
            
            var quer = db.JobVibrationMachinePointResult.Where(x => x.MachinePointID == PointID);
            return quer.ToList();
        }

        public static List<MachinePoint> GetMacPointList(int MachineID)
        {
            return (from m in db.MachinePoint
                    where m.MachineID == MachineID
                    select m).ToList();
        }


        public static Machine GetMachine(int MachineID)
        {
            return db.Machine.SingleOrDefault(x => x.MachineID == MachineID);
        }

        public static List<JobVibrationMachine> GetJobVibrationMachineList(int MachineID)
        {
            return db.JobVibrationMachine.Where(x => x.MachineID == MachineID).ToList();
        }

        public static void DeleteMachine(Machine OldsMac)
        {
            db.Machine.Remove(OldsMac);
          

        }

        public static void DeletePoints(List<MachinePoint> OldMacPoints)
        {
            OldMacPoints.ForEach(x => db.MachinePoint.Remove(x));
            db.SaveChanges();
        }


        public static void MergeMachines(int MacNewMergeID, int MacOldMergeID)
        {
            try
            {
                db.spMergeMachines(MacOldMergeID, MacNewMergeID);
            }
            catch (Exception)
            {                
                throw;
            }
           
        }

        public static void InsertUser(User newUser)
        {
            db.User.Add(newUser);
            db.SaveChanges();
        }

        public static void UpdateUser(User user)
        {
            db.SaveChanges();
        }

      

      
    }
}
