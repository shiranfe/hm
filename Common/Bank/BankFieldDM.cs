using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Common
{
    public class BankFieldDM
    {
        public int BankFieldID { get; set; }

        public string FieldTypeLabel { get; set; }
        public string FieldNameStr { get; set; }
        [UIHint("AutoComplete")]
        public int FieldPoolID { get; set; }
        [UIHint("AutoComplete")]
        public Nullable<int> CatalogItemID { get; set; }

        [Required(ErrorMessage = "יש להזין שם שדה בעברית")]
        public string FieldNameHeb { get; set; }
        [Required(ErrorMessage = "יש להזין שם שדה באנגלית")]
        public string FieldNameEng { get; set; }
    }

    public class BankFieldFilterDm : Pager
    {
 
        public List<BankFieldDM> TableList { get; set; }
    }
}
