using System.Collections.Generic;

namespace Common
{
    public class JobTaskFieldDM : StepGroupFieldDM
    {
        public int Id { get; set; }

        public int JobTaskFieldID { get; set; }
        public int JobTaskID { get; set; }
        public bool IsForQuote { get; set; }
        public int BankFieldID { get; set; }
    }

    public class JobTaskFieldFilterDm : Pager
    {
       

        public List<JobTaskFieldDM> TableList { get; set; }
    }
}
