using System.Web.Mvc;
using BL;
using Common;
using Microsoft.Practices.Unity;
using MvcBlox.Models;
using PagedList;
//using HMErp.Helper;


namespace MVC.Controllers
{
   // [EnableCompression]
    public class VbController : Controller
    {
        private readonly IGlobalManager _globalManager;
        private readonly VbBL _vbBL;

        public VbController([Dependency] VbBL vbBL, [Dependency] IGlobalManager globalManager)
        {
            _globalManager = globalManager;
            _vbBL = vbBL;
        }

        private const int PageSize = 50;

        [Authorize]
        public ActionResult CurrentSts()
        {
            if (Session["ClientID"] == null)
                return RedirectToAction("Login", "User");

            var clientID = (int) Session["ClientID"];

            //var FilterSortPageDM = new FilterSortPageDM
            //{
            //    Sort = "Global_ReportStatus",
            //    Direction = false,
            //    Page = 1
            //};
            //var VBs = _vbBL.GetCurrentMachineSts(ClientID, FilterSortPageDM);
            //VBs.ForEach(x => x.MacPic = PicHelper.GetMacPic(x.MachineID, x.MachineTypeID));


            //var pageSize = 50;
            //var pageNumber = 1;
            //return PartialView(VBs.ToPagedList(pageNumber, pageSize));
           
            ViewBag.PageCount = _vbBL.GetPageCount(clientID) / PageSize; ;
            return PartialView();
        }

        public ActionResult CurrentStsContent(FilterSortPageDM filterSortPageDM)
        {
            var clientID = (int) Session["ClientID"];

            var vBs = _vbBL.GetCurrentMachineSts(clientID, filterSortPageDM);
            vBs.ForEach(x => x.MacPic = PicHelper.GetMacPic(x.MachineID, x.MachineTypeID));

          
            var pageNumber = (filterSortPageDM.Page ?? 1);
            return PartialView(vBs.ToPagedList(pageNumber, PageSize));
        }

        [Authorize]
        public ActionResult History()
        {
            var clientID = (int) Session["ClientID"];

            var vBs = _vbBL.GetClientHistory(clientID);
            return PartialView(vBs);
        }

        public ActionResult VbReport(int jobID)
        {
             var isEnglish = Session["dir"].ToString() == "False"; ;

            var rprt = _vbBL.GetVbReport(jobID, isEnglish);

            if (rprt == null || !_globalManager.AuthorizeUser(rprt.JobDM.ClientID))
                return Redirect("/Home/NotValidate");

            //int ClientID = (int)Session["ClientID"];
            //bool match = _clientBL.MatchClientID(ClientID, rprt.ClientID) || Session["ClientID"]!=null;
            //if (!match) return Redirect("/Home/NotValidate");

            return PartialView(rprt);
        }

        [Authorize]
        public ActionResult MachineJobs(int id)
        {

            var model = _vbBL.GetMachineJobsHistory(id);
            ViewBag.MachineID = id;
            return PartialView(model);

        }
    }
}