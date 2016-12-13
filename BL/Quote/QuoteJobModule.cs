using AutoMapper;
using Common;
using DAL;
using Microsoft.Practices.Unity;
using Repository;
using System.Collections.Generic;
using System.Linq;

namespace BL
{
    public class QuoteJobModule
    {
        private readonly IUnitOfWork _uow;
        private readonly QuoteModule _quoteModule;

        private readonly IRepository<Job> _entityDal;
      
        public QuoteJobModule([Dependency]IUnitOfWork uow,
             [Dependency]QuoteModule quoteModule)
        {
            _uow = uow;
            _quoteModule = quoteModule;
            _entityDal = _uow.Repository<Job>();
          
        }


        /***************************************************/



        private Job GetSingle(int jobID)
        {
            return _entityDal.SingleOrDefault(x => x.JobID == jobID);
        }


        //internal QuoteJobDM GetSingleDM(int QuoteJobID)
        //{
        //    var entity  =  GetSingle(QuoteJobID);
        //    var model = new QuoteJobDM();
          
        //    EntityToModel(model, entity);

        //    return model;

        //}


        internal List<QuoteJobDM> GetList(int quoteID)
        {

            var quer = GetQuer().Where(x=>x.QuoteID ==quoteID);

            return CreateJobList(quer);

         
        }

        private static List<QuoteJobDM> CreateJobList(IQueryable<Job> quer)
        {
            return (from x in quer

                    select new QuoteJobDM
                    {
                        JobID = x.JobID,
                        JobName = x.JobName,
                        StartDate = x.StartDate,
                    })
                    .OrderByDescending(x => x.JobID)
                    .ToList();
        }


        internal List<QuoteJobDM> GetUnlinkedJobsByClient(int clientID)
        {
            var quer = GetQuer()
                .Where(x => x.ClientID == clientID && !x.QuoteID.HasValue);

            return CreateJobList(quer);
        }

        internal IQueryable<Job> GetQuer()
        {
            return _entityDal.GetQueryableFresh();
        }


        /********************         CHANGE       **********************/



        public void Update(int jobID, int? quoteID)
        {

            var entity = GetSingle(jobID);

            entity.QuoteID = quoteID;

        }



        /// <summary>
        /// somwhere in refubrish procces decide that needs quote
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        internal int CreateQuoteFromJob(Job job, int creatorID)
        { 
             
            QuoteDM quoteDm = new QuoteDM
            {
                QuoteTitle = job.JobName,
                ClientID = job.ClientID,
                UserID = job.Client.User.FirstOrDefault()?.UserID ?? null,
                CreatorID = creatorID,
                QuoteStatusID = 541, //wating for quote
            };

            _quoteModule.Update(quoteDm);

            job.QuoteID = quoteDm.QuoteID;

            return quoteDm.QuoteID;
        }

        //private void ModelToEntity(QuoteJobDM model, Job entity)
        //{        
        //    Mapper.DynamicMap<QuoteJobDM, Job>(model, entity);
        //}

        //private void EntityToModel(QuoteJobDM model, Job entity)
        //{         
        //   Mapper.DynamicMap<Job, QuoteJobDM>(entity, model);
        //}


        //internal void Delete(int QuoteJobID)
        //{
        //    var entity = GetSingle(QuoteJobID);
        //    _entityDal.Remove(entity);
        //}


    }
}
