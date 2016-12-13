using BL;
using Common;
//using HMErp.Helper;
using Microsoft.Practices.Unity;
using MVC.Areas.Admin.Models;
using MvcBlox.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace MVC.Areas.Admin.Controllers
{
    public class MachinePartController : _BasicController
    {
        private LangBL _langBL;  
        private readonly MachinePartBL _machinePartBL;
        private ClientBL _clientBL;
        private readonly IGlobalManager _globalManager;

        public MachinePartController(
            [Dependency]LangBL langBL,
          
            [Dependency]MachinePartBL machinePartBL, 
            [Dependency]ClientBL clientBL, 
            [Dependency] IGlobalManager globalManager)
        {
            _globalManager = globalManager;
            _langBL = langBL;
         
            _clientBL = clientBL;
            _machinePartBL = machinePartBL;
            AdminClientID = _globalManager.GetAdminClientID();
        }

        private int AdminClientID;



        /*************   GET  ***********/

 
        public ActionResult Index(int MachineID)
        {
            var model = _machinePartBL.GetItemsList(MachineID);
            return PartialView(model);
        }


        /*************      ADD        ***********/


        public ActionResult Update(int? id, int? ForeignID)
        {

            MachinePartDM model;// 

            if (id.HasValue)
            {
                model = _machinePartBL.GetSingleItemDM((int)id);
                ViewBag.PopTitle = "עדכון חלק";
            }
            else
            {
                model = new MachinePartDM
                {
                    MachineTypeID = 1,
                    MachineID = (int)ForeignID,
                    Groups = new List<StepGroupDM>()
                };
                ViewBag.PopTitle = "הוספת חלק";
            }

            PopulateDrop(model);


            return PartialView(model);

        }

        private void PopulateDrop(MachinePartDM model)
        {
            //var types = _langBL.GetPickListValue("MachineType");
            //ViewBag.MachineType = new SelectList(types, "PickListID", "TransStr", model.MachineTypeID);
        }

        [HttpPost]
        public ActionResult Update(FormCollection formCollection, MachinePartDM model)
        {
            try
            {
                var fields = DynamicFieldsHelper.collectionToFields(formCollection);
                _machinePartBL.Edit(model, fields);

                return Json(new { msg = "Success" });
            }
            catch (Exception e)
            {

                return ExceptionObj(e);
                //modeds.AddFormError("No class was selected.");
            }

        }

        [HttpPost]
        public ActionResult Add(MachinePartDM model)
        {
            try
            {
                var MachinePartID = _machinePartBL.Add(model);

                return Json(new { msg = "Success", MachinePartID=MachinePartID });
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
                _machinePartBL.Delete(id);

                return Json(new { msg = "Success" });
            }
            catch (Exception e)
            {

                return ExceptionObj(e);
            }
        }

   
    }
}
