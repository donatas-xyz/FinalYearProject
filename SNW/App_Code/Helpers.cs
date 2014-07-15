using System;
using System.Web.Mvc;

namespace SNW.App_Code
{
    public static class Helpers
    {
        /// <summary>
        /// Role's checker. A helper function to only return output when a criteria is met. 
        /// Solution found at: http://stackoverflow.com/questions/6981853/asp-net-mvc3-razor-display-actionlink-based-on-user-role
        /// </summary>
        /// <param name="value">Content to be shown/hidden</param>
        /// <param name="evaluation">Evaluation value</param>
        /// <param name="falseValue">Default value</param>
        /// <returns>The result of evaluation</returns>
        public static MvcHtmlString IfRole(this MvcHtmlString value, bool evaluation, MvcHtmlString falseValue = default(MvcHtmlString))
        {
            return evaluation ? value : falseValue;
        }
    }
}