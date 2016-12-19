using BL;
using Common;
using Microsoft.Practices.Unity;
using MvcBlox.Models;
using PagedList;
using System.Web.Mvc;

namespace MVC.Controllers
{
    public class RequestController : Controller
    {
        int _userID=1;//(int)session["UserID"];

        private readonly RequestBL _requestBL;
        private readonly LangBL _langBL;
        private readonly UserBL _userBL;
        private readonly ClientBL _clientBL;
        private readonly IGlobalManager _globalManager;


        public RequestController([Dependency]RequestBL requestBL, [Dependency]LangBL langBL, [Dependency]UserBL userBL, [Dependency]ClientBL clientBL, [Dependency]IGlobalManager globalManager)
        {
            _globalManager = globalManager;
            _langBL = langBL;
            _userBL = userBL;
            _clientBL = clientBL;

            _requestBL = requestBL;
        }

        public ActionResult RequestList(string sortOrder, string currentFilter, string searchString, int? page)
        {
            if (Request.HttpMethod == "GET")
            {
                searchString = currentFilter;
            }
            else
            {
                page = 1;
            }
            ViewBag.CurrentFilter = searchString;
            ViewBag.CurrentSort = sortOrder;
            ViewBag.JobType = sortOrder == "JobType" ? "JobTypeDesc" : "JobType";
            ViewBag.ClientName = sortOrder == "ClientName" ? "ClientNameDesc" : "ClientName";
            ViewBag.CreatedUserName = sortOrder == "CreatedUserName" ? "CreatedUserNameDesc" : "CreatedUserName";
            ViewBag.Priority = sortOrder == "Priority" ? "PriorityDesc" : "Priority";
            ViewBag.Date = sortOrder == "Date" ? "DateDesc" : "Date";
            ViewBag.Status = sortOrder == "Status" ? "StatusDesc" : "Status";

            var pageSize = 10;
            var pageNumber = (page ?? 1);

            return PartialView(_requestBL.GetRequestList(_userID, sortOrder, searchString).ToPagedList(pageNumber, pageSize));
        }

        public ActionResult RequestDetails(int id)
        {
            var requestDM = new RequestDM();
            requestDM = _requestBL.GetRequestDetails(id);

            if (requestDM.CreatedUserFullName == null)
            {
                requestDM.CreatedUserFullName = _userBL.GetUserFullName(_userID);
            }

            requestDM.Locked = id > 0 && requestDM.CreatedUserID != _userID;

            var clientID = _userBL.GetClientID(_userID);

            ViewBag.JobType = new SelectList(_langBL.GetPickListValue("JobType"), "Key", "TransStr");
            ViewBag.Client = new SelectList(_clientBL.GetClientList(), "ClientID", "ClientName");
            ViewBag.Priority = new SelectList(_langBL.GetPickListValue("JobPriority"), "Key", "TransStr");

            return PartialView(requestDM);
        }

        //[HttpPost]
        //public ActionResult RequestDetails(RequestDM RequestDM)
        //{
        //    _requestBL.AddRequest(RequestDM);
        //    return RedirectToAction("RequestList");
        //}
    }
}