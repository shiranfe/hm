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
    
    public partial class Job
    {
        public Job()
        {
            this.JobTask = new HashSet<JobTask>();
            this.JobEquipment = new HashSet<JobEquipment>();
            this.JobRefubrish1 = new HashSet<JobRefubrish>();
        }
    
        public int JobID { get; set; }
        public int ClientID { get; set; }
        public Nullable<System.DateTime> DueDate { get; set; }
        public string JobName { get; set; }
        public bool IsPosted { get; set; }
        public System.DateTime StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public Nullable<int> CreatorID { get; set; }
        public string Comments { get; set; }
        public Nullable<System.DateTime> TimeStamp { get; set; }
        public Nullable<int> QuoteID { get; set; }
        public Nullable<int> ContactID { get; set; }
    
        public virtual Client Client { get; set; }
        public virtual JobVibration JobVibration { get; set; }
        public virtual Quote Quote { get; set; }
        public virtual JobAlignment JobAlignment { get; set; }
        public virtual User Contact { get; set; }
        public virtual Employee Employee { get; set; }
        public virtual JobOutside JobOutside { get; set; }
        public virtual ICollection<JobTask> JobTask { get; set; }
        public virtual ICollection<JobEquipment> JobEquipment { get; set; }
        public virtual JobRefubrish JobRefubrish { get; set; }
        public virtual ICollection<JobRefubrish> JobRefubrish1 { get; set; }
    }
}
