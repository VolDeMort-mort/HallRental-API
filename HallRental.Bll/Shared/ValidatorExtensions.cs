using FluentValidation;
using HallRental.Bll.Common.Shared.Exceptions;

namespace HallRental.Bll.Shared;

internal static class ValidatorExtensions
{
    public static async Task EnsureValidAsync<T>(this IValidator<T> validator, T instance, CancellationToken cancellationToken)
    {
        var result = await validator.ValidateAsync(instance, cancellationToken);

        if (result.IsValid)
            return;

        throw new ValidationFailedException(result.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray()));
    }
}
