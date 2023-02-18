using Logic.ValueObjects;

namespace Logic.Entities;

public abstract class Movie : Entity
{
    public virtual string Name { get; protected set; }

    protected virtual LicensingModel LicensingModel { get; set; }

    public abstract ExpirationDate GetExpirationDate();

    public virtual Dollars CalculatePrice(CustomerStatus customerStatus)
    {
        decimal modifier = 1 - customerStatus.GetDiscount();

        return GetBasePrice() * modifier;
    }

    protected abstract Dollars GetBasePrice();
}

public class TwoDaysMovie : Movie
{
    public override ExpirationDate GetExpirationDate()
    {
        return (ExpirationDate)DateTime.UtcNow.AddDays(2);
    }

    protected override Dollars GetBasePrice()
    {
        return Dollars.Of(4);
    }
}

public class LifeLongMovie : Movie
{
    public override ExpirationDate GetExpirationDate()
    {
        return ExpirationDate.Infinity;
    }

    protected override Dollars GetBasePrice()
    {
        return Dollars.Of(8);
    }
}