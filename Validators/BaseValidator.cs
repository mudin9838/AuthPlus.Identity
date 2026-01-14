using FluentValidation;
using FluentValidation.Results;

namespace AuthPlus.Identity.Validators;



public abstract class BaseValidator<T> : AbstractValidator<T>, IBaseValidator<T>
{
    public async Task<ValidationResult> ValidateAsync(T instance)
    {
        // Calls FluentValidation's native async method
        return await base.ValidateAsync(instance);
    }
}
