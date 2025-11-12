namespace Inv.Application.Contracts.Persistence
{
    public interface IRepository<TEntity, TId>
        where TId : struct
        where TEntity : class
    {
        Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default);
        Task<TEntity> InsertAsync(TEntity entity, CancellationToken cancellationToken = default);
        Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);
    }
}