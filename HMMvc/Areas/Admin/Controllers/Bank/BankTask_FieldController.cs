using BL;
using Common;
using Microsoft.Practices.Unity;
using System;
using System.Web.Mvc;

namespace MVC.Areas.Admin.Controllers
{
    public class BankTask_FieldController : _BasicController
    {
      
        private readonly BankTask_FieldBL _entityBL;

        public BankTask_FieldController(

            [Dependency]BankTask_FieldBL entityBL)
        {
            _entityBL = entityBL;
        }


    
        public ActionResult Index(BankTask_FieldFilterDm filter)
        {

            _entityBL.GetItemsList(filter);
        
            return PartialView(filter);
        }


        public ActionResult Update(int? id)
        {
            BankTask_FieldDM model;// 

            if (id.HasValue)
            {
                model = _entityBL.GetSingleItemDM((int)id);
                ViewBag.PopTitle = "עדכון ";
            }
            else
            {
                model = new BankTask_FieldDM {  };
                ViewBag.PopTitle = "חדשה";
            }

            PopulateDrop(model);


            return PartialView(model);
        }


        [HttpPost]
        public ActionResult Update(BankTask_FieldDM model)
        {
            try
            {
                var view= _entityBL.Update(model);

                if(view==null)
                    return Json(new { BankTask_FieldID = model.BankTask_FieldID });

                return PartialView("FieldTmpl", view);
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


        private void PopulateDrop(BankTask_FieldDM model)
        {
            //var clnts = _quoteBL.GetAllSrcs();
           // ViewBag.Srcs = new SelectList(clnts, "ClientID", "ClientName", mac.ClientParentID);
        }



       

    }
}
