using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using Common;
using DAL;
using Microsoft.Practices.Unity;
using Repository;

namespace BL
{
    public class QuoteModule
    {
        private readonly IRepository<Quote> _entityDal;
        private readonly IRepository<vwQuoteReport> _reportDal;
        //private QuoteVersionModule _quoteVersionModule;
        private readonly MachinePartModule _machinePartModule;
        private readonly IUnitOfWork _uow;

        public QuoteModule([Dependency] IUnitOfWork uow,
            [Dependency] MachinePartModule machinePartModule
            // [Dependency]QuoteVersionModule quoteVersionModule
            )
        {
            _uow = uow;
            // _quoteVersionModule = quoteVersionModule;
            _entityDal = uow.Repository<Quote>();
            _reportDal=uow.Repository<vwQuoteReport>();
            _machinePartModule = machinePartModule;
        }

        /***************************************************/

        private Quote GetSingleFresh(int quoteID)
        {
            return _entityDal.GetQueryableFresh().SingleOrDefault(x => x.QuoteID == quoteID);
        }

      

        internal Quote GetSingle(int quoteID)
        {
            return _entityDal.SingleOrDefault(x => x.QuoteID == quoteID);
        }

        internal QuoteDM GetSingleDM(int quoteID, bool includeJob=true)
        {
            var entity = GetSingleFresh(quoteID);
            return GetSingleDM(entity, includeJob);
        }

        internal List<vwQuoteReport> GetReport()
        {
            return _reportDal.ToList();
        }

        internal QuoteDM GetSingleDM(Quote entity,bool includeJob )
        {
            var model = new QuoteDM();

            EntityToModel(model, entity);

            model.QuoteStatusKey = entity.QuoteStatus.Key;

            model.Creator = entity.Employee.FullName;
            model.ClientName = entity.Client.ClientName;
            if (entity.UserID.HasValue)
                model.ContactName = entity.User.FullName;

            if (includeJob)
                SetJobAndMachineInfo(entity, model);


            return model;
        }


        private void SetJobAndMachineInfo(Quote entity, QuoteDM model)
        {
            if (!entity.Job.Any())
                return;


            var job = entity.Job.First();

            model.JobID = job.JobID;
            model.JobName = job.JobName;
            model.JobDate = job.StartDate;
            model.JobStatus = GlobalDM.GetTransStr(job.JobRefubrish.RefubrishStatus.Key) ; 

            if (job.JobRefubrish?.Machine == null)
                return;

            try
            {
                model.JobRefubrishPartID = job.JobRefubrish.JobRefubrish_Part.First().JobRefubrishPartID;
            }
            catch (Exception)
            { }
         
            var mac = job.JobRefubrish.Machine;
            model.MachineDM = new MachineEditDM
            {
                MachineName = mac.MachineName,
                MachineID= mac.MachineID,
                MacPic = PicHelper.GetMacPic(mac.MachineID, null),
                Rpm = mac.Rpm,
                Kw = mac.Kw,
                Parts = job.JobRefubrish.JobRefubrish_Part
                    .Select(x => _machinePartModule.CreateMachinePartDm(x.MachinePart))
                    .ToList()
            };


            //model.MachineDM.Parts = _machineModule.GetMachineParts();
        }

      

        internal int GetLastVersionID(int quoteID)
        {
            var version = _entityDal.Where(x => x.QuoteID == quoteID)
                .SelectMany(x => x.QuoteVersion).ToList();

            if (!version.Any())
                throw new Exception("quote doesnt have version");

            return version.Last().QuoteVersionID;
        }

        internal void GetList(QuoteFilterDm filter,bool isMaterial = true)
        {
            var quer = GetQuer();

            filter.TableList =  GetListByFilter(quer,filter).Select(x => new QuoteDM
            {
                QuoteID = x.QuoteID,
                ClientID = x.ClientID,
                ClientName = x.Client.ClientName,
                Creator = x.Employee.FirstName + " " + x.Employee.LastName,
                CreatorID = x.CreatorID,
                QuoteTitle = x.QuoteTitle,
                Comments = x.Comments,
                IsCover = x.IsCover,
                QuoteStatusKey = x.QuoteStatus.Key,
                QuoteStatusID = x.QuoteStatusID,
                StatusIsOpen = x.StatusIsOpen,
                FollowDate = x.FollowDate,
                CurrentVersionDate = x.QuoteVersion.Last().VersionDate,
                CurrentVersionID = x.QuoteVersion.Last().QuoteVersionID,
                JobCardNumber = x.JobCardNumber,
                InvoiceNumber = x.InvoiceNumber,
                OrderAttachment = x.OrderAttachment,
                OrderNumber = x.OrderNumber,
                LastTalk = x.QuoteTalk.LastOrDefault()?.Message ?? string.Empty
            }) 
                //   .ThenByDescending(x => x.QuoteID)
                .ToList();
        }


        internal void GetSearchList(QuoteFilterDm filter)
        {
            var quer = GetQuer().Where(x=>x.QuoteVersion.Any(y=>y.QuoteVersionItem.Any()));

            filter.TableList = GetListByFilter(quer, filter).Select(x => new QuoteDM
            {
                //PickListID = x.QuoteID,
                //Key = x.QuoteID + " | " + x.QuoteTitle + " | " + x.Client.ClientName
                QuoteID = x.QuoteID,
                ClientName = x.Client.ClientName,
                CurrentVersionID = x.QuoteVersion.Last().QuoteVersionID,
                Creator = x.Employee.FullName,
                QuoteTitle = x.QuoteTitle,
                FollowDate = x.FollowDate,
                Comments = x.Comments,
                IsCover = x.IsCover,
                JobCardNumber = x.JobCardNumber,
                InvoiceNumber = x.InvoiceNumber
            })
                 .OrderByDescending(x => x.FollowDate)
                .ToList();
        }


        private List<Quote> GetListByFilter(IQueryable<Quote> quer, QuoteFilterDm filter)
        {
            if (filter == null)
                return quer.ToList();

            /** filter by status*/
            var condition = StatusCondtion(filter);

            /** filter by IsCover*/
            if (filter.IsCover.HasValue)
                condition = condition.AndAlso(i=> i.IsCover==filter.IsCover);

            /** filter by CreatorID*/
            if (filter.CreatorID.HasValue && filter.CreatorID>-1)
                condition = condition.AndAlso(i => i.CreatorID == filter.CreatorID);

            /** filter by Srch*/
            if (!string.IsNullOrEmpty(filter.Srch))
                condition = condition.AndAlso(i => i.SearchStr.Contains(filter.Srch) );

            var list = quer.AsEnumerable().Where(condition.Compile()).OrderBy(x => x.FollowDate);

            return LinqHelpers.FilterByPage(filter, list);
        }

        private static Expression<Func<Quote, bool>> StatusCondtion(QuoteFilterDm filter)
        { 
            /** not status sent - defualt status is open*/
            if (!filter.QuoteStatusID.HasValue)
                return i => i.StatusIsOpen;

            /** status from db status list*/
            var dbStatus = Enum.GetValues(typeof(QuoteStatus)).Cast<int>().Any(v=> v==filter.QuoteStatusID);
            if (dbStatus)
                return i =>  i.QuoteStatusID == filter.QuoteStatusID ;


            /** consolidated statuses -  open,  close, all*/
            switch ((int)filter.QuoteStatusID)
            {
                case 100:
                    return i => i.StatusIsOpen; /** open*/
                case 101:
                    return i => !i.StatusIsOpen; /** close*/
                default:
                    return i => i.QuoteID > 0; /** all*/
            }
        }

     

        internal IQueryable<Quote> GetQuer()
        {
            return _entityDal.GetQueryableFresh();
        }

        /********************         CHANGE       **********************/


        public void Update(QuoteDM model)
        { 
            if (model.QuoteID > 0)
                Edit(model);
            else
                Add(model);

        }

        private void Add(QuoteDM model)
        {
            var entity = new Quote {TimeStamp = DateTime.Now};
             
            ModelToEntity(model, entity);
            CreateVersion(entity);

            _entityDal.Add(entity);

            _uow.SaveChanges();

            model.QuoteID = entity.QuoteID;
            model.CurrentVersionID = entity.QuoteVersion.Last().QuoteVersionID;
        }

        private static void CreateVersion(Quote entity)
        {
            entity.QuoteVersion = new List<QuoteVersion>();
            var version = new QuoteVersion
            {
                Version = 1,
                IsSelected = false,
                VersionDate = DateTime.Now,
                Terms = GlobalDM.GetTransStr("Quote_Terms_Defualt")// + (IsHeb() ? "IL" : "En")
            };

            entity.QuoteVersion.Add(version);
        }

        private void Edit(QuoteDM model)
        { 
            var entity = GetSingle(model.QuoteID);
            ModelToEntity(model, entity);

            Update(entity);

            _uow.SaveChanges();
        }
        public void Update(Quote entity)
        {
            _entityDal.Update(entity);

        } 

        private static void ModelToEntity(QuoteDM model, Quote entity)
        {
            Mapper.Map(model, entity);
        }

        private static void EntityToModel(QuoteDM model, Quote entity)
        {
            Mapper.Map(entity, model);
        }

        internal void Delete(int quoteID)
        {
            var entity = GetSingle(quoteID);
          
            _entityDal.Remove(entity);
        }

       
    }
}