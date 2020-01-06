using Newtonsoft.Json;
using System;

namespace Carbon.ExceptionHandling.Abstractions
{
    public abstract class CarbonException<T> : Exception
    {
        public T Code { get; set; }
        public string SerializedModel { get; set; }
        public object[] Arguments { get; set; }


        public CarbonException()
        {

        }

        public CarbonException(T code)
        {
            Code = code;
        }

        public CarbonException(T code, object model) : this(code)
        {
            SerializedModel = JsonConvert.SerializeObject(model);
        }

        public CarbonException(T code, params object[] arguments) : this(code)
        {
            Arguments = arguments;
        }

        public CarbonException(string message, params object[] args) : this(default(T), message, args)
        {

        }

        public CarbonException(T code, string message, params object[] args) : this(null, code, message, args)
        {

        }

        public CarbonException(Exception innerException, string message, params object[] args)
            : this(innerException, default(T), message, args)
        {

        }

        public CarbonException(Exception innerException, T code)
            : base(null, innerException)
        {
            Code = code;
        }

        public CarbonException(Exception innerException, T code, string message, params object[] args)
            : base(string.Format(message, args), innerException)
        {
            Code = code;
        }
    }
}
