using Newtonsoft.Json;
using System;

namespace Carbon.ExceptionHandling.Abstractions
{
    /// <summary>
    /// Represents exceptions with various CarbonException methods. 
    /// </summary>
    public abstract class CarbonException : Exception
    {
        /// <summary>
        /// The code of the defined error.
        /// </summary>
        public int ErrorCode { get; set; }

        /// <summary>
        /// The model of the defined error.
        /// </summary>
        public string SerializedModel { get; set; }

        /// <summary>
        /// The arguments of the defined error. Any other objects than error code.
        /// </summary>
        public object[] Arguments { get; set; }

        /// <summary>
        /// CarbonException with only error code 5000.
        /// </summary>
        public CarbonException()
        {
            ErrorCode = 5000;
        }

        /// <summary>
        /// CarbonException with error code.
        /// </summary>
        /// <param name="code">The code of the exception.</param>
        public CarbonException(int code)
        {
            ErrorCode = code;
        }

        /// <summary>
        /// CarbonException with error code and model.
        /// </summary>
        /// <param name="code">The code of the exception.</param>
        /// <param name="model">The model of the exception.</param>
        public CarbonException(int code, object model) : this(code)
        {
            SerializedModel = JsonConvert.SerializeObject(model);
        }

        /// <summary>
        /// CarbonException with error code and arguments.
        /// </summary>
        /// <param name="code">The code of the exception.</param>
        /// <param name="arguments">The argument object array of the exception.</param>
        public CarbonException(int code, params object[] arguments) : this(code)
        {
            Arguments = arguments;
        }

        /// <summary>
        /// CarbonException with message and arguments.
        /// </summary>
        /// <param name="message">The message of the exception.</param>
        /// <param name="args">The argument object array of the exception.</param>
        public CarbonException(string message, params object[] args) : this(default(int), message, args)
        {

        }

        /// <summary>
        /// CarbonException with error code message and arguments.
        /// </summary>
        /// <param name="code">The code of the exception.</param>
        /// <param name="message">The message of the exception.</param>
        /// <param name="args">The argument object array of the exception.</param>
        public CarbonException(int code, string message, params object[] args) : this(null, code, message, args)
        {

        }

        /// <summary>
        /// CarbonException with inner exception with message and arguments.
        /// </summary>
        /// <param name="innerException">The inner exception of the exception.</param>
        /// <param name="message">The message of the exception.</param>
        /// <param name="args">The argument object array of the exception.</param>
        public CarbonException(Exception innerException, string message, params object[] args)
            : this(innerException, default(int), message, args)
        {

        }

        /// <summary>
        /// CarbonException with inner exception with error code.
        /// </summary>
        /// <param name="innerException">The inner exception of the exception.</param>
        /// <param name="code">The code of the exception.</param>
        public CarbonException(Exception innerException, int code)
            : base(null, innerException)
        {
            ErrorCode = code;
        }

        /// <summary>
        /// CarbonException with inner exception with error code, message and arguments.
        /// </summary>
        /// <param name="innerException">The inner exception of the exception.</param>
        /// <param name="code">The code of the exception.</param>
        /// <param name="message">The message of the exception.</param>
        /// <param name="args">The argument object array of the exception.</param>
        public CarbonException(Exception innerException, int code, string message, params object[] args)
            : base(string.Format(message, args), innerException)
        {
            ErrorCode = code;
        }
    }
}
