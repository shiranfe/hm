using BL;
using Common;
using Microsoft.Practices.Unity;
using MVC.Areas.Admin.Models;
using MvcBlox.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace MVC.Areas.Admin.Controllers
{
    public class RefubrishController : _BasicController
    {
      
        private readonly LangBL _langBL;
        private readonly RefubrishBL _refubrishBL;
        private readonly EmployeeBL _employeeBL;

        public RefubrishController( 
            [Dependency]LangBL langBL,
            [Dependency]RefubrishBL refebrishBL,
            [Dependency]EmployeeBL employeeBL,
            [Dependency] IGlobalManager globalManager)
        {
            var globalManager1 = globalManager;         
            _langBL = langBL;
            _refubrishBL = refebrishBL;
            _employeeBL = employeeBL;
            _adminClientID = globalManager1.GetAdminClientID();
            _empID = globalManager1.GetEmpID();
        }

        private readonly int _adminClientID;
        private readonly int _empID;

     

        /*************   Select  ***********/

        

        public ActionResult Index(RefubrishFilterDm filter)
        { 
     
            ViewBag.ClientName = "כל העבודות";

            //if(filter.On)
                _refubrishBL.GetAllJobs(_adminClientID, filter);

         
            ViewBag.RefubrishStatusID = new SelectList(_langBL.GetRefubrishStatuses(), "PickListID", "TransStr", filter.RefubrishStatusID) ;
             
            ViewBag.Creator = new SelectList(_employeeBL.GetEmployeesList(true), "EmployeeID", "FullName", filter.CreatorID );

            //if (filter.On)
            //    return PartialView("Filterd", filter);

            return PartialView(filter);
        }

        public ActionResult Filterd(RefubrishFilterDm filter)
        {
            ViewBag.ClientName = "כל העבודות";
           // _refubrishBL.GetAllJobs(_adminClientID, filter); ;
            return PartialView(filter);
        }

        public ActionResult Refubrish(int jobRefubrishPartID, int? JobID,RefubrishStep step= RefubrishStep.None)
        {
            if (JobID.HasValue)
                jobRefubrishPartID = _refubrishBL.GetFirstJobRefubrishPartID((int)JobID);

            var model = GetRefubrishPage(jobRefubrishPartID);
            ViewBag.MobileTitle = model.ClientName;

            model.CurrentStep = GetCurrenStep(step, model);

            return PartialView(model);
        }

        private static string GetCurrenStep(RefubrishStep step, RefubrishDM model)
        {
            if (step != RefubrishStep.None)
                return step.ToString();

            if (!model.RefubrishStepDM.Any())
                throw new Exception("cant get last step. JobRefubrishPartID " + model.JobRefubrishPartID);

            return   model.RefubrishStepDM.Last(x => x.HasStarted).FormName;
        }

        private RefubrishDM GetRefubrishPage(int jobRefubrishPartID)
        {
            var jobDetails = _refubrishBL.GetJobPartCard(jobRefubrishPartID);
            ViewBag.Parts = new SelectList(jobDetails.Parts, "id", "MachineTypeLangStr", jobRefubrishPartID);
            return jobDetails;
        }

        public ActionResult ByJob(int JobID)
        {
            var jobRefubrishPartID = _refubrishBL.GetFirstJobRefubrishPartID(JobID);
            var model = GetRefubrishPage(jobRefubrishPartID);
            model.CurrentStep = model.RefubrishStepDM.Last(x => x.HasStarted).FormName;

            return PartialView(nameof(Refubrish), model);
        }


        public ActionResult Step(int jobRefubrishPartID, RefubrishStep step)
        {
            var model = _refubrishBL.GetStep(jobRefubrishPartID, step);

            if (step == RefubrishStep.ExtraJobsStep)
                return PartialView(step.ToString(), model);

            SetStepPics(model, step);
            ViewBag.Title = GlobalDM.GetTransStr(model.StepName);

            return PartialView(model);
        }

        private void SetStepPics(BasicStepDM model, RefubrishStep step)
        {
            string folder = PicHelper.JobPathPhys + model.JobID;

            /** no picture uploaded*/
            if (!Directory.Exists(folder))
            {
                model.StepPics = new List<string>();
                return;
            }
                

            string searchPattern =  step.ToString() + "*.jpg";
            var list = Directory.GetFiles(folder, searchPattern, SearchOption.TopDirectoryOnly).ToList();

            model.StepPics = list.Select(x=> PicHelper.PhysicalToUrl(x,true)).ToList() ;

        }

        public ActionResult MachineJobs(int id)
        {
             
            var model = _refubrishBL.GetMachineJobsHistory(id);
            ViewBag.MachineID = id;
            return PartialView(model);

        }

        /*************      UPDATE        ***********/



        //[HttpPost]
        //public ActionResult UpdateStep(FormCollection formCollection, BasicStepDM model)
        //{
        //    try
        //    {

        //       var fields = DynamicFieldsHelper.collectionToFields(formCollection);
        //        model.CreatorID = _empID;

        //        _refubrishBL.ChangeStep(model, fields);


        //        //JobRefubrishPreTestDM model = new JobRefubrishPreTestDM();
        //        //_refubrishBL.ChangeJobRefubrishPreTest(model, EmpID);
        //        if (model.NextStatus== JobRefubrishStatus.Done)
        //            return RedirectToAction("Index");

        //        if (model.NextStatus == JobRefubrishStatus.WaitForQuote)
        //            return Json(new { QuoteID = model.QuoteID });

        //        /** update step wirhout miving to the next one (speed change...) */
        //        if (model.StayInStep != RefubrishStep.None)
        //            return Json(new { NextStep= model.StayInStep.ToString() });

        //        var jobDetails = GetRefubrishPage(model.JobRefubrishPartID);
        //        return PartialView("Refubrish", jobDetails);

        //        // return RedirectToAction("Refubrish", new { JobRefubrishPartID = model.JobRefubrishPartID });

        //    }
        //    catch (Exception e)
        //    {
        //        return ExceptionObj(e);
        //    }

        //}

        [HttpPost]
        public ActionResult UpdateStep(FormCollection formCollection, BasicStepDM model)
        {
            try
            {

                var fields = DynamicFieldsHelper.collectionToFields(formCollection);
                model.CreatorID = _empID;

                _refubrishBL.UpdateStepFields(model, fields);


               return Json(new {  });

           
            }
            catch (Exception e)
            {
                return ExceptionObj(e);
            }

        }

        [HttpPost]
        public ActionResult NextStep(FormCollection formCollection, BasicStepDM model)
        {
            try
            {
                 
                var fields = DynamicFieldsHelper.collectionToFields(formCollection);
                model.CreatorID = _empID;
 
                _refubrishBL.ChangeStep(model, fields);


                //JobRefubrishPreTestDM model = new JobRefubrishPreTestDM();
                //_refubrishBL.ChangeJobRefubrishPreTest(model, EmpID);
                if (model.NextStatus == JobRefubrishStatus.Done || model.NextStatus == JobRefubrishStatus.Rejected)
                    return Json(new { Closed = true });

                if (model.NextStatus == JobRefubrishStatus.WaitForQuote)
                    return Json(new { QuoteID = model.QuoteID });

                ///** update step wirhout miving to the next one (speed change...) */
                //if (model.StayInStep != RefubrishStep.None)
                //    return Json(new { NextStep = model.StayInStep.ToString() });

                return Json(new { model.JobRefubrishPartID, NextStep=model.NextStep.ToString() });
                //var jobDetails = GetRefubrishPage(model.JobRefubrishPartID);
                //return PartialView("Refubrish", jobDetails);

                // return RedirectToAction("Refubrish", new { JobRefubrishPartID = model.JobRefubrishPartID });

            }
            catch (Exception e)
            {
                return ExceptionObj(e);
            }

        }


        [HttpPost]
        public ActionResult Update(string key, string lang, string word)
        {
            _langBL.UpdateWord(key, lang, word);

            return Json(new {  });
        }



    }
}
