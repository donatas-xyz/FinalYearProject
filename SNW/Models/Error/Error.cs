using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SNW.Models.Error
{
    public class ErrorInformation
    {
        public string entity { get; set; }
        public string error { get; set; }
        public string code { get; set; }
    }
}