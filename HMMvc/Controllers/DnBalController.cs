using Microsoft.Practices.Unity;
using MvcBlox.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVC.Controllers
{
    public class DnBalController : Controller
    {
        private readonly IGlobalManager _globalManager;

        public DnBalController([Dependency]IGlobalManager globalManager)
        {
            _globalManager = globalManager;
        }

        public ActionResult About()
        {
            return PartialView();
        }

    }
}
