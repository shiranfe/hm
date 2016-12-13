using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class SiteGlobals
    {
        //public static string Lang = "he-IL";
        //public static List<PickListDM> LangDict;

        public  static DateTime LangDictModified = DateTime.Now;
       
        public  static List<LangStringDM> Dict;
        public static decimal Vat = 0.17M;
       
    }

    
}
