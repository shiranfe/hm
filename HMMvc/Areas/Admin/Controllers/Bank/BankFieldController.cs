using BL;
using Common;
using Microsoft.Practices.Unity;
using System;
using System.Web.Mvc;

namespace MVC.Areas.Admin.Controllers
{
    public class BankFieldController : _BasicController
    {

        private readonly BankFieldBL _entityBL;
        private readonly CatalogBL _catatlogBL;
        public BankFieldController(
               [Dependency]CatalogBL catatlogBL,
            [Dependency]BankFieldBL entityBL)
        {
            _entityBL = entityBL;
            _catatlogBL = catatlogBL;
        }



        public ActionResult Index(BankFieldFilterDm filter)
        {

            _entityBL.GetItemsList(filter);

            return PartialView(filter);
        }


        public ActionResult Update(int? id)
        {
            BankFieldDM model;// 

            if (id.HasValue)
            {
                model = _entityBL.GetSingleItemDM((int)id);
                ViewBag.PopTitle = "עדכון ";
            }
            else
            {
                model = new BankFieldDM { };
                ViewBag.PopTitle = "חדשה";
            }

            PopulateDrop(model);


            return PartialView(model);
        }


        [HttpPost]
        public ActionResult Update(BankFieldDM model)
        {
            try
            {
                _entityBL.Update(model);
                return Json(new { BankFieldID = model.BankFieldID });
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
                _entityBL.Delete(id);
                return Json(new { msg = "Success" });
            }
            catch (Exception e)
            {
                return ExceptionObj(e);
            }
        }


        private void PopulateDrop(BankFieldDM model)
        {
            var poolFields = _entityBL.GetPoolFields();
            ViewBag.poolFields = new SelectList(poolFields, "FieldPoolID", "FieldLabel", 29);

            ViewBag.catatlogItems = new SelectList(_catatlogBL.GetDropItems(), "CatalogItemID", "ItemName", model.CatalogItemID);

        }





    }
}
