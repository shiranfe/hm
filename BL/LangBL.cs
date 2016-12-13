using BL.Moduls;
using Common;
using DAL;
using Microsoft.Practices.Unity;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Collections;

namespace BL
{
    public class LangBL
    {


        private readonly IUnitOfWork _uow;
        private readonly LangModule _langModule;
        
        //public LangBL()
        //    : this(new LangModule(), new LangCache())
        //{
            
        //}

        public LangBL([Dependency]IUnitOfWork uow, [Dependency]LangModule langModule)
        {
            _uow = uow;
            _langModule = langModule;
           
        }

        /***************************************************/

        public List<KeyValueDM> GetPickListValue(string entity)
        {
            return _langModule.GePickListDM(entity);
        }


        public List<LangStringDM> GetAllLangDictionary()
        {
            return _langModule.GetLangDictionary();
             
        }

        public void UpdateWord(string key, string lang, string word)
        {
            _langModule.UpdateWord(key, lang, word);
            //GetAllLangDictionary();
            //GetLangDictionary();
            _uow.SaveChanges();

            RefreshDictionary();
        }

        public void RefreshDictionary()
        {
            _langModule.RefreshLangDictionary();
            //DateTime lst = _langModule.GetLastDictUpdate();
            //if (SiteGlobals.LangDictModified < lst)
            //{
            //    GetAllLangDictionary();

            //}



        }

        public List<KeyValueDM> GetQuoteIndexStatuses()
        {
            var list = GetPickListValue("QuoteStatus");
            list.Insert(0,new KeyValueDM {  Key = "כל הסטטוסים", PickListID = -1 });
            list.Insert(1, new KeyValueDM { Key = "הצעות פתוחות", PickListID = 100 });
            list.Insert(2,new KeyValueDM { Key = "הצעות סגורות", PickListID = 101 });

            return list;
        }

        public List<KeyValueDM> GetRefubrishStatuses()
        {
            var list = Enum.GetValues(typeof(JobRefubrishStatus)).Cast<JobRefubrishStatus>()
                .Select(x=> new KeyValueDM {
                    PickListID = (int)x,
                    Key = "JobRefubrishStatus_"+x.ToString()
                }).ToList();

            list.Insert(0,new KeyValueDM {  Key = "כל הסטטוסים", PickListID = -1 });
            list.Insert(1, new KeyValueDM { Key = "עבודות פתוחות", PickListID = 100 });
            list.Insert(2,new KeyValueDM { Key = "עבודות סגורות", PickListID = 101 });

            return list;
        }

        public List<KeyValueDM> GetRefubrishSteps()
        {

            var list = new List<KeyValueDM> {
                GetStep(RefubrishStep.DetailsStep),
                GetStep(RefubrishStep.PreTestStep),
                GetStep(RefubrishStep.DisassembleStep),
                GetStep(RefubrishStep.RepairStep),
            };


            return list;
        }

        private static KeyValueDM GetStep(RefubrishStep step)
        {
            return new KeyValueDM { Key = step.ToString(), PickListID = (int)step };
        }
    }
}
