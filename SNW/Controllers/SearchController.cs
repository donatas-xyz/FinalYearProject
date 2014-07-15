using SNW.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SNW.Controllers
{
    /// <summary>
    /// Allows searching for products and potentially for other things in the future
    /// </summary>
    public class SearchController : Controller
    {
        private DBModelContainer db = new DBModelContainer();

        /// Interface to searches
        [HttpPost]
        public ActionResult Search(string name, string searchProduct)
        {
            ProductController products = new ProductController();
            /// Get current referrer url. Not the best way of doing it. 
            string controller = Request.UrlReferrer.Segments[1];
            string action = Request.UrlReferrer.Segments[2];

            if (searchProduct != null && name != null)
            {
                return RedirectToAction(action, controller, new { search = name });
            }
            else 
            {
                return RedirectToAction(action, controller); /// Should be some warning perhaps
            }
        }
	}
}