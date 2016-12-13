using BL;
using Common;

using Microsoft.Practices.Unity;
using MvcBlox.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace MVC.Areas.Admin.Controllers
{

    public class VBController : _BasicController
    {
        private readonly VbBL _vbBL;
        private EmployeeBL _employeeBL;
        private AdminBL _adminBL;
        private readonly ClientBL _clientBL;
        private readonly IGlobalManager _globalManager;

        public VBController([Dependency]EmployeeBL employeeBL,
              [Dependency]ClientBL clientBL,
            [Dependency]AdminBL adminBL, [Dependency] VbBL vbBL, [Dependency] IGlobalManager globalManager)
        {
            _globalManager = globalManager;
            _employeeBL = employeeBL;
             _vbBL = vbBL;
             _adminBL = adminBL;
            _clientBL = clientBL;
        }

      
        public int GetAdminClientID()
        {
            return _globalManager.GetAdminClientID();

        }

        public void GetMacPic(SelectedMachine mac)
        {
            _globalManager.GetMacPic(mac);
        }

        /*************   Select  ***********/


        public ActionResult Index(VbFilterDm filter)
        {

            // var ClientID = GetAdminClientID();
            var tree = _clientBL.GetClientDrop(true);
            var root = tree.First();
            /** remove add option*/
            root.Childs = root.Childs.Where(x => x.ClientID != -2).ToList();


            ViewBag.Clients = tree;
            ViewBag.IsPostedOps = new SelectList(new List<SelectListItem>() {
                new SelectListItem { Value="", Text="כל הדוחות"},
                 new SelectListItem { Value="true", Text="פורסם"},
                new SelectListItem { Value="false", Text="לא פורסם"}
            }, "Value", "Text", filter.IsPosted );

            //ViewBag.InputName = "ClientIndexID"; //to ignore 'required' message
            _vbBL.GetClientReports(filter);


            return PartialView(filter);

        }

        public ActionResult VBAnalysis(int JobID)
        {
        
            var ClientID = GetAdminClientID();
            var rep = _vbBL.GetVbReportEdit(JobID);

            PopulateJobDetailsDrops();

            return PartialView(rep);
        }

        private void PopulateJobDetailsDrops()
        {
             // ViewBag.Urgency = jobBL.GetUrgency();

            //ViewBag.TesterName = _employeeBL.GetEmpByRole("Tester");
            //ViewBag.AnalyzerName = _employeeBL.GetEmpByRole("Analyzer");
            ViewBag.ClientID = "";

        }

        public ActionResult VBMachine(int JobID, int MachineID)
        {
            
            var mac = _vbBL.GetSelectedVbEditMachine(MachineID, JobID);


            return PartialView(mac);
        }


        public ActionResult VBEditJobTemplateNotes()
        {

            var dict = _vbBL.GetJobTemplateNotes();
            return PartialView(dict);
        }

        public ActionResult PointResualt(int? JobIDNuulable)
        {
            var JobID = JobIDNuulable ?? 2814;
            var mac = _vbBL.GetPointResualt(JobID);
            mac.SelectedPointResualt.SpectrumPic = PicHelper.GetPointResualt(mac.SelectedPointResualt.PointResualtID);
            mac.JobID = JobID;
            return PartialView(mac);
        }


        public ActionResult PointResualtSelected(int PointResualtID)
        {

            var mac = _vbBL.GetSelectedPointResualt(PointResualtID);
            mac.SpectrumPic = PicHelper.GetPointResualt(PointResualtID);

            return PartialView(mac);

        }


        public ActionResult MachineJobs(int id)
        {

            var model = _vbBL.GetMachineJobsHistory(id);
            ViewBag.MachineID = id;
            return PartialView(model);

        }

        /*************      UPDATE        ***********/

        [HttpPost]
        public ActionResult UpdateJobTemplateNote(string key, string lang, string word)
        {
            var ID = Convert.ToInt32(key);
            _vbBL.UpdateJobTemplateNote(ID, lang, word);
            return Json(new { res = 1 });
        }


        [HttpPost]
        public ActionResult VBUpdateHiddenResualt(int PointResultID, bool Hidden)
        {
            _vbBL.UpdateHiddenResualt(PointResultID, Hidden);

            return Json(new {  });
        }


        [HttpPost]
        public ActionResult VBUpdateResualtStatus(int PointResultID, int StatusID)
        {
            _vbBL.UpdateResualtStatus(PointResultID, StatusID);

            return Json(new {  });
        }

        [HttpPost]
        public ActionResult VBUpdateMultyResualtStatus(int MachineID, int JobID, int StatusID)
        {
            _vbBL.UpdateMultyResualtStatus(MachineID, JobID, StatusID);

            return Json(new {  });
        }

        [HttpPost]
        public ActionResult UpdateVBGeneralNote(VBNotes vbNotes)
        {
            _vbBL.UpdateVbGeneralNote(vbNotes);

            return Json(new {  });
        }

        [HttpPost]
        public ActionResult VBUpdateIsPosted(int JobID, bool IsPosted)
        {
            _vbBL.VbUpdateIsPosted(JobID, IsPosted);

            return Json(new {  });
        }

        /*************      DELETE        ***********/
        [HttpPost]
        public ActionResult DeleteJobVB(int JobID)
        {
            _vbBL.DeleteJobVb(JobID);

            return Json(new { });
        }
    }
}
