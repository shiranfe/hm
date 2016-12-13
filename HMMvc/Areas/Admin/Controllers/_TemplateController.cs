using BL;
using Common;
using Microsoft.Practices.Unity;
using System;
using System.Web.Mvc;

namespace MVC.Areas.Admin.Controllers
{
    public class _TemplateController : _BasicController
    {
      
        private readonly _TemplateBL _entityBL;

        public _TemplateController(

            [Dependency]_TemplateBL entityBL)
        {
            _entityBL = entityBL;
        }


    
        public ActionResult Index(_TemplateFilterDm filter)
        {

            _entityBL.GetItemsList(filter);
        
            return PartialView(filter);
        }


        public ActionResult Update(int? id)
        {
            _TemplateDM model;// 

            if (id.HasValue)
            {
                model = _entityBL.GetSingleItemDM((int)id);
                ViewBag.PopTitle = "עדכון ";
            }
            else
            {
                model = new _TemplateDM {  };
                ViewBag.PopTitle = "חדשה";
            }

            PopulateDrop(model);


            return PartialView(model);
        }


        [HttpPost]
        public ActionResult Update(_TemplateDM model)
        {
            try
            {
                _entityBL.Update(model);
                return Json(new { _TemplateID = model._TemplateID});
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


        private void PopulateDrop(_TemplateDM model)
        {
            //var clnts = _quoteBL.GetAllSrcs();
           // ViewBag.Srcs = new SelectList(clnts, "ClientID", "ClientName", mac.ClientParentID);
        }



       

    }
}
