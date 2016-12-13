using BL;
using Common;
using Microsoft.Practices.Unity;
using System;
using System.Web.Mvc;

namespace MVC.Areas.Admin.Controllers
{
    public class JobTaskController : _BasicController
    {
      
        private readonly JobTaskBL _entityBL;
        private readonly LangBL _langBL;
        public JobTaskController(
               [Dependency]LangBL langBL,
            [Dependency]JobTaskBL entityBL)
        {
            _langBL = langBL;
            _entityBL = entityBL;
        }


    
        public ActionResult Index(JobTaskFilterDm filter)
        {

            _entityBL.GetItemsList(filter);
        
            return PartialView(filter);
        }


        public ActionResult Update(int? id, int? JobID)
        {
            JobTaskDM model;// 

            if (id.HasValue)
            {
                model = _entityBL.GetSingleItemDM((int)id);
                ViewBag.PopTitle = "עדכון משימה";
            }
            else
            {
                if (!JobID.HasValue)
                    throw new Exception("task needs jobId");

                model = new JobTaskDM {
                    JobID = (int)JobID,
                    JobDM = _entityBL.GetJobDetails((int)JobID)
                };
                ViewBag.PopTitle = "משימה חדשה";
            }

            // PopulateDrop(model);

           // ViewBag.Steps = new SelectList(_langBL.GetRefubrishSteps(), "PickListID", "TransStr", model.JobRefubrishStepID ?? (int)RefubrishStep.RepairStep);

            return PartialView(model);
        }


        [HttpPost]
        public ActionResult Update(JobTaskDM model)
        {
            try
            {
                _entityBL.Update(model);
                return Json(new { JobTaskID = model.JobTaskID});
            }
            catch (Exception e)
            {
                return ExceptionObj( e);
            }

        }

       

      
        [HttpPost]
        public ActionResult Delete(int id)
        {
            try
            {
                _entityBL.Delete(id);
                return Json(new { msg = "Success" });
            }
            catch (Exception e)
            {
                return ExceptionObj(e);
            }
        }


        private void PopulateDrop(JobTaskDM model)
        {
            //var clnts = _quoteBL.GetAllSrcs();
           // ViewBag.Srcs = new SelectList(clnts, "ClientID", "ClientName", mac.ClientParentID);
        }



       

    }
}
