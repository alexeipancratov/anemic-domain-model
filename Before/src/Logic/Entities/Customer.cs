﻿using Logic.ValueObjects;

namespace Logic.Entities;

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

    public virtual void PurchaseMovie(Movie movie)
    {
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
    
    public virtual bool PromoteCustomer()
    {
        // at least 2 active movies during the last 30 days
        if (PurchasedMovies.Count(pm => pm.ExpirationDate == ExpirationDate.Infinity
                                                 || pm.ExpirationDate.Date >= DateTime.UtcNow.AddDays(-30)) < 2)
            return false;

        // at least 100 dollars spent during the last year
        if (PurchasedMovies
                .Where(x => x.PurchaseDate > DateTime.UtcNow.AddYears(-1))
                .Sum(x => x.Price) < 100m)
            return false;

        Status = Status.Promote();

        return true;
    }
}
