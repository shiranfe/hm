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
    
    public partial class vwRefubrishReport
    {
        public int JobID { get; set; }
        public string IL { get; set; }
        public Nullable<System.DateTime> DueDate { get; set; }
        public System.DateTime StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public string EmpName { get; set; }
        public string ClientName { get; set; }
        public int IsRetunring { get; set; }
        public Nullable<int> CreatorID { get; set; }
    }
}