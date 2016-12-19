using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Common
{
   
    
    public class MachineDetailsDM
    {
        string undefined = GlobalDM.GetTransStr("Undefined");    
        public int MachineID { get; set; }
        public string MachineName { get; set; }
        public string MachineType { get; set; }
        public string ClientName { get; set; }
        public string MacName { get; set; }
        public string Details { get; set; }
        public string Manufacturer { get; set; }
        public string MachineModel { get; set; }
        public int? EngPower { get; set; }
        public string Bearing { get; set; }
        public int? BearingSpeed { get; set; }
        public string EngType { get; set; }
        public string Comments { get; set; }
        public string BearingMachineType { get; set; }
        public string MachineTypeAdditional { get; set; }
        public string SKU { get; set; }

        public string MachineTypeLangStr => GlobalDM.GetTransStr(MachineType) ?? undefined;
        public string EngTypeLangStr => GlobalDM.GetTransStr(EngType) ?? undefined;
        public string EngPowerW => EngPower != null ? EngPower + "W" : undefined;
        public string BearingSpeedCPM => BearingSpeed != null ? BearingSpeed + "CPM" : undefined;
        public bool ShowBearingMachineType => MachineTypeAdditional == null && MachineType != null;
    }

    public class MachineEditDM
    {
        public int ClientID { get; set; }
        public int MachineID { get; set; }
        [Required(ErrorMessage = "יש לבחור שם מכונה")]
        public string MachineName { get; set; }
        public string ClientName { get; set; }
        public string MacPic { get; set; }
        public string SKU { get; set; }
        public string Details { get; set; }
        [UIHint("AutoComplete")]
        public int? MachineTypeID { get; set; }
        public int? EngTypeID { get; set; }
        public string Manufacturer { get; set; }
        public string MachineModel { get; set; }
        public int? EngPower { get; set; }
        public string Comments { get; set; }
        public string Bearing { get; set; }
        public string BearingMachineType { get; set; }
        public int? BearingSpeed { get; set; }

        public string MachineTypeAdditional { get; set; }
        public bool ShowBearingMachineType => MachineTypeAdditional == null && MachineTypeID.HasValue;

        public string MachineTypeStr { get; set; }

        public string Rpm { get; set; }
        public string Kw { get; set; }
       
        public string Address { get; set; }


        public List<MachinePartDM> Parts { get; set; }
    }

    public class MachineVBDM
    {
        public int MachineID { get; set; }
        public string MachineName { get; set; }
         [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }
       
        public string ClientNotes { get; set; }
        public string MachineTypeID { get; set; }
        public string MacPointPic { get; set; }
        public int JobID { get; set; }
        public string NotesIL { get; set; }
        public string NotesEN { get; set; }
        public string Notes => (string)HttpContext.Current.Session["lang"] == "he-IL" ? NotesIL ?? NotesEN : NotesEN ?? NotesIL;

        //public RequestMinDM NextRequest { get; set; }
        public  List<JobDatesDM> JobDates { get; set; }
        public  MachinePointDM PointSelected { get; set; }
        public  List<MachinePointMinDM> PointPicDMs { get; set; }
    }

    public class MachinePointMinDM
    {

        public int MachinePointID { get; set; }

        public string PointNumber { get; set; }
        public int? HtmlX { get; set; }
        public int? HtmlY { get; set; }

        [DataType(DataType.Date)]
        public DateTime LastDate { get; set; }
        public string StatusID { get; set; }
        public string LangStr { get; set; }
        public string Status => GlobalDM.GetTransStr(LangStr);
        public bool ShowPoint { get; set; }

        public virtual MachineVBDM MachineVBDM { get; set; }
    }

    public class MachinePointDM
    {
        
        public int MachinePointID { get; set; }
      
        public string PointNumber { get; set; }
        public string Bearing { get; set; }
        public string Grease { get; set; }
        public string TrackWheels { get; set; }
        public string Track { get; set; }
        public string Tfrlok { get; set; }
        public string Connector { get; set; }
        public string GreaseAmount { get; set; }
        public string PoinPic { get; set; }

        public int? HtmlX { get; set; }
        public int? HtmlY { get; set; }

        public  List<PointResultDM> PointResultDMs { get; set; }
        public  MachineVBDM MachineVBDM { get; set; }
    }

    public class SpectrumDM
    {
        public List<AdminPointResualtDM> PointResualts { get; set; }
        public AdminPointResualtDM SelectedPointResualt { get; set; }
        public int JobID  { get; set; }

    }
    public class AdminPointResualtDM
    {
        public int PointResualtID { get; set; }
        public string ClientName { get; set; }
        public string MachineName { get; set; }
       [DataType(DataType.Date)]
        public DateTime Date { get; set; }
        public string PointName { get; set; }

        public int? ScheduleEntryID { get; set; }
        public string ScheduleEntryKey { get; set; }
        public string ScheduleEntryStr
        {
            get
            {
                try
                {
                    return ScheduleEntryKey.Split(' ')[0];
                }
                catch (Exception)
                {
                    return "";
                    throw;
                }
                
            }
        }

        public int DirectionID { get; set; }
        public string DirectionStr { get; set; }
        public string ValueName => GlobalDM.GetTransStr(DirectionStr) + " - " + GlobalDM.GetTransStr(ScheduleEntryStr);


        [DisplayFormat(DataFormatString = "{0:#,##0.000#}")]
        public double? Value { get; set; }
        public string SpectrumPic { get; set; }
    }

    public class PointResultDM
    {
        public int JobVibrationMachinePointResultID { get; set; }
        public int JobID { get; set; }
        public int MachinePointID { get; set; }
        public System.DateTime Date { get; set; }
        public int? ScheduleEntryID { get; set; }
        public string ScheduleEntryKey { get; set; }
        public string ScheduleEntryStr => ScheduleEntryKey.Split(' ')[0];

        public int DirectionID { get; set; }
        public string DirectionStr { get; set; }
        public string ValueName => GlobalDM.GetTransStr(DirectionStr) + " - " + GlobalDM.GetTransStr(ScheduleEntryStr);

        [DisplayFormat(DataFormatString = "{0:#,##0.000#}")]
        public double? Value { get; set; }
        public string ValueUnit => ScheduleEntryStr == "Demod" ? "g" : "mm/s";
        public string StatusID { get; set; }
        public string LangStr { get; set; }
        public string Status => GlobalDM.GetTransStr(LangStr);
        public double? PrcntChange { get; set; }

        public virtual MachinePointDM MachinePointDM { get; set; }
        public virtual ICollection<GraphPointDM> GraphPointDMs { get; set; }
        public virtual ICollection<GraphNavigatorDM> GraphNavigatorDMs { get; set; } 

        public List<LegendDM> Legend{ get; set; }
        public string[][] series { get; set; }

        public string SpectrumPic { get; set; }
        public DateTime From{ get; set; }
        public  DateTime To{ get; set; }
    }

    public class LegendDM
    {
        public string label { get; set; }
        public string[][] data { get; set; }
      
    }

    public class GraphNavigatorDM
    {
        public int JobVibrationMachinePointResultID { get; set; }
        public string ScheduleEntryKey { get; set; }
        public string ScheduleEntryStr => ScheduleEntryKey.Split(' ')[0];
        public string DirectionStr { get; set; }
        public string ValueName => GlobalDM.GetTransStr(DirectionStr) + " - " + GlobalDM.GetTransStr(ScheduleEntryStr);

        public virtual PointResultDM PointResultDM { get; set; }
    }

    public class GraphPointDM
    {
        
        public int JobID { get; set; }
        public System.DateTime Date { get; set; }
        public double Value { get; set; }
        public string ScheduleEntryKey { get; set; }
        public string ScheduleEntryStr => ScheduleEntryKey.Split(' ')[0];
        public string DirectionStr { get; set; }
        public string ValueName => GlobalDM.GetTransStr(DirectionStr) + " - " + GlobalDM.GetTransStr(ScheduleEntryStr);


        public virtual PointResultDM PointResultDM { get; set; }
    }

    public class PointPositionDM
    {
        public List<MachineBasicDM> DefualtMachines { get; set; }
        public SelectedMachine SelectedMachine { get; set; }
    }

    public class MachineBasicDM
    {
      
        public int MachineID { get; set; }
        public string MachineName { get; set; }
        public string Rpm { get; set; }
        public string Kw { get; set; }
        public string Address { get; set; }

        public string MachineNameFull => MachineName + (Kw != null ? " | " + Kw + "kw" : "") + (Rpm != null ? " | " + Rpm + "rpm" : "");
    }

    public class MachinePageDM: MachineBasicDM
    {
    
        public string MacPic => PicHelper.GetMacPic(MachineID, MachineTypeID.ToString());
        //public string ClientName { get; set; }
        public string vwParentsName2 { get; set; }

        public int MachineTypeID { get; set; }

        public int? ClientID { get; set; }
        public MachineVBDM VBDetails { get; set; }

    }

    public class MachineFilterDm : Pager
    {
        public int ClientID { get; set; }

        public List<MachinePageDM> TableList { get; set; }
    }

    public class SelectedMachine
    {
        public int MachineID { get; set; }
        public string MachineName { get; set; }
        public string MacPic { get; set; }
        public string MachineTypeID { get; set; }
        public int ObjID { get; set; }

        public List<MachinePointMinDM> PointDMs { get; set; }


        public bool IsDefualtMac { get; set; }
    }


    

}
