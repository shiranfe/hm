using BL;
using Common;
using Microsoft.Practices.Unity;
using MVC.Helper;
using MVC.Models;
using MvcBlox.Models;
using System;
using System.Collections.Generic;
using System.IO;

using System.Linq;
using System.Web.Mvc;
//using HMErp.Helper;

namespace MVC.Areas.Admin.Controllers
{
    public class QuoteTrackController : _BasicController
    {
        private readonly QuoteTrackBL _entityBL;
        private readonly QuoteVersionBL _quoteVersionBL;
        private readonly LangBL _langBL;
        private readonly EmployeeBL _employeeBL;

        private readonly int _adminClientID;
        private readonly int _empID;
        private const string _quoteOrderPath = "\\QuoteOrders\\";

        public QuoteTrackController(
            [Dependency] LangBL langBL,
            [Dependency] QuoteTrackBL quoteTrackBL,
            [Dependency]EmployeeBL employeeBL,
            [Dependency] IGlobalManager globalManager, 
            [Dependency]QuoteVersionBL quoteVersionBL)
        {
            
            _entityBL = quoteTrackBL;
            _quoteVersionBL = quoteVersionBL;
            _langBL = langBL;
            _employeeBL = employeeBL;

            _adminClientID = globalManager.GetAdminClientID();
            _empID = globalManager.GetEmpID();
        }

        public ActionResult Index(bool dontFilter=false)
        {
            var model = _entityBL.GetItemsList(true);
            ViewBag.dontFilter = dontFilter;

            PopuplateIndex();

            return PartialView(model);
        }

        private void PopuplateIndex()
        {
            ViewBag.QuoteStatus = new SelectList(_langBL.GetQuoteIndexStatuses(), "PickListID", "TransStr", 100);
            ViewBag.Creator = new SelectList(_employeeBL.GetEmployeesList(true), "EmployeeID", "FullName");
            ViewBag.IsCoverOps = new SelectList(new List<SelectListItem>() {
                new SelectListItem { Value="", Text="כל ההצעות"},
                new SelectListItem { Value="true", Text="הצעת כיסוי"},
                new SelectListItem { Value="false", Text="הצעות רגילות"}
            }, "Value", "Text", "");
        }

        public ActionResult Update(int id)
        {
          
            var model = _entityBL.GetSingleItemDM(id);

            ViewBag.PopTitle = "מעקב הצעה";

            PopulateDrop(model);

            model.FollowDate = model.FollowDate;
            //if (!string.IsNullOrEmpty (model.OrderAttachment))
            //{
            //    model.OrderAttachment = _quoteOrderPath + model.OrderAttachment;
            //}
            return PartialView(model);
        }

        private void PopulateDrop(QuoteDM model)
        {
            // model.Clients = _clientBL.GetClientDrop();

            // ViewBag.Creator = new SelectList(_employeeBL.GetEmployeesList(), "EmployeeID", "FullName", model.CreatorID);

            ViewBag.QuoteStatus = new SelectList(_langBL.GetPickListValue("QuoteStatus"), "PickListID", "TransStr",
                model.QuoteStatusID);

            var version = _quoteVersionBL.GetItemsList(model.QuoteID);
            ViewBag.Versions = new SelectList(version, "QuoteVersionID", "VersionTitle", version.Last().QuoteVersionID);

            // ViewBag.For = new SelectList(_userBL.GetClientUsers(model.ClientID), "UserID", "FullName", model.UserID);
        }

        [HttpPost]
        public ActionResult Update(QuoteDM model, bool DeleteAttachment=false)
        {
            try 
            { 
                if (DeleteAttachment)
                {
                    string fileName = _entityBL.DeleteAttachment(model.QuoteID);
                    DeleteAttach(fileName);
                }
                else
                    _entityBL.Update(model);

                return Json(new {model.QuoteID});
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

                return Json(new {msg = "Success"});
            }
            catch (Exception e)
            {
                return ExceptionObj(e);
            }
        }

        [HttpPost]
        public ActionResult UploadAttachment(string qqfile,int id)
        {
            try
            {
                var extension = Path.GetExtension(qqfile);
                var fileName = "Order_" + id + extension;
                var filePath = _quoteOrderPath + fileName;
                var fullPath = HttpContext.Server.MapPath(".").Replace("\\QuoteTrack", "") + filePath;


                var upld = new UploadFileHelper();
                UploadFileHelper.UploadFile(fullPath, Request);

                _entityBL.UpdateOrderAttchment(id, fileName);


                return Json(new { success = true, folder = _quoteOrderPath.Replace("\\", "/"), fileName= fileName });


            }
            catch (Exception e)
            {
                return ExceptionObj(e);
            }

           
        }


        private void DeleteAttach(string fileName)
        {
            var filePath = _quoteOrderPath + fileName;
            var fullPath = HttpContext.Server.MapPath(".").Replace("\\Admin\\QuoteTrack", "") + filePath;

            FileManager.Delete(fullPath,true);

        }


        /// <summary>
        /// for each atachment exist in folder update quote OrderAttchment
        /// </summary>
        /// <returns></returns>
        public ActionResult UpdateAll()
        {
            
            var all = _entityBL.GetItemsList(true);

            string[] exts= new string[] {".pdf", ".PDF", ".jpg", ".bmp", ".html", ".htm", ".mht" };

            foreach (var q in all)
            {
                var id = q.QuoteID;
                foreach (var ext in exts)
                {
                    if (NewMethod( id, ext))
                        break;
                }
   
            }

            return Content("good");
        }

        private bool NewMethod( int id, string ext)
        {
            string fileName = "C:\\Dropbox\\HMErpSolution\\HMMvc\\QuoteOrders\\Order_" + id + ext;
            if (FileManager.Exists(fileName))
            {
                _entityBL.UpdateOrderAttchment(id, "Order_" +id + ext);
                System.Diagnostics.Debug.WriteLine("Order_" + id + ext);
                return true;
            }

            return false;
        }
    }
}