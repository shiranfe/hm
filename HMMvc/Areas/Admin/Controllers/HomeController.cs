using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Common;
using BL;
//using HMErp.Helper;
using Microsoft.Practices.Unity;
using MvcBlox.Models;
using MVC.Controllers;
using MVC.Models;



namespace MVC.Areas.Admin.Controllers
{


    public class HomeController : _BasicController
    {

        private readonly IGlobalManager _globalManager;

        private readonly ClientBL _clientBL;
      
        private LangBL _langBL;
        private AdminBL _adminBL;
        private JobBL _jobBL;
        private MachineBL _machineBL;

        public HomeController([Dependency]ClientBL clientBL,
            [Dependency] IGlobalManager globalManager,
            [Dependency]MachineBL machineBL,
          
            [Dependency]LangBL langBL, [Dependency]JobBL jobBL,
            [Dependency] AdminBL adminBL)
        {
            _globalManager = globalManager;
            _clientBL = clientBL;
          
            _langBL = langBL;
            _machineBL = machineBL;
            _adminBL = adminBL;
            _jobBL = jobBL;
            AdminClientID = _globalManager.GetAdminClientID();
            EmpID = _globalManager.GetEmpID();

            emp = _globalManager.GetEmployeePermision();
            
        }

        private int AdminClientID;
        private int EmpID;
        private readonly EmployeeDM emp;

        /*************   Select  ***********/

        public ActionResult Index()
        {
            return View();
        }

        [EmpAuthFilter]
        public ActionResult Dashboard()
        {

            ViewBag.MobileTitle = "ראשי";
            return PartialView();
        }

        public ActionResult Test()
        {
           // _clientBL.Test(i);
            return PartialView();
        }


        public ActionResult AdminHeader(string MobileTitle)
        {
            ViewBag.MobileTitle = MobileTitle;

           //var model = new UserLayoutDM
           // {
           //     Perrmisions = emp.Perrmisions,
           //     FullName = emp.FullName,
           //     Lang = Session["lang"].ToString()
           // };
         

            return PartialView();
        }

        public ActionResult MenuAdmin(ClientTreeToShow ClientToShow, bool ShowTree)
        {
           
            var model = new MenuDM { 
                Perrmisions = emp.Perrmisions
            };

            model.Perrmisions.ShowTree = ShowTree;


            model.tree = _clientBL.GetClientTreeWithAll(ClientToShow);




            ViewBag.AdminClientID = AdminClientID;
            return PartialView(model);
        }

        public ActionResult SideMenuAdmin()
        {

            var model = new UserLayoutDM
            {
                Perrmisions = emp.Perrmisions,
                FullName = emp.FullName,
                Lang = Session["lang"].ToString()
            };


            return PartialView(model);
        }





        [HttpPost]
        public ActionResult ChangeAdminClient(int ClientID)
        {

            Session[nameof(AdminClientID)] = ClientID;
            return Json(new { sts = 1 });
        }


        [HttpPost]
        public ActionResult RefreshCache()
        {

            _globalManager.RefreshCache();
            return Json(new { sts = 1 });
        }


        public ActionResult CreateEntity()
        {

            var model = new CreateEntityModules() ;

            return PartialView(model);

        }

        [HttpPost]
        public ActionResult CreateEntity(CreateEntityModules model)
        {
            try
            {
                model.Create();

                return Content("success");
            }
            catch (Exception)
            {

                throw;
            }
            
           
        }


        public ActionResult NotPermited()
        {
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return RedirectToAction(nameof(NotPermitedPartial));

            return PartialView();
        }


        public ActionResult NotPermitedPartial()
        {
          

            return PartialView();
        }

    }



}
