using System.Collections.Generic;
using BL.Moduls;
using Common;
using DAL;
using Microsoft.Practices.Unity;
using Repository;

namespace BL
{
    public class RequestBL
    {

        private readonly IUnitOfWork _uow;
        private readonly IRepository<Request> _requestDal;
        private readonly UserModule _userModule;

        //public RequestBL()
        //    : this(new UserModule(), _uow.Repository<Request>())
        //{

        //}

        public RequestBL([Dependency]IUnitOfWork uow, [Dependency]UserModule userModule)
        {
            _uow = uow;
            _requestDal = _uow.Repository<Request>();
            _userModule = userModule;
        }

       

     

        /***************************************************/


        public List<RequestDM> GetRequestList(int userID, string sortOrder, string searchString)
        {

            int clientID = _userModule.GetClientID(userID);
            return SelectRequestList(clientID, sortOrder, searchString);


        }

      

        public RequestDM GetRequestDetails(int requestID)
        {
            return SelectRequestDetails(requestID);
        }



        public List<RequestDM> SelectRequestList(int clientID, string sortOrder, string searchString)
        {

            //List<RequestDM> requestListDMs =
            //        (from r in _requestDal.GetQueryable()
            //             where x=>x.User.ClientID == ClientID
            //         select new RequestDM
            //         {
            //             RequestID = r.RequestID,
            //             ClientName = r.User.Client.ClientName,
            //             CreatedUserFullName = r.User.FullName,
            //             Date = r.Date,
            //             JobType = Extensions.GetString(r.PickList2.Key),
            //             Priority = Extensions.GetString(r.PickList1.Key),
            //             Status = Extensions.GetString(r.PickList.Key)

            //         }).ToList();


            //    if (!String.IsNullOrEmpty(SearchString))
            //    {
            //        requestListDMs = requestListDMs.Where(s => s.JobType.ToUpper().Contains(SearchString.ToUpper())
            //                                   || s.ClientName.ToUpper().Contains(SearchString.ToUpper())
            //                                   || s.CreatedUserFullName.ToUpper().Contains(SearchString.ToUpper())
            //                                   || s.Priority.ToUpper().Contains(SearchString.ToUpper())
            //                                   || s.Status.ToUpper().Contains(SearchString.ToUpper())
            //                                   ).ToList();
            //    }

            //    switch (SortOrder)
            //    {
            //        case "JobType":
            //            return requestListDMs.OrderBy(s => s.JobType).ToList();
            //        case "JobTypeDesc":
            //            return requestListDMs.OrderByDescending(s => s.JobType).ToList();
            //        case "ClientName":
            //            return requestListDMs.OrderBy(s => s.ClientName).ToList();
            //        case "ClientNameDesc":
            //            return requestListDMs.OrderByDescending(s => s.ClientName).ToList();
            //        case "CreatedUserName":
            //            return requestListDMs.OrderBy(s => s.CreatedUserFullName).ToList();
            //        case "CreatedUserNameDesc":
            //            return requestListDMs.OrderByDescending(s => s.CreatedUserFullName).ToList();
            //        case "Priority":
            //            return requestListDMs.OrderBy(s => s.Priority).ToList();
            //        case "PriorityDesc":
            //            return requestListDMs.OrderByDescending(s => s.Priority).ToList();
            //        case "Date":
            //            return requestListDMs.OrderBy(s => s.Date).ToList();
            //        case "DateDesc":
            //            return requestListDMs.OrderByDescending(s => s.Date).ToList();
            //        case "Status":
            //            return requestListDMs.OrderBy(s => s.Status).ToList();
            //        case "StatusDesc":
            //            return requestListDMs.OrderByDescending(s => s.Status).ToList();
            //        default:
            //            return requestListDMs.OrderByDescending(s => s.Date).ToList();
            //    }

            return null;
        }

       

        public RequestDM SelectRequestDetails(int requestID)
        {

            RequestDM requestDM = new RequestDM();
            Request r = _requestDal.SingleOrDefault(x => x.RequestID == requestID);

            requestDM.ClientName = r.User.Client.ClientName;
            requestDM.CreatedUserID = r.CtratedUserID;
            requestDM.CreatedUserFullName = r.User.FullName;
            requestDM.Date = r.Date;
            requestDM.JobType = Extensions.GetString(r.PickList2.Key);
            requestDM.Priority = Extensions.GetString(r.PickList1.Key);
            requestDM.Status = Extensions.GetString(r.PickList.Key);

            return requestDM;

        }


        public void AddRequest(RequestDM requestDM)
        {
            InsertRequest(requestDM);
        }

        public void InsertRequest(RequestDM requestDM)
        {

            Request r = new Request();
            r.ClientID = requestDM.ClientID;
            r.CtratedUserID = requestDM.CreatedUserID;
            r.Date = requestDM.Date;
            r.JobTypePL = requestDM.JobTypePL;
            r.PriorityID = requestDM.PriorityPL;
            r.StatusID = requestDM.StatusPL;

            _requestDal.Add(r);
            _uow.SaveChanges();

        }
    }
}
