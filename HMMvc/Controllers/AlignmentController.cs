using BL;
using Common;
using Microsoft.Practices.Unity;
using MvcBlox.Models;
using System.Linq;
using System.Web.Mvc;

namespace MVC.Controllers
{
    [Authorize]
    public class AlignmentController : Controller
    {
         private readonly IGlobalManager _globalManager;
        private readonly JobAlignmentBL _entityBL;

        public AlignmentController(
               [Dependency] JobAlignmentBL entityBL,
            [Dependency]IGlobalManager globalManager)
        {
            _globalManager = globalManager;
            _entityBL = entityBL;
            _userClientID = _globalManager.GetUserClientID();
        }

        private readonly int _userClientID;

        public ActionResult About()
        {
            return PartialView();
        }

        public ActionResult Sample()
        {
            return PartialView();
        }


        public ActionResult Index()
        {
            var model = _entityBL.GetItemsList(_userClientID);

          //  ViewBag.ClientName =  model.First().JobDM.ClientName ;

            return PartialView(model);
        }

        public ActionResult MachineJobs(int id)
        {

            var model = _entityBL.GetMachineJobsHistory(id);
            ViewBag.MachineID = id;
            return PartialView(model);

        }

       
    }
}
