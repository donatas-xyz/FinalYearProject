using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SNW.Models;
using SNW.Models.CustomerOrderInfo;
using System.Data.Entity.Infrastructure;

namespace SNW.Controllers
{
    /// <summary>
    /// Takes care of Members' Orders
    /// </summary>
    public class CustomerOrderController : Controller
    {
        /// Gets access to data entities via DBModelContainer class, which derives from DbContext
        private DBModelContainer db = new DBModelContainer();

        /// Get user's ID
        private int userID()
        {
            var customer = from Customers in db.Customers
                   where Customers.customerUsername == User.Identity.Name
                   select Customers.customerID;

            return customer.Single();
        }
        /// Get user order's ID
        private int orderID(int userID)
        {
            var order = from CustomerOrders in db.CustomerOrders
                        join Customers in db.Customers on CustomerOrders.customerID equals Customers.customerID
                        where Customers.customerID == userID && CustomerOrders.customerOrderStateID == 1
                        select CustomerOrders.customerOrderID;

            return order.FirstOrDefault();
        }

        /// GET: /CustomerOrder/
        /// Get all members' orders
        public ActionResult Index()
        {
            /// Check what orders to deliver to whom by role
            /// and then deliver the list
            if (User.Identity.Name == "administrator")
            {
                var customerOrders = (from CustomerOrders in db.CustomerOrders
                                      join OrderStates in db.OrderStates on CustomerOrders.customerOrderStateID equals OrderStates.orderStateID
                                      join Customers in db.Customers on CustomerOrders.customerID equals Customers.customerID
                                      orderby CustomerOrders.customerOrderStateID
                                      select new MemberOrder
                                      {
                                          customerOrderID = CustomerOrders.customerOrderID,
                                          orderStateName = OrderStates.orderStateName,
                                          customerOrderDate = CustomerOrders.customerOrderDate,
                                          customerOrderStateID = CustomerOrders.customerOrderStateID,
                                          customerName = (Customers.customerForename + " " + Customers.customerSurname),
                                          customerID = Customers.customerID
                                      });
                return View(customerOrders.ToList());
            }
            else 
            {
                var customerOrders = (from CustomerOrders in db.CustomerOrders
                                      join OrderStates in db.OrderStates on CustomerOrders.customerOrderStateID equals OrderStates.orderStateID
                                      join Customers in db.Customers on CustomerOrders.customerID equals Customers.customerID
                                      where Customers.customerUsername == User.Identity.Name
                                      orderby CustomerOrders.customerOrderStateID
                                      select new MemberOrder
                                      {
                                          customerOrderID = CustomerOrders.customerOrderID,
                                          orderStateName = OrderStates.orderStateName,
                                          customerOrderDate = CustomerOrders.customerOrderDate,
                                          customerOrderStateID = CustomerOrders.customerOrderStateID,
                                          customerName = (Customers.customerForename + " " + Customers.customerSurname),
                                          customerID = Customers.customerID
                                      });
                return View(customerOrders.ToList());
            }
        }

        /// <summary>
        /// Returns Order's Details
        /// </summary>
        /// <param name="id">Order's ID</param>
        /// <param name="state">Order's State</param>
        /// <param name="user">Order Owner's ID</param>
        /// <returns>Order details</returns>
        public ActionResult CustomerOrder(int? id, int? state, int? user) 
        {
                /// Request Order's details
                var listOfOrders = GetOrderInfo(id, state, user).ToList(); 

                /// If requested query isn't empty, show order details
                if (listOfOrders.Count > 0)
                {
                    return View(listOfOrders);
                }
                /// Redirect to proposal of creating a new Order.
                else
                {
                    string url = HttpContext.Request.Url.AbsolutePath;
                    return RedirectToAction("NoActiveOrders", "Error", new { path = url }); 
                }
        }

        /// Kind of interface to request order details
        public IEnumerable<MemberOrder> GetOrderInfo(int? id, int? state, int? user)
        {
            /// Instantiate an object of type FrequentlyAccessedQueries, or in other words - 
            /// get an access to frequently accessed queries class
            FrequentlyAccessedQueries query = new FrequentlyAccessedQueries();

            if (id == null && state == null && user == null) /// Means user went straight to CustomerOrder/CustomerOrder page.
            {                                                /// No info about user is being tranmitted from View to Controller.
                user = userID();
                id = orderID(userID());
                state = 1; /// Show current Pending Order.
                return query.OrderInfo(id, state).Where(d => d.customerOrderStateID == state && d.customerID == user && d.customerOrderID == id);
            }
            else /// Means user has been referred by another page for more information about the Order
            {
                return query.OrderInfo(id, state).Where(d => d.customerOrderStateID == state && d.customerOrderID == id && d.customerID == user);
            }
            
        }

        /// Amend order line depending on button pressed
        public ActionResult AmendOrderLine([Bind(Prefix = "MemberOrder")] CustomerOrderLine line, string SaveChanges, string RemoveLine, int id, int oID)
        {
            if (SaveChanges != null) 
            {
                /// Verifies if Model contains no errors
                if (ModelState.IsValid)
                {
                    db.Entry(line).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            else if (RemoveLine != null) 
            {
                CustomerOrderLine lineTemp = db.CustomerOrderLines.Find(id, oID);
                db.CustomerOrderLines.Remove(lineTemp);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(line);
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
