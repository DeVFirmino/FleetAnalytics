namespace FleetAnalytics.Domain.Interfaces;

/// <summary>
/// Commits every change staged by the repositories in the current scope
/// as a single atomic database transaction. Implemented by the DbContext itself.
/// </summary>
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
