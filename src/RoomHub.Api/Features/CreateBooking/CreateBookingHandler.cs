using RoomHub.Api.Common;
using RoomHub.Api.Domain;

namespace RoomHub.Api.Features.CreateBooking;

public sealed class CreateBookingHandler(
    IRoomRepository rooms,
    IBookingRepository bookings,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider)
    : ICommandHandler<CreateBookingCommand, CreateBookingResponse>
{
    public async Task<Result<CreateBookingResponse>> Handle(
        CreateBookingCommand command, CancellationToken cancellationToken)
    {
        // Contract: 404 for a missing room comes before any validation.
        var room = await rooms.GetByIdAsync(command.RoomId, cancellationToken);
        if (room is null)
            return Error.NotFound("RoomNotFound", "The room does not exist.");

        // R2-R6 live in the domain; "now" comes from TimeProvider (R5).
        var nowUtc = timeProvider.GetUtcNow().UtcDateTime;
        var bookingResult = Booking.Create(
            room, command.BookedBy, command.StartUtc, command.EndUtc, command.Attendees, nowUtc);
        if (bookingResult.IsFailure)
            return bookingResult.Error!;

        var booking = bookingResult.Value;

        // R7: only active bookings of this room can conflict; the overlap rule is in Booking.Overlaps.
        var activeBookings = await bookings.ListActiveForRoomAsync(room.Id, cancellationToken);
        if (activeBookings.Any(existing => existing.Overlaps(booking.StartUtc, booking.EndUtc)))
            return Error.Conflict("RoomOccupied", "The room is occupied in this interval.");

        // R9: max 2 active bookings per participant per UTC day, across all rooms.
        var activeThatDay = await bookings.CountActiveForPersonOnDayAsync(
            booking.BookedBy, booking.DayUtc, cancellationToken);
        if (activeThatDay >= Booking.MaxActiveBookingsPerPersonPerDay)
            return Error.Conflict(
                "DailyBookingLimitReached",
                $"{booking.BookedBy} already has {Booking.MaxActiveBookingsPerPersonPerDay} active bookings on this day.");

        bookings.Add(booking);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateBookingResponse(booking.Id, booking.RoomId, booking.Status.ToString());
    }
}
