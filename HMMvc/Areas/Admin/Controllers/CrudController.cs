using BL;
using Microsoft.Practices.Unity;
using MvcBlox.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVC.Areas.Admin.Controllers
{
    public class CrudController<TModel> : Controller
    {
        private CrudBL<TModel> _serviceBL;
        private readonly IGlobalManager _globalManager;

        public CrudController(
            [Dependency]CrudBL<TModel> serviceBL,
            [Dependency] IGlobalManager globalManager)
        {
            _globalManager = globalManager;

            _serviceBL = serviceBL;
            AdminClientID = _globalManager.GetAdminClientID();
        }

        private int AdminClientID;



        public ActionResult Index()
        {

            List<TModel> model = _serviceBL.GetIndex();

            return View(model);
        }    

        

        [HttpPost]
        public ActionResult Delete(int id)
        {
            try
            {
                _serviceBL.Delete(id);

                return Json(new { msg = "Success" });
            }
            catch (Exception e)
            {
                return ExceptionObj(e);

            }
              

        }


       

        private ActionResult ExceptionObj(Exception e)
        {
            string msg = (e.InnerException != null) ? e.InnerException.InnerException.Message : e.Message;
            ModelState.AddModelError(string.Empty, e.Message);

            return Json(new { msg = msg });
        }

    }
}
