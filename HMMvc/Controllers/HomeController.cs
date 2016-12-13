using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BL;
using Microsoft.Practices.Unity;
using MvcBlox.Models;
using Common;

namespace MVC.Controllers
{
    public class HomeController : Controller
    {
         private readonly IGlobalManager _globalManager;
        private readonly BugLogBL _bugLogBL;


        public HomeController([Dependency]IGlobalManager globalManager, 
            [Dependency]BugLogBL bugLogBL)
        {
            _globalManager = globalManager;
            _bugLogBL = bugLogBL;
        }

        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public ActionResult Dashboard()
        {

            ViewBag.MobileTitle = "ראשי";
            return PartialView();
        }

        public ActionResult Lay()
        {
            
            return PartialView();
        }

        public ActionResult Warm()
        {

            return PartialView();
        }

        public ActionResult NotValidate()
        {
            return PartialView();
        }

        public ActionResult Bugs()
        {
            List<BugLogDM> model = _bugLogBL.Get();

            return PartialView(model);
        }


    }
}
