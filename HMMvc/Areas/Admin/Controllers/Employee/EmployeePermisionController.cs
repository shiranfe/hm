using System.Web.Mvc;
using BL;
using Common;
using Microsoft.Practices.Unity;
using MvcBlox.Models;
using System;

namespace MVC.Areas.Admin.Controllers
{
    //[AllowAnonymous]
    public class EmployeePermisionController : _BasicController
    {
        private readonly EmployeeBL _employeeBL;
        private readonly IGlobalManager _globalManager;

        public EmployeePermisionController([Dependency] EmployeeBL employeeBL,
            [Dependency] IGlobalManager globalManager)
        {
            _employeeBL = employeeBL;
            _globalManager = globalManager;
        }

      
        public ActionResult Update(int id)
        {
            var employee = _globalManager.GetEmployeePermision(id);
            ViewBag.PopTitle = "הרשאות עובד במערכת";
            return PartialView(employee);
        }

        [HttpPost]
        public ActionResult Update(EmployeeDM employee)
        {
            try
            {
                _employeeBL.UpdatePermision(employee);

                return Json(employee);
            }
            catch (Exception e)
            {

                return ExceptionObj(e);
                //modeds.AddFormError("No class was selected.");
            }
            
        } 
        
    }

    //public class CustAuthFilter : ActionFilterAttribute
    //{
    //    /*protected override bool AuthorizeCore(HttpContextBase httpContext)
    //    {
    //        return AdminController.EmpID!=0;


    //    }*/


    //    public override void OnActionExecuting(ActionExecutingContext filterContext)
    //    {


    //        bool IsLoged = _


    //        if (!IsLoged)
    //        {
    //            var path = System.Web.Security.FormsAuthentication.LoginUrl;
    //            path = "/Admin/Account/Login";
    //            filterContext.Result = new RedirectResult(
    //                path + "?returnUrl"
    //                + "%2fAdmin%2fLogin"
    //                /* +filterContext.HttpContext.Server
    //                .UrlEncode(filterContext.HttpContext.Request.RawUrl)*/
    //            );
    //        }

    //    }

    //}
}