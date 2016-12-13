
using System.Collections.Generic;

namespace Common
{

    public class BankTaskDM
    {
        public BankTaskDM()
        {
            TaskFields = new List<JobTaskGroupFieldDM>();
        }
        public int BankTaskID { get; set; }
        public string TaskName { get; set; }
        public string ManagerNotes { get; set; }

        public List<JobTaskGroupFieldDM> TaskFields{ get; set; }
    }

    public class BankTaskFilterDm : Pager
    {
        public List<BankTaskDM> TableList { get; set; }
    }
}
