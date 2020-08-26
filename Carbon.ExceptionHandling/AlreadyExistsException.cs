namespace Carbon.ExceptionHandling.Abstractions
{
    /// <summary>
    /// Represents a specific exception that already exist exception.
    /// </summary>
    public class AlreadyExistsException : CarbonException
    {
        /// <summary>
        /// Already Exist Carbon exception with message and arguments.
        /// </summary>
        public AlreadyExistsException(string message, params object[] args) : base(message, args)
        {
        }

        /// <summary>
        /// Already Exist Carbon exception with error code, message and arguments.
        /// </summary>
        public AlreadyExistsException(int code, string message, params object[] args) : base(code, message, args)
        {
        }
    }
}
