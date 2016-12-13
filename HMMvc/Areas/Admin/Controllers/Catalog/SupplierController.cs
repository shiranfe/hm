using BL;
using Common;

using Microsoft.Practices.Unity;
using MvcBlox.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace MVC.Areas.Admin.Controllers
{
    public class SupplierController : _BasicController
    {
      
        private readonly SupplierBL _supplierBL;
        private readonly IGlobalManager _globalManager;

        public SupplierController(
            [Dependency]SupplierBL supplierBL,
            [Dependency] IGlobalManager globalManager)
        {
            _globalManager = globalManager;
           
            _supplierBL = supplierBL;
            AdminClientID = _globalManager.GetAdminClientID();
        }

        private int AdminClientID;



        public ActionResult Index()
        {

            var model = _supplierBL.GetIndex();

            return PartialView(model);
        }    

        public ActionResult Update(int? id)
        {
            ClientDM model;// 

            if (id.HasValue)
            {
                model =_supplierBL.GetSingleDM((int)id);
                ViewBag.PopTitle = "עדכון ספק";
            }
            else
            {
                model = new ClientDM { ClientID = 0 };
                ViewBag.PopTitle = "ספק חדש";
            }

            PopulateDrops(model);

            return PartialView(model);
        }


        [HttpPost]
        public ActionResult Update(ClientDM model)
        {
            try
            {
                _supplierBL.Update(model);

                return RedirectToAction("Index");
            }
            catch (Exception e)
            {        
                return ExceptionObj(e);
            }
            
        }


        [HttpPost]
        public ActionResult Delete(int id)
        {
            try
            {
                _supplierBL.Delete(id);

                return Json(new { msg = "Success" });
            }
            catch (Exception e)
            {
                return ExceptionObj(e);

            }
              

        }


        private void PopulateDrops(ClientDM model)
        {
            var clnts = _supplierBL.GetIndex().Where(x => x.ClientID != model.ClientID).ToList();
            ViewBag.Client = new SelectList(clnts, "ClientID", "ClientName", model.ClientParentID);
        }


      
    }
}
