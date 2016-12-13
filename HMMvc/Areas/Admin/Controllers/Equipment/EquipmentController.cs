using BL;
using Common;
using Microsoft.Practices.Unity;
using System;
using System.Web.Mvc;

namespace MVC.Areas.Admin.Controllers
{
    public class EquipmentController : _BasicController
    {
      
        private readonly EquipmentBL _entityBL;

        public EquipmentController( [Dependency]EquipmentBL entityBL)
        {
            _entityBL = entityBL;
        }


    
        public ActionResult Index(EquipmentFilterDm filter)
        {

            _entityBL.GetItemsList(filter);
        
            return PartialView(filter);
        }


        public ActionResult Update(int? id)
        {
            EquipmentDM model;// 

            if (id.HasValue)
            {
                model = _entityBL.GetSingleItemDM((int)id);
                ViewBag.PopTitle = "עדכון ";
            }
            else
            {
                model = new EquipmentDM {  };
                ViewBag.PopTitle = "חדשה";
            }

            PopulateDrop(model);


            return PartialView(model);
        }


        [HttpPost]
        public ActionResult Update(EquipmentDM model)
        {
            try
            {
                _entityBL.Update(model);
                return Json(new { EquipmentID = model.EquipmentID});
            }
            catch (Exception e)
            {
                return ExceptionObj( e);
            }

        }

        //[HttpPost]
        //public ActionResult QuickAdd(EquipmentDM model)
        //{
        //    try
        //    {
        //        _entityBL.QuickAdd(model);
        //        return Json(model);
        //    }
        //    catch (Exception e)
        //    {
        //        return ExceptionObj( e);
        //    }

        //}

      
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


        private void PopulateDrop(EquipmentDM model)
        {
            //var clnts = _quoteBL.GetAllSrcs();
           // ViewBag.Srcs = new SelectList(clnts, "ClientID", "ClientName", mac.ClientParentID);
        }



       

    }
}
