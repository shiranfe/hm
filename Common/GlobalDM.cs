using System;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace Common
{
    public class GlobalDM : IRequiresSessionState
    {
        public static string GetTransStr(string key)
        {
            var ans = HttpContext.Current.Session["lang"].ToString() == "he-IL"
                ? GetTransByLang(key, x => x.IL ?? key)
                : GetTransByLang(key, x => x.EN ?? key);
            return ans;
        }

        internal static string GetTransByLang(string key, Func<LangStringDM, string> func)
        {
            
            var str = SiteGlobals.Dict.Where(x => x.Key == key).Select(func).SingleOrDefault();


            return str ?? key;
        }
    }

    public class Lang
    {
        public static string GetTransStr(string key)
        {
            return GlobalDM.GetTransStr(key);
        }

        public static string GetEngStr(string key)
        {
            return GlobalDM.GetTransByLang(key, x => x.EN ?? key);
        }

        public static string GetHebStr(string key)
        {
            return GlobalDM.GetTransByLang(key, x => x.IL ?? key);
        }
    }

    public class KeyValueDM
    {
        public int PickListID { get; set; }
        public string Key { get; set; }

        public string TransStr
        {
            get { return GlobalDM.GetTransStr(Key); }
        }
        public int CategoryID { get; set; }
        public string ExtraValue { get; set; }
    }

    public class PickListDM
    {
        public string Value { get; set; }
        public string Text { get; set; }
    }

    public class DropListDM
    {
        public int id { get; set; }
        public string Text { get; set; }
    }

    public class FilterSortPageDM
    {
        public string Filter { get; set; }
        public string Sort { get; set; }
        public bool Direction { get; set; }
        public int? Page { get; set; }
    }

    public class LangStringDM
    {
        public string Key { get; set; }
        public string EN { get; set; }
        public string IL { get; set; }
    }
}