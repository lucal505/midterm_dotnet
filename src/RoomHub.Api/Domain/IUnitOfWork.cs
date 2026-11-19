namespace RoomHub.Api.Domain;

// Commits all changes tracked by the repositories in one go.
public interface IUnitOfWork
{
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
