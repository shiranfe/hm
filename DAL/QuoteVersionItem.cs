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
    
    public partial class QuoteVersionItem
    {
        public int QuoteVersionItemID { get; set; }
        public int QuoteVersionID { get; set; }
        public Nullable<int> CatalogItemID { get; set; }
        public Nullable<int> ItemParentID { get; set; }
        public string ItemTitle { get; set; }
        public string ItemNotes { get; set; }
        public int ItemQuntity { get; set; }
        public decimal ItemPricePerUnit { get; set; }
        public int ItemSort { get; set; }
        public Nullable<int> FieldPoolID { get; set; }
        public string FieldValue { get; set; }
    
        public virtual CatalogItem CatalogItem { get; set; }
        public virtual FieldPool FieldPool { get; set; }
        public virtual QuoteVersion QuoteVersion { get; set; }
    }
}