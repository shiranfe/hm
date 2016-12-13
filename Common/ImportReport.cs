using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Common
{
    public class VbHtmlReportDM
    {
        public string MachineName { get; set; }
        public string Location { get; set; }
        public string ScheduleEntry { get; set; }
        public string Type { get; set; }
        public string Latest { get; set; }
        public string Date { get; set; }
    }

    public class VbHtmlReportChooseJobDM
    {
        [DataType(DataType.Date)]
        public DateTime JobDate { get; set; }
        public int? JobID { get; set; }


        public int ID { get; set; }
    }


}