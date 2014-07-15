using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SNW.Models.CustomerOrderInfo
{
    /// Model for CustomerOrderController
    public class MemberOrder : CustomerOrder
    {
        /// Allows to access property (variable) from other classes
        public new int customerOrderID { get; set; }
        public string customerName { get; set; }
        public string customerUsername { get; set; }
        public int customerID { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public new System.DateTime customerOrderDate { get; set; }
        public string productName { get; set; }
        public int customerOrderLineID { get; set; }
        public int productID { get; set; }
        public string productOrganicStatus { get; set; }
        public decimal productWHSQty { get; set; }
        public decimal productSNWQty { get; set; }
        public string productWHSSize { get; set; }
        public decimal productWHSPrice { get; set; }
        /// Calculate Product SNW Price (price per item)
        public int _productSNWDivisor;
        public int productSNWDivisor
        {
            get
            {
                return _productSNWDivisor;
            }
            set
            {
                _productSNWDivisor = value;
            }
        }
        public decimal _productSNWPrice;
        public decimal productSNWPrice
        {
            get
            {
                return _productSNWPrice;
            }
            set
            {
                _productSNWPrice = productWHSPrice / _productSNWDivisor;
            }
        }
        public decimal? customerOrderUnitPrice { get; set; }
        public decimal customerOrderQtyMin { get; set; }
        public decimal customerOrderQtyMax { get; set; }
        public string customerOrderPaymentMethod { get; set; }
        public int customerOrderPaymentMethodID { get; set; }
        public decimal? customerOrderAdvancePayment { get; set; }
        public decimal? customerOrderQtyActual { get; set; }
        public decimal? customerOrderTotal { get; set; }
        public decimal? customerOrderActualTotal { get; set; }
        public decimal? customerOrderBalance { get; set; }
        public decimal? customerOrderNetTotal { get; set; }
        public string _orderStateName;
        public string orderStateName
        {
            get
            {
                return _orderStateName;
            }
            set
            {
                _orderStateName = value;
            }
        }
        public new int customerOrderStateID { get; set; }
        public int? supplierOrderID { get; set; }
    }
}