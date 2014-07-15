using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SNW.Models.SupplierOrderInfo
{
    /// Model for SupplierOrders
    public class SupplierOrderInfo : SupplierOrder
    {
        /// Allows to access property (variable) from other classes
        public new int? supplierOrderID { get; set; }
        public string supplierOrderStateName { get; set; }
        public int supplierOrderStateID { get; set; }
        public string supplierName { get; set; }
        public string supplierPhone { get; set; }
        /// When Order gets Confirmed
        public DateTime supplierOrderDate { get; set; }
        public DateTime? supplierOrderReqDeliveryDate { get; set; }
        public DateTime? supplierOrderDeadline { get; set; }
        public int? productID { get; set; }
        public string productName { get; set; }
        public string productOrganicStatus { get; set; }
        public decimal? productWHSQty { get; set; }
        public string productWHSSize { get; set; }
        public decimal? productWHSPrice { get; set; }
        public decimal? qtyOrdered { get; set; }
        public decimal? _supplierOrderTotal;
        public decimal? supplierOrderTotal
        {
            get
            {
                return _supplierOrderTotal;
            }
            set
            {
                _supplierOrderTotal = qtyOrdered * productWHSPrice;
            }
        }
        /// Calculate Order's total
        public decimal? supplierOrderNetTotal { get; set; }
        public string supplierAddressHouseNo { get; set; }
        public string supplierAddressStreet1 { get; set; }
        public string supplierAddressTown { get; set; }
        public string supplierAddressCounty { get; set; }
        public string supplierAddressPostCode { get; set; }
        public int customerRoleID { get; set; }
        public int customerOrderStateID { get; set; }
    }

    public class CoordinatorInfo
    {
        public string customerName { get; set; }
        public string customerUsername { get; set; }
        public string customerAddressHouseNo { get; set; }
        public string customerAddressStreet1 { get; set; }
        public string customerAddressTown { get; set; }
        public string customerAddressCounty { get; set; }
        public string customerAddressPostalCode { get; set; }
    }

    public class WholeInformation 
    {
        public List<SupplierOrderInfo> SupplierOrderInfo { get; set; }
        public List<CoordinatorInfo> CoordinatorInfo { get; set; }
    }
}