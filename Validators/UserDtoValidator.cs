using AuthPlus.Identity.Dtos;
using FluentValidation;

namespace AuthPlus.Identity.Validators;
public class UserDtoValidator : BaseValidator<UserDto>
{
    public UserDtoValidator()
    {
        RuleFor(x => x.UserName).NotEmpty().When(x => !string.IsNullOrEmpty(x.UserName));
        RuleFor(x => x.Email).NotEmpty().EmailAddress().When(x => !string.IsNullOrEmpty(x.Email));
        RuleFor(x => x.FullName).MaximumLength(100).When(x => !string.IsNullOrEmpty(x.FullName));
        RuleFor(x => x.ProfileImageUrl).MaximumLength(500).When(x => !string.IsNullOrEmpty(x.ProfileImageUrl));

        RuleFor(x => x.NewPassword)
            .MinimumLength(6)
            .When(x => !string.IsNullOrEmpty(x.NewPassword))
            .WithMessage("New password must be at least 6 characters long.");

        RuleFor(x => x.ConfirmNewPassword)
            .Equal(x => x.NewPassword)
            .When(x => !string.IsNullOrEmpty(x.NewPassword) && !string.IsNullOrEmpty(x.ConfirmNewPassword))
            .WithMessage("New passwords do not match.");
    }
}
