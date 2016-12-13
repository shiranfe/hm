using AutoMapper;
using Common;
using Common.Helpers;
using DAL;
using Microsoft.Practices.Unity;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BL
{
    public class BugLogBL 
    {

        private readonly IUnitOfWork _uow;

        private readonly IRepository<BugLog> _bugLogDal;

        public BugLogBL([Dependency]IUnitOfWork uow)
        {
            _uow = uow;
            _bugLogDal = _uow.Repository<BugLog>() ;

        }

        public void Add(BugLogDM model)
        {
            try
            {
                var entity = new BugLog();
                ModelToEntity(model, entity);

                _bugLogDal.Add(entity);

            }
            catch (Exception e)
            {
                _bugLogDal.Add(new BugLog
                {
                    CreationTime = DateTime.Now,
                    Message = e.Message,
                    InnerException = ErrorHelper.InnerExceptionMessage(e),
                    StackTrace = e.StackTrace,
                    Status = "log error"
                });
            }

            _uow.SaveChanges();
        }

        public List<BugLogDM> Get()
        {
            var list = _bugLogDal.ToList().Select(Mapper.DynamicMap<BugLogDM>)
                .OrderByDescending(x=>x.BugID)
                .ToList();
         
            return list;
        }

        private static void ModelToEntity(BugLogDM model, BugLog entity)
        {
            Mapper.DynamicMap(model, entity);
        }

        public void Delete(int id)
        {
            var entity = _bugLogDal.SingleOrDefault(x => x.BugID == id);
            _bugLogDal.Remove(entity);
            _uow.SaveChanges();
        }
    }


}
