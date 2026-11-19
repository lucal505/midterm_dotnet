using RoomHub.Api.Common;

namespace RoomHub.Api.Domain;

public class Room
{
    // For persistence frameworks (EF Core).
    private Room() { }

    private Room(Guid id, string name, int capacity)
    {
        Id = id;
        Name = name;
        Capacity = capacity;
    }

    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public int Capacity { get; private set; }

    // R10: rooms with capacity > 10 allow at most 2 hours; the others keep the 4-hour limit (R6).
    public const int LargeRoomCapacityThreshold = 10;
    public static readonly TimeSpan LargeRoomMaxBookingDuration = TimeSpan.FromHours(2);
    public static readonly TimeSpan DefaultMaxBookingDuration = TimeSpan.FromHours(4);

    public TimeSpan GetMaxBookingDuration() =>
        Capacity > LargeRoomCapacityThreshold ? LargeRoomMaxBookingDuration : DefaultMaxBookingDuration;

    public static Result<Room> Create(string? name, int capacity)
    {
        // R1: non-empty name, strictly positive capacity.
        if (string.IsNullOrWhiteSpace(name))
            return Error.Validation("InvalidName", "Room name is required.");

        if (capacity <= 0)
            return Error.Validation("InvalidCapacity", "Capacity must be positive.");

        return new Room(Guid.NewGuid(), name.Trim(), capacity);
    }
}
