using Common.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Common
{

    
    public class QuoteDM
    {
        public QuoteDM()
        {
            Clients = new List<ClientTreeDM>();
            Jobs = new List<QuoteJobDM>();
            FollowDate = DateTime.Now;
        }

        public int QuoteID { get; set; }

        [Required(ErrorMessage = "יש להזין כותרת להצעת מחיר")]
        public string QuoteTitle { get; set; }

        public int ClientID { get; set; }
        [UIHint("AutoComplete")]
        public int? UserID { get; set; }
        public string ContactName { get; set; }
        public string Comments { get; set; }

        [UIHint("AutoComplete")]
        public int QuoteStatusID { get; set; }
        public string QuoteStatusKey { get; set; }
        public string Status => GlobalDM.GetTransStr(QuoteStatusKey);

        /** accepeted or rejected*/
        public bool StatusIsOpen { get; set; }
        //public bool StatusIsOpen => QuoteStatusID != (int)Common.QuoteStatus.Done && QuoteStatusID != (int)Common.QuoteStatus.Rejected;


        public string OrderNumber { get; set; }
        public string OrderAttachment { get; set; }
        public string OrderAttachmentPath => string.IsNullOrEmpty(OrderAttachment) ? null :  "\\QuoteOrders\\" + OrderAttachment;

        [UIHint("AutoComplete")]
        public int CreatorID { get; set; }
        public string ClientName { get; set; }
        public bool IsCover { get; set; }
        public string JobName { get; set; }
        public Nullable<int> JobCardNumber { get; set; }
        public string InvoiceNumber { get; set; }

        [DataType(DataType.Date)]
        public DateTime? JobDate { get; set; }

        public virtual List<QuoteVersionDM> Versions { get; set; }
        public List<ClientTreeDM> Clients { get; set; }
        public List<QuoteJobDM> Jobs { get; set; }
        public MachineEditDM MachineDM { get; set; }

        [DataType(DataType.Date)]
        public DateTime LastVersion { get; set; }

        public int CurrentVersionID { get; set; }
        [DataType(DataType.Date)]
        public DateTime CurrentVersionDate { get; set; }
        public int JobID { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yy HH:mm}")]
        public DateTime FollowDate { get; set; }
        public string RelativeTime => TimeHelper.RelativeTime(FollowDate);

        public List<QuoteTalkDM> Talks { get; set; }
        public string Creator { get; set; }
        public string EmpSignture { get; set; }
        public int JobRefubrishPartID { get; set; }
        public string LastTalk { get; set; }
        public string JobStatus { get; set; }

        public static int[] OpenStatus = new int[] { (int)QuoteStatus.Done, (int)QuoteStatus.Rejected };
       
    }


    public class QuoteFilterDm: Pager
    {
        public QuoteFilterDm()
        {
            QuoteStatusID = 100;
            CreatorID = -1;
        }
        public bool? IsCover { get; set; }
        public int? CreatorID { get; set; }
        public int? QuoteStatusID { get; set; }


        public List<QuoteDM> TableList { get; set; }
    }

    public class Pager
    {
        public Pager()
        {
            Page = 1;
            On = false;
        }
        public string Srch { get; set; }
        public int Page { get; set; }
        public int PageTotal { get; set; }
        public bool On { get; set; }
    }

}