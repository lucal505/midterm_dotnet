using RoomHub.Api.Domain;

namespace RoomHub.Api.Persistence;

public sealed class UnitOfWork(RoomHubDbContext db) : IUnitOfWork
{
    public Task SaveChangesAsync(CancellationToken cancellationToken) =>
        db.SaveChangesAsync(cancellationToken);
}
