using Microsoft.AspNetCore.Mvc;

namespace comidas_backend.Utils;

public static class ResultExtension
{
    public static ActionResult<T> ToActionResult<T>(this Result<T> result)
    {
        if (result.Success)
            return new OkObjectResult(result.Value);

        return new ObjectResult(new { error = result.Error, field = result.Field }) { StatusCode = result.StatusCode};
    }
}