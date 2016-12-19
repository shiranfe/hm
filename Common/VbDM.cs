using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Common
{

    public class VbReportDM
    {
        //public int JobID { get; set; }

        //[DataType(DataType.Date)]
        //public DateTime StartDate { get; set; }
        //public string JobName { get; set; }
     
        //public string VbStatus { get; set; }
        //public bool IsPosted { get; set; }
        //public string ClientName { get; set; }
        //public string Urgency { get { return GlobalDM.GetTransStr("Priority_Normal"); } }
        //public int ClientID { get; set; }

        public string Areas { get; set; }
        public string TesterName { get; set; }
        public string AnalyzerName{ get; set; }
        public string InviterName { get; set; }

       
        public JobDM JobDM { get; set; }
        public virtual ICollection<VbReportMachineDM> VbReportMachineDMs { get; set; }

        public string SearchStr =>
          Areas + "|" + JobDM.StartDate + "|" + JobDM.JobID;
    }

    public class VbReportEditDM:VbReportDM
    {
        public List<VbMachineDM> VbEditMachineDMs { get; set; }
        public VbMachineDM SelectedVbEditMachine { get; set; }

        public string[] JobTemplateNotesIL{ get; set; }
        public string[] JobTemplateNotesEN { get; set; }
    }

    public class VbFilterDm : Pager
    {
        public int ClientID { get; set; }
        public bool? IsPosted { get; set; }
        public List<VbReportDM> TableList { get; set; }
    }


    public class VbMachineDM
    {
        public int MachineID { get; set; }
        public string MachineName { get; set; }
       
        public string ClientNote { get; set; }
        public string LastTipul { get; set; }
        public string GeneralNoteIL { get; set; }
        public string GeneralNoteEN { get; set; }
      

        public List<NotesHisory> NotesHisory{ get; set; }
        public List<VbPointResultDM> VbPointResultDMs { get; set; }
        public List<VbStatusDM> VbStatusDMs { get; set; }
    }

    public class MachineVbDetailsDM
    {
       
        public List<NotesHisory> History { get; set; }
        public SelectedMachine Machine { get; set; }
       
    }

    public class NotesHisory
    {
        [DataType(DataType.Date)]
        public DateTime JobStartDate{ get; set; }
        public string GeneralNote{ get; set; }
    }

    public class VbPointResultDM : VbStatusDM
    {
        public int JobVibrationMachinePointResultID { get; set; }
        public int MachinePointID { get; set; }
        public string PoineNumber { get; set; }

        public int? ScheduleEntryID { get; set; }
        public string ScheduleEntryKey { get; set; }
        public string ScheduleEntryStr
        {
            get
            {
                var a =  ScheduleEntryKey.Split(' ')[0];
                return GlobalDM.GetTransStr(a);
            }
        }

        public int DirectionID { get; set; }
        public string DirectionKey { get; set; }
        public string DirectionStr => GlobalDM.GetTransStr(DirectionKey);

        [DisplayFormat(DataFormatString = "{0:#,##0.000#}")]
        public double? Value { get; set; }
        public string ValueUnit => ScheduleEntryKey == "Demod" ? "g" : "mm/s";
        public string ResualTypeID => PoineNumber + ScheduleEntryID.ToString() + DirectionID.ToString();


        public bool IsHidden  { get; set; }
    }

    public class VbReportMachineDM : VbStatusDM
    {

        public int MachineID { get; set; }
        public string MachineName { get; set; }
        public string Areas { get; set; }
        public string Details { get; set; }

        [DisplayFormat(DataFormatString = "{0:#,##0.000#}")]
        public double? MaxValue { get; set; }

       

        public virtual string NotesIL { get; set; }
        public virtual string NotesEN { get; set; }
        public string Notes => ReferenceEquals(HttpContext.Current.Session["lang"], "he-IL") ? NotesIL ?? NotesEN : NotesEN ?? NotesIL;

    }

    public class VbReportExcelDM
    {

        
        public string MachineName { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        [DisplayFormat(DataFormatString = @"{0:#,##0.000#}")]
        public double? MaxValue { get; set; }
        public string Status { get; set; }
        public string Notes { get ; set;}
        
    }

    public class ClientVBCurentStsDM
    {
        public int ClientID { get; set; }
        public DateTime RepDate { get; set; }

        public List<VbCurentMachineStsDM> VBs { get; set; }
    }

    public class VbStatusDM
    {
        public string StatusID { get; set; }
        public string LangStr { get; set; }
        public string Status => GlobalDM.GetTransStr(LangStr);
    }

    public class VbCurentMachineStsDM: VbStatusDM
    {

        public int MachineID { get; set; }
        public string MachineName { get; set; }
        public string ClientName { get; set; }
        public string Areas { get; set; }
        public int? JobID { get; set; }
        [DataType(DataType.Date)]
        public DateTime? LastDate { get; set; }

     

        public string MacPic { get; set; }
        public string MachineTypeID { get; set; }
        public int? StatusValue { get; set; }

        public string NotesIL { get; set; }
        public string NotesEN { get; set; }
        public string Notes =>
            string.Equals((string) HttpContext.Current.Session["lang"], "he-IL", StringComparison.Ordinal)
                ? NotesIL ?? NotesEN : NotesEN ?? NotesIL;
    }


    public class vwMachinePointDm
    {
        public int MachinePointID { get; set; }
        public string PointNumber { get; set; }
        public string MachineName { get; set; }

        public int StatusID { get; set; }
    } 
    
    public class PointLastStatusDm
    {

        public int StatusID { get; set; }
        public int MachinePointID { get; set; }

    }

    public class VBNotes
    {
        public int MachineID { get; set; }
        public int JobID { get; set; }
        public string GeneralNote { get; set; }
        public string Lang { get; set; }
    }
}
