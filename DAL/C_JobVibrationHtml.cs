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
    
    public partial class C_JobVibrationHtml
    {
        public int ID { get; set; }
        public System.DateTime Date { get; set; }
        public string MachineName { get; set; }
        public string PointNumber { get; set; }
        public string Direction { get; set; }
        public string ScheduleEntry { get; set; }
        public Nullable<double> Value { get; set; }
    }
}