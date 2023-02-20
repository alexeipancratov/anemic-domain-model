using Api.Utils;
using CSharpFunctionalExtensions;
using Logic.Customers;
using Logic.Movies;
using Logic.Utils;
using Microsoft.AspNetCore.Mvc;

namespace Api.Customers;

[Route("api/[controller]")]
public class CustomersController : BaseController
{
    private readonly MovieRepository _movieRepository;
    private readonly CustomerRepository _customerRepository;

    public CustomersController(UnitOfWork unitOfWork, MovieRepository movieRepository, CustomerRepository customerRepository)
        : base(unitOfWork)
    {
        _customerRepository = customerRepository;
        _movieRepository = movieRepository;
    }

    [HttpGet]
    [Route("{id}")]
    public IActionResult Get(long id)
    {
        Customer customer = _customerRepository.GetById(id);
        if (customer == null)
        {
            return NotFound();
        }

        var dto = new CustomerDto
        {
            Email = customer.Email.Value,
            Id = customer.Id,
            MoneySpent = customer.MoneySpent,
            Name = customer.Name.Value,
            PurchasedMovies = customer.PurchasedMovies.Select(m => new PurchasedMovieDto
            {
                ExpirationDate = m.ExpirationDate,
                Id = m.Id,
                Movie = new MovieDto
                {
                    Id = m.Movie.Id,
                    Name = m.Movie.Name
                },
                Price = m.Price,
                PurchaseDate = m.PurchaseDate
            }).ToList()
        };

        return Ok(dto);
    }

    [HttpGet]
    public IActionResult GetList()
    {
        IReadOnlyList<Customer> customers = _customerRepository.GetList();

        var dtoCustomers = customers.Select(c => new CustomerInListDto
        {
            Email = c.Email.Value,
            Id = c.Id,
            MoneySpent = c.MoneySpent,
            Name = c.Name.Value,
            Status = c.Status.Type.ToString(),
            StatusExpirationDate = c.Status.ExpirationDate
        }).ToList();
        
        return Ok(dtoCustomers);
    }

    [HttpPost]
    public IActionResult Create([FromBody] CreateCustomerDto item)
    {
        var customerNameResult = CustomerName.Create(item.Name);
        var emailResult = Email.Create(item.Email);
        var combinedResult = Result.Combine(customerNameResult, emailResult);
        if (combinedResult.IsFailure)
        {
            return Error(combinedResult.Error);
        }

        if (_customerRepository.GetByEmail(emailResult.Value) != null)
        {
            return Error("Email is already in use: " + item.Email);
        }

        var customer = new Customer(customerNameResult.Value, emailResult.Value);

        _customerRepository.Add(customer);

        return Ok();
    }

    [HttpPut]
    [Route("{id}")]
    public IActionResult Update(long id, [FromBody] UpdateCustomerDto item)
    {
        var customerNameResult = CustomerName.Create(item.Name);
        if (customerNameResult.IsFailure)
        {
            return Error(customerNameResult.Error);
        }

        Customer customer = _customerRepository.GetById(id);
        if (customer == null)
        {
            return Error("Invalid customer id: " + id);
        }

        customer.Name = customerNameResult.Value;

        return Ok();
    }

    [HttpPost]
    [Route("{id}/movies")]
    public IActionResult PurchaseMovie(long id, [FromBody] long movieId)
    {
        Movie movie = _movieRepository.GetById(movieId);
        if (movie == null)
        {
            return Error("Invalid movie id: " + movieId);
        }

        Customer customer = _customerRepository.GetById(id);
        if (customer == null)
        {
            return Error("Invalid customer id: " + id);
        }

        if (customer.HasPurchasedMovie(movie))
        {
            return Error("The movie is already purchased: " + movie.Name);
        }

        customer.PurchaseMovie(movie);

        return Ok();
    }

    [HttpPost]
    [Route("{id}/promotion")]
    public IActionResult PromoteCustomer(long id)
    {
        Customer customer = _customerRepository.GetById(id);
        if (customer == null)
        {
            return Error("Invalid customer id: " + id);
        }

        Result canPromoteResult = customer.CanPromote();
        if (canPromoteResult.IsFailure)
            return Error(canPromoteResult.Error);

        customer.Promote();
        
        return Ok();
    }
}
