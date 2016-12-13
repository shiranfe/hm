using System;
using System.Web;
using System.Web.Mvc;
using BL;
using Common;
using Microsoft.Practices.Unity;
using MVC.Models;

namespace MVC.Areas.Admin.Controllers
{
    //[AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly EmployeeBL _employeeBL;

        public AccountController([Dependency] EmployeeBL employeeBL)
        {
            _employeeBL = employeeBL;
        }

        public ActionResult HeaderNotLoged()
        {

            var clnt = new UserLayoutDM
            {
                Lang = Session["lang"].ToString()
            };

            return PartialView(clnt);
        }

        public ActionResult Login()
        {
            return PartialView();
        }

        [HttpPost]
        public ActionResult Login(LoginVM loginVM)
        {
            if (ModelState.IsValid)
            {
                var userAccountDM = loginVM.ToUserAccountDM();

                var UserLoged = _employeeBL.EmpIsLoginValid(userAccountDM);

                if (UserLoged != null)
                {
                    Session["EmpID"] = UserLoged.UserID;
                    Session["ClientID"] = null;
                    if (loginVM.IsRemember)
                    {
                        var EmpCookie = new HttpCookie("EmpID")
                        {
                            Expires = DateTime.Now.AddYears(1),
                            Value = UserLoged.UserID.ToString()
                        };
                        Response.Cookies.Add(EmpCookie);
                    }

                    var returnUrl = Request.QueryString["returnUrl"]; ;
                    if(!string.IsNullOrEmpty(returnUrl))
                        return Redirect(returnUrl);

                    return Redirect("/Admin");
                }
                ModelState.AddModelError("", "");
            }

            // If we got this far, something failed, redisplay form
            return PartialView();
        }

        public ActionResult LogOff()
        {
            if (Request.Cookies["EmpID"] != null)
            {
                var c = new HttpCookie("EmpID")
                {
                    Expires = DateTime.Now.AddDays(-1)
                };
                Response.Cookies.Add(c);
            }

            Session["EmpID"] = null;

            return RedirectToAction(nameof(Login));
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