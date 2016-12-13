using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class RequestDM
    {
        public int RequestID { get; set; }
        public int ClientID { get; set; }
        public DateTime Date { get; set; }
        public string Notes { get; set; }
        public int CreatedUserID { get; set; }
        public int StatusPL { get; set; }
        public int PriorityPL { get; set; }
        public int JobTypePL { get; set; }

        public string Priority { get; set; }
        public string Status { get; set; }
        public string JobType { get; set; }

        public string ClientName { get; set; }
        public string CreatedUserFullName { get; set; }

        public bool Locked { get; set; }
    }

    public class RequestMinDM
    {
        public int RequestID { get; set; }
        public DateTime Date { get; set; }
        
    }
}
