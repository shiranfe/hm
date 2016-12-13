using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Helper
{
    public class TimeHelper
    {
        const int SECOND = 1;
        const int MINUTE = 60 * SECOND;
        const int HOUR = 60 * MINUTE;
        const int DAY = 1;//24 * HOUR;
        const int MONTH = 30 * DAY;

        public static string RelativeTime(DateTime yourDate, string minTime = "days")
        {
             
            var delta = (yourDate.Date -DateTime.Now.Date).TotalDays;// new TimeSpan(yourDate.Ticks - DateTime.UtcNow.Ticks).TotalDays;
         

            if (delta > 30 * DAY)
                return "בעוד יותר מחודש";

            if (delta < -30 * DAY)
                return "לפני יותר מחודש";

            if (delta >= 14 * DAY)
                return "בחודש הקרוב";

            if (delta <= -14 * DAY)
                return "בחודש האחרון";

            if (delta >= 7 * DAY)
                return "בשבועיים הקרובים";

            if (delta <= -7 * DAY)
                return "בשבועיים האחרונים";

            if (delta >= 3 * DAY)
                return "בשבוע הקרוב";

            if (delta <= -3 * DAY)
                return "בשבוע האחרון";

            if (delta == 2 * DAY)
                return "בעוד יומיים";

            if (delta == -2 * DAY)
                return "לפני יומיים";

            if (delta == 1 * DAY)
                return "מחר";

            if (delta == -1 * DAY)
                return "אתמול";

            return "היום";

        }

    }
}
