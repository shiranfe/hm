using System;
using System.Linq;
using System.Web.Mvc;
using BL;
using Common;
using Microsoft.Practices.Unity;
using MvcBlox.Models;
//using HMErp.Helper;

namespace MVC.Areas.Admin.Controllers
{
    public class ClientController : _BasicController
    {
        private readonly int _adminClientId;
        private readonly ClientBL _clientBL;

        public ClientController(
            [Dependency] ClientBL clientBL,
            [Dependency] IGlobalManager globalManager)
        {
            _clientBL = clientBL;
            _adminClientId = globalManager.GetAdminClientID();
        }

        public ActionResult Index(ClientFilterDm filter)
        {
            _clientBL.GetClientListIndex(filter);

            return PartialView(filter);
        }

        public ActionResult MergeClients()
        {
            ViewBag.PopTitle = "איחוד לקוחות";
            var filter = new ClientFilterDm { PageTotal=1, Page=1};
            _clientBL.GetClientListIndex(filter);
            return PartialView(filter.TableList.OrderBy(x=>x.ClientName).ToList());
        }


        public ActionResult Update(int? id, string ClientName=null)
        {
            ClientDM model; // 

            if (id.HasValue)
            {
                model = GetClientDetails(id);
                ViewBag.PopTitle = "עדכון לקוח";
            }
            else
            {
                model = new ClientDM {ClientID = 0, ClientName= ClientName, IsClient = true, ShowInVb = true, ShowInRefubrish = true};
                ViewBag.PopTitle = "לקוח חדש";
            }

            PopulateClientParents(model);

            return PartialView(model);
        }

        [HttpPost]
        public ActionResult Update(ClientDM model)
        {
            try
            {
                var updated = model.ClientID > 0;
                _clientBL.Update(model);
                 
                //Session["AdminClientID"] = model.ClientID;

            
                return Json(new {
                    model.ClientID, model.ClientName,model.ClientFullName,
                    updated

                });

                //PopulateClientParents(model);
                //return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                return ExceptionObj(e);
                //modeds.AddFormError("No class was selected.");
            }
        }

        private void PopulateClientParents(ClientDM mac)
        {
            ViewBag.Client = _clientBL.GetClientDrop();
            //var clnts = _clientBL.GetClientList().Where(x => x.ClientID != mac.ClientID).ToList();
            //ViewBag.Client = new SelectList(clnts, "ClientID", "ClientName", mac.ClientParentID);
        }

        [HttpPost]
        public ActionResult MergeClients(int OldMergeID, int NewMergeID)
        {
            try
            {
                _clientBL.MergeClients(OldMergeID, NewMergeID);

                return Json(new {});

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
                _clientBL.Delete(id);
                Session["AdminClientID"] = 0;

                return Json(new { sts = "Success" });
            }
            catch (Exception e)
            {

                return ExceptionObj(e);
            }
           
        }


        public ActionResult ClientSelectedLogo()
        {
            var mac = GetClientDetails(null);

            return PartialView(mac);
        }

        private ClientDM GetClientDetails(int? id)
        {
            var clientId = id ?? _adminClientId;
            var model = _clientBL.GetClient(clientId);
            model.ClientLogo = PicHelper.GetClientLogo(model.ClientID);
            return model;
        }
    }
}