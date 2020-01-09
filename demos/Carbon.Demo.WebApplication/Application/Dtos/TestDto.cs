using Carbon.FluentValidation;
using FluentValidation;

namespace Carbon.Demo.WebApplication.Application.Dtos
{
    public class TestDtoValidator : BaseModelValidator<TestDto>
    {
        public TestDtoValidator()
        {
            RuleFor(x => x.UserName.Length == 10);
            RuleFor(x => x.Password).NotEmpty();
        }
    }

    public class TestDto
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
