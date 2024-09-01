using AuthPlus.Identity.Dtos;
using FluentValidation;

namespace AuthPlus.Identity.Validators;

public class RegisterDtoValidator : AbstractValidator<RegisterDto>
{
    public RegisterDtoValidator()
    {
        RuleFor(x => x.UserName).NotEmpty();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.FullName).MaximumLength(100);
        RuleFor(x => x.ProfileImageUrl).MaximumLength(500);
    }
}
