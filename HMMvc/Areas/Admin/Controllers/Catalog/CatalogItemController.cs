using BL;
using Common;
//using HMErp.Helper;
using Microsoft.Practices.Unity;
using System;
using System.Web.Mvc;

namespace MVC.Areas.Admin.Controllers
{
    public class CatalogItemController : _BasicController
    {
      
        private readonly CatalogBL _catalogBL;
private readonly DynamicFieldBL _dynamicFieldBL;

        public CatalogItemController(
          [Dependency] DynamicFieldBL dynamicFieldBL,
            [Dependency]CatalogBL catalogBL)
        {         
            _catalogBL = catalogBL;
            _dynamicFieldBL = dynamicFieldBL;
        }

       


    
        public ActionResult Index(CatalogFilterDm filter)
        {

            _catalogBL.GetItemsList(filter);
        
            return PartialView(filter);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ItemTitle">if was opened from quote and tries to add new item</param>
        /// <returns></returns>
        public ActionResult Update(int? id, string ItemTitle)
        {
            CatalogItemDM model;// 

            if (id.HasValue)
            {
                model = _catalogBL.GetSingleItemDM((int)id);
                ViewBag.PopTitle = "עדכון פריט";
            }
            else
            {
                model = new CatalogItemDM { 
                    ItemName = ItemTitle
                };
                ViewBag.PopTitle = "פריט חדש";
            }

            PopulateDrop(model);


            return PartialView(model);
        }


        [HttpPost]
        public ActionResult Update(CatalogItemDM model)
        {
            try
            {
                var ent= _catalogBL.Update(model);

                return Json(ent);
            }
            catch (Exception e)
            {
           
                return ExceptionObj( e);
                //modeds.AddFormError("No class was selected.");
            }

        }

       

      
        [HttpPost]
        public ActionResult Delete(int id)
        {

            try
            {
                _catalogBL.Delete(id);

                return Json(new { msg = "Success" });
            }
            catch (Exception e)
            {

                return ExceptionObj(e);
            }
        }


        private void PopulateDrop(CatalogItemDM model)
        {
            var poolFields = _dynamicFieldBL.GetPoolSelectFields(false);
            ViewBag.poolFields = new SelectList(poolFields, "FieldPoolID", "FieldLabel", model.FieldPoolID);

            //var clnts = _catalogBL.GetAllSrcs();
            // ViewBag.Srcs = new SelectList(clnts, "ClientID", "ClientName", mac.ClientParentID);
        }





    }
}
