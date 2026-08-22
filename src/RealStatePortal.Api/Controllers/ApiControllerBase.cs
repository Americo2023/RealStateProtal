using Microsoft.AspNetCore.Mvc;
using RealStatePortal.Application.Common;

namespace RealStatePortal.Api.Controllers;

[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    protected IActionResult FromResult(Result result)
    {
        return result.IsSuccess ? NoContent() : ProblemFor(result.Error!);
    }

    protected IActionResult FromResult<T>(Result<T> result)
    {
        return result.IsSuccess ? Ok(result.Value) : ProblemFor(result.Error!);
    }

    private ObjectResult ProblemFor(string error)
    {
        var statusCode = error switch
        {
            "Property was not found." => StatusCodes.Status404NotFound,
            "User was not found." => StatusCodes.Status404NotFound,
            "Authentication is required." or "An authenticated broker is required." => StatusCodes.Status401Unauthorized,
            "The current user cannot manage this property." or "Only brokers can create properties." => StatusCodes.Status403Forbidden,
            _ when error.Contains("already", StringComparison.OrdinalIgnoreCase) => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status400BadRequest
        };

        return Problem(statusCode: statusCode, detail: error);
    }
}