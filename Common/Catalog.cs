using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public  class CatalogItemDM
    {

        public int CatalogItemID { get; set; }
        [Required(ErrorMessage = "יש להזין שם פריט")]
        public string ItemName { get; set; }

        [DataType(DataType.Currency)]
        public decimal CatalogItemCost { get; set; }
        [DataType(DataType.Currency)]
        public decimal CatalogItemPrice { get; set; }

        public List<CatalogItemComponentDM> Components { get; set; }
        public Nullable<int> FieldPoolID { get; set; }

        public List<ComponentTypeDM> AllSrcs { get; set; }

        public string ItemNotes { get; set; }
        public Nullable<int> Pid { get; set; }
        public Nullable<int> LinkedId { get; set; }
        public int SortId { get; set; }
        public bool IsGroup { get; set; }

        public string SearchStr =>ItemName + "|" + ItemNotes;
    }

    public class CatalogFilterDm : Pager
    {
        public List<CatalogItemDM> TableList { get; set; }
    }

    public  class CatalogItemComponentDM
    {
        public int CatalogItemComponentID { get; set; }
        public string ComponentName { get; set; }
        public int CatalogItemID { get; set; }
        public CatalogItemType ComponentTypeID { get; set; }
        public string ComponentType { get { return ComponentTypeID.ToString(); } }
        public int ComponentSrcID { get; set; }
        [Required(ErrorMessage = "יש להזין כמות")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "יש להזין מחיר פריט")]
        [DataType(DataType.Currency)]
        public decimal ComponentPrice { get; set; }
        [DataType(DataType.Currency)]
        public decimal ComponentCost { get; set; }
        [DataType(DataType.Currency)]
        public decimal SumPrice { get { return Quantity * ComponentPrice; } }


        public string ItemName { get; set; }

        public string ItemNotes { get; set; }
        public object CatalogItemDM { get; set; }
    }

    public  class ComponentTypeDM
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public CatalogItemType ComponentTypeID { get; set; }

        [DataType(DataType.Currency)]
        public decimal Cost { get; set; }

        public decimal Price { get; set; }
    }

}
