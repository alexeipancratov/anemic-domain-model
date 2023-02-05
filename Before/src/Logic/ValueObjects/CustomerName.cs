using CSharpFunctionalExtensions;

namespace Logic.ValueObjects;

public class CustomerName : ValueObject<CustomerName>
{
    public string Value { get; }

    private CustomerName(string name)
    {
        Value = name;
    }

    public static Result<CustomerName> Create(string customerName)
    {
        var trimmedCustomerName = customerName.Trim();

        if (trimmedCustomerName.Length == 0)
        {
            Result.Failure<CustomerName>("Customer should not be empty");
        }

        if (trimmedCustomerName.Length > 100)
        {
            Result.Failure<CustomerName>("Customer name should not exceed 100 characters");
        }

        return Result.Success(new CustomerName(customerName));
    }
    
    protected override bool EqualsCore(CustomerName other)
    {
        return Value.Equals(other.Value, StringComparison.InvariantCultureIgnoreCase);
    }

    protected override int GetHashCodeCore()
    {
        return Value.GetHashCode();
    }

    public static implicit operator string(CustomerName customerName)
    {
        return customerName.Value;
    }

    public static implicit operator CustomerName(string customerName)
    {
        return Create(customerName).Value;
    }
}