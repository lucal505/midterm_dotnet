
namespace RoomHub.Api.Common;

public sealed class ExceptionMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (BadHttpRequestException ex)
        {
            // Malformed / unreadable request body -> 400 ProblemDetails, not 500.
            await Results.Problem(
                    detail: ex.Message,
                    statusCode: ex.StatusCode,
                    title: "InvalidRequest")
                .ExecuteAsync(context);
        }
    }
}
