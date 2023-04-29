using CSharpFunctionalExtensions;
using Logic.Movies;

namespace Logic.Customers;

public class CustomerStatus : ValueObject<CustomerStatus>
{
    public static readonly CustomerStatus Regular = new(CustomerStatusType.Regular, ExpirationDate.Infinity);
    
    public CustomerStatusType Type { get; }
    
    private readonly DateTime? _expirationDate;
    public ExpirationDate ExpirationDate => (ExpirationDate)_expirationDate;

    // Ctor for ORM.
    private CustomerStatus()
    {
        
    }

    public CustomerStatus(CustomerStatusType type, ExpirationDate expirationDate) : this()
    {
        Type = type;
        _expirationDate = expirationDate;
    }

    public decimal GetDiscount() => IsAdvanced ? 0.25m : 0m;

    public bool IsAdvanced => Type == CustomerStatusType.Advanced && !ExpirationDate.IsExpired;

    public CustomerStatus Promote() => new(CustomerStatusType.Advanced, (ExpirationDate)DateTime.UtcNow.AddYears(1));

    protected override bool EqualsCore(CustomerStatus other)
    {
        return Type == other.Type && ExpirationDate == other.ExpirationDate;
    }

    protected override int GetHashCodeCore()
    {
        return Type.GetHashCode() ^ ExpirationDate.GetHashCode();
    }
}

public enum CustomerStatusType
{
    Regular = 1,
    Advanced = 2
}
