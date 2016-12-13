using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using BL;
using Common;
using Microsoft.Practices.Unity;
using MvcBlox.Models;

namespace MVC.Areas.Admin.Controllers
{
    public class AlignmentController : _BasicController
    {
        private readonly JobAlignmentBL _entityBL;
        private readonly int _adminClientId;

        public AlignmentController(
            [Dependency] JobAlignmentBL entityBL,
            [Dependency] IGlobalManager globalManager)
        {
            _entityBL = entityBL;
            _adminClientId = globalManager.GetAdminClientID();
        }

        public ActionResult Index()
        {
            var model = _entityBL.GetItemsList(_adminClientId);

            ViewBag.ClientName = (_adminClientId > 0 && model.Any()) ? model.First().JobDM.ClientName : "כל המפעלים";

            return PartialView(model);
        }

        public ActionResult MachineJobs(int id)
        {

            var model = _entityBL.GetMachineJobsHistory(id);
            ViewBag.MachineID = id;
            return PartialView(model);

        }

        /// UPDATING in JOBctrl
        public ActionResult Update(int PartID)
        {
            JobAlignmentDM model; // 

            ViewBag.PopTitle = "עדכון הצעה";

            model = _entityBL.GetJobPartCard(PartID);
            model.FirstPartID = PartID;
            ViewBag.Parts = new SelectList(model.Parts, "id", "MachineTypeLangStr", PartID);


            return PartialView(model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Update(JobAlignmentDM model)
        {
            try
            {
            
                _entityBL.Update(model);
                return Json(new {model.JobID});
            }
            catch (Exception e)
            {
                return ExceptionObj(e);
            }
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            try
            {
                _entityBL.Delete(id);
                return Json(new {msg = "Success"});
            }
            catch (Exception e)
            {
                return ExceptionObj(e);
            }
        }
    }
}