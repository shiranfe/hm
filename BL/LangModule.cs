using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;
using Common;
using Microsoft.Practices.Unity;
using Repository;
using System.Linq.Expressions;

namespace BL.Moduls
{
    public class LangModule
    {
        private readonly IUnitOfWork _uow; 
        private readonly IRepository<PickList> _pickListDal;
        private readonly IRepository<LangString> _langDal;
        private readonly IRepository<LastUpdate> _lastUpdateDal;
        private readonly LangCache _langRepository;
        //public LangModule()
        //    : this(_uow.Repository<PickList>(), _uow.Repository<LangString>(), _uow.Repository<LastUpdate>())
        //{

        //}

        public LangModule([Dependency]IUnitOfWork uow, [Dependency]LangCache langRepository)
        {
            _uow = uow;
            _pickListDal = _uow.Repository<PickList>();
            _langDal = _uow.Repository<LangString>();
            _lastUpdateDal = _uow.Repository<LastUpdate>();
             _langRepository = langRepository;
        }



        /***************************************************/

        public List<PickList> GePickList()
        {
            return _pickListDal.ToList();
        }

        public List<PickList> GePickList(string entity)
        {
            return _pickListDal.Where(x => x.Entity == entity).ToList();
        }       
        
        public List<KeyValueDM> GePickListDM(string entity)
        {
            return GePickListDMQuer(entity).ToList();
        }

        public IQueryable<KeyValueDM> GePickListDMQuer(string entity)
        {
            return _pickListDal.Where(x => x.Entity == entity).Select(x => new KeyValueDM
            {
                PickListID = x.PickListID,
                Key = x.Key,
                ExtraValue = x.Value
            });
        }

        public List<KeyValueDM> GePickListDM(Expression<Func<PickList, bool>> condition)
        {
            return _pickListDal.Where(condition).Select(x => new KeyValueDM
            {
                PickListID = x.PickListID,
                Key = x.Key
            }).ToList().OrderBy(x=>x.TransStr).ToList();
        }

       


        /* public string GeTransStr(string key)
         {
             return db.LangString.Where(x => x.Key == key).Select(x => x.IL).SingleOrDefault();
           
         }*/
        public DateTime GetLastDictUpdate()
        {
            return _lastUpdateDal.Where(d=> d.Entity == "Dictionary")
                .Select(d=>  d.DateModified).Single();
       
        }

        public List<LangStringDM> GetAllLangDictionary()
        {

            return (from d in _langDal.GetQueryableFresh()
                    select new LangStringDM
                    {
                        Key = d.Key,
                        EN = d.EN,
                        IL = d.IL
                    }).ToList();

        }

        internal List<LangStringDM> GetLangDictionary()
        {
            return _langRepository.GetAllLangDictionary();
        }

        internal void RefreshLangDictionary()
        {
             _langRepository.LoadDictionary();
        }



        public void UpdateWord(string key, string lang, string word)
        {

            var rw = _langDal.SingleOrDefault(x => x.Key == key);
            switch (lang)
            {
                case "il":
                    rw.IL = word;
                    break;
                case "en":
                    rw.EN = word;
                    break;
                default:
                    break;
            }

            

        }

        public List<VbStatusDM> GetVbStatusDM()
        {
            return _pickListDal
                .Where(x => x.Entity == "JobVibrationStatus" && x.Value != "6")
                .Select(x => new VbStatusDM
                {
                    StatusID = x.Value,
                    LangStr = x.Key
                }).ToList();
        }

        public int GetPickListIDForVbStatus(string statusID)
        {
            return _pickListDal
                .SingleOrDefault(x => x.Entity == "JobVibrationStatus" && x.Value == statusID)
                .PickListID;
        }


        /*************************     ADD      **************************/


        internal void AddOrUpdateWord(LangString langString)
        {
            var entity =_langDal.SingleOrDefault(x => x.Key == langString.Key);

            if (entity == null)
            {
                _langDal.Add(langString);
                return;
            }

            entity.IL = langString.IL;
            entity.EN = langString.EN;

            _langDal.Update(entity);
        }

     
    }
}
