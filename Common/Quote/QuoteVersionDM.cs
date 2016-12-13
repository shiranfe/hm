using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace Common
{
    public class QuoteVersionDM
    {
        public int QuoteVersionID { get; set; }
        public int QuoteID { get; set; }
        public int Version { get; set; }
        public string VersionTitle { get { return "גרסא " + Version.ToString(); } }
        [Required(ErrorMessage = "יש להזין תאריך גרסא")]
        [DataType(DataType.Date)]
        public System.DateTime VersionDate { get; set; }
        public bool IsSelected { get; set; }
        public string Terms { get; set; }
        public string Appendices { get; set; }

        [Required(ErrorMessage = "יש להזין הנחה")]
        public decimal Disscount { get; set; }

        public virtual QuoteDM QuoteDM { get; set; }
        public virtual EmployeeDM EmployeeDM { get; set; }
        public virtual List<QuoteVersionItemDM> Items { get; set; }
         [DataType(DataType.Currency)]
        public decimal TotalSum { get; set; }

         public object CatalogItems { get; set; }


         public decimal Vat { get; set; }
 
    }


}
