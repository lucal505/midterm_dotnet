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
