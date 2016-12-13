using BL;
using Common;
using Microsoft.Practices.Unity;
using MvcBlox.Models;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;
using System.Web;
using System.Xml;

namespace MVC.Areas.Admin.Controllers
{
    public class AlignmentXmlController : _BasicController
    {

        private JobAlignmentBL _entityBL;
        private readonly IGlobalManager _globalManager;

        public AlignmentXmlController(

            [Dependency]JobAlignmentBL entityBL,
            [Dependency] IGlobalManager globalManager)
        {
            _globalManager = globalManager;
            _entityBL = entityBL;
            AdminClientID = _globalManager.GetAdminClientID();
        }

        private int AdminClientID;


        public ActionResult ImportXml()
        {
          
            return PartialView();
        }

        [HttpPost]
        public ActionResult UploadXml(HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0 && file.ContentType == "text/xml")
            {
                var document = new XmlDocument();
                document.Load(file.InputStream);
                // TODO: work with the document here
            }
            return Json(file);
        }







    }
}
