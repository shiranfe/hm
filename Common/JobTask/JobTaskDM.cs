using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Common
{
    public class JobTaskDM
    {
        public JobTaskDM()
        {
            JobTaskFieldDMs = new List<JobTaskFieldDM>();

        }

        public int JobTaskID { get; set; }
        public int JobID { get; set; }
        [Required]
        public string TaskName { get; set; }
        public string ManagerNotes { get; set; }
        public string EmpNotes { get; set; }
       

        public List<JobTaskEmployeeDM> TaskEmployees { get; set; }
        public JobDM JobDM { get; set; }
        public DateTime TaskTime { get; set; }
        public string[] EmployeeNames { get; set; }
        public string[] EmployeeStr { get; set; }

        public virtual ICollection<JobTaskFieldDM> JobTaskFieldDMs { get; set; }
        //public List<JobTaskGroupDM> TaskGroups { get; set; }
    }

    public class JobTaskFilterDm : Pager
    {
        public JobTaskFilterDm()
        {

        }

        public List<JobTaskDM> TableList { get; set; }
    }
}
