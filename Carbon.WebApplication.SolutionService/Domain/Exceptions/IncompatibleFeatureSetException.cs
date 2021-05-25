using Carbon.ExceptionHandling.Abstractions;

namespace Carbon.WebApplication.SolutionService.Domain.Exceptions
{
    /// <summary>
    /// Represents a specific exception that IncompatibleFeatureSetException.
    /// </summary>
    public class IncompatibleFeatureSetException : CarbonException
    {
        /// <summary>
        /// IncompatibleFeatureSetException Carbon exception with message and arguments.
        /// </summary>
        /// <param name="message">The message of the not found exception.</param>
        /// <param name="args">The argument object array of the not found exception.</param>
        public IncompatibleFeatureSetException(string message, params object[] args) : base(message, args)
        {
        }

        /// <summary>
        /// IncompatibleFeatureSetException Carbon exception with code, message and arguments.
        /// </summary>
        /// <param name="code">The code of the not found exception.</param>
        /// <param name="message">The message of the not found exception.</param>
        /// <param name="args">The argument object array of the not found exception.</param>
        public IncompatibleFeatureSetException(int code, string message, params object[] args) : base(code, message, args)
        {
        }
    }
}
