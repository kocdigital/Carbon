using Carbon.ExceptionHandling.Abstractions;
using System;

namespace Carbon.Demo.WebApplication
{
    public enum MyErrorCodes
    {
        Blabla = 1,
        TiraTira = 2
    }

    public class MyTestException : CarbonException<MyErrorCodes>
    {
        public MyTestException()
        {
        }

        public MyTestException(MyErrorCodes code) : base(code)
        {
        }

        public MyTestException(MyErrorCodes code, object model) : base(code, model)
        {
        }

        public MyTestException(MyErrorCodes code, params object[] arguments) : base(code, arguments)
        {
        }

        public MyTestException(Exception innerException, MyErrorCodes code) : base(innerException, code)
        {
        }

        public MyTestException(Exception innerException, MyErrorCodes code, params object[] args) : base(innerException, code, args)
        {
        }
    }
}
