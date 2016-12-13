using System;
using System.Collections.Generic;

namespace Common
{
    public class JobTaskEmployeeDM
    {
        public int JobTaskEmployeeID { get; set; }
        public int JobTaskID { get; set; }
        public int EmployeeID { get; set; }
   
        public DateTime VisitStart { get; set; }
        public DateTime VisitEnd { get; set; }

        public string EmployeeName { get; set; }

        public string VisitTime => VisitStart.ToString("dddd, dd/MM/yy") + " " + VisitStart.ToShortTimeString() + "-" + VisitEnd.ToShortTimeString(); 
    }

    public class JobTaskEmployeeFilterDm : Pager
    {
        public JobTaskEmployeeFilterDm()
        {
           
        }

        public List<JobTaskEmployeeDM> TableList { get; set; }
    }
}
