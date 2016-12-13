using System.ComponentModel.DataAnnotations;

namespace Common
{
    public class EmailDm
    {
        public int Id { get; set; }
        public string EmailBody { get; set; }
        [Required]
        public string From { get; set; }
        [Required]
        public string To { get; set; }
        public string Subject { get; set; }
        public string Creator { get; set; }
        public string EmailPassword { get; set; }
    }
}
