using AssetRegistry.Domain.Exceptions;

namespace AssetRegistry.Domain.Validation;

public static class Ensure
{
    public static string NotBlank(string? value, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainValidationException($"{fieldName} is required.");
        }

        return value.Trim();
    }

    public static string MaxLength(string value, int maxLength, string fieldName)
    {
        if (value.Length > maxLength)
        {
            throw new DomainValidationException($"{fieldName} must be {maxLength} characters or fewer.");
        }

        return value;
    }

    public static decimal NotNegative(decimal value, string fieldName)
    {
        if (value < 0)
        {
            throw new DomainValidationException($"{fieldName} cannot be negative.");
        }

        return value;
    }

    public static DateOnly NotInFuture(DateOnly value, DateOnly today, string fieldName)
    {
        if (value > today)
        {
            throw new DomainValidationException($"{fieldName} cannot be in the future.");
        }

        return value;
    }
}
