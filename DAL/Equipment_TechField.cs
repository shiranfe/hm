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
    
    public partial class Equipment_TechField
    {
        public int Equipment_TechFieldID { get; set; }
        public int EquipmentID { get; set; }
        public int DynamicGroupFieldID { get; set; }
        public string FieldValue { get; set; }
        public Nullable<int> SubGroupID { get; set; }
    
        public virtual DynamicGroupField DynamicGroupField { get; set; }
        public virtual Equipment Equipment { get; set; }
    }
}
