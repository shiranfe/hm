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
    
    public partial class vwJobVibrationMachine
    {
        public int JobID { get; set; }
        public int MachineID { get; set; }
        public string MachineName { get; set; }
        public int ClientID { get; set; }
        public Nullable<int> StatusID { get; set; }
        public string StatusLangStr { get; set; }
        public string StatusValue { get; set; }
        public string vwParentsName2 { get; set; }
        public Nullable<double> MaxValue { get; set; }
        public string StatusPicture { get; set; }
        public string GeneralNoteIL { get; set; }
        public string GeneralNoteEN { get; set; }
        public System.DateTime JobDueDate { get; set; }
        public System.DateTime JobStartDate { get; set; }
        public string Details { get; set; }
    }
}
