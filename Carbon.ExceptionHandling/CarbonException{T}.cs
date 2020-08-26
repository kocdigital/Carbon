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
        public CarbonException(T code) : base(Convert.ToInt32(code), code.ToString())
        {
            Code = code;
        }

        /// <summary>
        /// The Carbon Exception with Code and model
        /// </summary>
        public CarbonException(T code, object model) : base(Convert.ToInt32(code), code.ToString(), model)
        {
            Code = code;
        }

        /// <summary>
        /// The Carbon Exception with Code and arguments
        /// </summary>
        public CarbonException(T code, params object[] arguments) : base(Convert.ToInt32(code), code.ToString(), arguments)
        {
            Code = code;
        }

        /// <summary>
        /// The Carbon Exception with Code and inner exception
        /// </summary>
        public CarbonException(Exception innerException, T code) : base(innerException, Convert.ToInt32(code), code.ToString())
        {
            Code = code;
        }

        /// <summary>
        /// The Carbon Exception with Code, inner exception and arguments.
        /// </summary>
        public CarbonException(Exception innerException, T code, params object[] args) : base(innerException, Convert.ToInt32(code), code.ToString(), args)
        {
            Code = code;
        }
    }
}
