using ErrorOr;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using NutriTrack.Domain.Users;
using System.Security.Claims;

namespace NutriTrack.Api.Controllers
{
    [ApiController]
    public class ApiController : ControllerBase
    {
        //private readonly ICurrentUserService _currentUserService;

        public ApiController(/*ICurrentUserService currentUserService*/)
        {
            //_currentUserService = currentUserService;
        }

        protected UserId GetUserId()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
                throw new InvalidOperationException("User id claim is missing.");

            return new UserId(Guid.Parse(userIdClaim));
        }

        protected IActionResult Problem(List<Error> errors)
        {
            if (errors.Count is 0)
            {
                return Problem();
            }

            if (errors.All(error => error.Type == ErrorType.Validation))
            {
                return ValidationProblem(errors);
            }

            HttpContext.Items["errors"] = errors;

            return Problem(errors[0]);
        }

        private IActionResult Problem(Error error)
        {
            var statusCode = error.Type switch
            {
                ErrorType.Conflict => StatusCodes.Status409Conflict,
                ErrorType.Validation => StatusCodes.Status400BadRequest,
                ErrorType.NotFound => StatusCodes.Status404NotFound,
                _ => StatusCodes.Status500InternalServerError
            };

            return Problem(statusCode: statusCode, title: error.Description);
        }

        private IActionResult ValidationProblem(List<Error> errors)
        {
            var modelStateDictionary = new ModelStateDictionary();

            foreach (var error in errors)
            {
                modelStateDictionary.AddModelError(
                    error.Code,
                    error.Description);
            }

            return ValidationProblem(modelStateDictionary);
        }
    }
}
