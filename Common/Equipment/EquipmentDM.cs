using System.Collections.Generic;

namespace Common
{


    public class EquipmentDM
    {
        public int EquipmentID { get; set; }
        public int ClientID { get; set; }
        public string EquipmentTitle { get; set; }
        public string MachineTypeName { get; set; }
        public int MachineTypeID { get; set; }
        public string MachineType { get; set; }
        public string ClientName { get; set; }

        public string Details { get; set; }
        public string Rpm { get; set; }
        public string Kw { get; set; }

        public string Address { get; set; }


        public string EquipmentName => MachineTypeName + EquipmentID + (EquipmentTitle != null ? "_"+EquipmentTitle : "");
        public string MachineTypeLangStr { get { return GlobalDM.GetTransStr(MachineType) ?? ""; } }

        public List<JobEquipmentDM> Jobs { get; set; }

    }

  

    public class EquipmentFilterDm : Pager
    {
        public EquipmentFilterDm()
        {
        
        }

        public List<EquipmentDM> TableList { get; set; }
    }
}
