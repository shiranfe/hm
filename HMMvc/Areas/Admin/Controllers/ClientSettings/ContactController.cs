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
    public class ContactController : _BasicController
    {
        private readonly ContactBL _contactBL;
        private readonly ClientBL _clientBL;

        public ContactController(
            [Dependency]ContactBL contactBL, 
            [Dependency]ClientBL clientBL, 
            [Dependency] IGlobalManager globalManager)
        {
            _contactBL = contactBL;
            _clientBL = clientBL;
            _adminClientId = globalManager.GetAdminClientID();
        }

        private readonly int _adminClientId;
       
 
     
        public ActionResult Index()
        {
            ViewBag.ClientID = _adminClientId;
          //  var users = _userBL.GetUsersIndex(_adminClientId);
            //PopulateClientDrop(new UserDetailsDM { ClientID= 259 } );
            return PartialView();
        }

 public ActionResult ClientContacts(int ClientID)
        {
            
            var users = _contactBL.GetIndex(ClientID);
            //PopulateClientDrop(new UserDetailsDM { ClientID= 259 } );
            return PartialView(users);
        }


        public ActionResult GetClientContacts(int id)
        {

            var model =_contactBL.GetIndex(id).Select(x => new {
                Value = x.UserID.ToString(),
                Text = x.FullName
            }) .ToList();
                
            return Json(model, JsonRequestBehavior.AllowGet);
        }


        public ActionResult Update(int? id, int? ForeignID)
        {

            UserDetailsDM model;
           
            if (id.HasValue)
            {
                model = _contactBL.Get((int)id);
                ViewBag.PopTitle = "עדכון משתמש";
            }
            else
            {
                model = new UserDetailsDM { 
                    ClientID = ForeignID.HasValue ? (int)ForeignID : _adminClientId ,
                    ContactInfoDM = new ContactInfoDM()
                };

                ViewBag.PopTitle = "משתמש חדש";
            }
            PopulateClientDrop(model);

            return PartialView(model);
        }

        private void PopulateClientDrop(UserDetailsDM model)
        {

            model.Clients = _clientBL.GetClientDrop();
            //var sel = new SelectList(_clientBL.GetClientList(), "ClientID", "ClientName", id);
            //ViewBag.Client = sel;
        }

        [HttpPost]
        public ActionResult Update(UserDetailsDM model, ContactInfoDM contactInfoDM)
        {
            try
            {
                model.ContactInfoDM = contactInfoDM;
                _contactBL.Update(model);
                return RedirectToAction("ClientContacts",new { ClientID = _adminClientId==0 ? model.ClientID : _adminClientId });
            }
            catch (Exception e)
            {
                return ExceptionObj(e);
                //modeds.AddFormError("No class was selected.");
            }
          
            // PopulateClientDrop();

            
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            try
            {
                int ClientID = _contactBL.Delete(id);
                return RedirectToAction("ClientContacts", new { ClientID = _adminClientId == 0 ? ClientID : _adminClientId });
            }
            catch (Exception e)
            {
                return ExceptionObj(e);
                //modeds.AddFormError("No class was selected.");
            }

  
        } 
      

        



    }
}
