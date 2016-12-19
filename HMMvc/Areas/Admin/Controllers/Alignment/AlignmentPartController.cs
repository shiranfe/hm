using BL;
using Common;
using Microsoft.Practices.Unity;
using MvcBlox.Models;
using System;
using System.Web.Mvc;

namespace MVC.Areas.Admin.Controllers
{
    public class AlignmentPartController : _BasicController
    {
      
        private readonly JobAlignmentPartBL _entityBL;
        private readonly IGlobalManager _globalManager;

        public AlignmentPartController(

            [Dependency]JobAlignmentPartBL entityBL,
            [Dependency] IGlobalManager globalManager)
        {
            _globalManager = globalManager;
            _entityBL = entityBL;
            AdminClientID = _globalManager.GetAdminClientID();
        }

        private int AdminClientID;



    
        //public ActionResult Index(int JobID)
        //{

        //    var model = _entityBL.GetItemsList(JobID);
        
        //    return PartialView(model);
        //}


        public ActionResult Update(int id)
        {
            JobAlignmentPartDM model;// 

           
                model = _entityBL.GetSingleItemDM(id);
                ViewBag.PopTitle = "עדכון הצעה";
          
          
            PopulateDrop(model);


            return PartialView(model);
        }


        [HttpPost]
        public ActionResult Update(JobAlignmentPartDM model)
        {
            try
            {
                _entityBL.Update(model);
                return Json(new { JobAlignmentPartID = model.JobAlignmentPartID});
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


        private static void PopulateDrop(JobAlignmentPartDM model)
        {
            //var clnts = _quoteBL.GetAllSrcs();
           // ViewBag.Srcs = new SelectList(clnts, "ClientID", "ClientName", mac.ClientParentID);
        }



       

    }
}
