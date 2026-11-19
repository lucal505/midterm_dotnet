using RoomHub.Api.Common;
using RoomHub.Api.Domain;

namespace RoomHub.Api.Features.CancelBooking;

public sealed class CancelBookingHandler(IBookingRepository bookings, IUnitOfWork unitOfWork)
    : ICommandHandler<CancelBookingCommand, CancelBookingResponse>
{
    public async Task<Result<CancelBookingResponse>> Handle(
        CancelBookingCommand command, CancellationToken cancellationToken)
    {
        // R8: missing -> 404.
        var booking = await bookings.GetByIdAsync(command.Id, cancellationToken);
        if (booking is null)
            return Error.NotFound("BookingNotFound", "The booking does not exist.");

        // R8: already cancelled -> 409 (rule lives in Booking.Cancel).
        var cancelResult = booking.Cancel();
        if (cancelResult.IsFailure)
            return cancelResult.Error!;

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return new CancelBookingResponse();
    }
}
