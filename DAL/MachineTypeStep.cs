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
    
    public partial class MachineTypeStep
    {
        public MachineTypeStep()
        {
            this.JobRefubrish_Step = new HashSet<JobRefubrish_Step>();
        }
    
        public int MachineTypeStepID { get; set; }
        public int MachineTypeID { get; set; }
        public int JobRefubrishStepID { get; set; }
    
        public virtual ICollection<JobRefubrish_Step> JobRefubrish_Step { get; set; }
        public virtual JobRefubrishStep JobRefubrishStep { get; set; }
        public virtual PickList MachineType { get; set; }
    }
}