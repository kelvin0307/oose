using Core.Common;
using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseApiController : ControllerBase
    {
        protected IActionResult HandleResponse<T>(Response<T> response, bool noContentOnSuccess = false)
        {
            if (response.Success)
                return noContentOnSuccess ? NoContent() : Ok(response.Result);

            return response.Status switch
            {
                ResponseStatus.NotFound => NotFound(new { message = response.Message }),
                ResponseStatus.ValidationError => BadRequest(response.ValidationErrors),
                ResponseStatus.InvalidOperation => BadRequest(new { message = response.Message }),
                ResponseStatus.Unauthorized => Unauthorized(new { message = response.Message }),
                ResponseStatus.Forbidden => Forbid(),
                _ => StatusCode(500, new { message = response.Message })
            };
        }

        protected IActionResult HandleCreatedResponse<T>(Response<T> response, string actionName, object routeValues)
        {
            if (!response.Success)
                return HandleResponse(response);

            return CreatedAtAction(actionName, routeValues, response.Result);
        }
    }
}