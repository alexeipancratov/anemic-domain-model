namespace Logic.ValueObjects;

public class CustomerName : ValueObject<CustomerName>
{
    public string Value { get; }

    public CustomerName(string name)
    {
        Value = name;
    }
    
    protected override bool EqualsCore(CustomerName other)
    {
        return Value.Equals(other.Value, StringComparison.InvariantCultureIgnoreCase);
    }

    protected override int GetHashCodeCore()
    {
        return Value.GetHashCode();
    }
}