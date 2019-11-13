//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BazarStore
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class Product
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Product()
        {
            this.Order_Details = new HashSet<Order_Detail>();
        }
        
        public int ProductID { get; set; }
        [Required]
        [Display(Name ="Product Name")]
        public string ProductName { get; set; }
        [Required]
        [Display(Name = "Supplier")]
        public Nullable<int> SupplierID { get; set; }
        [Required]
        [Display(Name = "Category")]
        public Nullable<int> CategoryID { get; set; }
        [Required]
        [Display(Name = "Quantity Per Unit")]
        public string QuantityPerUnit { get; set; }
        [Required]
        [Display(Name = "Unit Price")]
        public Nullable<decimal> UnitPrice { get; set; }
        [Required]
        [Display(Name = "Units in Stock")]
        public Nullable<short> UnitsInStock { get; set; }
        public Nullable<short> UnitsOnOrder { get; set; }
        public Nullable<short> ReorderLevel { get; set; }
        public bool Discontinued { get; set; }
        public string ImageUrl { get; set; }
    
        public virtual Category Category { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Order_Detail> Order_Details { get; set; }
        public virtual Supplier Supplier { get; set; }
    }
}
