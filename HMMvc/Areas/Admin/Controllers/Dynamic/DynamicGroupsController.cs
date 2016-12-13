using System;
using System.Linq;
using System.Web.Mvc;
using BL;
using Common;
using Microsoft.Practices.Unity;
//using HMErp.Helper;

namespace MVC.Areas.Admin.Controllers
{
    public class DynamicGroupsController : _BasicController
    {
        private readonly DynamicFieldBL _dynamicFieldBL;
        private readonly LangBL _langBL;

        public DynamicGroupsController(
            [Dependency] LangBL langBL,
            [Dependency] DynamicFieldBL dynamicFieldBL)
        {
            _langBL = langBL;
            _dynamicFieldBL = dynamicFieldBL;
        }




        /*************   Select  ***********/
        //DynamicFields - get page
        public ActionResult Page()
        {
            var types = _langBL.GetPickListValue("MachineType");
            ViewBag.MachineType = new SelectList(types, "PickListID", "TransStr", types.First().PickListID);

            var steps = _dynamicFieldBL.GetAllSteps();

            ViewBag.Steps = new SelectList(steps, "Value", "Text", steps.First().Value);

            //ViewBag.allPossibleFields = _dynamicFieldBL.GetAllPossibleFields();

            return PartialView();
        }

        //DynamicFieldsList get groups
        public ActionResult Index(DynamicEditDM dynamicEditDM)
        {
            var model = _dynamicFieldBL.DynamicFieldsList(dynamicEditDM);

          
            return PartialView("StepGroups", model);
        }

   

        /*************      ADD        ***********/

        [HttpPost]
        public ActionResult Add(DynamicEditDM model)
        {
            _dynamicFieldBL.AddPhase(model);

            //return RedirectToAction("DynamicFields");
            return Json(new {sts = "Success"});
        }


        [HttpPost]
        public ActionResult Copy(DynamicGroupDM model, bool IsCut)
        {
            _dynamicFieldBL.CopyGroups(model, IsCut);

            //return RedirectToAction("DynamicFields");
            return Json(new {sts = "Success"});
        }

     


        /*************      UPDATE        ***********/

      


        public ActionResult Update(DynamicGroupDM model, int? GroupID)
        {
            if (GroupID.HasValue)
                model = new DynamicGroupDM {DynamicGroupID = (int) GroupID};

            model = _dynamicFieldBL.LoadChangeGroup(model);


            return PartialView(model);
        }


        [HttpPost]
        public ActionResult Update(DynamicGroupDM model)
        {
            try
            {
                _dynamicFieldBL.ChangeGroup(model);


                //return RedirectToAction("DynamicFields");
                return Json(new {sts = "Success"});
            }
            catch (Exception e)
            {
                return ExceptionObj(e);
            }
        }


        [HttpPost]
        public ActionResult Sort(int[] ItemsSelected)
        {
            try
            {
                _dynamicFieldBL.SortGroups(ItemsSelected);

                //return RedirectToAction("DynamicFields");
                return Json(new { sts = "Success", });
            }
            catch (Exception e)
            {
                return ExceptionObj(e);

            }
         
        }

        /*************      DELETE        ***********/

        [HttpPost]
        public ActionResult Delete(int[] ids)
        {
            try
            {
                _dynamicFieldBL.DeleteGroups(ids);

                return Json(new {sts = "Success"});
            }
            catch (Exception e)
            {
                return ExceptionObj(e);
               
            }
        }

     
    }
}