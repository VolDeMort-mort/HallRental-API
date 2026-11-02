namespace HallRental.Common.Exceptions;

public sealed class ValidationFailedException(IReadOnlyDictionary<string, string[]> errors)
    : Exception("Validation failed")
{
    public IReadOnlyDictionary<string, string[]> Errors { get; } = errors;
}

