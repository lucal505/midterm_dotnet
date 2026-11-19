using RoomHub.Api.Common;

namespace RoomHub.Api.Domain;

public enum BookingStatus
{
    Active,
    Cancelled
}

public class Booking
{
    public static readonly TimeSpan MaxDuration = TimeSpan.FromHours(4);

    // For persistence frameworks (EF Core).
    private Booking() { }

    private Booking(Guid id, Guid roomId, string bookedBy, DateTime startUtc, DateTime endUtc, int attendees)
    {
        Id = id;
        RoomId = roomId;
        BookedBy = bookedBy;
        StartUtc = startUtc;
        EndUtc = endUtc;
        Attendees = attendees;
        Status = BookingStatus.Active;
    }

    public Guid Id { get; private set; }
    public Guid RoomId { get; private set; }
    public string BookedBy { get; private set; } = string.Empty;
    public DateTime StartUtc { get; private set; }
    public DateTime EndUtc { get; private set; }
    public int Attendees { get; private set; }
    public BookingStatus Status { get; private set; }

    public static Result<Booking> Create(
        Room room, string? bookedBy, DateTime startUtc, DateTime endUtc, int attendees, DateTime nowUtc)
    {
        startUtc = AsUtc(startUtc);
        endUtc = AsUtc(endUtc);

        // R2
        if (string.IsNullOrWhiteSpace(bookedBy))
            return Error.Validation("InvalidBookedBy", "BookedBy is required.");

        // R3
        if (endUtc <= startUtc)
            return Error.Validation("InvalidRange", "The end must be after the start.");

        // R4
        if (attendees <= 0)
            return Error.Validation("InvalidAttendees", "Attendees must be positive.");

        if (attendees > room.Capacity)
            return Error.Validation("AttendeesExceedCapacity", "The room does not have enough seats.");

        // R5: "now" is passed in (it comes from TimeProvider in the handler).
        if (startUtc < nowUtc)
            return Error.Validation("StartInPast", "A booking cannot start in the past.");

        // R6: exactly 4 hours is allowed.
        if (endUtc - startUtc > MaxDuration)
            return Error.Validation("DurationTooLong", "A booking can last at most 4 hours.");

        return new Booking(Guid.NewGuid(), room.Id, bookedBy.Trim(), startUtc, endUtc, attendees);
    }

    // Dates without an offset are treated as UTC; dates with an offset are converted to UTC.
    private static DateTime AsUtc(DateTime value) => value.Kind switch
    {
        DateTimeKind.Utc => value,
        DateTimeKind.Local => value.ToUniversalTime(),
        _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
    };

    // R7: starter overlap logic (touching intervals + active-only are fixed at level C).
    public bool Overlaps(DateTime startUtc, DateTime endUtc) =>
        StartUtc <= endUtc && startUtc <= EndUtc;

    // R8: already-cancelled -> 409 is fixed at level C.
    public Result Cancel()
    {
        Status = BookingStatus.Cancelled;
        return Result.Success();
    }
}
