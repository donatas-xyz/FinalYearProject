using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace SNW.Controllers
{
    /// <summary>
    /// Takes care of system users - authenticates and logs them in.
    /// </summary>
    public class AccountController : Controller
    {
        /// <summary>
        /// Co-ordinator's page
        /// </summary>
        /// <returns>List of pages to administer</returns>
        [Authorize(Roles = "Administrator")]
        public ActionResult AdminArea()
        {
            return View();
        }

        /// <summary>
        /// Login form. 
        /// Code taken from: http://www.codeproject.com/Articles/482546/Creating-a-custom-user-login-form-with-NET-Csharp
        /// </summary>
        /// <returns>Login page</returns>
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        /// <summary>
        /// Verifies username and password entered
        /// </summary>
        /// <param name="account">SNW.Models.Account.AccountLogin object</param>
        /// <returns>Home page on success, error on failure</returns>
        [HttpPost]
        public ActionResult Login(Models.Account.AccountLogin account)
        {
            /// Verifies if Model contains no errors
            if (ModelState.IsValid)
            {
                /// Verifies user's credentials
                if (account.IsValid(account.customerUsername, account.customerPassword))
                {
                    /// Sets browser cookie to initiate the user's session
                    FormsAuthentication.SetAuthCookie(account.customerUsername, account.rememberMe);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    /// Assigns an error to the Model
                    ModelState.AddModelError("", "Login data is incorrect!");
                }
            }
            return View(account);
        }

        /// <summary>
        /// Logs the user out of the system
        /// </summary>
        /// <returns>Home page</returns>
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Simple SHA1 (160-bit hash value) encoder
        /// </summary>
        public class SHA1
        {
            /// <summary>
            /// Encode user's password
            /// </summary>
            /// <param name="value">User's password in plain text</param>
            /// <returns>Returns encoded password</returns>
            public static string Encode(string value)
            {
                /// Creates an instance of the default implementation of SHA1
                var hash = System.Security.Cryptography.SHA1.Create();
                /// Initializes a new instance of the ASCIIEncoding class
                var encoder = new System.Text.ASCIIEncoding();
                /// Encodes all the characters in the 'value' into a sequence of bytes ot uses empty string if 'value' is empty
                var combined = encoder.GetBytes(value ?? "");
                /// Returns a string of computed hash value of 'combined' byte array
                return BitConverter.ToString(hash.ComputeHash(combined)).ToLower().Replace("-", "");
            }
        }
	}
}