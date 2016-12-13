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
    public class CatalogItemComponentController : _BasicController
    {

        private readonly CatalogItemComponentBL _catalogItemComponentBL;
        private readonly IGlobalManager _globalManager;

        public CatalogItemComponentController(

            [Dependency]CatalogItemComponentBL catalogItemComponentBL,
            [Dependency] IGlobalManager globalManager)
        {
            _globalManager = globalManager;
            _catalogItemComponentBL = catalogItemComponentBL;
            AdminClientID = _globalManager.GetAdminClientID();
        }

        private int AdminClientID;




        public ActionResult ItemComponents(int CatalogItemID)
        {

            var model = _catalogItemComponentBL.GetItemsList(CatalogItemID);
          
            return PartialView(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">CatalogItemComponentID</param>
        /// <param name="ForeignID">CatalogItemID</param>
        /// <returns></returns>
        public ActionResult Update(int? id, int? ForeignID)
        {
            try
            {
                CatalogItemComponentDM model;// 
                ViewBag.AllSrcs = _catalogItemComponentBL.GetAllSrcs();

                if (id.HasValue)
                {
                    model = _catalogItemComponentBL.GetSingleItemDM((int)id);
                    ViewBag.PopTitle = "עדכון פריט";
                }
                else
                {
                    if (!ForeignID.HasValue)
                        throw new Exception("ForeignID must have value");

                    var frstSrc = ((List<ComponentTypeDM>)ViewBag.AllSrcs).First();
                    model = new CatalogItemComponentDM
                    {
                        ComponentTypeID = CatalogItemType.Material,
                        ComponentCost = frstSrc.Cost,
                        ComponentPrice = frstSrc.Price,
                        Quantity = 1,
                        CatalogItemID = (int)ForeignID
                    };
                    ViewBag.PopTitle = "פריט חדש";
                }

                PopulateDrop(model);

                return PartialView(model);
            }
            catch (Exception e)
            {
                return ExceptionObj(e);
            }

          
        }


        [HttpPost]
        public ActionResult Update(CatalogItemComponentDM model, int? ForeignID)
        {
            try
            {
                if (ForeignID.HasValue)
                    model.CatalogItemID = (int)ForeignID;

                if(model.CatalogItemID==0)
                    throw new Exception("no CatalogItemID ");

               
                _catalogItemComponentBL.Update(model);
                
                return Json(new { });
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
                _catalogItemComponentBL.Delete(id);

                return Json(new { msg = "Success" });
            }
            catch (Exception e)
            {

                return ExceptionObj(e);
            }
        }


        private void PopulateDrop(CatalogItemComponentDM model)
        {

            var types = new Dictionary<CatalogItemType, string>();
            types.Add(CatalogItemType.Material, "חומר");
            types.Add(CatalogItemType.Outsource, "שירות חוץ");
            types.Add(CatalogItemType.Personnel, "שעות עבודה");


            ViewBag.ComponTypes = new SelectList(types, "Key", "Value", model.ComponentTypeID);
           
        }



       

    }
}
