using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;
using BL.Moduls;
using Repository;
using Microsoft.Practices.Unity;

namespace BL
{
    public class LangCache : Cache
    {
         //private CacheModule _cacheModule;
        private readonly IUnitOfWork _uow;
        private readonly IRepository<LangString> _langstringDal;



        public LangCache([Dependency]IUnitOfWork uow)
        {
            _uow = uow;
            _langstringDal = _uow.Repository<LangString>();
          //  _cacheModule = cacheModule;
          
        }

        /*****************         Methods              ******************/



        public List<LangStringDM> GetAllLangDictionary()
        {
  
            return SiteGlobals.Dict ?? LoadDictionary();
        }

        public List<LangStringDM> LoadDictionary()
        {
            SiteGlobals.Dict = _langstringDal.GetQueryableFresh()
                .Select(x => new LangStringDM
                {
                    EN = x.EN,
                    IL = x.IL,
                    Key = x.Key,
                }).ToList();

            return SiteGlobals.Dict; 
        }

        public  List<PickListDM> GetHebLangDictionary()
        {
            try
            {
                
                return (from d in GetAllLangDictionary()
                        select new PickListDM
                        {
                            Value = d.Key,
                            Text = d.IL,
                        }).ToList();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public  List<PickListDM> GetEngLangDictionary()
        {
            try
            {
              
                return (from d in GetAllLangDictionary()
                        select new PickListDM
                        {
                            Value = d.Key,
                            Text = d.EN,
                        }).ToList();
            }
            catch (Exception)
            {

                throw;
            }
        }

      


    }
}
