using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SNW.Models;
using SNW.Models.ProductInfo;

namespace SNW.Controllers
{
    /// <summary>
    /// Controls everything about the products
    /// </summary>
    public class ProductController : Controller
    {
        private DBModelContainer db = new DBModelContainer();

        /// Get user ID
        private int userID()
        {
            var customer = from Customers in db.Customers
                   where Customers.customerUsername == User.Identity.Name
                   select Customers.customerID;

            return customer.Single();
        }
        /// get orderID
        private int orderID(int userID) 
        {
            var order = from CustomerOrders in db.CustomerOrders
                        join Customers in db.Customers on CustomerOrders.customerID equals Customers.customerID
                        where Customers.customerID == userID && CustomerOrders.customerOrderStateID == 1
                        select CustomerOrders.customerOrderID;

            return order.FirstOrDefault();
        }

        /// GET: /Product/
        /// Display list of products for administration
        [Authorize(Roles = "Administrator")]
        public ActionResult Index(string search)
        {
            var products = db.Products.Include(p => p.ProductCategory).Include(p => p.ProductApproval);
            return View(products.ToList());
        }

        /// <summary>
        /// Shows list of products that can be ordered
        /// </summary>
        /// <param name="search">Search query</param>
        /// <returns>List of Products</returns>
        public ActionResult ProductList(string search)
        {
            var productList = GetProductInfo(search);
            return View(productList.ToList());
        }

        /// Kind of interface to request product details
        public IEnumerable<MemberProductList> GetProductInfo(string search) 
        {
            FrequentlyAccessedQueries query = new FrequentlyAccessedQueries();

            if (search != null && search.Length > 1) // Retrieve products based on search query
            {
                return query.ProductInfo().Where(d => d.productName.ToUpper().Contains(search.ToUpper()) || d.productID.ToString().Contains(search));
            }
            else /// Retrieve all available products.
            {
                return query.ProductInfo();
            }
        }

        /// <summary>
        /// Adds products to pending order
        /// </summary>
        /// <param name="id">Product id</param>
        /// <returns>List of Products</returns>
        [HttpPost, ActionName("AddToOrder")]
        public ActionResult AddToOrder(int id)
        {
            /// Get Member Order details 
            var memberOrder = (from CustomerOrders in db.CustomerOrders
                               join OrderStates in db.OrderStates on CustomerOrders.customerOrderStateID equals OrderStates.orderStateID
                               join CustomerOrderLines in db.CustomerOrderLines on CustomerOrders.customerOrderID equals CustomerOrderLines.customerOrderID
                               join Customers in db.Customers on CustomerOrders.customerID equals Customers.customerID
                               join Products in db.Products on CustomerOrderLines.productID equals Products.productID
                               where Customers.customerUsername == User.Identity.Name
                               select new MemberOrderDetails()
                               {
                                   customerOrderID = CustomerOrders.customerOrderID,
                                   customerOrderQtyMin = CustomerOrderLines.customerOrderQtyMin,
                                   productID = CustomerOrderLines.productID,
                                   customerOrderQtyMax = CustomerOrderLines.customerOrderQtyMax,
                                   customerOrderUnitPrice = CustomerOrderLines.customerOrderUnitPrice,
                                   customerID = Customers.customerID,
                                   customerOrderStateID = OrderStates.orderStateID
                               }).ToList();

            /// Check current price in DB
            Product price = db.Products.AsNoTracking().Single(x => x.productID == id);
            /// Check how many Orders are on a list
            int memberOrderCount = memberOrder.Count();
            /// Check if there are any Pending Orders
            IEnumerable<MemberOrderDetails> customerStateID = memberOrder.Where(e => e.customerOrderStateID == 1);

            /// Check if there exists any orders. If no:
            if (memberOrderCount == 0 || !customerStateID.Any())
            {
                /// Create and add a new Order.
                db.CustomerOrders.Add(new CustomerOrder
                {
                    customerOrderID = default(int),
                    customerOrderStateID = 1,
                    customerID = userID(),
                    customerOrderPaymentMethodID = 1,
                    customerOrderDate = DateTime.Now,
                    customerOrderAdvancePayment = 0
                });

                /// Create and add a new Order Line to the Order.
                db.CustomerOrderLines.Add(new CustomerOrderLine
                {
                    customerOrderLineID = default(int),
                    customerOrderID = orderID(userID()),
                    supplierOrderID = default(int?),
                    productID = id,
                    customerOrderUnitPrice = price.productSNWPrice,
                    customerOrderQtyMin = 0,
                    customerOrderQtyMax = 1,
                    customerOrderQtyActual = 0
                });

                /// Ask the DataContext to save all the changes
                db.SaveChanges();
            }///if yes:
            else
            {
                try 
                {
                    /// Query for a specific Order Line. 
                    var orderLine = (from a in db.CustomerOrderLines
                                    join b in db.CustomerOrders on a.customerOrderID equals b.customerOrderID
                                    join c in db.OrderStates on b.customerOrderStateID equals c.orderStateID
                                    where a.productID == id && c.orderStateID == 1 && b.Customer.customerUsername == User.Identity.Name
                                    select a).FirstOrDefault();

                    /// Check if particular product already exist in Order
                    if (orderLine != null && orderLine.productID == id)
                    {
                        /// Change the Max Qty.
                        ++orderLine.customerOrderQtyMax;
                    }
                    else 
                    {
                        int customerOID = orderID(userID());
                        int prderPID = id;
                        /// Create and add a new Order Line to the Order.
                        db.CustomerOrderLines.Add(new CustomerOrderLine
                        {
                            customerOrderLineID = default(int),
                            customerOrderID = customerOID,
                            supplierOrderID = default(int?),
                            productID = prderPID,
                            customerOrderUnitPrice = price.productSNWPrice,
                            customerOrderQtyMin = 0,
                            customerOrderQtyMax = 1,
                            customerOrderQtyActual = 0
                        });
                    }
                }
                catch (MySql.Data.MySqlClient.MySqlException err)
                {
                    Console.WriteLine("Error Message: \n" + err.Message + 
                                       "Error Code: " + err.Number);
                }
                /// Ask the DataContext to save all the changes
                db.SaveChanges();
            }
            return RedirectToAction("ProductList");
        }

        /// <summary>
        /// Send report of wrong price to co-ordinator
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <returns>Confirmation View</returns>
        [HttpPost, ActionName("ReportPrice")]
        public ActionResult ReportPrice(int id)
        {
            /// Check current Product information
            Product information = db.Products.AsNoTracking().Single(x => x.productID == id);
            /// Set required parameters 
            string destination = "ds286@uni.brighton.ac.uk";
            string subject = "SNW Price report";
            string body = "The Product " + id + " " + information.productName + " has been reported for a wrong price of £" + information.productWHSPrice;
            /// Send email to co-ordinator
            return RedirectToAction("Confirmation", "Mail", new { destination, subject, body }); // Redirect to Mail Controller.
        }

        /// GET: /Product/Details/5
        public ActionResult Details(int? id)
        {
            /// Check if product's ID has been submitted by View
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            /// Retrieve product details
            Product product = db.Products.Find(id);

            /// Check if returned query contains any records
            if (product == null)
            {
                return HttpNotFound();
            }
            
            return View(product);
        }

        /// GET: /Product/Create
        public ActionResult Create()
        {
            // Get form's lists
            ViewBag.productCategoryID = new SelectList(db.ProductCategories, "productCategoryID", "productCategoryName");
            ViewBag.productApprovalID = new SelectList(db.ProductApprovals, "productApprovalID", "productApprovalName");
            return View();
        }

        /// POST: /Product/Create
        /// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        /// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="productID,productCategoryID,productApprovalID,productName,productDescription,productOrganicStatus,productOrigin,productWHSQty,productWHSsize,productWHSPrice,productSNWDivisor,productSNWPrice,productDateLastChanged,productPriceLastChanged")] Product product)
        {
            /// Verifies if Model contains no errors
            if (ModelState.IsValid)
            {
                /// Create temporary product with derived values
                Product productTemp = new Product()
                    {
                        productID = product.productID,
                        productCategoryID = product.productCategoryID,
                        productApprovalID = product.productApprovalID,
                        productName = product.productName,
                        productDescription = product.productDescription,
                        productOrganicStatus = product.productOrganicStatus,
                        productOrigin = product.productOrigin,
                        productWHSQty = product.productWHSQty,
                        productWHSsize = product.productWHSsize,
                        productWHSPrice = product.productWHSPrice,
                        productSNWDivisor = product.productSNWDivisor,
                        productSNWPrice = product.productWHSPrice / product.productSNWDivisor,
                        productDateLastChanged = DateTime.Now,
                        productPriceLastChanged = DateTime.Now
                    };

                /// Add product to database
                db.Products.Add(productTemp);
                db.SaveChanges();
                return RedirectToAction("ProductList");
            }

            ViewBag.productCategoryID = new SelectList(db.ProductCategories, "productCategoryID", "productCategoryName", product.productCategoryID);
            ViewBag.productApprovalID = new SelectList(db.ProductApprovals, "productApprovalID", "productApprovalName", product.productApprovalID = 1);
            return View(product);
        }

        /// GET: /Product/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.productCategoryID = new SelectList(db.ProductCategories, "productCategoryID", "productCategoryName", product.productCategoryID);
            ViewBag.productApprovalID = new SelectList(db.ProductApprovals, "productApprovalID", "productApprovalName", product.productApprovalID);
            return View(product);
        }

        /// POST: /Product/Edit/5
        /// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        /// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="productID,productCategoryID,productApprovalID,productName,productDescription,productOrganicStatus,productOrigin,productWHSQty,productWHSsize,productWHSPrice,productSNWDivisor,productSNWPrice,productDateLastChanged,productPriceLastChanged")] Product product)
        {
            if (ModelState.IsValid)
            {
                /// Check current values in DB
                Product price = db.Products.AsNoTracking().Single(x => x.productID == product.productID);

                /// Change the date
                product.productDateLastChanged = DateTime.Now;

                /// Check if price has been changed
                if (price.productWHSPrice != product.productWHSPrice)
                {
                    product.productPriceLastChanged = DateTime.Now;
                    product.productSNWPrice = product.productWHSPrice / product.productSNWDivisor;
                }
                else
                {
                    product.productPriceLastChanged = product.productPriceLastChanged;
                };

                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.productCategoryID = new SelectList(db.ProductCategories, "productCategoryID", "productCategoryName", product.productCategoryID);
            ViewBag.productApprovalID = new SelectList(db.ProductApprovals, "productApprovalID", "productApprovalName", product.productApprovalID);
            return View(product);
        }

        /// GET: /Product/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        /// POST: /Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);
            try
            {
                db.Products.Remove(product);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException err)
            {
                return RedirectToAction("CannotDelete", "Error", new { message = err.InnerException.InnerException.Message, result = err.InnerException.HResult, error = err.InnerException.Message}); // Redirect to Error Controller.
            }
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
