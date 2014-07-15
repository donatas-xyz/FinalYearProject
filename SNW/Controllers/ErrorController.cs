using SNW.Models.Error;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SNW.Controllers
{
    /// <summary>
    /// Manages errors
    /// </summary>
    public class ErrorController : Controller
    {
        private ErrorInformation information = new ErrorInformation();

        /// Return No Active Orders page with some options
        public ActionResult NoActiveOrders(string path)
        {
            if (path == "/SupplierOrder/SupplierOrder") 
            {
                information.entity = "Supplier";
                return View(information);
            }
            else if (path == "/CustomerOrder/CustomerOrder")
            {
                information.entity = "Customer";
                return View(information);
            }
            else 
            {
                information.entity = "";
                return View(information);
            }
        }

        /// Errors of 'No Access to resource' type
        public ActionResult NoAccess(string error)
        {
            information.error = error;
            return View(information);
        }

        /// Record cannot be deleted due to relations with other tables (PK -> FK)
        public ActionResult CannotDelete(string message, string result, string error)
        {
            information.error = message;
            information.code = result;
            information.entity = error;
            return View(information);
        }
	}
}