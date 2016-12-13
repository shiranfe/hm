using System;

namespace Common
{
    public class BugLogDM
    {
        public int BugID { get; set; }
        public string Message { get; set; }
        public string Status { get; set; }
        public string StackTrace { get; set; }
        public string InnerException { get; set; }
        public string UserAgent { get; set; }
        public string AppVersion { get; set; }
        public Nullable<System.DateTime> CreationTime { get; set; }
        public Nullable<int> CardID { get; set; }
        public string url { get; set; }

        public string TimeDiff => ((TimeSpan)(DateTime.Now - CreationTime)).TotalDays.ToString("0") + " days";

    }
}
