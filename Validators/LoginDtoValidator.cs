using AuthPlus.Identity.Dtos;
using FluentValidation;

namespace AuthPlus.Identity.Validators;

public class LoginDtoValidator : BaseValidator<LoginDto>
{
    public LoginDtoValidator()
    {
        RuleFor(x => x.UserName).NotEmpty();
        RuleFor(x => x.Password).NotEmpty();
    }
}
