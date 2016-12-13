using AutoMapper;
using Common;
using DAL;
using Microsoft.Practices.Unity;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public class QuoteVersionModule
    {
        private readonly IUnitOfWork _uow;
       
        private readonly IRepository<QuoteVersion> _entityDal;
        private readonly IRepository<Quote> _quoteDal;

        public QuoteVersionModule([Dependency]IUnitOfWork uow)
        {
            _uow = uow;
      
            _entityDal = _uow.Repository<QuoteVersion>();
            _quoteDal = _uow.Repository<Quote>();
        }


        /***************************************************/



        internal QuoteVersion GetSingle(int quoteVersionID)
        {
            return _entityDal.SingleOrDefault(x => x.QuoteVersionID == quoteVersionID);
        }


        internal QuoteVersionDM GetSingleDM(int quoteVersionID)
        {
            var entity  =  GetSingle(quoteVersionID);
            var model = new QuoteVersionDM();
          
            EntityToModel(model, entity);

            return model;

        }

        internal List<QuoteVersionDM> GetDropList(int quoteID)
        {
            
            return (from x in GetQuer(quoteID)
                    select new QuoteVersionDM 
                    {
                        QuoteVersionID = x.QuoteVersionID,
                       Version = x.Version,
                       
                    })
                    .OrderByDescending(x=>x.QuoteVersionID)
                    .ToList();
        }

        internal List<QuoteVersionDM> GetList(int quoteID)
        {

            return (from x in GetQuer(quoteID)                
                    select new QuoteVersionDM 
                    {
                        QuoteVersionID = x.QuoteVersionID,
                       Version = x.Version,
                       IsSelected = x.IsSelected,
                       Appendices =x.Appendices,
                       Terms = x.Terms,
                        VersionDate=x.VersionDate
                       
                    })
                    .OrderByDescending(x=>x.QuoteVersionID)
                    .ToList();

         
        }

        internal int GetQuoteId(int id)
        {
            throw new NotImplementedException();
        }

        internal void DeleteByQuote(int id)
        {
            var items = _entityDal.Where(x => x.QuoteID == id).ToList();
            items.ForEach(x => _entityDal.Remove(x));
        }
      
        internal int GetClientID(int id)
        {

            return _entityDal.GetQueryableFresh().Where(x => x.QuoteVersionID == id)
                .Select(x => x.Quote.ClientID).Single();
                 
            //if (quoteVersion == null)
            //    throw new Exception("quoteVersion doesnt exist, id " + id);

            //if (quoteVersion.Quote == null)
            //    throw new Exception("quoteVersion Quote doesnt exist, id " + id);

            //return quoteVersion.Quote.ClientID;
        }

        internal IQueryable<QuoteVersion> GetQuer(int quoteID)
        {
            return _entityDal.Where(x=> x.QuoteID==quoteID);
        }


        /********************         CHANGE       **********************/



        public void Update(QuoteVersionDM model)
        {
           
            if (model.QuoteVersionID > 0)
                Edit(model);
            else
                Add(model);

        }

        internal void Update(QuoteVersion destVersion)
        {
            _entityDal.Update(destVersion);
           
        }

        private void Add(QuoteVersionDM model)
        {
            QuoteVersion entity = new QuoteVersion();
            
            ModelToEntity(model, entity);

            _entityDal.Add(entity);    
        }

        private void Edit(QuoteVersionDM model)
        {
            var entity = GetSingle(model.QuoteVersionID);
            
            ModelToEntity(model, entity);
        }

      
        internal QuoteVersion Copy(int srcVersionID)
        {
            var srcVersion = GetSingle(srcVersionID);

            int lastVersionNumber = GetQuer(srcVersion.QuoteID).Max(x => x.Version);

            return DuplicateVersion(srcVersion, lastVersionNumber);
        }

        private QuoteVersion DuplicateVersion(QuoteVersion srcVersion, int lastVersionNumber)
        {

            var entity = new QuoteVersion {
                QuoteID = srcVersion.QuoteID,
                Version = lastVersionNumber + 1,
                VersionDate = DateTime.Now,
                IsSelected=false,
                Disscount = srcVersion.Disscount,
                Terms = srcVersion.Terms,
                Appendices = srcVersion.Appendices
            };

            _entityDal.Add(entity);

            return entity;
        }


        private void ModelToEntity(QuoteVersionDM model, QuoteVersion entity)
        {        
            Mapper.DynamicMap<QuoteVersionDM, QuoteVersion>(model, entity);
        }

        private void EntityToModel(QuoteVersionDM model, QuoteVersion entity)
        {         
           Mapper.DynamicMap<QuoteVersion, QuoteVersionDM>(entity, model);
        }


        internal void Delete(int quoteVersionID)
        {
            var entity = GetSingle(quoteVersionID);
            _entityDal.Remove(entity);
        }







      
    }
}
