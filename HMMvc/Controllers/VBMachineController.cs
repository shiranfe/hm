using BL;
//using HMErp.Helper;
using Common;
using Microsoft.Practices.Unity;
using MvcBlox.Models;
using StackExchange.Profiling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace MVC.Controllers
{
    [Authorize]
    public class VBMachineController : Controller
    {
        private readonly MachineBL _machineBL;
        private readonly MachineVB _machineVB;
        private readonly IGlobalManager _globalManager;



        public VBMachineController([Dependency]MachineBL machineBL, 
            [Dependency]MachineVB machineVB,
            [Dependency]IGlobalManager globalManager)
        {
            _globalManager = globalManager;
            _machineBL = machineBL;
            _machineVB = machineVB;
        }

        /*******************        GET        *******************/
        public ActionResult MachineList()
        {
            var userID = (int)Session["UserID"];
            return PartialView(_machineBL.GetMachineDetailsList(userID));
        }

        public ActionResult MachinePage(int machineID, int jobID, int? machinePointID) //, string activeTab
        {

            var model = _machineBL.GetMachinePage(machineID);
            if (model == null || !_globalManager.AuthorizeUser((int)model.ClientID))
                return Redirect("/Home/NotValidate");

            MachineVBDM mac = GetPointPage(machineID, jobID, machinePointID);
            model.VBDetails = mac;


            ViewBag.JobDates = new SelectList(model.VBDetails.JobDates, "JobID", "StartDate", jobID);
                 
            return PartialView(model);



        }


        public ActionResult PointPage(int machineID, int jobID, int machinePointID) //, string activeTab
        {

            MachineVBDM mac = GetPointPage(machineID, jobID, machinePointID);

            return PartialView(mac);

        }


        private MachineVBDM GetPointPage(int machineID, int jobID, int? machinePointID)
        {
            var clientID = (int)Session["ClientID"];
            var mac = _machineVB.GetMachineVb(machineID, clientID, jobID, machinePointID);
            PopulateVbTabDrops(mac);
            mac.MacPointPic = PicHelper.GetMacPic(mac.MachineID, mac.MachineTypeID);
            //mac.PointSelected.PoinPic = PicHelper.GetPointPic(mac.PointSelected.MachinePointID);
            mac.PointSelected.PointResultDMs.ForEach(x => x.SpectrumPic = PicHelper.GetPointResualt(x.JobVibrationMachinePointResultID));

            //if (jobID.HasValue)
            mac.JobDates.Single(x => x.JobID == jobID).IsSelected = true;
            //ViewBag.DateCombo = new SelectList(mac.JobDates, "JobID", "StartDate", mac.JobDates.First().JobID);

            return mac;
        }


      


        public ActionResult MachineDetailsTab(int machineID)
        {
            var undifined = GlobalDM.GetTransStr("Undefined");
            var mac = _machineBL.GetMachineDetails(machineID,undifined);
           // mac.MacPic = PicHelper.GetMacPic(mac.MachineID, mac.MachineTypeID);
            
            return PartialView(mac);
        }

        [HttpPost]
        public ActionResult MachineDetailsTab(MachineDetailsDM machineDetailsDM)
        {
            Extensions.Strlz(machineDetailsDM);
            _machineVB.ChangeMachineDetails(machineDetailsDM);
            var macID = machineDetailsDM.MachineID;
            return PartialView("MachinePage",  macID );
        }    

        public ActionResult MachineVbTab(int machineID, int? jobID)
        {

            var clientID = (int)Session["ClientID"];
            var mac = _machineVB.GetMachineVb(machineID, clientID, jobID);
            PopulateVbTabDrops(mac);
            mac.MacPointPic = PicHelper.GetMacPic(mac.MachineID, mac.MachineTypeID);
            //mac.PointSelected.PoinPic = PicHelper.GetPointPic(mac.PointSelected.MachinePointID);
            mac.PointSelected.PointResultDMs.ForEach(x => x.SpectrumPic = PicHelper.GetPointResualt(x.JobVibrationMachinePointResultID));

            if (jobID.HasValue) mac.JobDates.Single(x => x.JobID == jobID).IsSelected = true;
            //ViewBag.DateCombo = new SelectList(mac.JobDates, "JobID", "StartDate", mac.JobDates.First().JobID);
            return PartialView(mac);
           
        }




       

        [HttpPost]
        public ActionResult PointDetails(int machinePointID, int jobID)
        {
            
            var mac = _machineVB.GetPointSelected(machinePointID, jobID);
          //  mac.PoinPic = PicHelper.GetPointPic(MachinePointID);
            mac.PointResultDMs.ForEach(x => x.SpectrumPic = PicHelper.GetPointResualt(x.JobVibrationMachinePointResultID));
            return PartialView(mac);
        }

        public ActionResult MachineVbTabPointVal(int pointResultID, string startDate, string endDate)
        {
            var @from = Convert.ToDateTime(startDate);
            var to = Convert.ToDateTime(endDate);
            var mac = _machineVB.GetPointResualt(pointResultID, (DateTime)@from, (DateTime)to);
            mac.SpectrumPic = PicHelper.GetPointResualt(mac.JobVibrationMachinePointResultID);
            return PartialView(mac);
        }


        [HttpPost]
        public ActionResult GetChart()
        {
            string[][] ans = { new[] { "0", "4" }, new[] { "1", "7" } };
            //ans = 

           
            return Json(new { labal = "gdfgdfg", data = ans });
        }

        /*******************        UPDATE        *******************/

        [HttpPost]
        public ActionResult Update(MachineVBDM model)
        {

            _machineVB.Update(model);
            return Json(new { });
        }

        [HttpPost]
        public ActionResult EditClientNotes(int machineID, string clientNotes, int jobID)
        {

            _machineVB.ChangeClientNotes(machineID, jobID, clientNotes);
            return Json(new {  });
        }

       [HttpPost]
       public ActionResult EditComments(int machineID, string comments)
       {


            _machineVB.ChangeComments(machineID, comments);
           return Json(new {  });
       }

       [HttpPost]
       public ActionResult EditSku(int machineID, string sku)
       {


            _machineVB.ChangeSku(machineID, sku);
           return Json(new {  });
       }

       [HttpPost]
       public ActionResult EditDetails(int machineID, string details)
       {

            _machineVB.ChangeDetails(machineID, details);
           return Json(new {  });
       }

        private void PopulateVbTabDrops(MachineVBDM mac)
        {
            ViewBag.JobDates = new SelectList(mac.JobDates, "JobID", "StartDate", mac.JobDates.First().JobID);

            var p = new List<SelectListItem>
            {
                new SelectListItem {Value = "1", Text = "שם נקודה", Selected = true},
                new SelectListItem {Value = "2", Text = "סטטוס"},
                new SelectListItem {Value = "3", Text = "תאריך בדיקה"}
            };
            ViewBag.PointSts = p;


        }


    }
}
