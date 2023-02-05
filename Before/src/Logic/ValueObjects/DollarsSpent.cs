using System.Net;
using CSharpFunctionalExtensions;

namespace Logic.ValueObjects;

public class DollarsSpent : ValueObject<DollarsSpent>
{
    private const decimal MaxAmount = 1_000_000;
    
    public decimal Value { get; }

    private DollarsSpent(decimal value)
    {
        Value = value;
    }

    public static Result<DollarsSpent> Create(decimal dollarAmount)
    {
        if (dollarAmount < 0)
        {
            return Result.Failure<DollarsSpent>("Dollar amount cannot be negative");
        }

        if (dollarAmount > MaxAmount)
        {
            return Result.Failure<DollarsSpent>($"Dollar amount cannot be greater than {MaxAmount}");
        }

        if (dollarAmount % 0.01m > 0)
        {
            return Result.Failure<DollarsSpent>("Dollar amount cannot contain a part of a penny");
        }

        return Result.Success(new DollarsSpent(dollarAmount));
    }

    public static DollarsSpent Of(decimal dollarsSpent)
    {
        return Create(dollarsSpent).Value;
    }

    protected override bool EqualsCore(DollarsSpent other)
    {
        return Value == other.Value;
    }

    protected override int GetHashCodeCore()
    {
        return Value.GetHashCode();
    }

    public static DollarsSpent operator *(DollarsSpent dollarsSpent, decimal multiplier)
    {
        return new DollarsSpent(dollarsSpent.Value * multiplier);
    }

    public static DollarsSpent operator +(DollarsSpent dollarsSpent1, DollarsSpent dollarsSpent2)
    {
        return new DollarsSpent(dollarsSpent1.Value + dollarsSpent2.Value);
    }

    public static implicit operator decimal(DollarsSpent dollarsSpent)
    {
        return dollarsSpent.Value;
    }
}