using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Application.Common
{
    /// <summary>
    /// One place that knows "ServiceResultStatus.Conflict means HTTP 409" etc.
    /// Every controller action ends with `return result.ToActionResult(this);`
    /// instead of re-deriving the same switch statement per endpoint.
    /// </summary>
    public static class ServiceResultExtensions
    {
        public static ActionResult<T> ToActionResult<T>(this ServiceResult<T> result, ControllerBase controller)
        {
            if (result.IsSuccess)
            {
                return result.Status switch
                {
                    ServiceResultStatus.Created => controller.StatusCode(StatusCodes.Status201Created, result.Data),
                    _ => controller.Ok(result.Data)
                };
            }

            return result.Status switch
            {
                ServiceResultStatus.BadRequest => controller.BadRequest(result.Message),
                ServiceResultStatus.Unauthorized => controller.Unauthorized(),
                ServiceResultStatus.Forbidden => controller.StatusCode(StatusCodes.Status403Forbidden, result.Message),
                ServiceResultStatus.NotFound => controller.NotFound(result.Message),
                ServiceResultStatus.Conflict => controller.Conflict(result.Message),
                _ => controller.StatusCode(StatusCodes.Status500InternalServerError, result.Message)
            };
        }

        public static ActionResult ToActionResult(this ServiceResult result, ControllerBase controller)
        {
            if (result.IsSuccess)
            {
                return result.Status switch
                {
                    ServiceResultStatus.Created => controller.StatusCode(StatusCodes.Status201Created),
                    _ => controller.Ok()
                };
            }

            return result.Status switch
            {
                ServiceResultStatus.BadRequest => controller.BadRequest(result.Message),
                ServiceResultStatus.Unauthorized => controller.Unauthorized(),
                ServiceResultStatus.Forbidden => controller.StatusCode(StatusCodes.Status403Forbidden, result.Message),
                ServiceResultStatus.NotFound => controller.NotFound(result.Message),
                ServiceResultStatus.Conflict => controller.Conflict(result.Message),
                _ => controller.StatusCode(StatusCodes.Status500InternalServerError, result.Message)
            };
        }
    }
}
