﻿using CSharpFunctionalExtensions;
using Logic.Dtos;
using Logic.Entities;
using Logic.Repositories;
using Logic.Services;
using Logic.ValueObjects;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("api/[controller]")]
public class CustomersController : Controller
{
    private readonly MovieRepository _movieRepository;
    private readonly CustomerRepository _customerRepository;
    private readonly CustomerService _customerService;

    public CustomersController(MovieRepository movieRepository, CustomerRepository customerRepository, CustomerService customerService)
    {
        _customerRepository = customerRepository;
        _movieRepository = movieRepository;
        _customerService = customerService;
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

        return Json(dto);
    }

    [HttpGet]
    public JsonResult GetList()
    {
        IReadOnlyList<Customer> customers = _customerRepository.GetList();

        var dtoCustomers = customers.Select(c => new CustomerInListDto
        {
            Email = c.Email.Value,
            Id = c.Id,
            MoneySpent = c.MoneySpent,
            Name = c.Name.Value,
            Status = c.Status.ToString(),
            StatusExpirationDate = c.StatusExpirationDate
        }).ToList();
        
        return Json(dtoCustomers);
    }

    [HttpPost]
    public IActionResult Create([FromBody] CreateCustomerDto item)
    {
        try
        {
            var customerNameResult = CustomerName.Create(item.Name);
            var emailResult = Email.Create(item.Email);
            var combinedResult = Result.Combine(customerNameResult, emailResult);
            if (combinedResult.IsFailure)
            {
                return BadRequest(combinedResult.Error);
            }

            if (_customerRepository.GetByEmail(emailResult.Value) != null)
            {
                return BadRequest("Email is already in use: " + item.Email);
            }

            var customer = new Customer
            {
                Email = emailResult.Value,
                Name = customerNameResult.Value,
                MoneySpent = Dollars.Of(0),
                Status = CustomerStatus.Regular,
                StatusExpirationDate = null
            };
            
            _customerRepository.Add(customer);
            _customerRepository.SaveChanges();

            return Ok();
        }
        catch (Exception e)
        {
            return StatusCode(500, new { error = e.Message });
        }
    }

    [HttpPut]
    [Route("{id}")]
    public IActionResult Update(long id, [FromBody] UpdateCustomerDto item)
    {
        try
        {
            var customerNameResult = CustomerName.Create(item.Name);
            if (customerNameResult.IsFailure)
            {
                return BadRequest(customerNameResult.Error);
            }

            Customer customer = _customerRepository.GetById(id);
            if (customer == null)
            {
                return BadRequest("Invalid customer id: " + id);
            }

            customer.Name = customerNameResult.Value;
            _customerRepository.SaveChanges();

            return Ok();
        }
        catch (Exception e)
        {
            return StatusCode(500, new { error = e.Message });
        }
    }

    [HttpPost]
    [Route("{id}/movies")]
    public IActionResult PurchaseMovie(long id, [FromBody] long movieId)
    {
        try
        {
            Movie movie = _movieRepository.GetById(movieId);
            if (movie == null)
            {
                return BadRequest("Invalid movie id: " + movieId);
            }

            Customer customer = _customerRepository.GetById(id);
            if (customer == null)
            {
                return BadRequest("Invalid customer id: " + id);
            }

            if (customer.PurchasedMovies.Any(pm => pm.MovieId == movie.Id && !pm.ExpirationDate.IsExpired))
            {
                return BadRequest("The movie is already purchased: " + movie.Name);
            }

            _customerService.PurchaseMovie(customer, movie);

            _customerRepository.SaveChanges();

            return Ok();
        }
        catch (Exception e)
        {
            return StatusCode(500, new { error = e.Message });
        }
    }

    [HttpPost]
    [Route("{id}/promotion")]
    public IActionResult PromoteCustomer(long id)
    {
        try
        {
            Customer customer = _customerRepository.GetById(id);
            if (customer == null)
            {
                return BadRequest("Invalid customer id: " + id);
            }

            if (customer.Status == CustomerStatus.Advanced && !customer.StatusExpirationDate.IsExpired)
            {
                return BadRequest("The customer already has the Advanced status");
            }

            bool success = _customerService.PromoteCustomer(customer);
            if (!success)
            {
                return BadRequest("Cannot promote the customer");
            }

            _customerRepository.SaveChanges();

            return Ok();
        }
        catch (Exception e)
        {
            return StatusCode(500, new { error = e.Message });
        }
    }
}
