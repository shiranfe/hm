namespace Common
{
    public class QuoteJobDM
    {
        public int JobID { get; set; }

        public string JobName { get; set; }

        public System.DateTime? StartDate { get; set; }

        public string MacPic { get; set; }

        public string MachineName { get; set; }
    }

    public class QuoteJobFields
    {

        public string FieldValue { get; set; }

        public string FieldNameStr { get; set; }
        public string FieldLabel { get { return GlobalDM.GetTransStr(FieldNameStr); } }

        public ControlType FieldTypeID { get; set; }
        public string FieldTypeValue{ get; set; }


        public string StepName { get; set; }

        public string PickListEntity { get; set; }

        public bool PickListFromTable { get; set; }
        public string GroupName { get; set; }

        public int? CatalogItemID { get; set; }
    }
}
