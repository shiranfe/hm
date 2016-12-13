using BL;
using Common;
//using HMErp.Helper;
using Microsoft.Practices.Unity;
using MvcBlox.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace MVC.Areas.Admin.Controllers
{
    public class MachineController : _BasicController
    {
        private readonly LangBL _langBL;  
        private readonly MachineBL _machineBL;
        private readonly MachineVB _machineVB;
        private readonly ClientBL _clientBL;
        private readonly IGlobalManager _globalManager;

        public MachineController(
            [Dependency]LangBL langBL,
           [Dependency]MachineVB machineVB,
            [Dependency]MachineBL machineBL, 
            [Dependency]ClientBL clientBL, 
            [Dependency] IGlobalManager globalManager)
        {
            _globalManager = globalManager;
            _langBL = langBL;
            _machineVB = machineVB;
            _clientBL = clientBL;
            _machineBL = machineBL;
            AdminClientID = _globalManager.GetAdminClientID();
          
        }

        private readonly int AdminClientID;



        /*************   Machines  ***********/

        public void GetMacPic(SelectedMachine mac)
        {
            _globalManager.GetMacPic(mac);
        }


        public ActionResult Index(MachineFilterDm filter )
        {

            var tree=  _clientBL.GetClientDrop();
            ViewBag.Clients = tree;


            if (filter.ClientID ==0)
                filter.ClientID = tree.First().ClientID;

            _machineBL.GetClientMachines(filter);

            //ViewBag.ClientID = ClientID;
           
            //ViewBag.MachineID = MachineID;

            return PartialView(filter);

            //MachinePicChangeDM mac = _adminBL.GetClientAndMachine();
            //GetMacPic(mac.SelectedClient.SelectedMachine);
        
            //return PartialView(mac);
        }


 


        public ActionResult MergeMachines(int ClientId)
        {
            ViewBag.PopTitle = "איחוד מכונות";
           
            var mac = _machineBL.GetClientAndMachine(ClientId);
            ViewBag.ClientID = new SelectList(mac.Clients, "ClientID", "ClientName", mac.SelectedClient.ClientID);
            return PartialView(mac);
        }

        public ActionResult MergeMachinesClientSelected(int ClientID)
        {

            var mac = _machineBL.GetSelectedClient(ClientID);
            return PartialView(mac);
        }

        [HttpPost]
        public ActionResult MergeMachines(int MacNewMergeID, int MacOldMergeID)
        {

            _machineBL.MergeMachines(MacNewMergeID, MacOldMergeID);
            return Json(new { });
        }



        public ActionResult ClientSelectedMachines()
        {
            var mac = GetMachinePicClientSelected();
            return PartialView(mac);
        }

     
        public ActionResult GetClientMachines(int ClientID)
        {
            var model= _machineBL.GetClientMachines(ClientID)
                .Select(x=> new {
                    Value =x.MachineID.ToString(),
                    Text = x.MachineNameFull,
                    x.Kw,
                    x.Rpm,
                    x.Address
                })
                .ToList();
            return Json(model, JsonRequestBehavior.AllowGet);
        }

    


        private ClientDM GetMachinePicClientSelected()
        {
           
            var mac = (AdminClientID > 0) ?
                _machineBL.GetSelectedClient(AdminClientID) :
                _machineBL.GetDefualtMachine();

           // GetMacPic(mac.SelectedMachine);

            return mac;
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

        /*************      Update        ***********/

            /// <summary>
            /// 
            /// </summary>
            /// <param name="id">MachineId</param>
            /// <param name="ForeignID">ClientID</param>
            /// <param name="MachineName"></param>
            /// <returns></returns>
        public ActionResult Update(int? id, int? ForeignID, string MachineName=null)
        {
            MachineEditDM machine;

            if (id.HasValue)
            {
                machine = _machineBL.GetMachineEditDM((int)id);
              
                ViewBag.PopTitle = "עדכון מכונה";
            }
            else
            {
                ForeignID = ForeignID ?? AdminClientID;
                if (!ForeignID.HasValue || ForeignID == 0)
                    throw new Exception("new machine need client id");

                machine = new MachineEditDM
                {
                    ClientID = (int)ForeignID,
                    MachineName= MachineName,
                    MacPic =PicHelper.GetMacPic(0, ((int)MachineType.EngineAC).ToString()),
                    Parts = new List<MachinePartDM>()
                };
                ViewBag.PopTitle = "מכונה חדשה";
            }

            PopulateMachineDrop(machine);

            return PartialView(machine);
        }

        private void PopulateMachineDrop(MachineEditDM machine)
        {
            var clnts = _clientBL.GetClientList();

            ViewBag.Client = new SelectList(clnts, "ClientID", "ClientName", machine.ClientID);
            ViewBag.MachineType = new SelectList(_langBL.GetPickListValue("MachineType"), "PickListID", "TransStr", machine.MachineTypeID);
            ViewBag.EngType = new SelectList(_langBL.GetPickListValue("EngType"), "PickListID", "TransStr", machine.EngTypeID);


            //var types = _langBL.GetPickListValue("MachineType");
            //ViewBag.MachineType = new SelectList(types, "PickListID", "TransStr");


        }

        [HttpPost]
        public ActionResult Update(MachineEditDM machine)
        {
            try
            {
                _machineBL.Update(machine);
                // PopulateClientDrop();

                return Json(new { sts = "Success", MachineID = machine.MachineID });
            }
            catch (Exception e)
            {
                return ExceptionObj(e);
            }

        }


      

        /*************      UPDATE        ***********/


        public ActionResult UpdatePointXY(int MachinePointID, int X, int Y)
        {

            _machineVB.ChangePointXy(MachinePointID, X, Y);
            return Json(new { });
        }

        public ActionResult UpdatePointShow(int MachinePointID, bool Show)
        {

            _machineVB.ChangePointShow(MachinePointID, Show);
            return Json(new { });
        }

        //[HttpPost]
        public ActionResult PointPositionSelected(int MachineID)
        {
            var mac = _machineBL.GetSelectedMachine(MachineID);
            GetMacPic(mac);
            return PartialView(mac);
        }


        /*************      UPDATE        ***********/


        [HttpPost]
        public ActionResult Delete(int id)
        {

            try
            {
                _machineBL.Delete(id);

                return Json(new { msg = "Success" });
            }
            catch (Exception e)
            {

                return ExceptionObj(e);
            }
        }

    }
}
