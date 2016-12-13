using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Common
{
    public class RefubrishDM
    {
        public int JobID { get; set; }
        public string JobName { get; set; }
        public int ClientID { get; set; }
        public string ClientName { get; set; }
        public string BranchName { get; set; }
        public string StatusStr { get; set; }
        public string StatusTrans { get { return GlobalDM.GetTransStr(StatusStr); } }

        [DataType(DataType.Date)]
        public DateTime? StartDate { get; set; }
        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }
        [DataType(DataType.Date)]
        public DateTime? DueDate { get; set; }

        public string Loaction { get; set; }
      
        public int MachineID { get; set; }
        public int JobRefubrishPartID { get; set; }
        public string MachineName { get; set; }

        public List<RefubrishStepDM> RefubrishStepDM { get; set; }
        public List<MachinePartDM> Parts { get; set; }
        public bool IsPosted { get; set; }
        public string Comments { get; set; }
        public string MachineAddress { get; set; }
        public string SKU { get; set; }
        public string EngPower { get; set; }
        public string Rpm { get; set; }
        public string ClinetNotes { get; set; }
        public int? CreatorID { get; set; }
        public string Creator { get; set; }
        public string CurrentStep { get; set; }
        public int? QuoteID { get; set; }
        public int? ReturningJobParentID { get; set; }
    }

    public class RefubrishDetailsDM
    {
        public int JobID { get; set; }
        public string Loaction { get; set; }
        [UIHint("AutoComplete")]
        public int BranchID { get; set; }
        public string ReportMemo { get; set; }
        public string ClinetNotes { get; set; }
        public int RefubrishStatusID { get; set; }
        [Required(ErrorMessage = "יש לבחור מכונה")]
        [UIHint("AutoComplete")]
        public int MachineID { get; set; }
        public string BranchName { get; set; }

        public string MachineName { get; set; }


        public JobDM JobDM { get; set; }

        public string JobName { get; set; }
        public int[] MachinePartID { get; set; }
 public int[] MachineTypeID { get; set; }
        public List<MachinePartDM> Parts { get; set; }
        public int FirstPartID { get; set; }
        public int? ReturningJobParentID { get; set; }

       
    }

    public class MachinePartDM
    {
        public int MachinePartID { get; set; }
        public int MachineID { get; set; }
        public int MachineTypeID { get; set; }
        public string MachineTypeStr { get; set; }
        public string PartName { get; set; }
        public string MachineTypeLangStr { get { return GlobalDM.GetTransStr(MachineTypeStr) ?? ""; } }
        public string PartFullName { get { return string.IsNullOrEmpty(PartName) ? MachineTypeLangStr : PartName + "(" + MachineTypeLangStr + ")"; } }

        public bool Selected { get; set; }

        public int id { get; set; }

        public List<QuoteJobFields> Fields { get; set; }
        public List<StepGroupDM> Groups { get; set; }
    }

   


    public class RefubrishStepDM
    {
        public int JobRefubrish_StepID { get; set; }
        //public int JobID { get; set; }
        public int JobRefubrishStepID { get; set; }
        public int CreatorID { get; set; }
        [DataType(DataType.Date)]
        public Nullable<System.DateTime> DoneDate { get; set; }
        public string Notes { get; set; }

        // public virtual JobRefubrish JobRefubrish { get; set; }

        public bool HasStarted { get; set; }
        public bool IsDone { get { return HasStarted && DoneDate.HasValue; } }
        public string StepName { get; set; }

        public string FormName { get; set; }

        public string CreatorName { get; set; }



        public int OrderVal { get; set; }
    }

    public class BasicStepDM
    {
        [Required(ErrorMessage = "מספר עובדה חובה")]
        public int JobID { get; set; }
public int QuoteID { get; set; }
        public int JobRefubrishPartID { get; set; }
        [Required(ErrorMessage = "מספר שלב חובה")]
        public int JobRefubrish_StepID { get; set; }
        public int JobRefubrishStepID { get; set; }
        public int MachineTypeID { get; set; }
        public virtual RefubrishStepDM JobRefubrish_Step { get; set; }

        // public EngtType EngtType { get; set; }
        public RefubrishStep NextStep { get; set; }
        public JobRefubrishStatus NextStatus { get; set; }
        public JobRefubrishStatus Status { get; set; }

        public int CreatorID { get; set; }
        [DataType(DataType.Date)]
        public DateTime DoneDate { get; set; }
        public string Notes { get; set; }

        public List<StepGroupDM> StepGroups { get; set; }

        public string CreatorName { get; set; }

        public string ErrorBtnText { get; set; }


        public RefubrishStep ErrorNextStep { get; set; }

        public int MachineTypeStepID { get; set; }
        public RefubrishStep StayInStep { get; set; }
        public string StepName { get; set; }
        public List<string> StepPics { get; set; }
        // public string StepPic { get; set; }
    }




    public class StepGroupDM
    {

        public int JobRefubrishStepGroupID { get; set; }
        public int JobRefubrishStepID { get; set; }
        public string GroupNameStr { get; set; }
        public bool IsRequired { get; set; }
        public int OrderVal { get; set; }
        public Nullable<int> Pid { get; set; }

        public string ConditionType { get; set; }
        public string ConditionFieldID { get; set; }
        public string ConditionMinValue { get; set; }

        public virtual ICollection<StepGroupFieldDM> StepGroupFieldDMs { get; set; }

        public int DynamicGroupID { get; set; }
    }


    public class StepGroupFieldValue
    {
        public string FieldValue { get; set; }
        public int? SubGroupID { get; set; }
    }

        public class StepGroupFieldDM
    {

        public StepGroupFieldDM()
        {
            currenValues = new List<StepGroupFieldValue>();
        }
        public int DynamicGroupFieldID { get; set; }
        public int JobRefubrishStepGroupID { get; set; }
        public string FieldNameStr { get; set; }
        public ControlType FieldTypeID { get; set; }
        public string PickListEntity { get; set; }
        public string FieldUnit { get; set; }
        public bool IsForAC { get; set; }
        public bool IsForDC { get; set; }
        public bool IsRequired { get; set; }
        public int OrderVal { get; set; }
        public string FieldValue { get; set; }
        public int? JobRefubrish_StepFieldID { get; set; }
        public string FieldLabel { get { return GlobalDM.GetTransStr(FieldNameStr); } }

        public int? SubGroupID { get; set; }

        /** values for stepGroupField html only. 
            will take the field value attached to sub group if exist else regular value of group field.
            HtmlID will be the name tag of html - "854_109" or "854"
            */
        [UIHint("AutoComplete")]
        public string HtmlID { get; set; } // return DynamicGroupFieldID + (SubGroupID.HasValue ? "_" + SubGroupID : "" ); }
        public string HtmlValue { get; set; } 

        public virtual StepGroupDM StepGroupDM { get; set; }
        public virtual List<KeyValueDM> PickListItems { get; set; }

        public List<StepGroupFieldValue> currenValues;

        public bool PickListFromTable { get; set; }
    }



    public class RefubrishFilterDm : Pager
    {
        public RefubrishFilterDm()
        {
            RefubrishStatusID = 100;
            CreatorID = -1;
        }
        public int? CreatorID { get; set; }
        public int? RefubrishStatusID { get; set; }

        public List<RefubrishDM> TableList { get; set; }
    }



    public class DynamicEditDM
    {


        public DynamicObject ForeignType { get; set; }
        public MachineType machineType { get; set; }
        public RefubrishStep step { get; set; }
    }


    public class DynamicGroupDM : DynamicEditDM
    {


        public int DynamicGroupID { get; set; }
        public int ForeignID { get; set; }
        [Required(ErrorMessage = "יש לבחור שם קבוצה")]
        public string GroupNameStr { get; set; }
        public bool IsRequired { get; set; }
        [Required(ErrorMessage = "יש להזין סדר מיון")]
        public int OrderVal { get; set; }
        public int[] ItemsSelected { get; set; }
        public int? Pid { get; set; }

    }

    public class DynamicGroupFieldDM
    {

        public int DynamicGroupFieldID { get; set; }
        [Required(ErrorMessage = "יש לבחור קבוצה")]
        public int DynamicGroupID { get; set; }
        [Required(ErrorMessage = "יש להזין סוג שדה")]
        public int FieldPoolID { get; set; }
        public string FieldNameStr { get; set; }
        public bool IsRequired { get; set; }
        public bool IsForQuote { get; set; }
        public int OrderVal { get; set; }
        [UIHint("AutoComplete")]
        public Nullable<int> CatalogItemID { get; set; }

        [Required(ErrorMessage = "יש להזין שם שדה בעברית")]
        public string FieldNameHeb { get; set; }
        [Required(ErrorMessage = "יש להזין שם שדה באנגלית")]
        public string FieldNameEng { get; set; }

        public List<PickListDM> HebStrings { get; set; }
    }

    public class FieldPoolDM
    {

        public int FieldPoolID { get; set; }
        public string FieldNameStr { get; set; }
        public ControlType FieldTypeID { get; set; }
        public string PickListEntity { get; set; }
        public bool PickListFromTable { get; set; }
        public string FieldUnit { get; set; }

        public string FieldLabel { get; set; }
    }

}
