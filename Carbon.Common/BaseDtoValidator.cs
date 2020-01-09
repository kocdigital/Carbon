using FluentValidation;

namespace Carbon.Common
{
    public abstract class BaseDtoValidator<T> : AbstractValidator<T>
    {
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
