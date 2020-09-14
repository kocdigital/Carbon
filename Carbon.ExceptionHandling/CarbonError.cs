using System;
using System.Collections.Generic;
using System.Text;

namespace Carbon.ExceptionHandling.Abstractions
{
    /// <summary>
    /// Common error class with given simple error code and message.
    /// </summary>
    public class CarbonError
    {
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
    }
}
