namespace Carbon.ExceptionHandling.Abstractions
{
    public class NotFoundException : CarbonException
    {
        public NotFoundException(string message, params object[] args) : base(message, args)
        {
        }

        public NotFoundException(int code, string message, params object[] args) : base(code, message, args)
        {
        }
    }
}
