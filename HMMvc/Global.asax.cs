using System;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using BL;
using Microsoft.Practices.Unity;
using MvcBlox.App_Start;
using System.Diagnostics;

namespace MVC
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : HttpApplication
    {
        //private static string Domain = HttpContext.Current.Request.Url.AbsoluteUri;
        //private ClientBL _clientBL;
        private readonly LangBL _langBL;
        private readonly UserBL _userBL;

        public MvcApplication()
        {
            //this._clientBL = UnityConfig.GetConfiguredContainer().Resolve<ClientBL>();
            _langBL = UnityConfig.GetConfiguredContainer().Resolve<LangBL>();
            _userBL = UnityConfig.GetConfiguredContainer().Resolve<UserBL>();
       
        }

        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new EnableCompressionAttribute());
            // filters.Add(new HandleErrorAttribute());
        }

        protected void Application_Start()
        {
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new RazorViewEngine());

            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);


            // _clientBL.LoadAllClientChilds();
            _langBL.GetAllLangDictionary();
            //_clientBL.GetAllClientTree();

            //MiniProfilerEF.InitializeEF42();

            //KeepWarm();
        }

        //private  void KeepWarm()
        //{

        //    System.Timers.Timer aTimer = new System.Timers.Timer();
        //    aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
        //    // Set the Interval to 5 min.
        //    aTimer.Interval = 1000 * 60 * 5;
        //    aTimer.Enabled = true;


        //}

        //private  void OnTimedEvent(object source, ElapsedEventArgs e)
        //{
        //    var uri = Domain; //+ "/Home/Warm";
        //    var a = WebRequest.Create(uri);
        //    _langBL.RefreshDictionary();
        //    _clientBL.LoadAllClientChilds();
        //}

        private void Session_Start(object sender, EventArgs e)
        {
            // Code that runs when a new session is started

            var cok = User.Identity.Name;
            if (cok != "")
            {
                var id = Convert.ToInt32(cok);
                Session["UserID"] = id;
                {
                    Session["RootClientID"] = Session["ClientID"] = _userBL.GetClientID(id);
                    Session["EmpID"] = null;
                }
            }


            var dir = HttpContext.Current.Request.Cookies["dir"];

            if (dir == null)
            {
                Extensions.SetSysLangCookie("he-IL");
            }
            else
            {
                var lngcok = Request.Cookies["lang"];
                Session["lang"] = lngcok.Value;
                Session["dir"] = dir.Value;
            }

            var emp = HttpContext.Current.Request.Cookies["EmpID"];
            if (emp != null)
            {
                Session["EmpID"] = Convert.ToInt32(emp.Value);
            }


            // BL.GlobalBL.GetLangDictionary();
        }
    }
}