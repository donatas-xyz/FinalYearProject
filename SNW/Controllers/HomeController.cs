using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SNW.Controllers
{
    /// <summary>
    /// Controls Home page
    /// </summary>
    public class HomeController : Controller
    {
        /// <summary>
        /// Display main page
        /// </summary>
        /// <returns>Home Page</returns>
       [Authorize(Roles = "Customer, Administrator")]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Display Contacts page
        /// </summary>
        /// <returns>Contacts page</returns>
        public ActionResult Contact()
        {
            /// Message passed to a View. 
            ViewBag.Message = "Should you have any questions, do not hesitate to contact me.";
            return View();
        }
    }
}