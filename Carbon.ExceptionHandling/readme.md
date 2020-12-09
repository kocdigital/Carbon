# Carbon.ExceptionHandling.Abstractions

Carbon provides to users GlobalExceptionHandlerFilter with some customized exception types.
Filter will be automatically registered as a Mvc Filter when your APIs Startup.cs file Inherited CarbonStartup class.

You can find this registration code block in 

Carbon.WebApplication/CarbonStartup.cs/ConfigureServices function.
options.Filters.Add(typeof(HttpGlobalExceptionFilter));

Carbon Framework has these exception types ,

 

- AlreadyExistsException
- ForbiddenOperationException
- NotFoundException
- CarbonException
- CarbonException<T> with Generic Type

 

Only CarbonException inherits from System.Exception and rest of all inherits from CarbonException type.

CarbonException only supports Error Code 5000 so the other exception types supports only ErrorCode 500 because of inheriting from CarbonException.

 

Feel to free constructing new custom Exception from CarbonException via inherit mechanism. And give it your favourite exception name (MyBestUnknownException)


You can find some custom exception implementation below with custom ErrorCode enumaration.

```csharp
using Carbon.ExceptionHandling.Abstractions;
using System;

namespace ExceptionHandling
{
    class Program
    {
        static void Main(string[] args)
        {
            for (int i = 0; i < 4; i++)
            {
                try
                {
                    var result = i / i;
                }
                catch (Exception ex)
                {
                    throw new MyBestUnknownException(ex, ErrorCodes.DivideByZeroException);
                }
            }
        }
    }


    enum ErrorCodes
    {
        DivideByZeroException,
        SameNameAlreadyExists,
        SameIdAreadyExists,
        StringFieldCannotBeNull,
        IntegerFieldMustBeGreaterThanZero,
        StringLengthMustBeLessThan100

    }

    class MyBestUnknownException : CarbonException<ErrorCodes>
    {
        public MyBestUnknownException()
        {
        }

        public MyBestUnknownException(ErrorCodes code) : base(code)
        {
        }

        public MyBestUnknownException(ErrorCodes code, object model) : base(code, model)
        {
        }

        public MyBestUnknownException(ErrorCodes code, params object[] arguments) : base(code, arguments)
        {
        }

        public MyBestUnknownException(Exception innerException, ErrorCodes code) : base(innerException, code)
        {
        }

        public MyBestUnknownException(ErrorCodes code, string message, params object[] args) : base(code, message, args)
        {
        }

        public MyBestUnknownException(Exception innerException, ErrorCodes code, string message, params object[] args) : base(innerException, code, message, args)
        {
        }
    }
}

```

