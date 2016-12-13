//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class JobAlignment
    {
        public JobAlignment()
        {
            this.JobAlignmentPart = new HashSet<JobAlignmentPart>();
        }
    
        public int JobID { get; set; }
        public int TesterID { get; set; }
        public int MachineID { get; set; }
        public string MeasurementProgram { get; set; }
        public bool ShowTollerance { get; set; }
        public string OpenNotes { get; set; }
    
        public virtual Employee Employee { get; set; }
        public virtual Job Job { get; set; }
        public virtual Machine Machine { get; set; }
        public virtual ICollection<JobAlignmentPart> JobAlignmentPart { get; set; }
    }
}
