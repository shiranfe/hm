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
    
    public partial class DynamicGroup
    {
        public DynamicGroup()
        {
            this.DynamicGroupField = new HashSet<DynamicGroupField>();
            this.SubGroups = new HashSet<DynamicGroup>();
            this.JobTaskGroup = new HashSet<JobTaskGroup>();
        }
    
        public int DynamicGroupID { get; set; }
        public int ForeignID { get; set; }
        public Common.DynamicObject ForeignType { get; set; }
        public string GroupNameStr { get; set; }
        public bool IsRequired { get; set; }
        public int OrderVal { get; set; }
        public Nullable<int> Pid { get; set; }
        public string ConditionType { get; set; }
        public string ConditionFieldID { get; set; }
        public string ConditionMinValue { get; set; }
    
        public virtual ICollection<DynamicGroupField> DynamicGroupField { get; set; }
        public virtual ICollection<DynamicGroup> SubGroups { get; set; }
        public virtual DynamicGroup ParentGroup { get; set; }
        public virtual ICollection<JobTaskGroup> JobTaskGroup { get; set; }
    }
}