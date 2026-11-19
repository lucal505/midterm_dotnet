namespace RoomHub.Api.Domain;

public interface IBookingRepository
{
    Task<Booking?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyList<Booking>> ListActiveForRoomAsync(Guid roomId, CancellationToken cancellationToken);

    // R9: active bookings of one participant whose StartUtc falls on the given UTC day (all rooms).
    Task<int> CountActiveForPersonOnDayAsync(string bookedBy, DateOnly dayUtc, CancellationToken cancellationToken);

    void Add(Booking booking);
}
