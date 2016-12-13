using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Common.Resources;

namespace HMErp
{
    public class FormatHelper
    {
        

        internal static string DynamicStarEnd(DateTime? st, DateTime? en, out int res)
        {
            //try
            //{
                res = 1;
                string str="" ;//= StartTime + "" + EndTime;
                string DateFormat = "dd'/'MM'/'yy";
                if (!st.HasValue)
                {
                    if (en.HasValue)
                    {
                        str = en.Value.Date.ToString(DateFormat);
                        str += SetTime(null, en.Value.TimeOfDay, out res);
                    }
                    
                }
                else
                {
                    if (!en.HasValue)
                    {
                        str = st.Value.Date.ToString(DateFormat);
                        str += SetTime(st.Value.TimeOfDay, null,  out res);
                    }
                    else
                    {
                       
                        if (st.Value.Date == en.Value.Date)
                        {
                            if (st.Value.Year == en.Value.Year && st.Value.Month == en.Value.Month)
                            {
                                str = st.Value.ToString("dd'/'MM") + "-" + en.Value.ToString("dd'/'MM");
                            }
                            else
                            {
                                str = st.Value.Date + "-" + en.Value.Date;
                            }
                        }
                        else
                        {
                            str = st.Value.Date.ToString(DateFormat);
                        }

                        str += SetTime(st.Value.TimeOfDay, en.Value.TimeOfDay, out res);
                    }
                }
                return str;
           // }
            //catch (Exception)
            //{
            //    res = 345;
            //    throw;
            //}
            
        }

        private static string SetTime(TimeSpan? st, TimeSpan? en, out int res)
        {
            //try
           // {
                res = 1;
                string str="";
                string timeFormat = "hh':'mm";
                if (st.HasValue &&  st.ToString() != "00:00:00")
                {
                    if (en.HasValue &&  en.ToString() != "00:00:00")
                    {
                        str = " " + st.Value.ToString(timeFormat) + "-" +
                            en.Value.ToString(timeFormat);
                    }
                    else
                    {
                        str = " " + st.Value.ToString(timeFormat);
                    }
                            
                }
                else
                {
                    if (en.HasValue &&  en.Value.ToString() != "00:00:00")
                    {
                        str = " " + en.Value.ToString(timeFormat);
                    }
                }

                return str;
            //}
            //catch (Exception)
            //{
            //    res = 634;
            //    throw;
            //}
        }

        internal static string DynDateTime(DateTime? DynDate)
        {
            if (DynDate != null)
            {
                string DateFormat = "dd'/'MM'/'yy";

                if (DynDate.Value.CompareTo(DateTime.Now) > 0)
                {
                    TimeSpan gap = DynDate.Value - DateTime.Now;
                    string ans = "";
                   int days= gap.Days;
                    if (days == 0)
                    {
                        int hrs = gap.Hours;
                        ///< do by hrs, by min..
                        ans = Global.Date_Now;
                    }
                    else if(days==1){
                        ans = Global.Date_Tommrow;
                    }
                    else if (days == 2)
                    {
                        ans = Global.Date_InTwoDays;
                    }
                    else if (days <=7 )
                    {
                        ans = Global.Date_ThisWeek;
                    }
                    else if (days <= 14)
                    {
                        ans = Global.Date_NextWeek;
                    }
                    else if (days <= 30)
                    {
                        ans = Global.Date_ThisMonth;
                    }
                    else if (days <= 60)
                    {
                        ans = Global.Date_NextMonth;
                    }
                    else
                    {
                        ans=DynDate.Value.Date.ToString(DateFormat);
                    }
                    return ans;
                }
                else
                {///< in the past
                    return DynDate.Value.Date.ToString(DateFormat);
                }

            }
            else
            {
                return "";
            }
           
        }

        internal static string DateFormat(DateTime? DynDate)
        {
            return DynDate.HasValue ? ((DateTime)DynDate).ToString("dd'/'MM'/'yy"): null;

        }

        internal static string TimeFormat(DateTime? DynDate)
        {

            return DynDate.HasValue ? ((DateTime)DynDate).ToString("HH':'mm") :null;
        }

        internal static string DateTimeFormat(DateTime? DynDate)
        {
            return DynDate.HasValue ? ((DateTime)DynDate).ToString("dd'/'MM'/'yyyy HH':'mm") : null;

        }

        public static DateTime TimeRoundUp(DateTime dateTime)
        {
            return new DateTime(dateTime.Hour+1, 0, 0);
        }

        internal static string DateTimeRoundUp(DateTime dateTime)
        {
            var a=new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour + 1, 0, 0);
            return DateTimeFormat(a);
        }
    }
}