using BL;
using Common;
using Microsoft.Practices.Unity;
using System;
using System.Web.Mvc;

namespace MVC.Areas.Admin.Controllers
{
    public class JobTaskFieldController : _BasicController
    {
      
        private readonly JobTaskFieldBL _entityBL;

        public JobTaskFieldController(

            [Dependency]JobTaskFieldBL entityBL)
        {
            _entityBL = entityBL;
        }


    
        public ActionResult Index(JobTaskFieldFilterDm filter)
        {

            _entityBL.GetItemsList(filter);
        
            return PartialView(filter);
        }


        public ActionResult Update(int? id)
        {
            JobTaskFieldDM model;// 

            if (id.HasValue)
            {
                model = _entityBL.GetSingleItemDM((int)id);
                ViewBag.PopTitle = "עדכון ";
            }
            else
            {
                model = new JobTaskFieldDM {  };
                ViewBag.PopTitle = "חדשה";
            }

            PopulateDrop(model);


            return PartialView(model);
        }


        [HttpPost]
        public ActionResult Update(JobTaskFieldDM model)
        {
            try
            {
                var view = _entityBL.Update(model);

                if (view == null)
                    return Json(new { Id = model.Id });

                return PartialView("FieldTmpl", view);
            }
            catch (Exception e)
            {
                return ExceptionObj(e);
            }

        }



        [HttpPost]
        public ActionResult Sort(int[] ids)
        {
            try
            {
                _entityBL.Sort(ids);

                //return RedirectToAction("DynamicFields");
                return Json(new { sts = "Success", });
            }
            catch (Exception e)
            {
                return ExceptionObj(e);

            }

        }

        [HttpPost]
        public ActionResult Delete(int[] ids)
        {
            try
            {
                _entityBL.Delete(ids);
                return Json(new { msg = "Success" });
            }
            catch (Exception e)
            {
                return ExceptionObj(e);
            }
        }


        private void PopulateDrop(JobTaskFieldDM model)
        {
            //var clnts = _quoteBL.GetAllSrcs();
           // ViewBag.Srcs = new SelectList(clnts, "ClientID", "ClientName", mac.ClientParentID);
        }



       

    }
}
