
using System.Collections.Generic;

namespace Common
{

    public class BankTaskDM
    {
        public BankTaskDM()
        {
            TaskFields = new List<JobTaskFieldDM>();
        }
        public int BankTaskID { get; set; }
        public string TaskName { get; set; }
        public string ManagerNotes { get; set; }

        public List<JobTaskFieldDM> TaskFields{ get; set; }
    }

    public class BankTaskFilterDm : Pager
    {
        public List<BankTaskDM> TableList { get; set; }
    }
}
