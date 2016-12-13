using BL;
using Common;
using Microsoft.Practices.Unity;
using System;
using System.Web.Mvc;

namespace MVC.Areas.Admin.Controllers
{
    public class JobEquipmentController : _BasicController
    {
      
        private readonly JobEquipmentBL _entityBL;
        private readonly EquipmentBL _equipmentBL;
        private readonly LangBL _langBL;

        public JobEquipmentController(
            [Dependency]EquipmentBL equipmentBL,
                 [Dependency]LangBL langBL,
            [Dependency]JobEquipmentBL entityBL)
        {
            _entityBL = entityBL;
            _equipmentBL = equipmentBL;
            _langBL = langBL;
        }



        public ActionResult Index(JobDM model)
        {

            //_entityBL.GetItemsList(filter);
            ViewBag.MachineType = new SelectList(_langBL.GetPickListValue(nameof(MachineType)), "PickListID", "TransStr");

            return PartialView(model);
        }


        public ActionResult Update(int? id)
        {
            JobEquipmentDM model;// 

            if (id.HasValue)
            {
                model = _entityBL.GetSingleItemDM((int)id);
                ViewBag.PopTitle = "עדכון ";
            }
            else
            {
                model = new JobEquipmentDM();
                ViewBag.PopTitle = "חדשה";
            }

            PopulateDrop(model);


            return PartialView(model);
        }


        [HttpPost]
        public ActionResult Add(JobEquipmentDM model)
        {
            try 
            { 
                if (model.EquipmentID == 0)
                {
                    _equipmentBL.Update(model.EquipmentDM);
                    model.EquipmentID = model.EquipmentDM.EquipmentID;
                }

                _entityBL.Update(model);
        
                return Json(new { JobEquipmentID =model.JobEquipmentID, EquipmentName=model.EquipmentDM.EquipmentName });
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


        private void PopulateDrop(JobEquipmentDM model)
        {
            //var clnts = _quoteBL.GetAllSrcs();
           // ViewBag.Srcs = new SelectList(clnts, "ClientID", "ClientName", mac.ClientParentID);
        }



       

    }
}
