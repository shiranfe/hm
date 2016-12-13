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
    public class ManufacturerController : _BasicController
    {
      
        private readonly ManufacturerBL _manufacturerBL;
        private readonly IGlobalManager _globalManager;

        public ManufacturerController(
            [Dependency]ManufacturerBL manufacturerBL,
            [Dependency] IGlobalManager globalManager)
        {
            _globalManager = globalManager;
           
            _manufacturerBL = manufacturerBL;
            AdminClientID = _globalManager.GetAdminClientID();
        }

        private int AdminClientID;



        public ActionResult Index()
        {

            var model = _manufacturerBL.GetIndex();

            return PartialView(model);
        }    

        public ActionResult Update(int? id)
        {
            ClientDM model;// 

            if (id.HasValue)
            {
                model =_manufacturerBL.GetSingleDM((int)id);
                ViewBag.PopTitle = "עדכון יצרן";
            }
            else
            {
                model = new ClientDM { ClientID = 0 };
                ViewBag.PopTitle = "יצרן חדש";
            }

            PopulateDrops(model);

            return PartialView(model);
        }


        [HttpPost]
        public ActionResult Update(ClientDM model)
        {
            try
            {
                _manufacturerBL.Update(model);

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
                _manufacturerBL.Delete(id);

                return Json(new { msg = "Success" });
            }
            catch (Exception e)
            {
                return ExceptionObj(e);

            }
              

        }


        private void PopulateDrops(ClientDM model)
        {
            var clnts = _manufacturerBL.GetIndex().Where(x => x.ClientID != model.ClientID).ToList();
            ViewBag.Client = new SelectList(clnts, "ClientID", "ClientName", model.ClientParentID);
        }


      
    }
}
