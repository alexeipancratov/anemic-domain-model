using CSharpFunctionalExtensions;

namespace Logic.Movies;

public class ExpirationDate : ValueObject<ExpirationDate>
{
    public static readonly ExpirationDate Infinity = new(null);
    
    public DateTime? Date { get; }

    public bool IsExpired => this != Infinity && Date < DateTime.UtcNow;
    
    private ExpirationDate(DateTime? date)
    {
        Date = date;
    }

    public static Result<ExpirationDate> Create(DateTime date)
    {
        return Result.Success(new ExpirationDate(date));
    }

    protected override bool EqualsCore(ExpirationDate other)
    {
        return Date == other.Date;
    }

    protected override int GetHashCodeCore()
    {
        return Date.GetHashCode();
    }

    public static explicit operator ExpirationDate(DateTime? date)
    {
        if (date.HasValue)
        {
            return Create(date.Value).Value;   
        }

        return Infinity;
    }

    public static implicit operator DateTime?(ExpirationDate expirationDate)
    {
        return expirationDate.Date;
    }
}