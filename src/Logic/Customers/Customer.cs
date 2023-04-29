using CSharpFunctionalExtensions;
using Logic.Movies;
using Entity = Logic.Common.Entity;

namespace Logic.Customers;

public class Customer : Entity
{
    private string _name;
    public virtual CustomerName Name
    {
        get => (CustomerName)_name;
        set => _name = value;
    }

    private readonly string _email;
    public virtual Email Email => (Email)_email;

    private decimal _moneySpent;
    public virtual Dollars MoneySpent
    {
        get => Dollars.Of(_moneySpent);
        protected set => _moneySpent = value;
    }
    
    public virtual CustomerStatus Status { get; protected set; }

    private readonly IList<PurchasedMovie> _purchasedMovies;
    public virtual IReadOnlyList<PurchasedMovie> PurchasedMovies => _purchasedMovies.ToList();

    protected Customer()
    {
        _purchasedMovies = new List<PurchasedMovie>();
    }
    
    public Customer(CustomerName customerName, Email email) : this()
    {
        _name = customerName;
        _email = email;

        MoneySpent = Dollars.Of(0);
        Status = CustomerStatus.Regular;
    }

    public virtual bool HasPurchasedMovie(Movie movie)
    {
        return PurchasedMovies.Any(pm => pm == movie && !pm.ExpirationDate.IsExpired);
    }

    public virtual void PurchaseMovie(Movie movie)
    {
        // In case client forgot to call this method beforehand.
        if (HasPurchasedMovie(movie))
            throw new Exception();
        
        ExpirationDate expirationDate = movie.GetExpirationDate();
        Dollars price = movie.CalculatePrice(Status);
        
        var purchasedMovie = new PurchasedMovie
        {
            MovieId = movie.Id,
            CustomerId = Id,
            ExpirationDate = expirationDate,
            Price = price,
            PurchaseDate = DateTime.UtcNow
        };
        
        _purchasedMovies.Add(purchasedMovie);
        MoneySpent += price;
    }

    // It's a good idea to have check and actual action to be separated in two separate methods
    // to adhere to the CQS principle, which in this case means
    // that an operation that should either return a value or mutate the state.
    // If CanPromote was part of Promote then it would violate it.
    public virtual Result CanPromote()
    {
        if (Status.IsAdvanced)
            return Result.Failure("The customer already has the advanced status");
        
        if (PurchasedMovies.Count(pm => pm.ExpirationDate == ExpirationDate.Infinity
                                        || pm.ExpirationDate.Date >= DateTime.UtcNow.AddDays(-30)) < 2)
            return Result.Failure("The customer has to have at least 2 active movies during the last 30 days");
        
        if (PurchasedMovies
                .Where(x => x.PurchaseDate > DateTime.UtcNow.AddYears(-1))
                .Sum(x => x.Price) < 100m)
            return Result.Failure("The customer has to have at least 100 dollars spent during the last year");

        Status = Status.Promote();

        return Result.Success();
    }
    
    public virtual void Promote()
    {
        if (CanPromote().IsFailure)
            throw new Exception();

        Status = Status.Promote();
    }
}
