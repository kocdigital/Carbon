using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Carbon.ExceptionHandling.Abstractions
{
    public class CarbonValidator<T, V> where T : AbstractValidator<V> where V : class
    {
        public List<CarbonError> Validate(V validatableClass)
        {
            T validator = (T)Activator.CreateInstance(typeof(T));
            var results = validator.Validate(validatableClass);
            var errors = new List<CarbonError>();

            if (results.Errors.Any())
            {
                foreach (var innerError in results.Errors)
                {
                    errors.Add(new CarbonError()
                    {
                        ErrorCode = innerError.ErrorCode,
                        ErrorMessage = innerError.ErrorMessage
                    });
                }
            }
            return errors;
        }

    }
}
