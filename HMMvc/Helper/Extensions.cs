using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Globalization;
using System.Threading;
using System.Web;
using System.Resources;
using Common;

namespace MVC
{
    public class Extensions : Common.Extensions
    {
        //public static object CopyObject(object ObjectFrom, object ObjectTo)
        //{
        //    foreach (PropertyInfo propTo in ObjectTo.GetType().GetProperties())
        //    {
        //        PropertyInfo propFrom = ObjectFrom.GetType().GetProperty(propTo.Name);
        //        propTo.SetValue(ObjectTo, propFrom.GetValue(ObjectFrom, null), null);
        //    }
        //    return ObjectTo;
        //}

        //public static void Strlz(object Object)
        //{

        //        //PropertyInfo[] properties = Object.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
        //        //foreach (PropertyInfo prop in properties)
        //        //{
        //        //    switch (prop.PropertyType[0].Replace("System.", ""))
        //        //    {
        //        //        case "bool": prop.SetValue(Object, Convert.ToBoolean(prop.GetValue(Object)));
        //        //            break;
        //        //        //case "string": prop.SetValue(Object, HttpUtility.HtmlDecode(prop.GetValue(Object)));
        //        //        //    break;
        //        //        case "string": prop.SetValue(Object, (string)(prop.GetValue(Object)));
        //        //            break;
        //        //        case "int": prop.SetValue(Object, Convert.ToInt32((int)prop.GetValue(Object)));
        //        //            break;
        //        //        case "date": prop.SetValue(Object, (DateTime)prop.GetValue(Object));
        //        //            break;
        //        //        default: prop.SetValue(Object, Convert.ToBoolean(prop.GetValue(Object)));
        //        //            break;
        //        //    }
        //        //}
            
        //}

        public static void SetSysLangCookie(string lng)
        {
            HttpContext.Current.Session["lang"] = lng;
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(lng);
            Thread.CurrentThread.CurrentCulture = new CultureInfo(lng);

            var languageCookie = new HttpCookie("lang")
            {
                Value = Thread.CurrentThread.CurrentCulture.ToString(),
                Expires = DateTime.Now.AddYears(1)
            };
            HttpContext.Current.Response.Cookies.Add(languageCookie);

            languageCookie = new HttpCookie("dir")
            {
                Expires = DateTime.Now.AddYears(1),
                Value = Thread.CurrentThread.CurrentCulture.TextInfo.IsRightToLeft.ToString()
            };
            HttpContext.Current.Response.Cookies.Add(languageCookie);

            HttpContext.Current.Session["dir"] = languageCookie.Value;
        }
    }
}
