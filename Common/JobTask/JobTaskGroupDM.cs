using System;
using System.Collections.Generic;

namespace Common
{
    public class JobTaskGroupDM
    {
        public int JobTaskGroupID { get; set; }
        public int JobTaskID { get; set; }
        public string GroupNameStr { get; set; }
        public bool IsGeneral => GroupNameStr == null;
        public int? LinkedGroupID { get; set; }

        public virtual JobTaskDM JobTaskDM { get; set; }
        public virtual ICollection<JobTaskGroupFieldDM> JobTaskGroupFieldDMs { get; set; }
    }

    public class JobTaskGroupFilterDm : Pager
    {
        public int? ClientID { get; set; }

        public List<JobTaskGroupDM> TableList { get; set; }
    }

}
