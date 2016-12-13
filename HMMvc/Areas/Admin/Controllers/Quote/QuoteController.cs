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
    public class QuoteController : _BasicController
    {

        private readonly IGlobalManager _globalManager;
        private readonly QuoteBL _entityBL;
        private readonly UserBL _userBL;
        private readonly ClientBL _clientBL;
        private readonly EmployeeBL _employeeBL;
        private readonly LangBL _langBL;

        public QuoteController(
          [Dependency]ClientBL clientBL,
              [Dependency]UserBL userBL,
             [Dependency]LangBL langBL,
             [Dependency]EmployeeBL employeeBL,
            [Dependency]QuoteBL quoteBL,
            [Dependency] IGlobalManager globalManager)
        {
            _globalManager = globalManager;
            _entityBL = quoteBL;
            _clientBL = clientBL;
            _employeeBL = employeeBL;
            _userBL = userBL;
            _langBL = langBL;
            AdminClientID = _globalManager.GetAdminClientID();
            EmpID = _globalManager.GetEmpID();
        }

        private readonly int AdminClientID;
        private readonly int EmpID;



        public ActionResult Index(QuoteFilterDm filter)
        {
             
            _entityBL.GetItemsList(filter);
            PopuplateIndex(filter);

            return PartialView(filter);
        }

        private void PopuplateIndex(QuoteFilterDm filter)
        {
            ViewBag.QuoteStatus = new SelectList(_langBL.GetQuoteIndexStatuses(), "PickListID", "TransStr", filter.QuoteStatusID ?? 100);
            ViewBag.Creator = new SelectList(_employeeBL.GetEmployeesList(true), "EmployeeID", "FullName", filter.CreatorID);
            ViewBag.IsCoverOps = new SelectList(new List<SelectListItem>() {
                new SelectListItem { Value="", Text="כל ההצעות"},
                 new SelectListItem { Value="true", Text="הצעת כיסוי"},
                new SelectListItem { Value="false", Text="הצעות רגילות"}
            }, "Value", "Text", filter.IsCover );
        }


        public ActionResult ImportFromQuote(QuoteFilterDm filter, int DestVersionID)
        {
            _entityBL.GetSearchList(filter);
            filter.TableList = filter.TableList.Where(x=>x.CurrentVersionID!= DestVersionID).ToList();

   
            PopuplateIndex(filter);
            ViewBag.DestVersionID = DestVersionID;
            ViewBag.PopTitle = "ייבוא מהצעת מחיר";

            //PopuplateIndex();

            return PartialView(filter);
        }

      



        public ActionResult Get(int id)
        {

            int versionID = _entityBL.GetLastVersionID(id);

            return RedirectToAction("Update","QuoteVersion", new { id = versionID });
        }

        /// <summary>
        /// update all quorw details - not version
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Update(int? id)
        {
            QuoteDM model;// 
             
            if (id.HasValue)
            {
                model = _entityBL.GetSingleItemDM((int)id);
                ViewBag.PopTitle = "עדכון הצעה";
            }
            else
            {
                model = new QuoteDM
                {
                    ClientID = AdminClientID,
                    CreatorID = EmpID,
                    FollowDate=DateTime.Now,
                    QuoteStatusID = 543,//QuoteStatus_InProccess                                   
                };


                ViewBag.PopTitle = "הצעה חדשה";
            }

            PopulateDrop(model);


            return PartialView(model);
        }

        private void PopulateDrop(QuoteDM model)
        {

            model.Clients = _clientBL.GetClientDrop();

            ViewBag.Creator = new SelectList(_employeeBL.GetEmployeesList(), "EmployeeID", "FullName", model.CreatorID);

            ViewBag.QuoteStatus = new SelectList(_langBL.GetPickListValue("QuoteStatus"), "PickListID", "TransStr", model.QuoteStatusID);

            ViewBag.For = new SelectList(_userBL.GetClientUsers(model.ClientID), "UserID", "FullName", model.UserID);


        }

      
        [HttpPost]
        public ActionResult Update(QuoteDM model)
        {
            try
            {
                var updated = model.QuoteID > 0;
                _entityBL.Update(model);

                return Json(new { model.QuoteID , model.CurrentVersionID, updated });
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
                _entityBL.Delete(id);

                return Json(new { msg = "Success" });
            }
            catch (Exception e)
            {

                return ExceptionObj(e);
            }
        }







    }
}
