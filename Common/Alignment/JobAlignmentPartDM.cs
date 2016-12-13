using System;
namespace Common
{
    public class JobAlignmentPartDM
    {
        public int JobAlignmentPartID { get; set; }
        public int JobID { get; set; }
        public int MachinePartID { get; set; }
        public double? CouplingDiameter { get; set; }
        public double? SC { get; set; }
        public double? SM { get; set; }
        public double? MMF1 { get; set; }
        public double? MF1MF2 { get; set; }
        public double? V_Offset { get; set; }
        public double? V_Angle { get; set; }
        public double? V_MF1 { get; set; }
        public double? V_MF2 { get; set; }
        public double? H_Offset { get; set; }
        public double? H_Angle { get; set; }
        public double? H_MF1 { get; set; }
        public double? H_MF2 { get; set; }

        public string MachineTypeStr { get; set; }
        public string MachineTypeLangStr { get { return GlobalDM.GetTransStr(MachineTypeStr) ?? ""; } }

        public string PartPic { get { return MachineTypeStr!=null ? PicHelper.GetPartPic(MachineTypeStr) : ""; } }

        public string StartDate { get; set; }
    }
}
