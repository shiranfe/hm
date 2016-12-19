using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Common;
using DAL;
using Microsoft.Practices.Unity;
using Repository;

namespace BL
{
    public class QuoteTalkModule
    {
        private readonly IUnitOfWork _uow;
       
        private readonly IRepository<QuoteTalk> _entityDal;
      
        public QuoteTalkModule([Dependency]IUnitOfWork uow)
        {
            _uow = uow;
      
            _entityDal = _uow.Repository<QuoteTalk>();
          
        }


        /***************************************************/


        private QuoteTalk GetSingleFresh(int id)
        {
            return _entityDal.GetQueryableFresh().SingleOrDefault(x => x.QuoteTalkID == id);
        }

        private QuoteTalk GetSingle(int id)
        {
            return _entityDal.SingleOrDefault(x => x.QuoteTalkID == id);
        }


        internal QuoteTalkDM GetSingleDM(int id)
        {
            var entity  = GetSingleFresh(id);
            var model = new QuoteTalkDM();
          
            EntityToModel(model, entity);

            model.Creator = entity.Employee.FullName;
            return model;

        }


        internal List<QuoteTalkDM> GetList(int quoteID)
        {
            var quer = GetQuer().Where(x => x.QuoteID == quoteID).ToList();
            return (from x in quer

                    select new QuoteTalkDM 
                    {
                        QuoteTalkID = x.QuoteTalkID,
                        Message = x.Message,
                        Creator = x.Employee.FullName,
                        TalkDate= x.TalkDate
                        
                    })
                    .OrderByDescending(x=>x.TalkDate)
                    .ToList();

        }

        internal IQueryable<QuoteTalk> GetQuer()
        {
            return _entityDal.GetQueryableFresh();
        }


        /********************         CHANGE       **********************/



        public QuoteTalkDM Update(QuoteTalkDM model)
        {
            var entity = (model.QuoteTalkID > 0) ? 
                GetSingle(model.QuoteTalkID) : new QuoteTalk();
            ModelToEntity(model,entity);

            if (model.QuoteTalkID > 0)
                _entityDal.Update(entity);
            else
            {
                entity.TalkDate = DateTime.Now;
                _entityDal.Add(entity);
            }
              

            _uow.SaveChanges();

            return GetSingleDM(entity.QuoteTalkID);

        }

      

        private static void ModelToEntity(QuoteTalkDM model, QuoteTalk entity)
        {        
            Mapper.Map(model, entity);
        }

        private static void EntityToModel(QuoteTalkDM model, QuoteTalk entity)
        {         
           Mapper.Map(entity, model);
        }


        internal void Delete(int templateID)
        {
            var entity = GetSingle(templateID);
            _entityDal.Remove(entity);
        }
    }
}
