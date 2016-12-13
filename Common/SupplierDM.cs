using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class SupplierProductDM
    {
        public int SupplierProductID { get; set; }
        [UIHint("AutoComplete")]
        public int ClientID { get; set; }
        public string SupplierName { get; set; }
        public string ManufactureName { get; set; }
        [Required(ErrorMessage = "יש להזין שם מוצר")]
        public string ProductName { get; set; }
        [DataType(DataType.Currency)]
        public decimal ProductCost { get; set; }
        [DataType(DataType.Currency)]
        public decimal ProductPrice { get; set; }
        [UIHint("AutoComplete")]
        public int ProductTypeID { get; set; }
        public string ProductTypeKey { get; set; }
        public string ProductTypeLangStr { get { return GlobalDM.GetTransStr(ProductTypeKey) ; } }
        [UIHint("AutoComplete")]
        public int? ManufactureID { get; set; }
        public bool IsForClients { get; set; }
        public decimal ProfitPrec { get; set; }


        [UIHint("AutoComplete")]
        public bool IsMaterial { get; set; }
    }

 
}
