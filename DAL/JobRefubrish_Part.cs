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
    
    public partial class JobRefubrish_Part
    {
        public JobRefubrish_Part()
        {
            this.JobRefubrish_Step = new HashSet<JobRefubrish_Step>();
        }
    
        public int JobRefubrishPartID { get; set; }
        public int JobID { get; set; }
        public int MachinePartID { get; set; }
    
        public virtual MachinePart MachinePart { get; set; }
        public virtual ICollection<JobRefubrish_Step> JobRefubrish_Step { get; set; }
        public virtual JobRefubrish JobRefubrish { get; set; }
    }
}
