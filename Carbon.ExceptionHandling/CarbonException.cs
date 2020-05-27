using Newtonsoft.Json;
using System;

namespace Carbon.ExceptionHandling.Abstractions
{

    public abstract class CarbonException : Exception
    {
        public int ErrorCode { get; set; }
        public string SerializedModel { get; set; }
        public object[] Arguments { get; set; }

        public CarbonException()
        {
            ErrorCode = 5000;
        }

        public CarbonException(int code)
        {
            ErrorCode = code;
        }

        public CarbonException(int code, object model) : this(code)
        {
            SerializedModel = JsonConvert.SerializeObject(model);
        }

        public CarbonException(int code, params object[] arguments) : this(code)
        {
            Arguments = arguments;
        }

        public CarbonException(string message, params object[] args) : this(default(int), message, args)
        {

        }

        public CarbonException(int code, string message, params object[] args) : this(null, code, message, args)
        {

        }

        public CarbonException(Exception innerException, string message, params object[] args)
            : this(innerException, default(int), message, args)
        {

        }

        public CarbonException(Exception innerException, int code)
            : base(null, innerException)
        {
            ErrorCode = code;
        }

        public CarbonException(Exception innerException, int code, string message, params object[] args)
            : base(string.Format(message, args), innerException)
        {
            ErrorCode = code;
        }
    }
}
