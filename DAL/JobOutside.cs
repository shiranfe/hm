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
    
    public partial class JobOutside
    {
        public int JobID { get; set; }
        public Nullable<int> MachineID { get; set; }
        public string Address { get; set; }
        public string Zone { get; set; }
    
        public virtual Job Job { get; set; }
        public virtual Machine Machine { get; set; }
    }
}
