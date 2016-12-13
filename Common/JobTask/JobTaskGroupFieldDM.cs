using System.Collections.Generic;

namespace Common
{
    public class JobTaskGroupFieldDM : StepGroupFieldDM
    {
        public int Id { get; set; }
        public int JobTaskGroupID { get; set; }

    }

    public class JobTaskGroupFieldFilterDm : Pager
    {
       

        public List<JobTaskGroupFieldDM> TableList { get; set; }
    }
}
