namespace RoomHub.Api.Features.GetBooking;

public sealed record GetBookingQuery(Guid Id);

public sealed record GetBookingResponse(
    Guid Id,
    Guid RoomId,
    string BookedBy,
    DateTime StartUtc,
    DateTime EndUtc,
    int Attendees,
    string Status);
