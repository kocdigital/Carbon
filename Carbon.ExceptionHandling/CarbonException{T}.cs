using System;

namespace Carbon.ExceptionHandling.Abstractions
{
    public abstract class CarbonException<T> : CarbonException where T : IConvertible
    {
        public T Code { get; set; }
        public CarbonException() : base()
        {
            if (!typeof(T).IsEnum)
                throw new ArgumentException("T must be enum type!");
        }

        public CarbonException(T code) : base(Convert.ToInt32(code), code.ToString())
        {
            Code = code;
        }

        public CarbonException(T code, object model) : base(Convert.ToInt32(code), code.ToString(), model)
        {
            Code = code;
        }

        public CarbonException(T code, params object[] arguments) : base(Convert.ToInt32(code), code.ToString(), arguments)
        {
            Code = code;
        }

        public CarbonException(Exception innerException, T code) : base(innerException, Convert.ToInt32(code), code.ToString())
        {
            Code = code;
        }

        public CarbonException(Exception innerException, T code, params object[] args) : base(innerException, Convert.ToInt32(code), code.ToString(), args)
        {
            Code = code;
        }
    }
}
