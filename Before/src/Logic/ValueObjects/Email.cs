namespace Logic.ValueObjects;

public class Email : ValueObject<Email>
{
    public string Value { get; }

    public Email(string email)
    {
        Value = email;
    }
    
    protected override bool EqualsCore(Email other)
    {
        return Value.Equals(other.Value, StringComparison.InvariantCultureIgnoreCase);
    }

    protected override int GetHashCodeCore()
    {
        return Value.GetHashCode();
    }
}