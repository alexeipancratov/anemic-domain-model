using Logic.ValueObjects;

namespace Logic.Entities;

public class Customer : Entity
{
    private string _name;
    public virtual CustomerName Name
    {
        get => (CustomerName)_name;
        set => _name = value;
    }

    private string _email;
    public virtual Email Email
    {
        get => (Email)_email;
        set => _email = value;
    }

    public virtual CustomerStatus Status { get; set; }

    public virtual DateTime? StatusExpirationDate { get; set; }

    private decimal _moneySpent;
    public virtual DollarsSpent MoneySpent
    {
        get => DollarsSpent.Of(_moneySpent);
        set => _moneySpent = value;
    }

    public virtual IList<PurchasedMovie> PurchasedMovies { get; set; }
}
