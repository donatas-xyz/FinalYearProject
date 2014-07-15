using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SNW.Models;

namespace SNW.Controllers
{
    /// <summary>
    /// Takes care of the member logged in the system
    /// </summary>
    public class CustomerController : Controller
    {
        /// Gets access to data entities via DBModelContainer class, which derives from DbContext
        private DBModelContainer db = new DBModelContainer();

        /// <summary>
        /// GET: /Customer/
        /// </summary>
        /// <returns>List of Members</returns>
        public ActionResult Index()
        {
            /// only administrator can access this page
            if (User.Identity.Name == "administrator")
            {
                /// Retrieve the list of members.
                var customers = db.Customers.Include(c => c.CustomerRole);
                return View(customers.ToList());
            }
            else
            /// Redirect to error page.
            {
                return RedirectToAction("NoAccess", "Error", new { error = "You do not have sufficient permissions to access this page." });
            }
        }

        /// <summary>
        ///  GET: /Customer/Details/5
        /// </summary>
        /// <param name="id">Member ID</param>
        /// <returns>Member's Details</returns>
        [Authorize(Roles = "Administrator")]
        public ActionResult Details([Bind(Exclude = "customerPassword")]int? id)
        {
            /// Check if member's ID has been submitted by View
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            /// Retrieve member's details
            Customer customer = db.Customers.Find(id);

            /// Do not retrieve current passwords
            customer.customerPassword = null;

            /// Check if returned query contains any records
            if (customer == null)
            {
                return HttpNotFound();
            }

            return View(customer);
        }

        /// <summary>
        /// GET: /Customer/Create
        /// </summary>
        /// <returns>Form to create new Member</returns>
        [Authorize(Roles = "Administrator")]
        public ActionResult Create()
        {
            /// Get form's lists
            ViewBag.customerRoleID = new SelectList(db.Roles, "roleID", "roleName");
            return View();
        }

        /// POST: /Customer/Create
        /// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        /// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public ActionResult Create([Bind(Include = "customerID,customerRoleID,customerForename,customerSurname,customerPhone,customerEmail,customerUsername,customerPassword,customerAddressHouseNo,customerAddressStreet1,customerAddressStreet2,customerAddressTown,customerAddressCounty,customerAddressPostCode,customerAddressCountry")] Customer customer)
        {
            /// Verifies if Model contains no errors
            if (ModelState.IsValid)
            {
                /// Encode new member's password before saving it to the database
                customer.customerPassword = Controllers.AccountController.SHA1.Encode(customer.customerPassword);

                /// Add new member
                db.Customers.Add(customer);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            /// Display list of roles with Customer selected as default option
            ViewBag.customerRoleID = new SelectList(db.Roles, "roleID", "roleName", customer.customerRoleID == 2);
            /// Display the form
            return View(customer);
        }

        /// <summary>
        ///  GET: /Customer/Edit/5
        /// </summary>
        /// <param name="id">Member ID</param>
        /// <returns>Form to edit the Member</returns>
        [Authorize(Roles = "Administrator")]
        public ActionResult Edit(int? id)
        {
            /// Check if member's ID has been submitted by View
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            /// Retrieve member's details
            Customer customer = db.Customers.Find(id);

            /// Do not retrieve current passwords
            customer.customerPassword = null;

            /// Check if returned query contains any records
            if (customer == null)
            {
                return HttpNotFound();
            }

            /// Display list of roles with Customer selected as default option
            ViewBag.customerRoleID = new SelectList(db.Roles, "roleID", "roleName", customer.customerRoleID);
            /// Display the form
            return View(customer);
        }

        /// POST: /Customer/Edit/5
        /// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        /// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public ActionResult Edit([Bind(Include = "customerID,customerRoleID,customerForename,customerSurname,customerPhone,customerEmail,customerPassword,customerAddressHouseNo,customerAddressStreet1,customerAddressTown,customerAddressCounty,customerAddressPostCode,customerAddressCountry")] Customer customer)
        {
            /// Check if member's ID has been submitted by View
            if (ModelState.IsValid)
            {
                /// Get member's information in order sensitive data not getting sent via HTTP POST
                Customer customerTemp = db.Customers.AsNoTracking().Single(x => x.customerID == customer.customerID);

                /// Pass information not being changed to current result
                customer.customerUsername = customerTemp.customerUsername;

                /// Check if password has been changed
                if(customer.customerPassword == null)
                {
                    customer.customerPassword = customerTemp.customerPassword;
                }
                else 
                {
                    customerTemp.customerPassword = Controllers.AccountController.SHA1.Encode(customer.customerPassword);
                    customer.customerPassword = customerTemp.customerPassword;
                }

                db.Entry(customer).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            /// Display list of roles with Customer selected as default option
            ViewBag.customerRoleID = new SelectList(db.Roles, "roleID", "roleName", customer.customerRoleID);
            /// Display the form
            return View(customer);
        }

        /// GET: /Customer/Delete/5
        [Authorize(Roles = "Administrator")]
        public ActionResult Delete(int? id)
        {
            /// Check if member's ID has been submitted by View
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            /// Retrieve member's details
            Customer customer = db.Customers.Find(id);

            /// Do not retrieve current passwords
            customer.customerPassword = null;

            /// Check if returned query contains any records
            if (customer == null)
            {
                return HttpNotFound();
            }

            return View(customer);
        }

        /// POST: /Customer/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public ActionResult DeleteConfirmed(int id)
        {
            /// Retrieve member's details
            Customer customer = db.Customers.Find(id);
            db.Customers.Remove(customer);
            db.SaveChanges();
            return RedirectToAction("Index");
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
