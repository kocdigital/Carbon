using Carbon.ExceptionHandling.Abstractions;

namespace Carbon.WebApplication.SolutionService.Domain.Exceptions
{
    /// <summary>
    /// Represents a specific exception that FeatureSet null exception.
    /// </summary>
    public class FeatureSetNullException : CarbonException
    {
        /// <summary>
        /// FeatureSet null Carbon exception with message and arguments.
        /// </summary>
        /// <param name="message">The message of the not found exception.</param>
        /// <param name="args">The argument object array of the not found exception.</param>
        public FeatureSetNullException(string message, params object[] args) : base(message, args)
        {
        }

        /// <summary>
        /// FeatureSet nullCarbon exception with code, message and arguments.
        /// </summary>
        /// <param name="code">The code of the not found exception.</param>
        /// <param name="message">The message of the not found exception.</param>
        /// <param name="args">The argument object array of the not found exception.</param>
        public FeatureSetNullException(int code, string message, params object[] args) : base(code, message, args)
        {
        }
    }
}
