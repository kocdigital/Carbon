namespace Carbon.ExceptionHandling.Abstractions
{
    public class AlreadyExistsException : CarbonException
    {
        public AlreadyExistsException(string message, params object[] args) : base(message, args)
        {
        }

        public AlreadyExistsException(int code, string message, params object[] args) : base(code, message, args)
        {
        }
    }
}
