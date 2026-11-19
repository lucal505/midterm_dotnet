using RoomHub.Api.Domain;

namespace RoomHub.Api.Persistence;

public sealed class RoomRepository(RoomHubDbContext db) : IRoomRepository
{
    public async Task<Room?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        await db.Rooms.FindAsync([id], cancellationToken);

    public void Add(Room room) => db.Rooms.Add(room);
}
