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
    
    public partial class JobTaskEmployee
    {
        public int JobTaskID { get; set; }
        public int EmployeeID { get; set; }
        public System.DateTime VisitStart { get; set; }
        public System.DateTime VisitEnd { get; set; }
        public int JobTaskEmployeeID { get; set; }
    
        public virtual Employee Employee { get; set; }
        public virtual JobTask JobTask { get; set; }
    }
}
