using RoomHub.Api.Common;
using RoomHub.Api.Domain;
using RoomHub.Api.Persistence;

namespace RoomHub.Api.Features.CreateBooking;

public sealed class CreateBookingHandler(
    RoomRepository rooms,
    BookingRepository bookings,
    TimeProvider timeProvider)
    : ICommandHandler<CreateBookingCommand, CreateBookingResponse>
{
    public Task<Result<CreateBookingResponse>> Handle(
        CreateBookingCommand command, CancellationToken cancellationToken) =>
        Task.FromResult(Execute(command));

    private Result<CreateBookingResponse> Execute(CreateBookingCommand command)
    {
        // Contract: 404 for a missing room comes before any validation.
        var room = rooms.Get(command.RoomId);
        if (room is null)
            return Error.NotFound("RoomNotFound", "The room does not exist.");

        // R2-R6 live in the domain; "now" comes from TimeProvider (R5).
        var nowUtc = timeProvider.GetUtcNow().UtcDateTime;
        var bookingResult = Booking.Create(
            room, command.BookedBy, command.StartUtc, command.EndUtc, command.Attendees, nowUtc);
        if (bookingResult.IsFailure)
            return bookingResult.Error!;

        var booking = bookingResult.Value;

        // R7
        if (bookings.ListForRoom(room.Id).Any(existing => existing.Overlaps(booking.StartUtc, booking.EndUtc)))
            return Error.Conflict("RoomOccupied", "The room is occupied in this interval.");

        bookings.Add(booking);
        return new CreateBookingResponse(booking.Id, booking.RoomId, booking.Status.ToString());
    }
}
