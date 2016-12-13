using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BL;
using MVC.Models;
using Microsoft.Practices.Unity;
using MvcBlox.Models;

namespace MVC.Controllers
{
    public class ContactController : Controller
    {
        int userID;//(int)session["UserID"];

        private UserBL _userBL;
        private LangBL _langBL;
        private readonly IGlobalManager _globalManager;

        public ContactController([Dependency]UserBL UserBL, [Dependency]LangBL langBL, [Dependency] IGlobalManager globalManager)
        {
            _globalManager = globalManager;
            _userBL = UserBL;
            _langBL = langBL;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ClientContactList()
        {
            List<ClientContactDM> clientContactListDM = new List<ClientContactDM>();
            clientContactListDM = _userBL.GetClientContactList(userID);
            foreach (var item in clientContactListDM)
            {
                item.PermissionNameLang = Extensions.GetString(item.PermissionNameLang);
            }

            return View(clientContactListDM);
        }

        public ActionResult ClientContactAdd()
        {
            UserDetailsVM userDetailsVM = new UserDetailsVM();

            List<KeyValueDM> l = new List<KeyValueDM>();
            l = _langBL.GetPickListValue("Permission");
            
            //ViewBag.Permission = new SelectList(GlobalBL.GetPickListValue("Permission"), "PickListPL", 
            //    "TransStr", userDetailsVM.PermissionPL);

            ViewBag.Client = new SelectList(_userBL.GetClientListByUserID(userID), "PickListPL", "LangStr", userDetailsVM.ClientID);
            return View(userDetailsVM);
        }

        [HttpPost]
        public ActionResult ClientContactAdd(UserDetailsVM UserDetailsVM)
        {
            _userBL.AddContact(UserDetailsVM.ToUserDetailsDM());
            return View();
        }



    }
}
