using FluentValidation;
using Inv.Application.Auth.Queries;

namespace Inv.Application.Auth.Validators
{
    public class LoginQueryValidator : AbstractValidator<LoginQuery>
    {
        public LoginQueryValidator()
        {
            RuleFor(x => x.Email)
                .EmailAddress().WithErrorCode("ErrorCodes.9")
                .When(c => !string.IsNullOrWhiteSpace(c.Email));

            RuleFor(x => x.Password)
                .NotEmpty().WithErrorCode("ErrorCodes.10");
        }
    }
}