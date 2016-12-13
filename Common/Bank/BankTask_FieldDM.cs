
using System.Collections.Generic;

namespace Common
{

    public class BankTask_FieldDM
    {
        public int BankTask_FieldID { get; set; }
        public int BankTaskID { get; set; }
        public int BankFieldID { get; set; }
        public int OrderVal { get; set; }
        public bool IsRequired { get; set; }
        public bool IsForQuote { get; set; }
    }

    public class BankTask_FieldFilterDm : Pager
    {
        public List<BankTask_FieldDM> TableList { get; set; }
    }
}
