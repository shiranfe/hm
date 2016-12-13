using BL;
using Common;
using Microsoft.Practices.Unity;
using System;
using System.Web.Mvc;

namespace MVC.Areas.Admin.Controllers
{
    //[AllowAnonymous]
    public class EmployeeController : _BasicController
    {
        private readonly AdminBL _adminBL;
        private readonly EmployeeBL _employeeBL;

        public EmployeeController([Dependency] EmployeeBL employeeBL,
            [Dependency] AdminBL adminBL)
        {
            _employeeBL = employeeBL;
            _adminBL = adminBL;
        }

      
        public ActionResult Index()
        {
            var emps = _employeeBL.GetEmployeesList();

            return PartialView(emps);
        }

        public ActionResult Update(int? id)
        {
            EmployeeDM Employee;
            if (id.HasValue)
            {
                Employee = _employeeBL.GetEmployeeDetails((int) id);
                ViewBag.PopTitle = "עדכון עובד";
            }
            else
            {
                Employee = new EmployeeDM
                {
                    Active = true,
                    ContactInfoDM = new ContactInfoDM()
                };
                ViewBag.PopTitle = "עובד חדש";
            }

            PopulateUpdateEmployee(id);
            return PartialView(Employee);
        }

        private void PopulateUpdateEmployee(int? EmployeeID)
        {
            ViewBag.ShowEmailPassword = true;
            ViewBag.Branch = new SelectList(_adminBL.GetBranchList(), "Value", "Text", EmployeeID);
        }


        [HttpPost]
        public ActionResult Update(EmployeeDM model, ContactInfoDM contactInfoDM)
        {
            try
            {
                model.ContactInfoDM = contactInfoDM;
                _employeeBL.Update(model);
                // PopulateClientDrop();


                return Json(model);
            }
            catch (Exception e)
            {

                return ExceptionObj(e);
                //modeds.AddFormError("No class was selected.");
            }

            //try
            //{
               
            //    return RedirectToAction("Index");
            //}
            //catch (Exception e)
            //{
            //    ModelState.AddModelError(string.Empty, e.Message);
            //    PopulateUpdateEmployee(model.EmployeeID);
            //    return PartialView(model);
            //    //modeds.AddFormError("No class was selected.");
            //}
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            _employeeBL.Delete(id);
            // PopulateClientDrop();

            return Json(new {});
        }

        
    }
    
}