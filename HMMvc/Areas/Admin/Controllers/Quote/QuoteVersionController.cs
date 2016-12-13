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
    public class QuoteVersionController : _BasicController
    {
      
        private readonly QuoteVersionBL _entityBL;
        private readonly DynamicFieldBL _dynamicFieldBL;

        public QuoteVersionController(
             [Dependency] DynamicFieldBL dynamicFieldBL,
            [Dependency]QuoteVersionBL entityBL,
            [Dependency] IGlobalManager globalManager)
        {
            _entityBL = entityBL;
            _dynamicFieldBL = dynamicFieldBL;
            _adminClientID = globalManager.GetAdminClientID();
        }

        private int _adminClientID;




        /// <summary>
        /// version page must have id already - created with quote OR duplicated
        /// </summary>
        /// <param name="id">QuoteVersionID</param>
        /// <returns></returns>
        public ActionResult Update(int id)
        {
            
            var model = _entityBL.GetSingleItemDM(id);
            // model.Terms = model.Terms ?? GlobalDM.GetTransStr("Quote_Terms");
            PopulateDrop(model);

            ViewBag.MobileTitle = model.QuoteDM.QuoteTitle;

            return PartialView(model);
        }

        //private static QuoteVersionDM CreateNewVersion()
        //{
        //    QuoteVersionDM model;
        //    model = new QuoteVersionDM
        //    {
        //        Version = 1,
        //        IsSelected = false,
        //        VersionDate = DateTime.Now,
        //        QuoteDM = new QuoteDM
        //        {
                    
        //        }

        //    };
        //    return model;
        //}


        [HttpPost, ValidateInput(false)]
        public ActionResult Update(QuoteVersionDM model)
        {
            try
            {
                _entityBL.Update(model);
                return Json(new { model.QuoteVersionID});
            }
            catch (Exception e)
            {
                return ExceptionObj( e);
            }

        }


        ///moved to export
        //public ActionResult Report(int id, bool disableHF = false)
        //{

        //    QuoteVersionDM model = _entityBL.GetSingleItemDM(id);
  
        //    ViewBag.disableHF = disableHF;
        //    ViewBag.ClientID = model.QuoteDM.ClientID;
         
        //    return PartialView(model);
        //}



        [HttpPost]
        public ActionResult Copy(int SrcVersionID)
        {
            try
            {
                var NewVersionID = _entityBL.Copy(SrcVersionID);
                return Json(new { NewVersionID });
            }
            catch (Exception e)
            {
                return ExceptionObj(e);
            }

        }

        [HttpPost]
        public ActionResult ImportFromQuote(int SrcVersionID, int DestVersionID)
        {
            try
            {
                _entityBL.ImportFromQuote(SrcVersionID, DestVersionID);

                return Json(new { msg = "success" });
            }
            catch (Exception e)
            {

                return ExceptionObj(e);
                //modeds.AddFormError("No class was selected.");
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


        private void PopulateDrop(QuoteVersionDM model)
        {
            //var clnts = _quoteBL.GetAllSrcs();
           // ViewBag.Srcs = new SelectList(clnts, "ClientID", "ClientName", mac.ClientParentID);

            ViewBag.Versions = new SelectList(_entityBL.GetItemsList(model.QuoteID), "QuoteVersionID", "VersionTitle", model.QuoteVersionID);

           
            ViewBag.poolFields = _dynamicFieldBL.GetPoolSelectFields();
            ViewBag.categoryItems = _dynamicFieldBL.GetAllDynamicSelectList();


        }





    }
}
