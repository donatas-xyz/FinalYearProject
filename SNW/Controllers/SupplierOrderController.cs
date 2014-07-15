using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SNW.Models;
using SNW.Models.SupplierOrderInfo;
using SNW.Controllers;

namespace SNW.Controllers
{
    /// <summary>
    /// Takes care of supplier order logic
    /// </summary>
    public class SupplierOrderController : Controller
    {
        private DBModelContainer db = new DBModelContainer();

        /// GET: /SupplierOrder list/
        public ActionResult Index()
        {
            /// only administrator can access this page
            if (User.Identity.Name == "administrator")
            {
                var supplierOrders = (from SupplierOrders in db.SupplierOrders
                                      join OrderStates in db.OrderStates on SupplierOrders.supplierOrderStateID equals OrderStates.orderStateID
                                      orderby SupplierOrders.supplierOrderStateID
                                      select new SupplierOrderInfo
                                      {
                                          supplierOrderID = SupplierOrders.supplierOrderID,
                                          supplierOrderStateName = OrderStates.orderStateName,
                                          supplierOrderStateID = OrderStates.orderStateID,
                                          supplierOrderDate = SupplierOrders.supplierOrderDate
                                      });
                return View(supplierOrders.ToList());
            }
            else
            /// Redirect to error page.
            {
                return RedirectToAction("NoAccess", "Error", new { error = "You do not have sufficient permissions to access this page." }); 
            }
        }

        /// <summary>
        /// Get information of supplier order
        /// </summary>
        /// <param name="id">Order ID</param>
        /// <param name="state">Order State</param>
        /// <param name="username">Username</param>
        /// <returns></returns>
        public ActionResult SupplierOrder(int? id, int? state, string username)
        {
            /// Get information from separate sets of entities (co-ordinator and supplier order)
            var coordinatorInfo = UserInfo(username).ToList();
            var orderInfo = GetOrderInfo(id, state).ToList();

            /// Merge these two list to one
            var listOfOrders = new WholeInformation
            {
                SupplierOrderInfo = orderInfo,
                CoordinatorInfo = coordinatorInfo
            };

            if (coordinatorInfo.Count > 0 && orderInfo.Count > 0)
            {
                return View(listOfOrders);
            }
            else
            {
                string url = HttpContext.Request.Url.AbsolutePath;
                return RedirectToAction("NoActiveOrders", "Error", new { path = url }); // Redirect to proposal of creating a new Order.
            }
        }

        /// Kind of interface to request user's details
        public IEnumerable<CoordinatorInfo> UserInfo(string username)
        { 
            FrequentlyAccessedQueries query = new FrequentlyAccessedQueries();
            if (username == null)
            {
                return query.UserInfo().Where(i => i.customerUsername == User.Identity.Name);
            }
            else 
            {
                return query.UserInfo().Where(i => i.customerUsername == username);
            }
            
        }

        /// Kind of interface to request order's details
        public IEnumerable<SupplierOrderInfo> GetOrderInfo(int? id, int? state)
        {
            FrequentlyAccessedQueries query = new FrequentlyAccessedQueries();

            if (id == null && state == null) /// Means user went straight to SupplierOrder/SupplierOrder page.
            {                                /// No info about user is being tranmitted from View to Controller.
                state = 1; /// Show current Pending Order.
                return query.OrderInfoPending(state);
            }
            else /// Means user has been referred by another page for more information about the Order
            {
                return query.OrderInfoConfirmedFulfilled(id, state);
            }

        }

        /// <summary>
        /// Changes order's required delivery date
        /// </summary>
        /// <param name="order">Order object</param>
        /// <returns>Return the order</returns>
        [HttpPost, ActionName("ChangeDate")]
        public ActionResult ChangeDate([Bind] SupplierOrder order)
        {
            /// Verifies if Model contains no errors
            if (ModelState.IsValid)
            {
                /// Check current values in DB
                SupplierOrder orders = db.SupplierOrders.AsNoTracking().Single(x => x.supplierOrderID == order.supplierOrderID);

                /// Check if date has been changed
                if (orders.supplierOrderReqDeliveryDate != order.supplierOrderReqDeliveryDate)
                {
                    orders.supplierOrderReqDeliveryDate = order.supplierOrderReqDeliveryDate;
                }
                else
                {
                    orders.supplierOrderReqDeliveryDate = orders.supplierOrderReqDeliveryDate;
                };

                db.Entry(orders).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(order);
        }

        [HttpPost, ActionName("ConfirmOrder")]
        public ActionResult ConfirmOrder(int id)
        {
            /// CUSTOMER
            /// Find all Pending Customer Order Lines
            var pendingCustomerOrderLines = (from CustomerOrderLines in db.CustomerOrderLines
                                             join CustomerOrders in db.CustomerOrders on CustomerOrderLines.customerOrderID equals CustomerOrders.customerOrderID
                                             where CustomerOrders.customerOrderStateID == 1
                                             select CustomerOrderLines);

            /// Set Pending Customer Order Lines to Confirmed
            foreach (CustomerOrderLine CustomerOrderLines in pendingCustomerOrderLines)
            {
                CustomerOrderLines.supplierOrderID = id;
            }

            /// Find all Pending Customer Orders
            var pendingCustomerOrders = (from CustomerOrders in db.CustomerOrders
                                        where CustomerOrders.customerOrderStateID == 1
                                        select CustomerOrders);

            /// Set Pending Customer Orders to Confirmed
            foreach (CustomerOrder CustomerOrders in pendingCustomerOrders)
            {
                CustomerOrders.customerOrderStateID = 2;
            }

            /// SUPPLIER
            /// Select current Supplier Order
            var pendingSupplierOrders = (from SupplierOrders in db.SupplierOrders
                                         where SupplierOrders.supplierOrderStateID == 1
                                         select SupplierOrders);

            /// Set Supplier Order to Confirmed
            foreach (SupplierOrder SupplierOrders in pendingSupplierOrders)
            {
                SupplierOrders.supplierOrderStateID = 2;
            }

            /// Create new Pending Supplier Order
            db.SupplierOrders.Add(new SupplierOrder
            {
                supplierOrderID = default(int),
                supplierOrderStateID = 1,
                supplierID = 1, // Ideally should be retrieved
                supplierOrderDeadline = default(DateTime),
                supplierOrderDate = DateTime.Now,
                supplierOrderReqDeliveryDate = default(DateTime)
            });

            /// SAVE CHANGES
            /// Submit the changes to the database. 
            try
            {
                db.SaveChanges();
            }
            catch (MySql.Data.MySqlClient.MySqlException err)
            {
                Console.WriteLine("Error Message: \n" + err.Message + 
                                   "Error Code: " + err.Number);
            }
            /// Refresh the page
            return RedirectToAction("SupplierOrder");
        }

        public ActionResult FulfilOrder(int id)
        {
            /// CUSTOMER
            /// Find all Confirmed Customer Order Lines
            var pendingCustomerOrderLines = (from CustomerOrderLines in db.CustomerOrderLines
                                             join CustomerOrders in db.CustomerOrders on CustomerOrderLines.customerOrderID equals CustomerOrders.customerOrderID
                                             where CustomerOrders.customerOrderStateID == 2
                                             select CustomerOrderLines);

            /// Set Confirmed Customer Order Lines to Fulfilled
            foreach (CustomerOrderLine CustomerOrderLines in pendingCustomerOrderLines)
            {
                CustomerOrderLines.supplierOrderID = id;
            }

            /// Find all Confirmed Customer Orders
            var pendingCustomerOrders = (from CustomerOrders in db.CustomerOrders
                                         where CustomerOrders.customerOrderStateID == 2
                                         select CustomerOrders);

            /// Set Confirmed Customer Orders to Fulfilled
            foreach (CustomerOrder CustomerOrders in pendingCustomerOrders)
            {
                CustomerOrders.customerOrderStateID = 3;
            }

            /// SUPPLIER
            /// Select Confirmed Supplier Order
            var pendingSupplierOrders = (from SupplierOrders in db.SupplierOrders
                                         where SupplierOrders.supplierOrderStateID == 2
                                         select SupplierOrders);

            /// Set Supplier Order to Fulfilled
            foreach (SupplierOrder SupplierOrders in pendingSupplierOrders)
            {
                SupplierOrders.supplierOrderStateID = 3;
            }

            /// SAVE CHANGES
            /// Submit the changes to the database. 
            try
            {
                db.SaveChanges();
            }
            catch (MySql.Data.MySqlClient.MySqlException err)
            {
                Console.WriteLine("Error Message: \n" + err.Message +
                                   "Error Code: " + err.Number);
            }
            /// Refresh the page
            return RedirectToAction("SupplierOrder");
        }

        /// <summary>
        /// Garbage collect unmanaged objects
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
