using FluentValidation;
using System;

namespace Carbon.Demo.WebApplication.Application.Dtos
{
    public class TestDtoValidator : AbstractValidator<TestDto>
    {
        public TestDtoValidator()
        {
            RuleFor(x => x.UserName.Length == 10);
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Password).NotEmpty();
        }
    }

    public class TestDto
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
