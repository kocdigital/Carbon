namespace Carbon.ExceptionHandling.Abstractions
{
    /// <summary>
    /// Represents a specific exception that not found exception.
    /// </summary>
    public class ForbiddenOperationException : CarbonException
    {
        /// <summary>
        /// Forbidden Operation Carbon exception with message and arguments.
        /// </summary>
        /// <param name="message">The message of the forbidden exception.</param>
        /// <param name="args">The argument object array of the forbidden exception.</param>
        public ForbiddenOperationException() : base("This Operation is Forbidden!")
        {
        }
        /// <summary>
        /// Forbidden Operation Carbon exception with message and arguments.
        /// </summary>
        /// <param name="message">The message of the forbidden exception.</param>
        /// <param name="args">The argument object array of the forbidden exception.</param>
        public ForbiddenOperationException(params object[] args) : base("This Operation is Forbidden!", args)
        {
        }
        /// <summary>
        /// Forbidden Operation Carbon exception with message and arguments.
        /// </summary>
        /// <param name="message">The message of the forbidden exception.</param>
        /// <param name="args">The argument object array of the forbidden exception.</param>
        public ForbiddenOperationException(string message, params object[] args) : base(message, args)
        {
        }

        /// <summary>
        /// Forbidden Operation Carbon exception with code, message and arguments.
        /// </summary>
        /// <param name="code">The code of the forbidden exception.</param>
        /// <param name="message">The message of the forbidden exception.</param>
        /// <param name="args">The argument object array of the forbidden exception.</param>
        public ForbiddenOperationException(int code, string message, params object[] args) : base(code, message, args)
        {
        }
    }
}
