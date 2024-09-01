using FluentValidation;
using FluentValidation.Results;

namespace AuthPlus.Identity.Validators;
public abstract class BaseValidator<T> : AbstractValidator<T>, IBaseValidator<T>
{
    public async Task<ValidationResult> ValidateAsync(T instance)
    {
        return await Task.FromResult(Validate(instance));
    }
}
