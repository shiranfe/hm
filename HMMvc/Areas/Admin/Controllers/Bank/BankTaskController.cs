using BL;
using Common;
using Microsoft.Practices.Unity;
using System;
using System.Web.Mvc;

namespace MVC.Areas.Admin.Controllers
{
    public class BankTaskController : _BasicController
    {
      
        private readonly BankTaskBL _entityBL;
        private readonly BankFieldBL _bankFieldBL;

        public BankTaskController(
             [Dependency]BankFieldBL bankFieldBL,
            [Dependency]BankTaskBL entityBL)
        {
            _entityBL = entityBL;
            _bankFieldBL = bankFieldBL;
        }


    
        public ActionResult Index(BankTaskFilterDm filter)
        {

            _entityBL.GetItemsList(filter);
        
            return PartialView(filter);
        }


        public ActionResult Update(int? id)
        {
            BankTaskDM model;// 

            if (id.HasValue)
            {
                model = _entityBL.GetSingleItemDM((int)id);
                ViewBag.PopTitle = "עדכון ";
            }
            else
            {
                model = new BankTaskDM();
                ViewBag.PopTitle = "חדשה";
            }

            PopulateDrop(model);


            return PartialView(model);
        }


        [HttpPost]
        public ActionResult Update(BankTaskDM model)
        {
            try
            {
                _entityBL.Update(model);
                return Json(new { BankTaskID = model.BankTaskID});
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


        private void PopulateDrop(BankTaskDM model)
        {
            ViewBag.BankFields = new SelectList(_bankFieldBL.GetDrop(), "id", "Text");
            //var clnts = _quoteBL.GetAllSrcs();
            // ViewBag.Srcs = new SelectList(clnts, "ClientID", "ClientName", mac.ClientParentID);
        }



       

    }
}
