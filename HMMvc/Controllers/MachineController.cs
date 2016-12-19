using BL;
//using HMErp.Helper;
using Common;
using Microsoft.Practices.Unity;
using MvcBlox.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace MVC.Controllers
{
    [Authorize]
    public class MachineController : Controller
    {
        private readonly MachineBL _machineBL;
        private readonly MachineVB _machineVB;
        private readonly IGlobalManager _globalManager;



        public MachineController([Dependency]MachineBL machineBL, 
            [Dependency]MachineVB machineVB,
            [Dependency]IGlobalManager globalManager)
        {
            _globalManager = globalManager;
            _machineBL = machineBL;
            _machineVB = machineVB;
            _userClientID = _globalManager.GetUserClientID();
        }

        private readonly int _userClientID;

        /*******************        GET        *******************/

        public ActionResult Index()
        {

            var macs = _machineBL.GetSelectedClient(_userClientID);

            return PartialView(macs);

        }

        public ActionResult Page(int id, string tab)
        {
            var model = _machineBL.GetMachineEditDM(id);

            ViewBag.PopTitle = "עמוד מכונה";
            ViewBag.Tab = tab;

            PopulateMachinePage(model);

            return PartialView(model);
        }

        private void PopulateMachinePage(MachineEditDM model)
        {
            var drop = new List<DropListDM>();
            drop.Add(new DropListDM { id = 0, Text = "מכונה" });

            model.Parts.ForEach(x => drop.Add(new DropListDM { id = x.MachinePartID, Text = " - " + x.MachineTypeLangStr }));

            ViewBag.Parts = new SelectList(drop, "id", "Text", 0);

        }

        /*******************        UPDATE        *******************/


        public ActionResult Update(int id)
        {

            var machine = _machineBL.GetMachineDetailsClientSide(id);

            ViewBag.PopTitle = "עדכון מכונה";

          //  PopulateMachineDrop(machine);

            return PartialView(machine);
        }

        //private void PopulateMachineDrop(MachineEditDM machine)
        //{
        //    var clnts = _clientBL.GetClientList();

        //    ViewBag.Client = new SelectList(clnts, "ClientID", "ClientName", machine.ClientID);
        //    ViewBag.MachineType = new SelectList(_langBL.GetPickListValue("MachineType"), "PickListID", "TransStr", machine.MachineTypeID);
        //    ViewBag.EngType = new SelectList(_langBL.GetPickListValue("EngType"), "PickListID", "TransStr", machine.EngTypeID);


        //    //var types = _langBL.GetPickListValue("MachineType");
        //    //ViewBag.MachineType = new SelectList(types, "PickListID", "TransStr");


        //}

        [HttpPost]
        public ActionResult Update(MachineEditDM machine)
        {
         
                _machineBL.Update(machine);
                // PopulateClientDrop();

                return Json(new { sts = "Success", MachineID = machine.MachineID });
           

        }


        [HttpPost]
        public ActionResult EditClientNotes(int machineID, string clientNotes, int jobID)
        {

            _machineVB.ChangeClientNotes(machineID, jobID, clientNotes);
            return Json(new {  });
        }

       [HttpPost]
       public ActionResult EditComments(int machineID, string comments)
       {


            _machineVB.ChangeComments(machineID, comments);
           return Json(new {  });
       }

       [HttpPost]
       public ActionResult EditSku(int machineID, string sku)
       {


            _machineVB.ChangeSku(machineID, sku);
           return Json(new {  });
       }

       [HttpPost]
       public ActionResult EditDetails(int machineID, string details)
       {

            _machineVB.ChangeDetails(machineID, details);
           return Json(new {  });
       }


    }
}
