using CSharpFunctionalExtensions;

namespace Logic.ValueObjects;

public class Dollars : ValueObject<Dollars>
{
    private const decimal MaxAmount = 1_000_000;
    
    public decimal Value { get; }

    private Dollars(decimal value)
    {
        Value = value;
    }

    // A factory method.
    public static Result<Dollars> Create(decimal dollarAmount)
    {
        if (dollarAmount < 0)
        {
            return Result.Failure<Dollars>("Dollar amount cannot be negative");
        }

        if (dollarAmount > MaxAmount)
        {
            return Result.Failure<Dollars>($"Dollar amount cannot be greater than {MaxAmount}");
        }

        if (dollarAmount % 0.01m > 0)
        {
            return Result.Failure<Dollars>("Dollar amount cannot contain a part of a penny");
        }

        return Result.Success(new Dollars(dollarAmount));
    }

    // Another factory method - an alternative to having an explicit operator (more readable).
    public static Dollars Of(decimal dollarsSpent)
    {
        return Create(dollarsSpent).Value;
    }

    protected override bool EqualsCore(Dollars other)
    {
        return Value == other.Value;
    }

    protected override int GetHashCodeCore()
    {
        return Value.GetHashCode();
    }

    public static Dollars operator *(Dollars dollars, decimal multiplier)
    {
        return new Dollars(dollars.Value * multiplier);
    }

    public static Dollars operator +(Dollars dollarsSpent1, Dollars dollarsSpent2)
    {
        return new Dollars(dollarsSpent1.Value + dollarsSpent2.Value);
    }

    public static implicit operator decimal(Dollars dollars)
    {
        return dollars.Value;
    }
}