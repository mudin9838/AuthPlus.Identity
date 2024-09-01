using AuthPlus.Identity.Dtos;
using FluentValidation;

namespace AuthPlus.Identity.Validators;

public class RegisterDtoValidator : BaseValidator<RegisterDto>
{
    public RegisterDtoValidator()
    {
        RuleFor(x => x.UserName).NotEmpty();
        RuleFor(x => x.Password).NotEmpty()
            .MinimumLength(6)
            .When(x => !string.IsNullOrEmpty(x.Password))
            .WithMessage("New password must be at least 6 characters long.");

        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.Password)
            .When(x => !string.IsNullOrEmpty(x.Password) && !string.IsNullOrEmpty(x.ConfirmPassword))
            .WithMessage("New passwords do not match.");
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.FullName).MaximumLength(100);
        RuleFor(x => x.ProfileImageUrl).MaximumLength(500);
    }
}