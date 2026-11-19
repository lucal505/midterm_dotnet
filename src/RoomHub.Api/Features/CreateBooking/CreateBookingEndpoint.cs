using RoomHub.Api.Common;

namespace RoomHub.Api.Features.CreateBooking;

// HTTP body; roomId comes from the route.
public sealed record CreateBookingRequest(string? BookedBy, DateTime StartUtc, DateTime EndUtc, int Attendees);

public static class CreateBookingEndpoint
{
    public static void MapCreateBooking(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/rooms/{roomId:guid}/bookings", async (
            Guid roomId,
            CreateBookingRequest request,
            ICommandHandler<CreateBookingCommand, CreateBookingResponse> handler,
            CancellationToken ct) =>
        {
            var command = new CreateBookingCommand(
                roomId, request.BookedBy, request.StartUtc, request.EndUtc, request.Attendees);

            var result = await handler.Handle(command, ct);
            return result.IsSuccess
                ? Results.Created($"/api/bookings/{result.Value.Id}", result.Value)
                : result.Error!.ToProblem();
        }).WithTags("Bookings");
    }
}
