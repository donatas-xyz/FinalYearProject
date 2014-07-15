using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SNW.Models;

namespace SNW.Models.Account
{
    public class AccountLogin : Customer
    {
        private DBModelContainer db = new DBModelContainer();

        /// Solution taken from http://www.codeproject.com/Articles/482546/Creating-a-custom-user-login-form-with-NET-Csharp
        [Required]
        [Display(Name = "Username")]
        public new string customerUsername { get; set; } /// override customerEmail in upperclass 

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public new string customerPassword { get; set; }

        [Display(Name = "Remember me on this device")]
        public bool rememberMe { get; set; }

        /// <summary>
        /// Checks if user with given password exists in the database
        /// </summary>
        /// <param name="_customerUsername">Username</param>
        /// <param name="_customerPassword">Password</param>
        /// <returns>True if user exist and password is correct</returns>
        public bool IsValid(string _customerUsername, string _customerPassword)
        {
        /// TO HERE
            string customerPasswordEncoded = Controllers.AccountController.SHA1.Encode(_customerPassword);
            /// Solution taken from http://stackoverflow.com/questions/10234307/writing-query-to-check-if-username-password-is-correct-in-linq-to-sql
            var temporaryCustomer = db.Customers.FirstOrDefault(u => u.customerUsername == _customerUsername && u.customerPassword == customerPasswordEncoded);

            if (temporaryCustomer == null)
            {
                return false;
            }
            /// TO HERE
            else 
            {
                return true;
            }
        }
    }
}