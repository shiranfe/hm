using BL;
using Microsoft.Practices.Unity;
using System.Web.Mvc;

namespace MVC.Areas.Admin.Controllers
{
    public class ManagReportController : _BasicController
    {

        private readonly QuoteBL _quoteBL;
        private readonly RefubrishBL _refubrishBL;

        public ManagReportController(
            [Dependency]QuoteBL quoteBL, [Dependency]RefubrishBL refubrishBL)
        {
            _quoteBL = quoteBL;
            _refubrishBL = refubrishBL;
        }



        public ActionResult Index()
        {
            return PartialView();
        }

        public ActionResult Quote()
        {
            var model = _quoteBL.GetReport();

            return PartialView(model);
        }

        public ActionResult Refubrish()
        {

            var model = _refubrishBL.GetReport();

            return PartialView(model);
        }
    }
}