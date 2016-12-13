using BL;
using Common;
using Microsoft.Practices.Unity;
using MvcBlox.Models;
using System;
using System.Globalization;
using System.Web.Mvc;

namespace MVC.Areas.Admin.Controllers
{
    public class QuoteTalkController : _BasicController
    {
      
        private readonly QuoteTalkBL _entityBL;
        private readonly IGlobalManager _globalManager;

        public QuoteTalkController(

            [Dependency]QuoteTalkBL entityBL,
            [Dependency] IGlobalManager globalManager)
        {
            _globalManager = globalManager;
            _entityBL = entityBL;
           
            _employeeId = _globalManager.GetEmpID();
        }

     
        private readonly int _employeeId;

        public ActionResult Index(int id)
        {

            var model = _entityBL.GetItemsList(id);

            return PartialView(model);
        }


        public ActionResult Update(int? id)
        {
            QuoteTalkDM model;

            if (id.HasValue)
            {
                model = _entityBL.GetSingleItemDM((int)id);
                ViewBag.PopTitle = "עדכון הצעה";
            }
            else
            {
                model = new QuoteTalkDM {  };
                ViewBag.PopTitle = "הצעה חדשה";
            }

            PopulateDrop(model);


          
            return PartialView(model);
        }


        [HttpPost]
        public ActionResult Update(QuoteTalkDM model)
        {
            try
            {
                model.CreatorID = _employeeId;
                var ans = _entityBL.Update(model);
                return Json(new  {
                    ans.Creator,
                    ans.QuoteTalkID,ans.Message,
                    TalkDate =ans.TalkDate.ToString("dd/MM/yy H:mm", CultureInfo.CurrentCulture)});
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
                return Json(new { id = id , deleted=true });
            }
            catch (Exception e)
            {
                return ExceptionObj(e);
            }
        }


        private void PopulateDrop(QuoteTalkDM model)
        {
            //var clnts = _quoteBL.GetAllSrcs();
           // ViewBag.Srcs = new SelectList(clnts, "ClientID", "ClientName", mac.ClientParentID);
        }



       

    }
}
