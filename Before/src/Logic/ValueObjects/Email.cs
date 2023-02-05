using System.Text.RegularExpressions;
using CSharpFunctionalExtensions;

namespace Logic.ValueObjects;

public class Email : ValueObject<Email>
{
    public string Value { get; }

    private Email(string email)
    {
        Value = email;
    }

    public static Result<Email> Create(string email)
    {
        var trimmedEmail = email.Trim();

        if (trimmedEmail.Length == 0)
        {
            return Result.Failure<Email>(trimmedEmail);
        }

        if (!Regex.IsMatch(trimmedEmail, @"^(.+)@(.+)$"))
        {
            return Result.Failure<Email>("Email is invalid");
        }

        return Result.Success(new Email(email));
    }

    protected override bool EqualsCore(Email other)
    {
        return Value.Equals(other.Value, StringComparison.InvariantCultureIgnoreCase);
    }

    protected override int GetHashCodeCore()
    {
        return Value.GetHashCode();
    }

    public static implicit operator string(Email email)
    {
        return email.Value;
    }

    public static implicit operator Email(string email)
    {
        return Create(email).Value;
    }
}