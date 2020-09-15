

using Carbon.Common;
using FluentValidation;

namespace Carbon.WebApplication
{
    public abstract class BaseDtoValidator<T> : AbstractValidator<T>
    {
        /// <summary>
        /// A Validator that inherited from AbstractValidator 
        /// </summary>
        public BaseDtoValidator()
        {
            if (typeof(IOrderableDto).IsAssignableFrom(typeof(T)))
            {

            }

            if (typeof(IPageableDto).IsAssignableFrom(typeof(T)))
            {
                RuleFor(x => ((IPageableDto)x).PageIndex).GreaterThanOrEqualTo(1);
                RuleFor(x => ((IPageableDto)x).PageSize).InclusiveBetween(10, 250);
            }
        }
    }
}
