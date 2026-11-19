using RoomHub.Api.Common;

namespace RoomHub.Api.Features.CancelBooking;

public static class CancelBookingEndpoint
{
    public static void MapCancelBooking(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/bookings/{id:guid}/cancel", async (
            Guid id,
            ICommandHandler<CancelBookingCommand, CancelBookingResponse> handler,
            CancellationToken ct) =>
        {
            var result = await handler.Handle(new CancelBookingCommand(id), ct);
            return result.IsSuccess
                ? Results.NoContent()
                : result.Error!.ToProblem();
        }).WithTags("Bookings");
    }
}
