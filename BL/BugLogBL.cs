using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Common;
using Common.Helpers;
using DAL;
using Microsoft.Practices.Unity;
using Repository;

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
            using (var db = new HMErpEntities())
            {
                var bugLogDal = db.BugLog;
                try
                {
                    var entity = new BugLog();
                    ModelToEntity(model, entity);

                    bugLogDal.Add(entity);

                }
                catch (Exception e)
                {
                    bugLogDal.Add(new BugLog
                    {
                        CreationTime = DateTime.Now,
                        Message = ErrorHelper.ExceptionMessage(e),
                        StackTrace = e.StackTrace,
                        Status = "log error"
                    });
                }

                db.SaveChanges();
            }
          
        }

        public List<BugLogDM> Get()
        {
            var list = _bugLogDal.GetQueryableFresh().Select(Mapper.Map<BugLogDM>).OrderByDescending(x=>x.BugID)
                .ToList();
         
            return list;
        }

        private static void ModelToEntity(BugLogDM model, BugLog entity)
        {
            Mapper.Map(model, entity);
        }

        public void Delete(int id)
        {
            var entity = _bugLogDal.SingleOrDefault(x => x.BugID == id);
            _bugLogDal.Remove(entity);
            _uow.SaveChanges();
        }
    }


}
