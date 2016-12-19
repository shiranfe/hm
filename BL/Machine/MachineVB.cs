using System;
using System.Collections.Generic;
using System.Linq;
using BL.Moduls;
using Common;
using Microsoft.Practices.Unity;
using Repository;

namespace BL
{
    public class MachineVB
    {


        private readonly IUnitOfWork _uow;
        private readonly MachineModule _machineModule;
        private readonly VbmachineModule _vbMachineModule;

        //public MachineBL()
        //    : this(new VbModule(), new MachineModule(), new VbmachineModule(), new UserModule())
        //{

        //}

        public MachineVB([Dependency]IUnitOfWork uow,  
            [Dependency]MachineModule machineModule, 
            [Dependency]VbmachineModule vbMachineModule)
        {
            _uow = uow;
            _machineModule = machineModule;
            _vbMachineModule = vbMachineModule;
        }

        /*****************         GET              ******************/

        public MachineVBDM GetMachineVb(int machineID, int clientID, int? jobID, int? machinePointID=null)
        {
            if(!jobID.HasValue)
                 jobID = _vbMachineModule.GetLastVbJobID(machineID) ;

            /** פרטי מכונה*/
            MachineVBDM mac = _vbMachineModule.GetMachineVb(machineID, (int)jobID);
            mac.JobDates = _vbMachineModule.GetPreviusMacVbJobs(mac);
            //  int JobID = mac.JobID;

            /** נקודות במכונה     */      
            mac.PointPicDMs = _vbMachineModule.GetMacPointList(mac);
            if (mac.PointPicDMs != null)
            {
                if(!machinePointID.HasValue)
                    machinePointID = mac.PointPicDMs.First().MachinePointID;

                mac.PointSelected = GetPointSelected((int)machinePointID, mac.JobID);

                /** ערכי נקודה */             
                PointResultDM pointScheduleEntry = mac.PointSelected.PointResultDMs.First();
                pointScheduleEntry.From = mac.StartDate.AddYears(-1);
                pointScheduleEntry.To = mac.StartDate;
                SetPointResualt(pointScheduleEntry );
                
            }
            

            return mac;


        }

        private void SetPointResualt(PointResultDM pointScheduleEntry)
        {

           

           SetPointGraph(pointScheduleEntry);

            SetPointGraphLegend(pointScheduleEntry);

            
        }

        public MachinePointDM GetPointSelected(int machinePointID, int jobID)
        {

            var pdm = _machineModule.GetMacPointDetails(machinePointID);
            pdm.PointResultDMs = _vbMachineModule.GetPointResult(machinePointID, jobID);

            return pdm;

        }

        public PointResultDM GetPointResualt(int pointResultID, DateTime from, DateTime to)
        {

            PointResultDM pointScheduleEntry = _vbMachineModule.GetPointResultDM(pointResultID);

            pointScheduleEntry.From = from;
            pointScheduleEntry.To = to;
            SetPointResualt(pointScheduleEntry);
            return pointScheduleEntry;

        }

        public String GetTimestamp(DateTime value)
        {
            long ticks = value.Ticks - DateTime.Parse("01/01/1970 00:00:00").Ticks;
            ticks /= 10000;

            return ticks.ToString();

            // return value.ToShortDateString();
        }

        private void SetPointGraph(PointResultDM pr)
        {
            var quer = _vbMachineModule.GetGraphPoint(pr);

            var lst = quer.Where(x => pr.From <= x.Date && x.Date <= pr.To).ToList();

            if (lst.Count() < 2)
            {
                var closestPreviusDate = quer.OrderByDescending(x => x.Date).Take(2).ToList();
                if (closestPreviusDate.Count() > 1)
                {
                    pr.From = closestPreviusDate.Last().Date;
                    lst = quer.Where(x => pr.From <= x.Date && x.Date <= pr.To).ToList();
                }

            }

            pr.series = lst.Select(x =>
                new[] { GetTimestamp(x.Date), x.Value.ToString() }).ToArray();//
      

        }

        private void SetPointGraphLegend(PointResultDM pr)
        {
            var quer = _vbMachineModule.GetGraphLegend(pr.MachinePointID);
            
            List<LegendDM> ans = new List<LegendDM>();

            var lst = quer.Where(x => pr.From <= x.Date && x.Date <= pr.To).ToList();
             
            string[] names = lst.Select(x => x.ValueName).Distinct().ToArray();
            foreach (string valueName in names)
            {
                var v = lst.Where(x => x.ValueName == valueName).ToList();
                ans.Add(new LegendDM
                {
                    label = valueName,
                    data = v.Select(x =>
                           new[] { GetTimestamp(x.Date), x.Value.ToString() }).ToArray()

                });
            }


            pr.Legend = ans;

        }

        public string GetPicCords(int cardID)
        {
            return _machineModule.GetPicCords(cardID);
        }


        /*****************         UPDATE              ******************/

        public void Update(MachineVBDM model)
        {
            _vbMachineModule.Update(model);
            _uow.SaveChanges();
        }

        public void ChangeClientNotes(int machineID, int jobID, string clientNotes)
        {
            _vbMachineModule.UpdateClientNotes(machineID, jobID, clientNotes);
            _uow.SaveChanges();
        }

        public void ChangeComments(int machineID, string comments)
        {
            _machineModule.UpdateComments(machineID, comments);
            _uow.SaveChanges();
        }

    

        public void ChangeMachineDetails(MachineDetailsDM machineDetailsDM)
        {
            _machineModule.UpdateMachineDetails(machineDetailsDM);
            _uow.SaveChanges();
        }


        public void ChangeSku(int machineID, string sku)
        {
            _machineModule.UpdateSku(machineID, sku);
            _uow.SaveChanges();
        }

        public void ChangeDetails(int machineID, string details)
        {
            _machineModule.UpdateDetails(machineID, details);
            _uow.SaveChanges();
        }



        /********************         Admin       **********************/

        public void ChangePointXy(int machinePointID, int x, int y)
        {
            _machineModule.UpdatePointXy(machinePointID, x, y);
            _uow.SaveChanges();
        }


        public void ChangePointShow(int machinePointID, bool show)
        {
            _machineModule.ChangePointShow(machinePointID, show);
            _uow.SaveChanges();
        }








     
    }
}
