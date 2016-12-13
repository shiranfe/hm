using BL;
using Common;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace MVC.Areas.Admin.Controllers
{
    public class JobTaskEmployeeController : _BasicController
    {
      
        private readonly JobTaskEmployeeBL _entityBL;
        private readonly EmployeeBL _employeeBL;

        public JobTaskEmployeeController(
               [Dependency]EmployeeBL employeeBL,
            [Dependency]JobTaskEmployeeBL entityBL)
        {
            _entityBL = entityBL;
            _employeeBL = employeeBL;
        }


    public ActionResult TaskEmployess(int JobTaskID)
        {

            var list = _entityBL.GetTaskEmps(JobTaskID);

            ViewBag.Employee = new SelectList(_employeeBL.GetEmployeesList(), "EmployeeID", "FullName");
            return PartialView(list);
        }

        public ActionResult Index(JobTaskEmployeeFilterDm filter)
        {

            _entityBL.GetItemsList(filter);
        
            return PartialView(filter);
        }


        public ActionResult Update(int? id)
        {
            JobTaskEmployeeDM model;// 

            if (id.HasValue)
            {
                model = _entityBL.GetSingleItemDM((int)id);
                ViewBag.PopTitle = "עדכון ";
            }
            else
            {
                model = new JobTaskEmployeeDM {  };
                ViewBag.PopTitle = "חדשה";
            }

            PopulateDrop(model);


            return PartialView(model);
        }


        [HttpPost]
        public ActionResult Add(JobTaskEmployeeDM model)
        {
            try
            {
                _entityBL.Update(model);
                return Json(new { JobTaskEmployeeID = model.JobTaskEmployeeID});
            }
            catch (Exception e)
            {
                return ExceptionObj( e);
            }

        }

       

      
        [HttpPost]
        public ActionResult Delete(int id)
        {
            try
            {
                _entityBL.Delete(id);
                return Json(new { msg = "Success" });
            }
            catch (Exception e)
            {
                return ExceptionObj(e);
            }
        }


        private void PopulateDrop(JobTaskEmployeeDM model)
        {
            //var clnts = _quoteBL.GetAllSrcs();
           // ViewBag.Srcs = new SelectList(clnts, "ClientID", "ClientName", mac.ClientParentID);
        }



       

    }
}
