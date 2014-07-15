using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SNW.Models.ProductInfo
{
        public class MemberProductList : Product
        {
            public int productID { get; set; }
            public string productCategoryName { get; set; }
            public string productName { get; set; }
            public string productOrganicStatus { get; set; }
            public string productOrigin { get; set; }
            public decimal productWHSQty { get; set; }
            public string productWHSsize { get; set; }
            public decimal productWHSPrice { get; set; }
        }

        public class MemberOrderDetails
        {
            public int customerOrderID { get; set; }
            public int customerOrderStateID { get; set; }
            public int productID { get; set; }
            public int customerID { get; set; }
            public decimal customerOrderQtyMin { get; set; }
            public decimal customerOrderQtyMax { get; set; }
            public decimal? customerOrderUnitPrice { get; set; }
        }
}