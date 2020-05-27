using Carbon.ExceptionHandling.Abstractions;
using System;

namespace Carbon.Demo.WebApplication
{
    public class MyTestException : CarbonException
    {
        public MyTestException()
        {
        }

        public MyTestException(int code) : base(code)
        {
        }

        public MyTestException(int code, object model) : base(code, model)
        {
        }

        public MyTestException(int code, params object[] arguments) : base(code, arguments)
        {
        }

        public MyTestException(string message, params object[] args) : base(message, args)
        {
        }

        public MyTestException(Exception innerException, int code) : base(innerException, code)
        {
        }

        public MyTestException(int code, string message, params object[] args) : base(code, message, args)
        {
        }

        public MyTestException(Exception innerException, string message, params object[] args) : base(innerException, message, args)
        {
        }

        public MyTestException(Exception innerException, int code, string message, params object[] args) : base(innerException, code, message, args)
        {
        }
    }
}
