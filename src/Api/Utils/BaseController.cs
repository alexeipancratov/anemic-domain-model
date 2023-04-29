using Logic.Utils;
using Microsoft.AspNetCore.Mvc;

namespace Api.Utils;

public class BaseController : Controller
{
    private readonly UnitOfWork _unitOfWork;

    public BaseController(UnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    // The original method is not virtual, but that's okay in our case since we don't
    // rely on polymorphism here.
    protected new IActionResult Ok()
    {
        _unitOfWork.Commit();
        return base.Ok(Envelope.Ok());
    }

    protected IActionResult Ok<T>(T result)
    {
        _unitOfWork.Commit();
        return base.Ok(Envelope.Ok(result));
    }

    // For consistency we created a method for error case as well.
    // Even though we don't work with Unit of Work here.
    // It's called Error because it seems like a better name.
    protected IActionResult Error(string errorMessage)
    {
        return BadRequest(Envelope.Error(errorMessage));
    }
}