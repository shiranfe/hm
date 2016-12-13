using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Common
{
    public class JobAlignmentDM
    {
        public int JobID { get; set; }
        [UIHint("AutoComplete")]
        public int TesterID { get; set; }
        [UIHint("AutoComplete")]
        public int MachineID { get; set; }
        public string MachineName { get; set; }
        public string MeasurementProgram { get; set; }
 public string MacPic { get; set; }
        public bool ShowTollerance { get; set; }
        public string OpenNotes { get; set; }
        public List<MachinePartDM> Parts { get; set; }
        public List<JobAlignmentPartDM> JobParts { get; set; }

        public JobDM JobDM { get; set; }
        public int[] MachinePartID { get; set; }

        public int FirstPartID { get; set; }

        public int? MachineTypeID { get; set; }
        public string EmpSignture { get; set; }
    }
}
