using BL;
using Common;
using Microsoft.Practices.Unity;
using System;
using System.Web.Mvc;

namespace MVC.Areas.Admin.Controllers
{
    public class JobTaskGroupController : _BasicController
    {
      
        private readonly JobTaskGroupBL _entityBL;

        public JobTaskGroupController(

            [Dependency]JobTaskGroupBL entityBL)
        {
            _entityBL = entityBL;
        }


    
        public ActionResult Index(JobTaskGroupFilterDm filter)
        {

            _entityBL.GetItemsList(filter);
        
            return PartialView(filter);
        }


        public ActionResult Update(int? id)
        {
            JobTaskGroupDM model;// 

            if (id.HasValue)
            {
                model = _entityBL.GetSingleItemDM((int)id);
                ViewBag.PopTitle = "עדכון ";
            }
            else
            {
                model = new JobTaskGroupDM {  };
                ViewBag.PopTitle = "חדשה";
            }

            PopulateDrop(model);


            return PartialView(model);
        }


        [HttpPost]
        public ActionResult Update(JobTaskGroupDM model)
        {
            try
            {
                _entityBL.Update(model);
                return Json(new { JobTaskGroupID = model.JobTaskGroupID});
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


        private void PopulateDrop(JobTaskGroupDM model)
        {
            //var clnts = _quoteBL.GetAllSrcs();
           // ViewBag.Srcs = new SelectList(clnts, "ClientID", "ClientName", mac.ClientParentID);
        }



       

    }
}
