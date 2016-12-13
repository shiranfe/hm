using BL;
using Microsoft.Practices.Unity;
using System.Web.Mvc;

namespace MVC.Controllers
{

    public class BugLogController : Controller
    {
        
        private readonly BugLogBL _bugLogBl;


        public BugLogController( [Dependency]BugLogBL bugLogBl)
        {
            _bugLogBl = bugLogBl;
        }

      
        public ActionResult Index()
        {
            var model = _bugLogBl.Get();

            return PartialView(model);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {

            _bugLogBl.Delete(id);
            return Json(new { id = id, deleted = true });

        }

    }
}
