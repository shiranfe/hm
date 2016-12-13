using BL;
using Common;
using Microsoft.Practices.Unity;
using MvcBlox.Models;
using System.Collections.Generic;
using System.Web.Mvc;

namespace MVC.Controllers
{

    [Authorize]
    public class RefubrishController : Controller
    {

        private readonly IGlobalManager _globalManager;
        private readonly RefubrishBL _refubrishBL;
        public RefubrishController([Dependency]IGlobalManager globalManager,
              [Dependency]RefubrishBL refebrishBL)
        {
            _globalManager = globalManager;
            _refubrishBL = refebrishBL;
            _userClientID = _globalManager.GetUserClientID();
        }

        private readonly int _userClientID;

        /*******************        GET        *******************/


        public ActionResult About()
        {
            return PartialView();
        }

        public ActionResult Index(RefubrishFilterDm filter)
        {

            ViewBag.ClientName = "כל העבודות";

            //if(filter.On)
            _refubrishBL.GetAllJobs(_userClientID, filter);

            return PartialView(filter);
        }

       
        public ActionResult MachineJobs(int id)
        {

            var model = _refubrishBL.GetMachineJobsHistory(id);
            ViewBag.MachineID = id;
            return PartialView(model);

        }
        
        public ActionResult Details(int JobRefubrishPartID,  RefubrishStep step = RefubrishStep.None)
        {
            //RefubrishDM model = GetRefubrishPage(jobRefubrishPartID);
            //ViewBag.MobileTitle = model.ClientName;

            //model.CurrentStep = step != RefubrishStep.None ? step.ToString() : model.RefubrishStepDM.Where(x => x.HasStarted).Last().FormName;

            return PartialView(new RefubrishDM());

        }

    }
}
