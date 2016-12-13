using System.Web.Mvc;
using MVC.Models;
using Common;
using BL;
using System.Web.Security;
//using HMErp.Helper;
using Microsoft.Practices.Unity;
using MvcBlox.Models;

namespace MVC.Controllers
{
    public class UserController : Controller
    {

        private readonly UserBL _userBL;
      
        private readonly IGlobalManager _globalManager;


        public UserController([Dependency]UserBL userBL, [Dependency]IGlobalManager globalManager)
        {
            _globalManager = globalManager;
            _userBL = userBL;
           
        }

        [HttpPost]
        public ActionResult ChangeLang()
        {
            var oldlang = Request.Cookies["lang"].Value.ToString();
            var lng = oldlang == "en-US" ? "he-IL" : "en-US";
            Extensions.SetSysLangCookie(lng);
            //Common.GlobalDM.GetLangDictionary();
            return PartialView();
        }

        [HttpPost]
        public ActionResult ChangeClient(int clientID)
        {
            Session["ClientID"] = clientID;
            return Json(new { sts = 1 });
        }

        public ActionResult Login()
        {
            return PartialView(new LoginVM());
        }

        [HttpPost]
        public ActionResult Login(LoginVM loginVm)
        {
            if (ModelState.IsValid)
            {
                //int res;
                Extensions.Strlz(loginVm);

                var userAccountDM = loginVm.ToUserAccountDM();
                var userLoged = _userBL.IsLoginValid(userAccountDM);

                if (userLoged != null)
                {
                    FormsAuthentication.SetAuthCookie(userLoged.UserID.ToString(), loginVm.IsRemember);
                    Session["UserID"] = userLoged.UserID;
                    Session["ClientID"] = userLoged.ClientID;
                    Session["RootClientID"] = userLoged.ClientID;
                    Session["EmpID"] = null;
                    return Redirect("/");
                }
                else
                {
                    ModelState.AddModelError("", "");
                }
            }
            else
            {
                //ModelState.AddModelError("", "Login data is incorrect!");
            }

            // If we got this far, something failed, redisplay form
            return PartialView();
        }

     
        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();

            return RedirectToAction("Login", "User");
        }


        public ActionResult Header(int? clientID)
        {
            if ( clientID.HasValue)
            {
                var clnt = new UserLayoutDM
                {
                    ClientLogo = PicHelper.GetClientLogo((int)clientID ),
                    Lang = Session["lang"].ToString(),
                    ClientID = (int)clientID
                };

                return PartialView(clnt);
            }
            else if (Session["UserID"] != null)
            {
                var userID = (int)Session["UserID"];
                var clnt = _userBL.GetUserLayout(userID);

                var clntID = clnt.ClientID;//ClientID.HasValue ? (int)ClientID :
                clnt.ClientLogo = PicHelper.GetClientLogo(clntID);
                clnt.Lang = Session["lang"].ToString();
                return PartialView(clnt);
            }
            
            return PartialView(new UserLayoutDM());
        }

        public ActionResult HeaderNotLoged()
        {

            var clnt = new UserLayoutDM
            {
                Lang = Session["lang"].ToString()
            };

            return PartialView(clnt);
        }

        //public ActionResult UserDetails()
        //{
        //    int UserID = (int)Session["UserID"];
        //    UserDetailsDM userDetailsDM = _userBL.GetUserDetails(UserID);
        //    UserDetailsVM userDetailsVM = new UserDetailsVM(userDetailsDM);

        //    return PartialView(userDetailsVM);
        //}

        //[HttpPost]
        //public ActionResult UserDetails(UserDetailsVM UserDetailsVM)
        //{
        //    Extensions.Strlz(UserDetailsVM);
        //    _userBL.Update(UserDetailsVM.ToUserDetailsDM());
        //    return PartialView(UserDetailsVM);
        //}

       
    }


}
