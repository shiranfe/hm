using BL;
using Common;
using Microsoft.Practices.Unity;
using MvcBlox.Models;
using System;
using System.Web.Mvc;

namespace MVC.Areas.Admin.Controllers
{
    public class QuoteVersionItemController : _BasicController
    {
      
        private readonly QuoteVersionItemBL _entityBL;
        private readonly IGlobalManager _globalManager;
        private readonly DynamicFieldBL _dynamicFieldBL;

        public QuoteVersionItemController(
            [Dependency] DynamicFieldBL dynamicFieldBL,
            [Dependency]QuoteVersionItemBL entityBL,
            [Dependency] IGlobalManager globalManager)
        {
            _globalManager = globalManager;
            _entityBL = entityBL;
            _dynamicFieldBL = dynamicFieldBL;
            AdminClientID = _globalManager.GetAdminClientID();
        }

        private int AdminClientID;




        public ActionResult Index(int QuoteVersionID)
        {

            var model = _entityBL.GetItemsList(QuoteVersionID);
            PopulateDrop();
            return PartialView(model);
        }


        //public ActionResult Update(int? id)
        //{
        //    QuoteVersionItemDM model;// 

        //    if (id.HasValue)
        //    {
        //        model = _entityBL.GetSingleItemDM((int)id);
        //        ViewBag.PopTitle = "עדכון הצעה";
        //    }
        //    else
        //    {
        //        model = new QuoteVersionItemDM {  };
        //        ViewBag.PopTitle = "הצעה חדשה";
        //    }

        //    PopulateDrop(model);


        //    return PartialView(model);
        //}


        [HttpPost]
        public ActionResult Update(QuoteVersionItemDM model)
        {
            try
            {
                if (model.FieldPoolID == 0)
                {
                    model.FieldPoolID = null;
                    model.FieldValue = null;
                }
                   

                _entityBL.Update(model);
                PopulateDrop();
                // return Json(new { model.QuoteVersionItemID});
                return PartialView(model);
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


        private void PopulateDrop()
        {
            ViewBag.poolFields = _dynamicFieldBL.GetPoolSelectFields();
            ViewBag.categoryItems = _dynamicFieldBL.GetAllDynamicSelectList();
            //var clnts = _quoteBL.GetAllSrcs();
            // ViewBag.Srcs = new SelectList(clnts, "ClientID", "ClientName", mac.ClientParentID);
        }





    }
}
