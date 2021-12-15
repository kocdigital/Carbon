using Carbon.ExceptionHandling.Abstractions;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Carbon.WebApplication
{
    /// <summary>
    /// Common DTO validator, can be used as external validation instead of fluent validation's middleware.
    /// </summary>
    public class CarbonValidator<T, V> where T : AbstractValidator<V> where V : class
    {
        /// <summary>
        /// Validates with given validatableClass according to T AbstractValidator.
        /// </summary>
        /// <param name="validatableClass">The class that will be validate.</param>
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
