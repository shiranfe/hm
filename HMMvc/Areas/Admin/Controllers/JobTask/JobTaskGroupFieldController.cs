using BL;
using Common;
using Microsoft.Practices.Unity;
using System;
using System.Web.Mvc;

namespace MVC.Areas.Admin.Controllers
{
    public class JobTaskGroupFieldController : _BasicController
    {
      
        private readonly JobTaskGroupFieldBL _entityBL;

        public JobTaskGroupFieldController(

            [Dependency]JobTaskGroupFieldBL entityBL)
        {
            _entityBL = entityBL;
        }


    
        public ActionResult Index(JobTaskGroupFieldFilterDm filter)
        {

            _entityBL.GetItemsList(filter);
        
            return PartialView(filter);
        }


        public ActionResult Update(int? id)
        {
            JobTaskGroupFieldDM model;// 

            if (id.HasValue)
            {
                model = _entityBL.GetSingleItemDM((int)id);
                ViewBag.PopTitle = "עדכון ";
            }
            else
            {
                model = new JobTaskGroupFieldDM {  };
                ViewBag.PopTitle = "חדשה";
            }

            PopulateDrop(model);


            return PartialView(model);
        }


        [HttpPost]
        public ActionResult Update(JobTaskGroupFieldDM model)
        {
            try
            {
                _entityBL.Update(model);
                return Json(new { JobTaskGroupFieldID = model.Id});
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


        private void PopulateDrop(JobTaskGroupFieldDM model)
        {
            //var clnts = _quoteBL.GetAllSrcs();
           // ViewBag.Srcs = new SelectList(clnts, "ClientID", "ClientName", mac.ClientParentID);
        }



       

    }
}
