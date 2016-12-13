using BL;
using Common;
using Microsoft.Practices.Unity;
using MvcBlox.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace MVC.Areas.Admin.Controllers
{
    public class OutsideController : _BasicController
    {
        private readonly JobOutsideBL _entityBL;
        private readonly EmployeeBL _employeeBL;
       
        private readonly int _empID;

        public OutsideController(
            [Dependency] JobOutsideBL entityBL,
             [Dependency]EmployeeBL employeeBL,
          
            [Dependency] IGlobalManager globalManager)
        {
            _entityBL = entityBL;
            _empID = globalManager.GetEmpID();
            _employeeBL = employeeBL;
          
        }

        public ActionResult Index(OutsideFilterDm filter)
        { 
           _entityBL.GetItemsList(filter);

            ViewBag.Creator = new SelectList(_employeeBL.GetEmployeesList(true), "EmployeeID", "FullName", filter.CreatorID);
            ViewBag.Zones = new SelectList(_entityBL.GetZones(), "Value", "Text", filter.Zone);

            return PartialView(filter);
        }

        public ActionResult MachineJobs(int id)
        {

            var model = _entityBL.GetMachineJobsHistory(id);
            ViewBag.MachineID = id;
            return PartialView(model);

        }

       
        public ActionResult Update(int id)
        {
            /** job opend  - set machine */
            var model = _entityBL.GetSingleItemDM(id);
           
            //ViewBag.Machine = new SelectList(_machineBL.GetClientMachines(model.ClientID), "MachineID", "MachineNameFull");
          
            return PartialView(model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Update(JobOutsideDM model)
        {
            try
            {
                _entityBL.Update(model);
                return Json(new { model.JobID });
            }
            catch (Exception e)
            {
                return ExceptionObj(e);
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
    }
}