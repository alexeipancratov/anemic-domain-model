using Logic.Entities;
using Logic.ValueObjects;

namespace Logic.Services;

public class CustomerService
{
    private readonly MovieService _movieService;

    public CustomerService(MovieService movieService)
    {
        _movieService = movieService;
    }

    private static Dollars CalculatePrice(CustomerStatus customerStatus, LicensingModel licensingModel)
    {
        Dollars price;
        switch (licensingModel)
        {
            case LicensingModel.TwoDays:
                price = Dollars.Of(4);
                break;

            case LicensingModel.LifeLong:
                price = Dollars.Of(8);
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }

        if (customerStatus.IsAdvanced)
        {
            price *= 0.75m;
        }

        return price;
    }

    public void PurchaseMovie(Customer customer, Movie movie)
    {
        ExpirationDate expirationDate = _movieService.GetExpirationDate(movie.LicensingModel);
        Dollars price = CalculatePrice(customer.Status, movie.LicensingModel);
        customer.AddPurchasedMovie(movie, expirationDate, price);
    }

    public bool PromoteCustomer(Customer customer)
    {
        // at least 2 active movies during the last 30 days
        if (customer.PurchasedMovies.Count(pm => pm.ExpirationDate == ExpirationDate.Infinity
                                                 || pm.ExpirationDate.Date >= DateTime.UtcNow.AddDays(-30)) < 2)
            return false;

        // at least 100 dollars spent during the last year
        if (customer.PurchasedMovies
                .Where(x => x.PurchaseDate > DateTime.UtcNow.AddYears(-1))
                .Sum(x => x.Price) < 100m)
            return false;

        customer.Status = customer.Status.Promote();

        return true;
    }
}
