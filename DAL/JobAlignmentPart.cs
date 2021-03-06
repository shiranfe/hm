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
    
    public partial class JobAlignmentPart
    {
        public int JobAlignmentPartID { get; set; }
        public int JobID { get; set; }
        public int MachinePartID { get; set; }
        public Nullable<double> CouplingDiameter { get; set; }
        public Nullable<double> SC { get; set; }
        public Nullable<double> SM { get; set; }
        public Nullable<double> MMF1 { get; set; }
        public Nullable<double> MF1MF2 { get; set; }
        public Nullable<double> V_Offset { get; set; }
        public Nullable<double> V_Angle { get; set; }
        public Nullable<double> V_MF1 { get; set; }
        public Nullable<double> V_MF2 { get; set; }
        public Nullable<double> H_Offset { get; set; }
        public Nullable<double> H_Angle { get; set; }
        public Nullable<double> H_MF1 { get; set; }
        public Nullable<double> H_MF2 { get; set; }
    
        public virtual MachinePart MachinePart { get; set; }
        public virtual JobAlignment JobAlignment { get; set; }
    }
}
