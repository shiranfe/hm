using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Common;
using BL;
using Microsoft.Practices.Unity;
using MvcBlox.Models;


namespace MVC.Controllers
{
    public class MenuController : Controller
    {


        private readonly ClientBL _clientBL;
        private readonly IGlobalManager _globalManager;

        public MenuController([Dependency]ClientBL clientBL, [Dependency]IGlobalManager globalManager)
        {
            _globalManager = globalManager;
          
            _clientBL = clientBL;
        }



        public ActionResult MenuUser()
        {
            return PartialView();
        }

        public ActionResult MenuMachine()
        {
            var clientID = (int)Session["RootClientID"];
            var node = _clientBL.GetClientTree(clientID);
            var tree = new List<ClientTreeDM>();
            tree.Add(node);
            return PartialView(tree);
        }

        public ActionResult Scroll()
        {
            return PartialView();
        }

    }
}
