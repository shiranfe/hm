//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class SupplierProduct
    {
        public int SupplierProductID { get; set; }
        public int ClientID { get; set; }
        public string ProductName { get; set; }
        public decimal ProductCost { get; set; }
        public decimal ProductPrice { get; set; }
        public decimal ProfitPrec { get; set; }
        public int ProductTypeID { get; set; }
        public Nullable<int> ManufactureID { get; set; }
        public Nullable<int> Inventory { get; set; }
        public bool IsForClients { get; set; }
        public bool IsMaterial { get; set; }
    
        public virtual Client Supplier { get; set; }
        public virtual PickList ProductType { get; set; }
        public virtual Client Manufacture { get; set; }
    }
}
