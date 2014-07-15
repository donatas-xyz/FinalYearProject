//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SNW.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class CustomerOrderLine
    {
        public CustomerOrderLine()
        {
            this.customerOrderUnitPrice = 0m;
        }
    
        public int customerOrderLineID { get; set; }
        public int customerOrderID { get; set; }
        public Nullable<int> supplierOrderID { get; set; }
        public int productID { get; set; }
        public Nullable<decimal> customerOrderUnitPrice { get; set; }
        public decimal customerOrderQtyMin { get; set; }
        public decimal customerOrderQtyMax { get; set; }
        public decimal customerOrderQtyActual { get; set; }
    
        public virtual Product Product { get; set; }
        public virtual CustomerOrder CustomerOrder { get; set; }
        public virtual SupplierOrder SupplierOrder { get; set; }
    }
}