using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Microsoft.Practices.Unity;
using Repository;
using DAL;

namespace BL
{
    public class QuoteTrackBL
    {
        private readonly IUnitOfWork _uow;

        private readonly QuoteModule _quoteModule;
        private readonly QuoteTalkModule _quoteTalkModule;
       
        // private readonly QuoteJobModule _quoteJobModule;
        public QuoteTrackBL([Dependency]IUnitOfWork uow,
          // [Dependency]QuoteJobModule quoteJobModule,
            [Dependency]QuoteTalkModule module,
            [Dependency]QuoteModule quoteModule)
        {
            _uow = uow;
            _quoteTalkModule = module;
            _quoteModule = quoteModule;
            //   _quoteJobModule = quoteJobModule;
        }

       

        public QuoteDM GetSingleItemDM(int id)
        {
            var model = _quoteModule.GetSingleDM(id, true);

            model.Talks = _quoteTalkModule.GetList(model.QuoteID);

            return model;
        }

     

        public List<QuoteDM> GetItemsList(bool dontFilter = false)
        {
            var quer = _quoteModule.GetQuer();
            if (!dontFilter)
            {
                var excludeIds = new[] { (int)QuoteStatus.Done, (int)QuoteStatus.Rejected };
                quer = quer.Where(x => !excludeIds.Contains(x.QuoteStatusID));
            }
             
            return (from x in quer.ToList()
                    let version = x.QuoteVersion.Last()
                    select new QuoteDM
                    {
                        QuoteID = x.QuoteID,
                        ClientID = x.ClientID,
                        ClientName = x.Client.ClientName,
                        QuoteTitle = x.QuoteTitle,
                        Comments = x.Comments,
                        QuoteStatusID = x.QuoteStatusID,
                        QuoteStatusKey = x.QuoteStatus.Key,
                        StatusIsOpen = x.StatusIsOpen,
                        Creator = version.Quote.Employee.FullName,
                        CreatorID= version.Quote.CreatorID,
                        CurrentVersionDate = version.VersionDate,
                        CurrentVersionID = version.QuoteVersionID,
                        IsCover = x.IsCover,
                        FollowDate = x.FollowDate,
                        OrderAttachment=x.OrderAttachment,
                        OrderNumber=x.OrderNumber
                    })
                    .OrderByDescending(x => x.FollowDate)
                    .ThenByDescending(x=>x.CurrentVersionDate)
                    .ToList();
        }


        public void Update(QuoteDM model)
        { 
            var entity = _quoteModule.GetSingle(model.QuoteID);
            if (entity.OrderNumber != null && model.OrderNumber == null)
            {
                var _bugLogDal = _uow.Repository<BugLog>();
                _bugLogDal.Add(new BugLog
                {
                    CreationTime = DateTime.Now,
                    Message = "try to set OrderNumber as null when its not QuoteID " + model.QuoteID,
                });
            }

            entity.OrderAttachment = model.OrderAttachment;
            entity.OrderNumber = model.OrderNumber;
            entity.FollowDate = model.FollowDate;
            entity.QuoteStatusID = model.QuoteStatusID;
            entity.InvoiceNumber = model.InvoiceNumber;

            _quoteModule.Update(entity);

            _uow.SaveChanges();
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public string DeleteAttachment(int id)
        {
            var entity = _quoteModule.GetSingle(id);
            string fileName = entity.OrderAttachment;

            entity.OrderAttachment = null;

            _quoteModule.Update(entity);

            _uow.SaveChanges();

            return fileName;

        }

        public void UpdateOrderAttchment(int id, string fileName)
        {
            var entity = _quoteModule.GetSingle(id);
            entity.OrderAttachment = fileName;
            _quoteModule.Update(entity);

            _uow.SaveChanges();
        }
    }
}
