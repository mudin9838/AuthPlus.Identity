using FluentValidation.Results;

namespace AuthPlus.Identity.Validators;
public interface IBaseValidator<T>
{
    Task<ValidationResult> ValidateAsync(T instance);
}