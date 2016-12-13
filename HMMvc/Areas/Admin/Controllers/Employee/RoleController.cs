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
    public class RoleController : _BasicController
    {
      
        private readonly RoleBL _entityBL;
        private readonly IGlobalManager _globalManager;

        public RoleController(

            [Dependency]RoleBL entityBL,
            [Dependency] IGlobalManager globalManager)
        {
            _globalManager = globalManager;
            _entityBL = entityBL;
            AdminClientID = _globalManager.GetAdminClientID();
        }

        private int AdminClientID;



    
        public ActionResult Index()
        {

            var model = _entityBL.GetItemsList();
        
            return PartialView(model);
        }


        public ActionResult Update(int? id)
        {
            RoleDM model;// 

            if (id.HasValue)
            {
                model = _entityBL.GetSingleItemDM((int)id);
                ViewBag.PopTitle = "עדכון תפקיד";
            }
            else
            {
                model = new RoleDM {  };
                ViewBag.PopTitle = "תפקיד חדש";
            }

            PopulateDrop(model);


            return PartialView(model);
        }


        [HttpPost]
        public ActionResult Update(RoleDM model)
        {
            try
            {
                _entityBL.Update(model);
                return Json(new { RoleID = model.RoleID});
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


        private void PopulateDrop(RoleDM model)
        {
            //var clnts = _quoteBL.GetAllSrcs();
           // ViewBag.Srcs = new SelectList(clnts, "ClientID", "ClientName", mac.ClientParentID);
        }



       

    }
}
