using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SNW.Models;
using SNW.Models.SupplierOrderInfo;
using SNW.Models.CustomerOrderInfo;
using SNW.Models.ProductInfo;

namespace SNW.Controllers
{
    /// <summary>
    /// Frequently Used Queries
    /// </summary>
    public class FrequentlyAccessedQueries : Controller
    {
        private DBModelContainer db = new DBModelContainer();

        /// <summary>
        /// USER QUERIES
        /// </summary>
        /// <returns>User information</returns>
        public IEnumerable<CoordinatorInfo> UserInfo()
        {
            var user = (from Customers in db.Customers
                        join Roles in db.Roles on Customers.customerRoleID equals Roles.roleID
                        select new CoordinatorInfo()
                        {
                            customerUsername = Customers.customerUsername,
                            customerName = (Customers.customerForename + " " + Customers.customerSurname),
                            customerAddressHouseNo = Customers.customerAddressHouseNo,
                            customerAddressStreet1 = Customers.customerAddressStreet1,
                            customerAddressTown = Customers.customerAddressTown,
                            customerAddressCounty = Customers.customerAddressCounty,
                            customerAddressPostalCode = Customers.customerAddressPostCode
                        });
            return user;
        }

        /// <summary>
        /// PRODUCT QUERIES
        /// </summary>
        /// <returns>Product information</returns>
        public IEnumerable<MemberProductList> ProductInfo() 
        {
            var productList = (from Products in db.Products
                               join ProductCategories in db.ProductCategories on Products.productCategoryID equals ProductCategories.productCategoryID
                               join ProductApprovals in db.ProductApprovals on Products.productApprovalID equals ProductApprovals.productApprovalID
                               select new MemberProductList()
                               {
                                   productID = Products.productID,
                                   productCategoryName = ProductCategories.productCategoryName,
                                   productApprovalID = Products.productApprovalID,
                                   productName = Products.productName,
                                   productOrganicStatus = Products.productOrganicStatus,
                                   productOrigin = Products.productOrigin,
                                   productWHSQty = Products.productWHSQty,
                                   productWHSsize = Products.productWHSsize,
                                   productWHSPrice = Products.productWHSPrice
                               });

            return productList;
        }

        /// <summary>
        /// MEMBER QUERIES
        /// </summary>
        /// <param name="id">Order ID</param>
        /// <param name="state">Order State</param>
        /// <returns>Order Information</returns>
        public IEnumerable<MemberOrder> OrderInfo(int? id, int? state)
        {
            /// Set initial order value
            decimal? customerOrderNetTotalSum = 0;

            try
            {
                /// get order details
                var memberOrder = (from CustomerOrders in db.CustomerOrders
                                   join OrderStates in db.OrderStates on CustomerOrders.customerOrderStateID equals OrderStates.orderStateID
                                   join CustomerOrderLines in db.CustomerOrderLines on CustomerOrders.customerOrderID equals CustomerOrderLines.customerOrderID
                                   join CustomerOrderPaymentMethods in db.CustomerOrderPaymentMethods on CustomerOrders.customerOrderPaymentMethodID equals CustomerOrderPaymentMethods.customerOrderPaymentMethodID
                                   join Customers in db.Customers on CustomerOrders.customerID equals Customers.customerID
                                   join Products in db.Products on CustomerOrderLines.productID equals Products.productID
                                   select new MemberOrder()
                                   {
                                       customerOrderID = CustomerOrders.customerOrderID,
                                       customerOrderAdvancePayment = CustomerOrders.customerOrderAdvancePayment,
                                       customerOrderPaymentMethodID = CustomerOrderPaymentMethods.customerOrderPaymentMethodID,
                                       orderStateName = OrderStates.orderStateName,
                                       customerOrderStateID = OrderStates.orderStateID,
                                       customerName = (Customers.customerForename + " " + Customers.customerSurname),
                                       customerID = Customers.customerID,
                                       customerOrderDate = CustomerOrders.customerOrderDate,
                                       customerOrderLineID = CustomerOrderLines.customerOrderLineID,
                                       productID = Products.productID,
                                       productName = Products.productName,
                                       productOrganicStatus = Products.productOrganicStatus,
                                       productWHSQty = Products.productWHSQty,
                                       productSNWQty = Products.productWHSQty / Products.productSNWDivisor,
                                       productWHSSize = Products.productWHSsize,
                                       productWHSPrice = Products.productWHSPrice,
                                       productSNWDivisor = Products.productSNWDivisor,
                                       productSNWPrice = Products.productWHSPrice / Products.productSNWDivisor,
                                       customerOrderUnitPrice = CustomerOrderLines.customerOrderUnitPrice,
                                       customerOrderQtyMin = CustomerOrderLines.customerOrderQtyMin,
                                       customerOrderQtyMax = CustomerOrderLines.customerOrderQtyMax,
                                       customerOrderQtyActual = CustomerOrderLines.customerOrderQtyActual,
                                       /// Add derived Totals to memberOrder list
                                       customerOrderTotal = CustomerOrderLines.customerOrderQtyMax * Products.productSNWPrice,
                                       customerOrderActualTotal = CustomerOrderLines.customerOrderQtyActual * CustomerOrderLines.customerOrderUnitPrice,
                                       customerOrderNetTotal = customerOrderNetTotalSum,
                                       customerOrderBalance = CustomerOrders.customerOrderAdvancePayment - customerOrderNetTotalSum,
                                       customerOrderPaymentMethod = (CustomerOrderPaymentMethods.customerOrderPaymentMethodName + ": " + CustomerOrderPaymentMethods.customerOrderPaymentMethodDescription)
                                   });

                /// If it's pending order, calculate total of QtyMax
                if (state == 1) 
                {
                    customerOrderNetTotalSum = memberOrder.Where(c => c.customerOrderID == id).Sum(s => s.customerOrderTotal);
                }
                /// If it's confirmed or fulfilled order, calculate total of actual (delivered) qty
                else 
                {
                    customerOrderNetTotalSum = memberOrder.Where(c => c.customerOrderID == id).Sum(s => s.customerOrderActualTotal); 
                }

                return memberOrder;
            }
            /// Stop if any MySQL exceptions occurs
            catch (MySql.Data.MySqlClient.MySqlException err)
            {
                Console.WriteLine("Error Message: \n" + err.Message +
                                   "Error Code: " + err.Number);
                return null;
            }
        }

        /// <summary>
        /// SUPPLIER QUERIES
        /// </summary>
        /// <param name="state">Order State</param>
        /// <returns>Order Details</returns>
        public IEnumerable<SupplierOrderInfo> OrderInfoPending(int? state)
        {
            /// Set initial order value
            decimal? supplierOrderNetTotalSum = 0;

            try
            {
                /// Get order details
                var supplierOrderInfo = (from SupplierOrders in db.SupplierOrders
                                         from Products in db.Products
                                         join OrderStates in db.OrderStates on SupplierOrders.supplierOrderStateID equals OrderStates.orderStateID
                                         join Suppliers in db.Suppliers on SupplierOrders.supplierID equals Suppliers.supplierID
                                         join CustomerOrderLines in db.CustomerOrderLines on Products.productID equals CustomerOrderLines.productID
                                         join CustomerOrders in db.CustomerOrders on CustomerOrderLines.customerOrderID equals CustomerOrders.customerOrderID
                                         where SupplierOrders.supplierOrderStateID == state && CustomerOrders.customerOrderStateID == state
                                         /// Solution taken from http://stackoverflow.com/questions/5072956/linq-to-entities-join-two-tables-then-group-and-take-sums-of-columns-from-both
                                         group new { Products, CustomerOrderLines, Suppliers, SupplierOrders, OrderStates, CustomerOrders } by new
                                         {
                                             Products.productID,
                                             SupplierOrders.supplierID,
                                             Suppliers.supplierName,
                                             OrderStates.orderStateName,
                                             SupplierOrders.supplierOrderDate,
                                             SupplierOrders.supplierOrderDeadline,
                                             SupplierOrders.supplierOrderID,
                                             Products.productName,
                                             Products.productOrganicStatus,
                                             Products.productWHSQty,
                                             Products.productWHSsize,
                                             Products.productWHSPrice,
                                             SupplierOrders.supplierOrderReqDeliveryDate,
                                             Suppliers.supplierAddressHouseNo,
                                             Suppliers.supplierAddressStreet1,
                                             Suppliers.supplierAddressTown,
                                             Suppliers.supplierAddressCounty,
                                             Suppliers.supplierAddressPostCode
                                         } into g
                                         select new SupplierOrderInfo()
                                         {
                                             supplierOrderID = g.Key.supplierOrderID,
                                             supplierOrderStateName = g.Key.orderStateName,
                                             supplierName = g.Key.supplierName,
                                             supplierOrderDate = g.Key.supplierOrderDate,
                                             supplierOrderDeadline = g.Key.supplierOrderDeadline,
                                             productID = g.Key.productID,
                                             productName = g.Key.productName,
                                             productOrganicStatus = g.Key.productOrganicStatus,
                                             productWHSQty = g.Key.productWHSQty,
                                             productWHSSize = g.Key.productWHSsize,
                                             productWHSPrice = g.Key.productWHSPrice,
                                             qtyOrdered = g.Sum(c => c.CustomerOrderLines.customerOrderQtyMax),
                                             supplierOrderTotal = 0,
                                             supplierOrderNetTotal = supplierOrderNetTotalSum,
                                             supplierOrderReqDeliveryDate = g.Key.supplierOrderReqDeliveryDate,
                                             supplierAddressHouseNo = g.Key.supplierAddressHouseNo,
                                             supplierAddressStreet1 = g.Key.supplierAddressStreet1,
                                             supplierAddressTown = g.Key.supplierAddressTown,
                                             supplierAddressCounty = g.Key.supplierAddressCounty,
                                             supplierAddressPostCode = g.Key.supplierAddressPostCode,
                                         });
                                        /// Solution ends here

                /// Get order's total from QtyMax
                supplierOrderNetTotalSum = supplierOrderInfo.ToList().Select(t => t.supplierOrderTotal).Sum();

                return supplierOrderInfo;
            }
            catch (MySql.Data.MySqlClient.MySqlException err)
            {
                Console.WriteLine("Error Message: \n" + err.Message +
                                   "Error Code: " + err.Number);
                return null;
            }
        }
        public IEnumerable<SupplierOrderInfo> OrderInfoConfirmedFulfilled(int? id, int? state)
        {
            decimal? supplierOrderNetTotalSum = 0;

            try
            {
                var supplierOrderInfo = (from SupplierOrders in db.SupplierOrders
                                         from Products in db.Products
                                         join OrderStates in db.OrderStates on SupplierOrders.supplierOrderStateID equals OrderStates.orderStateID
                                         join Suppliers in db.Suppliers on SupplierOrders.supplierID equals Suppliers.supplierID
                                         join CustomerOrderLines in db.CustomerOrderLines on Products.productID equals CustomerOrderLines.productID
                                         join CustomerOrders in db.CustomerOrders on CustomerOrderLines.customerOrderID equals CustomerOrders.customerOrderID
                                         where SupplierOrders.supplierOrderStateID == state && SupplierOrders.supplierOrderID == id && (CustomerOrderLines.supplierOrderID == id || CustomerOrderLines.supplierOrderID == null)
                                         group new { Products, CustomerOrderLines, Suppliers, SupplierOrders, OrderStates, CustomerOrders } by new
                                         {
                                             Products.productID,
                                             SupplierOrders.supplierID,
                                             Suppliers.supplierName,
                                             OrderStates.orderStateName,
                                             SupplierOrders.supplierOrderDate,
                                             SupplierOrders.supplierOrderDeadline,
                                             SupplierOrders.supplierOrderID,
                                             SupplierOrders.supplierOrderStateID,
                                             Products.productName,
                                             Products.productOrganicStatus,
                                             Products.productWHSQty,
                                             Products.productWHSsize,
                                             Products.productWHSPrice,
                                             SupplierOrders.supplierOrderReqDeliveryDate,
                                             Suppliers.supplierAddressHouseNo,
                                             Suppliers.supplierAddressStreet1,
                                             Suppliers.supplierAddressTown,
                                             Suppliers.supplierAddressCounty,
                                             Suppliers.supplierAddressPostCode
                                         } into g
                                         select new SupplierOrderInfo()
                                         {
                                             supplierOrderID = g.Key.supplierOrderID,
                                             supplierOrderStateID = g.Key.supplierOrderStateID,
                                             supplierOrderStateName = g.Key.orderStateName,
                                             supplierName = g.Key.supplierName,
                                             supplierOrderDate = g.Key.supplierOrderDate,
                                             supplierOrderDeadline = g.Key.supplierOrderDeadline,
                                             productID = g.Key.productID,
                                             productName = g.Key.productName,
                                             productOrganicStatus = g.Key.productOrganicStatus,
                                             productWHSQty = g.Key.productWHSQty,
                                             productWHSSize = g.Key.productWHSsize,
                                             productWHSPrice = g.Key.productWHSPrice,
                                             qtyOrdered = g.Sum(c => c.CustomerOrderLines.customerOrderQtyMax),
                                             supplierOrderTotal = 0,
                                             supplierOrderNetTotal = supplierOrderNetTotalSum,
                                             supplierOrderReqDeliveryDate = g.Key.supplierOrderReqDeliveryDate,
                                             supplierAddressHouseNo = g.Key.supplierAddressHouseNo,
                                             supplierAddressStreet1 = g.Key.supplierAddressStreet1,
                                             supplierAddressTown = g.Key.supplierAddressTown,
                                             supplierAddressCounty = g.Key.supplierAddressCounty,
                                             supplierAddressPostCode = g.Key.supplierAddressPostCode,
                                         });

                supplierOrderNetTotalSum = supplierOrderInfo.ToList().Select(t => t.supplierOrderTotal).Sum();

                return supplierOrderInfo;
            }
            catch (MySql.Data.MySqlClient.MySqlException err)
            {
                Console.WriteLine("Error Message: \n" + err.Message +
                                   "Error Code: " + err.Number);
                return null;
            }
        }
	}
}