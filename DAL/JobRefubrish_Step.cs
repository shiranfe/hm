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
    
    public partial class JobRefubrish_Step
    {
        public JobRefubrish_Step()
        {
            this.JobRefubrish_StepField = new HashSet<JobRefubrish_StepField>();
        }
    
        public int JobRefubrish_StepID { get; set; }
        public int JobRefubrishPartID { get; set; }
        public int CreatorID { get; set; }
        public Nullable<System.DateTime> DoneDate { get; set; }
        public string Notes { get; set; }
        public int MachineTypeStepID { get; set; }
    
        public virtual Employee Employee { get; set; }
        public virtual JobRefubrish_Part JobRefubrish_Part { get; set; }
        public virtual ICollection<JobRefubrish_StepField> JobRefubrish_StepField { get; set; }
        public virtual MachineTypeStep MachineTypeStep { get; set; }
    }
}
