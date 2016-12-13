using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Common
{
    public class JobDM
    {
        public JobDM()
        {
            Equipments = new List<JobEquipmentDM>();
        }


        public JobDM(bool isNew)
        {
            TempId = Math.Abs((int)DateTime.Now.Ticks);
        }

        public int JobID { get; set; }
        public int ClientID { get; set; }
        public int? MainClientID { get; set; }
 public int? ClientParentID { get; set; }

        [Required(ErrorMessage = "יש להזין את שם העבודה")]
        public string JobName { get; set; }
        public bool IsPosted { get; set; }
        [Required(ErrorMessage = "יש להזין תאריך התחלה")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? DueDate { get; set; }
        [UIHint("AutoComplete")]
        public int? CreatorID { get; set; }
        [UIHint("AutoComplete")]
        public int? ContactID { get; set; }
        public string Comments { get; set; }

        public string ClientName { get; set; }
        public string CreatorName { get; set; }
        public string ContactName { get; set; }
        public string Urgency { get { return GlobalDM.GetTransStr("Priority_Normal"); } }


        public bool IsRefubrish { get { return RefubrishDetailsDM != null; } }
        public bool IsVB { get { return VbReportDM != null; } }
        public bool IsAlignment { get { return JobAlignmentDM != null; } }

        public bool IsOutside { get { return JobOutsideDM != null; } }

        public RefubrishDetailsDM RefubrishDetailsDM  { get; set; }
        public VbReportDM VbReportDM { get; set; }
        public List<ClientTreeDM> Clients { get; set; }

        public JobAlignmentDM JobAlignmentDM { get; set; }

        public JobOutsideDM JobOutsideDM { get; set; }

        public string Creator { get; set; }
        public int TempId { get; set; }
        public int? ReturningJobParentID { get; set; }
        /** so we can upload mac pic to quote*/
        public bool YetArrvied { get; set; }
        public List<JobTaskDM> JobTasks { get; set; }
    public List<JobEquipmentDM> Equipments { get; set; }
        
    }
 

    public class JobRequestDM:JobDM
    {
        public JobRequestDM() 
        {
        }

        public JobRequestDM(bool isNew) : base(isNew)
        {
        }

        public string Rpm { get; set; }
        public string Kw { get; set; }
        public string MachineName { get; set; }
        public string Address { get; set; }

     
    
    }

    public class JobDatesDM
    {
        public JobDatesDM()
        {
            IsSelected = false;
        }
        public int JobID { get; set; }
        public string JobName { get; set; }
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }
        public bool IsSelected{ get; set; }
    }

}
