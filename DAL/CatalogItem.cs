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
    
    public partial class CatalogItem
    {
        public CatalogItem()
        {
            this.CatalogItemComponent = new HashSet<CatalogItemComponent>();
            this.vwCatalogItemComponent = new HashSet<vwCatalogItemComponent>();
            this.ItemsLinked = new HashSet<CatalogItem>();
            this.ItemChilds = new HashSet<CatalogItem>();
            this.QuoteVersionItem = new HashSet<QuoteVersionItem>();
            this.DynamicGroupField = new HashSet<DynamicGroupField>();
        }
    
        public int CatalogItemID { get; set; }
        public string ItemName { get; set; }
        public string ItemNotes { get; set; }
        public Nullable<int> Pid { get; set; }
        public Nullable<int> LinkedId { get; set; }
        public int SortId { get; set; }
        public Nullable<int> FieldPoolID { get; set; }
    
        public virtual ICollection<CatalogItemComponent> CatalogItemComponent { get; set; }
        public virtual ICollection<vwCatalogItemComponent> vwCatalogItemComponent { get; set; }
        public virtual ICollection<CatalogItem> ItemsLinked { get; set; }
        public virtual CatalogItem LinkedItem { get; set; }
        public virtual ICollection<CatalogItem> ItemChilds { get; set; }
        public virtual CatalogItem ParentItem { get; set; }
        public virtual FieldPool FieldPool { get; set; }
        public virtual ICollection<QuoteVersionItem> QuoteVersionItem { get; set; }
        public virtual ICollection<DynamicGroupField> DynamicGroupField { get; set; }
    }
}
