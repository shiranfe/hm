using System;
using System.Linq;
using System.Web.Mvc;
using BL;
using Common;
using Microsoft.Practices.Unity;
using System.Collections.Generic;
//using HMErp.Helper;

namespace MVC.Areas.Admin.Controllers
{
    public class DynamicFieldsController : _BasicController
    {
        private readonly DynamicFieldBL _dynamicFieldBL;
        private readonly LangBL _langBL;
        private readonly CatalogBL _catatlogBL;

        public DynamicFieldsController(
            [Dependency] LangBL langBL,
            [Dependency]CatalogBL catatlogBL,
            [Dependency] DynamicFieldBL dynamicFieldBL)
        {
            _langBL = langBL;
            _dynamicFieldBL = dynamicFieldBL;
            _catatlogBL = catatlogBL;
        }




        /*************   Select  ***********/


        public ActionResult Index(int FieldPoolID)
        {
            var model = _dynamicFieldBL.GetDynamicSelectList(FieldPoolID);
            return Json(new { model }, JsonRequestBehavior.AllowGet);
        }

        /*************      ADD        ***********/

        [HttpPost]
        public ActionResult Sort(DynamicGroupDM model)
        {
            try
            {
                _dynamicFieldBL.SortFields(model);

                //return RedirectToAction("DynamicFields");
                return Json(new { sts = "Success", });
            }
            catch (Exception e)
            {
                return ExceptionObj(e);
            }

        }

        [HttpPost]
        public ActionResult Copy(DynamicGroupDM model, bool IsCut)
        {
            try
            {
                _dynamicFieldBL.CopyFields(model, IsCut);

                //return RedirectToAction("DynamicFields");
                return Json(new { sts = "Success", });
            }
            catch (Exception e)
            {
                return ExceptionObj(e);
            }

        }


        /*************      UPDATE        ***********/




        public ActionResult Update(int? FieldID, int? GroupID)
        {
            try
            {
                

                var model = (FieldID.HasValue)
                    ? _dynamicFieldBL.GetField((int)FieldID)
                    : IntiateFieldModel((int)GroupID);


                PopulateViewBag(model);
                // model.HebStrings = SiteGlobals.Dict.Select(x => new PickListDM { Value = x.Key, Text = x.IL }).ToList();


                return PartialView(model);
            }
            catch (Exception e)
            {
                return ExceptionObj(e);
            }

        }

   

        private void PopulateViewBag(DynamicGroupFieldDM model)
        {
            var poolFields = _dynamicFieldBL.GetPoolFields();
            ViewBag.poolFields = new SelectList(poolFields, "FieldPoolID", "FieldLabel", 29);

            ViewBag.catatlogItems = new SelectList(_catatlogBL.GetDropItems(), "CatalogItemID", "ItemName", model.CatalogItemID);

   
        }

        [HttpPost]
        public ActionResult Update(DynamicGroupFieldDM model)
        {
            try
            {
                if (model.CatalogItemID == 0)
                    model.CatalogItemID = null;

                _dynamicFieldBL.ChangeField(model);

                //return RedirectToAction("DynamicFields");
                return Json(new { sts = "Success" });
            }
            catch (Exception e)
            {
                return ExceptionObj(e);
            }

        }

        private DynamicGroupFieldDM IntiateFieldModel(int GroupID)
        {
            return new DynamicGroupFieldDM
            {
                DynamicGroupID = GroupID,
                OrderVal = 1,
                IsRequired = false,
                IsForQuote = false
            };
        }

        /*************      DELETE        ***********/

        [HttpPost]
        public ActionResult Delete(int[] ids)
        {
            try
            {
                _dynamicFieldBL.DeleteFields(ids);

                return Json(new { sts = "Success" });
            }
            catch (Exception e)
            {
                return ExceptionObj(e);
            }
        }



    }
}