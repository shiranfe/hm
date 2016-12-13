using System.ComponentModel.DataAnnotations;
namespace Common
{
    public class QuoteVersionItemDM
    {
        public QuoteVersionItemDM()
        {
            ItemSort = 1;
        }
        public int QuoteVersionItemID { get; set; }
        public int QuoteVersionID { get; set; }
        public int? CatalogItemID { get; set; }
        public int? ItemParentID { get; set; }

        public string ItemTitle { get; set; }
        public string ItemNotes { get; set; }
        public int ItemQuntity { get; set; }
        [DisplayFormat(DataFormatString = "{0:C0}")] //  [DataType(DataType.Currency)]
        public decimal ItemPricePerUnit { get; set; }

        [DisplayFormat(DataFormatString = "{0:C0}")]
        public decimal ItemTotalPrice { get { return ItemPricePerUnit * ItemQuntity; } }

        public int ItemSort { get; set; }
        public int? FieldPoolID { get; set; }
        public string FieldValue { get; set; }
    }
}
