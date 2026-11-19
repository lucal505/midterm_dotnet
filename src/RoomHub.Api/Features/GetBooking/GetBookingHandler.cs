using RoomHub.Api.Common;
using RoomHub.Api.Persistence;

namespace RoomHub.Api.Features.GetBooking;

public sealed class GetBookingHandler(BookingRepository bookings)
    : IQueryHandler<GetBookingQuery, GetBookingResponse>
{
    public Task<Result<GetBookingResponse>> Handle(GetBookingQuery query, CancellationToken cancellationToken)
    {
        var b = bookings.Get(query.Id);
        Result<GetBookingResponse> result = b is null
            ? Error.NotFound("BookingNotFound", "The booking does not exist.")
            : new GetBookingResponse(
                b.Id, b.RoomId, b.BookedBy, b.StartUtc, b.EndUtc, b.Attendees, b.Status.ToString());

        return Task.FromResult(result);
    }
}
