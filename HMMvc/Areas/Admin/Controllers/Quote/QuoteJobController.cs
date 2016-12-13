using BL;
using Common;
//using HMErp.Helper;
using Microsoft.Practices.Unity;
using MvcBlox.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace MVC.Areas.Admin.Controllers
{
    public class QuoteJobController : _BasicController
    {
      
        private readonly QuoteJobBL _entityBL;
        private readonly IGlobalManager _globalManager;
        private readonly RefubrishBL _refubrishBL;

        public QuoteJobController(

            [Dependency]QuoteJobBL entityBL,
            [Dependency] RefubrishBL refubrishBL,
            [Dependency] IGlobalManager globalManager)
        {
            _globalManager = globalManager;
            _entityBL = entityBL;
            _refubrishBL=refubrishBL;
            AdminClientID = _globalManager.GetAdminClientID();
        }

        private int AdminClientID;



    
        public ActionResult Index(int QuoteID)
        {

            try
            {
                var model = _entityBL.GetItemsList(QuoteID);
              
                return PartialView(model);
            }
            catch (Exception e)
            {
                return ExceptionObj(e);
            }
        }

      
        public ActionResult JobDetails(int JobID)
        {
            try
            {
                var model = _refubrishBL.GetJobDetails(JobID);
                ViewBag.PopTitle = "פרטי עבודה";
                return PartialView(model);
            }
            catch (Exception e)
            {
                return ExceptionObj(e);
            }
          
           
        }
        //public ActionResult Update(int id, int? QuoteID)
        //{
        //    QuoteJobDM model;// 

        //    if (id.HasValue)
        //    {
        //        model = _entityBL.GetSingleItemDM((int)id);
        //        ViewBag.PopTitle = "עדכון הצעה";
        //    }
        //    else
        //    {
        //        model = new QuoteJobDM {  };
        //        ViewBag.PopTitle = "הצעה חדשה";
        //    }

        //    PopulateDrop(model);


        //    return PartialView(model);
        //}


        [HttpPost]
        public ActionResult Update(int JobID, int? QuoteID)
        {
            try
            {
                _entityBL.Update(JobID, QuoteID);
                return Json(new { });
            }
            catch (Exception e)
            {
                return ExceptionObj( e);
            }

        }


        
      
        //[HttpPost]
        //public ActionResult Delete(int id)
        //{
        //    try
        //    {
        //        _entityBL.Delete(id);
        //        return Json(new { msg = "Success" });
        //    }
        //    catch (Exception e)
        //    {
        //        return ExceptionObj(e);
        //    }
        //}


        private void PopulateDrop(QuoteJobDM model)
        {
            //var clnts = _quoteBL.GetAllSrcs();
           // ViewBag.Srcs = new SelectList(clnts, "ClientID", "ClientName", mac.ClientParentID);
        }



       

    }
}
