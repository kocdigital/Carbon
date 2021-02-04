using Serilog;
using System;

namespace Carbon.ExceptionHandling.Abstractions
{
    /// <summary>
    /// Represents exceptions with various CarbonException methods with enum error code.
    /// </summary>
    public abstract class CarbonException<T> : CarbonException where T : IConvertible
    {
        /// <summary>
        /// The code of the defined error.
        /// </summary>
        public T Code { get; set; }

        /// <summary>
        /// The base Carbon Exception.
        /// </summary>
        public CarbonException() : base()
        {
            if (!typeof(T).IsEnum)
                throw new ArgumentException("T must be enum type!");
        }

        /// <summary>
        /// The Carbon Exception with Code
        /// </summary>
        /// <param name="code">The code of the exception.</param>
        public CarbonException(T code) : base(Convert.ToInt32(code), code.ToString())
        {
            Code = code;
        }

        /// <summary>
        /// The Carbon Exception with Code and model
        /// </summary>
        /// <param name="code">The code of the exception.</param>
        /// <param name="model">The model of the exception.</param>
        public CarbonException(T code, object model) : base(Convert.ToInt32(code), code.ToString(), model)
        {
            Code = code;
        }

        /// <summary>
        /// The Carbon Exception with Code and arguments
        /// </summary>
        /// <param name="code">The code of the exception.</param>
        /// <param name="arguments">The argument object array of the exception.</param>
        public CarbonException(T code, params object[] arguments) : base(Convert.ToInt32(code), code.ToString(), arguments)
        {
            Code = code;
        }

        /// <summary>
        /// The Carbon Exception with Code and inner exception
        /// </summary>
        /// <param name="innerException">The inner exception of the exception.</param>
        /// <param name="code">The code of the exception.</param>
        public CarbonException(Exception innerException, T code) : base(innerException, Convert.ToInt32(code), code.ToString())
        {
            Code = code;
        }

        /// <summary>
        /// The Carbon Exception with Code, inner exception and arguments.
        /// </summary>
        /// <param name="innerException">The inner exception of the exception.</param>
        /// <param name="code">The code of the exception.</param>
        /// <param name="args">The argument object array of the exception.</param>
        public CarbonException(Exception innerException, T code, params object[] args) : base(innerException, Convert.ToInt32(code), code.ToString(), args)
        {
            Code = code;
        }
    }
}
