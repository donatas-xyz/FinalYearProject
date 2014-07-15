using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SNW.Controllers;
using System.Web.Mvc;

namespace SNWTests
{
    [TestClass]
    public class SupplierOrderTests
    {
        // Test if order details are retrieved
        [TestMethod]
        public void SupplierOrderTest()
        {
            // Set new order object
            SupplierOrderController order = new SupplierOrderController();

            // Define real values
            int id = 1;
            int state = 3;
            string username = "administrator";

            // Retrieve order details
            var listOfOrders = order.SupplierOrder(id, state, username);
        }
    }
}
