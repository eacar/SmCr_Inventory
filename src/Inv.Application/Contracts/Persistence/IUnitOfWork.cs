namespace Inv.Application.Contracts.Persistence
{
    public interface IUnitOfWork : IDisposable
    {
        Task BeginAsync(CancellationToken cancellationToken = default);
        Task CommitAsync(CancellationToken cancellationToken = default);
    }
}