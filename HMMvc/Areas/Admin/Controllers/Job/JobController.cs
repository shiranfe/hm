using BL;
using Common;
//using HMErp.Helper;
using Microsoft.Practices.Unity;
using MVC.Models;
using MvcBlox.Models;
using System;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using System.Web.Mvc;

namespace MVC.Areas.Admin.Controllers
{


    public class JobController : _BasicController
    {

        private readonly IGlobalManager _globalManager;

        private readonly ClientBL _clientBL;
        private readonly EmployeeBL _employeeBL;
        private readonly LangBL _langBL;
        private readonly AdminBL _adminBL;
        private readonly JobBL _jobBL;
        private readonly JobRequestBL _jobRequestBL;
        private readonly MachineBL _machineBL;
        private readonly UserBL _userBL;
        private readonly PicManager _picManger;

        public JobController([Dependency]ClientBL clientBL,
            [Dependency] IGlobalManager globalManager,
            [Dependency]MachineBL machineBL,
            [Dependency] UserBL userBL,
            [Dependency]EmployeeBL employeeBL,
             [Dependency]PicManager picManger,
            [Dependency]JobRequestBL jobRequestBL,
            [Dependency]LangBL langBL, [Dependency]JobBL jobBL,
            [Dependency] AdminBL adminBL)
        {
            _globalManager = globalManager;
            _clientBL = clientBL;
            _employeeBL = employeeBL;
            _langBL = langBL;
            _machineBL = machineBL;
            _adminBL = adminBL;
            _jobBL = jobBL;
            _userBL = userBL;
            _jobRequestBL = jobRequestBL;
            _picManger = picManger;

            AdminClientID = _globalManager.GetAdminClientID();
            EmpID = _globalManager.GetEmpID();
        }

        private readonly int AdminClientID;
        private readonly int EmpID;


        /*************      GET         ***********/
        public ActionResult MachineParts(int JobID, int MachineID, JobType jobType)//, bool json=false
        {
            var model = _machineBL.GetMachineParts(JobID, MachineID, jobType);
            ViewBag.MachineID = MachineID;

            if (jobType == JobType.Alignment)
                ViewBag.MachinePartID = "JobAlignmentDM.MachinePartID";
            else if (jobType == JobType.Refubrish)
                ViewBag.MachinePartID = "RefubrishDetailsDM.MachinePartID";

            //if (json)
            //    return Json(model.Select(x=>x.MachineTypeID).ToArray());

            return PartialView(model);
        }
      
        public ActionResult RequestMachineParts(int MachineID)//, bool json=false
        {


            try
            {
                var model = _machineBL.GetMachinePartTypeIds(MachineID);

                return Json(new { typeIds = model }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return ExceptionObj(e);
            }
        }


        public ActionResult Details(int? JobID, JobType jobType, bool IsReturning = false)
        {
            var model = GetDetailsModel(JobID, jobType, IsReturning);

            PopulateJobDrop(model);

            SetJobPics(model);

            return PartialView(model);
        }

        private JobDM GetDetailsModel(int? JobID, JobType jobType, bool IsReturning)
        {

            if (!JobID.HasValue)
            {
                ViewBag.PopTitle = "עבודה חדשה";
                return _jobBL.IntiateNewJob(EmpID, AdminClientID, jobType);
            }

            var model = _jobBL.GetJobDM((int)JobID);

            if (!IsReturning)
            {
                ViewBag.PopTitle = "עדכון עבודה";
                return model;
            }


            ViewBag.PopTitle = "עבודה חדשה";
            model.ReturningJobParentID = JobID;
            model.JobID = 0;
            model.RefubrishDetailsDM.JobID = 0;

            return model;
        }


        public ActionResult AddReturningJob()
        {
            @ViewBag.PopTitle = "עבודה חוזרת";
            return PartialView();
        }

        public ActionResult JobsUnQuoted(int ClientID)
        {
            var model = _jobBL.JobsUnQuoted(ClientID);
            ViewBag.PopTitle = "שיוך עבודות להצעת מחיר";
            return PartialView(model);
        }



        private void PopulateJobDrop(JobDM jobDM)
        {

            //ViewBag.Client = new SelectList(clnts, "ClientID", "ClientName", jobDM.ClientID);
            jobDM.Clients = _clientBL.GetClientDrop();

            ViewBag.Clients = new SelectList(_clientBL.GetClientList(), "ClientID", "ClientFullName");

            ViewBag.Creator = new SelectList(_employeeBL.GetEmployeesList(), "EmployeeID", "FullName", jobDM.CreatorID);
            ViewBag.Contacts = new SelectList(_userBL.GetClientUsers(jobDM.ClientID), "UserID", "FullName", jobDM.ContactID);



            if (jobDM.IsRefubrish)
                PopulateJobRefubrishDrop(jobDM);

            if (jobDM.IsVB)
                PopulateJobVBDrop(jobDM);

            if (jobDM.IsAlignment)
                PopulateJobAlignmentDrop(jobDM);

        }

        private void SetJobPics(JobDM model)
        {
            if (model.JobID == 0)
                return;

            var folder = PicHelper.JobPathPhys + model.JobID;

            /** no picture uploaded*/
            if (!Directory.Exists(folder))
                return;

            var detailPic = Directory.GetFiles(folder, "DetailsStep*.jpg", SearchOption.TopDirectoryOnly).FirstOrDefault();
            if (detailPic != null)
                ViewBag.DetailPic = PicHelper.PhysicalToUrl(detailPic);


            var preTestStepPic = Directory.GetFiles(folder, "PreTestStep*.jpg", SearchOption.TopDirectoryOnly).FirstOrDefault();
            if (preTestStepPic != null)
                ViewBag.PreTestStepPic = PicHelper.PhysicalToUrl(preTestStepPic);

        }

        private void PopulateJobAlignmentDrop(JobDM jobDM)
        {
            ViewBag.Testers = new SelectList(_employeeBL.GetEmpByRole("EmployeeRole_Tester"), "id", "text", jobDM.CreatorID);
            ViewBag.Machine = new SelectList(_machineBL.GetClientMachines(jobDM.ClientID), "MachineID", "MachineNameFull");
        }

        private void PopulateJobVBDrop(JobDM jobDM)
        {
            ViewBag.AnalyzerID = new SelectList(_employeeBL.GetEmpByRole("EmployeeRole_Analyzer"), "id", "text", jobDM.CreatorID);
            ViewBag.TesterID = new SelectList(_employeeBL.GetEmpByRole("EmployeeRole_Tester"), "id", "text", jobDM.CreatorID);
        }

        private void PopulateJobRefubrishDrop(JobDM jobDM)
        {
            ViewBag.Branch = new SelectList(_adminBL.GetBranchList(), "Value", "Text", jobDM.RefubrishDetailsDM.BranchID);
            ViewBag.Machine = new SelectList(_machineBL.GetClientMachines(jobDM.ClientID), "MachineID", "MachineNameFull");
            var rr = _langBL.GetPickListValue("JobRefubrishStatus");
            ViewBag.Status = new SelectList(rr, "PickListID", "TransStr", jobDM.RefubrishDetailsDM.RefubrishStatusID);
        }



        public ActionResult JobRequest()
        {

            var jobDM = _jobBL.InitiateJobRequest(EmpID);

            ViewBag.PopTitle = "בקשת עבודה";

            PopulateJobDrop(jobDM);
            ViewBag.Parts = new SelectList(_langBL.GetPickListValue("MachineType").OrderBy(x => x.TransStr), "PickListID", "TransStr");

            SetJobPics(jobDM);

            return PartialView(jobDM);
        }

        /*************      UPDATE         ***********/

        [HttpPost]
        public ActionResult Details(JobDM model)
        {

            try
            { 

                _jobBL.Change(model);
                JobPicChanged(model);
                // PopulateClientDrop();
                ViewBag.PopTitle = "פרטי עבודה";
                return Json(new { JobID=model.JobID });
            }
            catch (Exception e)
            {
                return ExceptionObj(e);
            }


        }

        [HttpPost]
        public ActionResult UpdateJobQuote(int JobID, int? QuoteID)
        {

            try
            {

                _jobBL.UpdateJobQuote(JobID, QuoteID);
                return Json(new { });
            }
            catch (Exception e)
            {
                return ExceptionObj(e);
            }


        }

        [HttpPost]
        public ActionResult ChangeJobClient(int JobID, int ClientID)
        {

            try
            {

                _jobBL.ChangeJobClient(JobID, ClientID);

                return Json(new { });
            }
            catch (Exception e)
            {
                return ExceptionObj(e);
            }


        }
        [HttpPost]
        public ActionResult JobRequest(JobRequestDM model)
        {
            try
            {
                _jobRequestBL.Add(model);
                JobPicChanged(model);

                return Json(new { model.JobID, model.RefubrishDetailsDM.FirstPartID });
            }
            catch (Exception e)
            {
                return ExceptionObj(e);
            }
        }



        internal void JobPicChanged(JobDM model)
        {
            if (model.IsOutside)
                return;

            var path = PicHelper.JobPathPhys;

            /** no picture uploaded*/
            if (!Directory.Exists(path + model.TempId))
                return;

            if (model.TempId > 0)
                PicManager.ChangeFolder(path + model.TempId, path + model.JobID);

            var macId = model.RefubrishDetailsDM.MachineID;

            /** try set machine pic if not exit*/
            if (PicHelper.GetMacPicOrNull(macId) == null)
            {
                var mac = new Avatar(macId, "MachinePicture", "MacPic_");
                var jobMac = new Avatar(model.JobID, "Job", RefubrishStep.PreTestStep.ToString(), true)
                {
                    Number = 1
                };

                if (FileManager.Exists(jobMac.path))
                    FileManager.Copy(jobMac.path, mac.path);
            }
        }





    }



}
