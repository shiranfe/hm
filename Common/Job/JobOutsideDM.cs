using System;
using System.Collections.Generic;

namespace Common
{
    public class JobOutsideDM
    {

        public int JobID { get; set; }
        public  Nullable<int> MachineID { get; set; }
        public string MachineName { get; set; }



        public string Address { get; set; }
        public string Zone { get; set; }
      
        public JobDM JobDM { get; set; }
        public int? QuoteID { get; set; }
        public int? FirstPartID { get; set; }
    }

    public class OutsideFilterDm : Pager
    {
        public OutsideFilterDm()
        {
            Zone = "-1";
            CreatorID = -1;
        }
        public int? CreatorID { get; set; }
        public string Zone { get; set; }

        public List<JobOutsideDM> TableList { get; set; }
    }


    
}
