using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Common
{
    public class ClientDM : ContactInfoDM
    {
        public int ClientID { get; set; }
        [Required(ErrorMessage = "יש לבחור לקוח")]
        public string ClientName { get; set; }
        public string ClientLogo { get; set; }
        [UIHint("AutoComplete")]
        public int? ClientParentID { get; set; }
        public string ClientParentName { get; set; }
        public bool ShowInRefubrish { get; set; }
        public bool ShowInVb { get; set; }
        public bool IsClient { get; set; }
        public bool IsSupplier { get; set; }
        public bool IsManufacture { get; set; }
        public bool HasVbService { get; set; }
        public int? AccountingID { get; set; }
        public int? ChatPeyID { get; set; }
        public List<MachinePageDM> Machines { get; set; }
        public SelectedMachine SelectedMachine { get; set; }





        public bool ShowInAlignment { get; set; }
        public string ClientFullName { get; set; }
        public string ClientFullNameEnglish { get; set; }
        public string ClientNameEnglish { get; set; }
        public string ClientParentNameEnglish { get; set; }

        public string SearchStr =>
           ClientName + "|"  +ClientFullName + "|" + ClientFullNameEnglish + "|" + ClientNameEnglish + "|" + ClientParentNameEnglish + "|" +
           ClientID ;
    }

    public class ClientFilterDm : Pager
    {
        public int? ClientID { get; set; }

        public List<ClientDM> TableList { get; set; }
    }

    public class MachinePicChangeDM
    {
        public List<ClientDM> Clients { get; set; }
        public ClientDM SelectedClient{ get; set; }
       
    }

    public class ClientChildDM
    {
        public int ClientID { get; set; }
        public int ChildID { get; set; }
    }

    public class ClientTreeDM
    {
        public int ClientID { get; set; }
        public string ClientName { get; set; }

        public List<ClientTreeDM> Childs { get; set; }
        public int[] childsArr { get; set; }


        public bool ShowInVb { get; set; }
        public bool ShowInAlignment { get; set; }
        public bool ShowInRefubrish { get; set; }
        public string ClientFullName { get; set; }
    }

}
