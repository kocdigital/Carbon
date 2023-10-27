namespace Carbon.ExceptionHandling.Abstractions
{
    /// <summary>
    /// Represents a specific exception that not found exception.
    /// </summary>
    public class UnauthorizedOperationException : CarbonException
    {
        /// <summary>
        /// Unauthorized Operation Carbon exception with message and arguments.
        /// </summary>
        /// <param name="message">The message of the unauthorized exception.</param>
        /// <param name="args">The argument object array of the unauthorized exception.</param>
        public UnauthorizedOperationException() : base(CarbonExceptionMessages.OperationUnauthorized)
        {
        }
        /// <summary>
        /// Unauthorized Operation Carbon exception with message and arguments.
        /// </summary>
        /// <param name="message">The message of the unauthorized exception.</param>
        /// <param name="args">The argument object array of the unauthorized exception.</param>
        public UnauthorizedOperationException(params object[] args) : base(CarbonExceptionMessages.OperationUnauthorized, args)
        {
        }
        /// <summary>
        /// Unauthorized Operation Carbon exception with message and arguments.
        /// </summary>
        /// <param name="message">The message of the unauthorized exception.</param>
        /// <param name="args">The argument object array of the unauthorized exception.</param>
        public UnauthorizedOperationException(string message, params object[] args) : base(message, args)
        {
        }

        /// <summary>
        /// Unauthorized Operation Carbon exception with code, message and arguments.
        /// </summary>
        /// <param name="code">The code of the unauthorized exception.</param>
        /// <param name="message">The message of the unauthorized exception.</param>
        /// <param name="args">The argument object array of the unauthorized exception.</param>
        public UnauthorizedOperationException(int code, string message, params object[] args) : base(code, message, args)
        {
        }
    }
}
