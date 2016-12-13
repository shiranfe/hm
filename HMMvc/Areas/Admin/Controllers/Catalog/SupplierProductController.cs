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
    public class SupplierProductController : _BasicController
    {
    
        private readonly SupplierBL _supplierBL;
        private readonly ManufacturerBL _manufacturerBL;

        private readonly IGlobalManager _globalManager;

        public SupplierProductController(
            [Dependency]SupplierBL supplierBL,
            [Dependency]ManufacturerBL manufacturerBL,
            [Dependency] IGlobalManager globalManager)
        {
            _globalManager = globalManager;
            _supplierBL = supplierBL;
            _manufacturerBL = manufacturerBL;
            AdminClientID = _globalManager.GetAdminClientID();
        }

        private int AdminClientID;



        public ActionResult Index()
        {

            return PartialView();
        }

        public ActionResult PriceListProducts()
        {

            var model = _supplierBL.GetPriceListProducts();
            return PartialView(model);
        }


        public ActionResult Update(int? id)
        {
            SupplierProductDM model;// 

            if (id.HasValue)
            {
                model = _supplierBL.GetSingleProductDM((int)id);
                ViewBag.PopTitle = "עדכון מוצר";
            }
            else
            {
                model = new SupplierProductDM { SupplierProductID = 0, IsForClients=true };
                ViewBag.PopTitle = "מוצר חדש";
            }
            PopulateDrops(model);

            return PartialView(model);
        }


        [HttpPost]
        public ActionResult Update(SupplierProductDM model)
        {
            try
            {
                _supplierBL.UpdateSupplierProduct(model);
  
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
                _supplierBL.DeleteSupplierProduct(id);

                return Json(new { msg = "Success" });
            }
            catch (Exception e)
            {

                return  ExceptionObj(e);
            }
         
        } 


        private void PopulateDrops(SupplierProductDM model)
        {

            var suppliers = _supplierBL.GetIndex();
            ViewBag.Supplier = new SelectList(suppliers, "ClientID", "ClientName", model.ClientID);

            ViewBag.Manufacture = new SelectList(_manufacturerBL.GetList(), "PickListID", "Key", model.ManufactureID);


            var kinds = new List<PickListDM>();
            kinds.Add(new PickListDM { Value = "True", Text = "חומר" });
            kinds.Add(new PickListDM { Value = "False", Text = "שירות חוץ" });
            ViewBag.ItemKind = new SelectList(kinds, "Value", "Text", model.ManufactureID);

            ViewBag.ProductType = new SelectList(_supplierBL.GetProductLisType(), "PickListID", "TransStr", model.ProductTypeKey);


        }



     

    }
}
