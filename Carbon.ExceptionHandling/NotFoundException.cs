namespace Carbon.ExceptionHandling.Abstractions
{
    /// <summary>
    /// Represents a specific exception that not found exception.
    /// </summary>
    public class NotFoundException : CarbonException
    {
        /// <summary>
        /// Not Found Carbon exception with message and arguments.
        /// </summary>
        /// <param name="message">The message of the not found exception.</param>
        /// <param name="args">The argument object array of the not found exception.</param>
        public NotFoundException(string message, params object[] args) : base(message, args)
        {
        }

        /// <summary>
        /// Not Found Carbon exception with code, message and arguments.
        /// </summary>
        /// <param name="code">The code of the not found exception.</param>
        /// <param name="message">The message of the not found exception.</param>
        /// <param name="args">The argument object array of the not found exception.</param>
        public NotFoundException(int code, string message, params object[] args) : base(code, message, args)
        {
        }
    }
}
