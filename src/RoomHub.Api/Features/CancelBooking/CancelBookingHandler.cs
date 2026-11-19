using RoomHub.Api.Common;
using RoomHub.Api.Persistence;

namespace RoomHub.Api.Features.CancelBooking;

public sealed class CancelBookingHandler(BookingRepository bookings)
    : ICommandHandler<CancelBookingCommand, CancelBookingResponse>
{
    public Task<Result<CancelBookingResponse>> Handle(
        CancelBookingCommand command, CancellationToken cancellationToken) =>
        Task.FromResult(Execute(command));

    private Result<CancelBookingResponse> Execute(CancelBookingCommand command)
    {
        var booking = bookings.Get(command.Id);
        if (booking is null)
            return Error.NotFound("BookingNotFound", "The booking does not exist.");

        var cancelResult = booking.Cancel();
        if (cancelResult.IsFailure)
            return cancelResult.Error!;

        return new CancelBookingResponse();
    }
}
