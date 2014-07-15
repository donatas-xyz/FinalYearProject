using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SNW.Controllers;
using System.Web.Mvc;

namespace SNWTests
{
    [TestClass]
    public class CustomerOrderTests
    {
        // Test if order details are retrieved
        [TestMethod]
        public void CustomerOrderTest()
        {
            // Set new order object
            CustomerOrderController order = new CustomerOrderController();

            // Define real values
            int id = 1;
            int state = 3;
            int user = 2;

            // Retrieve order details
            var listOfOrders = order.CustomerOrder(id, state, user);
        }
    }
}
