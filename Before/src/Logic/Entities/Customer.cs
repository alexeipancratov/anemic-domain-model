﻿using Logic.ValueObjects;
using NHibernate.Engine;

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
        protected set => _email = value; // protected to allow our ORM to set its value
    }

    private decimal _moneySpent;
    public virtual Dollars MoneySpent
    {
        get => Dollars.Of(_moneySpent);
        protected set => _moneySpent = value;
    }
    
    public virtual CustomerStatus Status { get; set; }

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

    public virtual void AddPurchasedMovie(Movie movie, ExpirationDate expirationDate, Dollars price)
    {
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
}
