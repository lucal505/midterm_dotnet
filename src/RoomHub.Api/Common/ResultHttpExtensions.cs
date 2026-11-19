namespace RoomHub.Api.Common;

public static class ResultHttpExtensions
{
    // Maps a business error to a ProblemDetails response with the right HTTP status.
    public static IResult ToProblem(this Error error)
    {
        var statusCode = error.Type switch
        {
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status500InternalServerError
        };

        return Results.Problem(detail: error.Message, statusCode: statusCode, title: error.Code);
    }
}
