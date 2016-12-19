using BL;
using Common;
using Common.Helpers;
using MvcBlox.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using MVC.App_Start;
using Microsoft.Practices.Unity;

namespace MVC.Areas.Admin.Controllers
{
    [EmpAuthFilter]
    public class _BasicController : Controller
    {

        private readonly BugLogBL _bugLogService;
     

        public _BasicController()
        {
            var unityConfig = UnityConfig.GetConfiguredContainer();
            _bugLogService = unityConfig.Resolve<BugLogBL>();
         
        }

        public ActionResult ExceptionObj(Exception e)
        {
            var msg = ErrorHelper.ExceptionMessage(e);
            ModelState.AddModelError(string.Empty, e.Message);

            var dict = new Dictionary<string, object>();
            Request.Form.CopyTo(dict);

            var param = UrlParams.GetJson(dict);

            Response.StatusCode = (int)HttpStatusCode.BadRequest;

            _bugLogService.Add(new BugLogDM
            {
               url= param,
                Message = msg,
                StackTrace = e.StackTrace,
                CreationTime = DateTime.Now
            });

            return Json(new
            {
                sts = "fail",
                msg = msg,
                stack = e.StackTrace
            }, JsonRequestBehavior.AllowGet);
        }



        public bool IsHeb()
        {
            return (string)Session["lang"] == "he-IL";
        }


    }

    public sealed class EmpAuthFilter : ActionFilterAttribute
    {


        //public EmpAuthFilter(
        //    [Dependency] IGlobalManager globalManager)
        //{
        //    _globalManager = globalManager;
        //    emp = ;
        //}

        private static Dictionary<string, string> ctrls;


        GlobalManager _globalManager = UnityConfig.GetConfiguredContainer().Resolve<GlobalManager>();
        EmployeeLogBL _employeeLogBL= UnityConfig.GetConfiguredContainer().Resolve<EmployeeLogBL>();
        BugLogBL _bugLogService = UnityConfig.GetConfiguredContainer().Resolve<BugLogBL>();

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
             
            if (!_globalManager.EmpIsLoged())
            {
                var path = System.Web.Security.FormsAuthentication.LoginUrl;
                path = "/Admin/Account/Login";
                filterContext.Result = new RedirectResult(
                    path
                //+ "?returnUrl="+ filterContext.HttpContext.Request.Url.LocalPath

                //filterContext.HttpContext.Server.UrlEncode(filterContext.HttpContext.Request.RawUrl)
                );
                return;
            }

            if (ctrls == null)
                ctrls = InitiateCtrls();

            //Example- Machine
            var ctrl = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
           

            //Example- {Key = "Machine", Value = "ClientSettings"}
            var permisionGroup = ctrls.FirstOrDefault(x => x.Key.Contains(ctrl) || ctrl.Contains(x.Value)); 

            /** Log employee update/delete/add activity*/
            if (filterContext.HttpContext.Request.HttpMethod != "GET")
            {
                try
                {
                    var dict = filterContext.ActionParameters
                           .Where(x => x.Key != "formCollection").ToDictionary(x => x.Key, x => x.Value);
                    var param = UrlParams.GetJson(dict);

                    var empId = _globalManager.GetEmpID();
                    if (empId == 0)
                        throw new Exception("empId cant be 0");

                    var actionName = filterContext.ActionDescriptor.ActionName;

                    _employeeLogBL.Add(empId, ctrl, actionName, param);
                }
                catch (Exception e)
                {
                    _bugLogService.Add(new BugLogDM
                    {
                        CardID = _globalManager.GetEmpID(),
                        //  Status = e.Data.s,
                        Message = ErrorHelper.ExceptionMessage(e),
                        StackTrace = e.StackTrace,
                        CreationTime = DateTime.Now
                    });

                }
                   
            }

            if (permisionGroup.Value == null)
                return;


            var emp = _globalManager.GetEmployeePermision();

            var permisions = emp.Perrmisions;

            //Example- emp.ShowClientSettings
            var employeeIsGroupPermited = (bool)permisions.GetType()
                .GetProperty("Show" + permisionGroup.Value).GetValue(permisions, null);


            if (!employeeIsGroupPermited)
            {
                var path = System.Web.Security.FormsAuthentication.LoginUrl;
                path = "/Admin/Home/NotPermited";
                filterContext.Result = new RedirectResult(path);
            }

          


        }

       

        private static Dictionary<string, string> InitiateCtrls()
        {
            return new Dictionary<string, string>
            {
                {"Refubrish", "Refubrish"},
                {"Quote", "Quote"},
                {"Alignment", "Alignment"},
                {"VB", "VB"},
                {"CatalogItem", "Catalog"},
                {"Supplier", "Catalog"},
                {"Manufacturer", "Catalog"},
                {"SupplierProduct", "Catalog"},
                {"Role", "Catalog"},
                {"DynamicGroups", "Settings"},
                {"DynamicFields", "Settings"},
                {"Employee", "Settings"},
                {"Lang", "Settings"},
                {"Client", "ClientSettings"},
                {"Machine", "ClientSettings"},
                {"Contact", "ClientSettings"},
                {"User", "ClientSettings"},
                {"ManagReports", "ManagReports"}
            };


        }



    }

    public class UrlParams
    {
        public static string GetJson(Dictionary<string,string> dict)
        {
           
            return JsonConvert.SerializeObject(dict, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            });
        }

        public static string GetJson(Dictionary<string, object> dict)
        {

            return JsonConvert.SerializeObject(dict, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            });
        }
    }
}
