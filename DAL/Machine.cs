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
    
    public partial class Machine
    {
        public Machine()
        {
            this.JobVibrationMachine = new HashSet<JobVibrationMachine>();
            this.MachinePoint = new HashSet<MachinePoint>();
            this.MachinePart = new HashSet<MachinePart>();
            this.JobAlignment = new HashSet<JobAlignment>();
            this.JobOutside = new HashSet<JobOutside>();
            this.JobRefubrish = new HashSet<JobRefubrish>();
        }
    
        public int MachineID { get; set; }
        public string MachineName { get; set; }
        public Nullable<int> ClientID { get; set; }
        public string Picture { get; set; }
        public string SKU { get; set; }
        public string PictureSKU { get; set; }
        public string MacName { get; set; }
        public string Details { get; set; }
        public Nullable<int> MachineTypeID { get; set; }
        public Nullable<int> EngTypeID { get; set; }
        public string Manufacturer { get; set; }
        public string MachineModel { get; set; }
        public Nullable<int> EngPower { get; set; }
        public string Comments { get; set; }
        public string Bearing { get; set; }
        public string BearingMachineType { get; set; }
        public Nullable<int> BearingSpeed { get; set; }
        public Nullable<System.DateTime> TimeStamp { get; set; }
        public string Rpm { get; set; }
        public string Kw { get; set; }
        public string Address { get; set; }
    
        public virtual Client Client { get; set; }
        public virtual ICollection<JobVibrationMachine> JobVibrationMachine { get; set; }
        public virtual ICollection<MachinePoint> MachinePoint { get; set; }
        public virtual PickList MachineType { get; set; }
        public virtual PickList EngType { get; set; }
        public virtual ICollection<MachinePart> MachinePart { get; set; }
        public virtual ICollection<JobAlignment> JobAlignment { get; set; }
        public virtual ICollection<JobOutside> JobOutside { get; set; }
        public virtual ICollection<JobRefubrish> JobRefubrish { get; set; }
    }
}
