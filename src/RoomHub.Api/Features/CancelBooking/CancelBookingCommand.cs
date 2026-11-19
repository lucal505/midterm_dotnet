namespace RoomHub.Api.Features.CancelBooking;

public sealed record CancelBookingCommand(Guid Id);

// Cancel returns 204 with no body, so the response carries no data.
public sealed record CancelBookingResponse;
