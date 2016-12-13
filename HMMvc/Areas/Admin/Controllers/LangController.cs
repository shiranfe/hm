using System;
using System.Linq;
using System.Web.Mvc;
using BL;
using Common;
using Microsoft.Practices.Unity;
//using HMErp.Helper;

namespace MVC.Areas.Admin.Controllers
{
    public class LangController : _BasicController
    {
        private readonly LangBL _langBL;

        public LangController(
            [Dependency] LangBL langBL)
        {
            _langBL = langBL;
        }

    
        public ActionResult EditDictionary()
        {
            var dict = _langBL.GetAllLangDictionary();
            return PartialView(dict);
        }

        [HttpPost]
        public ActionResult UpdateWord(string key, string lang, string word)
        {
            _langBL.UpdateWord(key, lang, word);

            return Json(new { });
        }

        [HttpPost]
        public ActionResult Refresh()
        {
            _langBL.RefreshDictionary();

            return Json(new { });
        }

     
        
    }
}