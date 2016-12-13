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
    
    public partial class Equipment
    {
        public Equipment()
        {
            this.JobEquipment = new HashSet<JobEquipment>();
            this.Equipment_TechField = new HashSet<Equipment_TechField>();
        }
    
        public int EquipmentID { get; set; }
        public int ClientID { get; set; }
        public int MachineTypeID { get; set; }
        public string EquipmentTitle { get; set; }
        public string Rpm { get; set; }
        public string Kw { get; set; }
        public string Address { get; set; }
        public string Details { get; set; }
        public Nullable<System.DateTime> TimeStamp { get; set; }
    
        public virtual Client Client { get; set; }
        public virtual PickList MachineType { get; set; }
        public virtual ICollection<JobEquipment> JobEquipment { get; set; }
        public virtual ICollection<Equipment_TechField> Equipment_TechField { get; set; }
    }
}