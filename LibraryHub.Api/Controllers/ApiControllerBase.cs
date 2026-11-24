using LibraryHub.Api.Services;
using LibraryHub.Common.Responses;
using Microsoft.AspNetCore.Mvc;

namespace LibraryHub.Api.Controllers
{
    [ApiController]
    public abstract class ApiControllerBase : ControllerBase
    {
        protected IActionResult Failure<T>(ServiceResult<T> result)
        {
            var body = ApiResponse<object>.Fail(result.Message);

            return result.Status switch
            {
                ServiceStatus.NotFound => NotFound(body),
                ServiceStatus.Conflict => Conflict(body),
                _ => BadRequest(body)
            };
        }
    }
}