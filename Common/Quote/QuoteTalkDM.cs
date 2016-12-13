using System.ComponentModel.DataAnnotations;

namespace Common
{
    public class QuoteTalkDM
    {
        public int QuoteTalkID { get; set; }
        public int QuoteID { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yy HH:mm}")]
        public System.DateTime TalkDate { get; set; }
        public string Message { get; set; }
        public int CreatorID { get; set; }

        public string Creator { get; set; }
    }
}
