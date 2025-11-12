using Inv.Application.Contracts.Persistence;
using Inv.Domain.Base;
using Microsoft.EntityFrameworkCore;

namespace Inv.Infrastructure.Repositories
{
    public class Repository<TDbContext, TEntity, TId> : IRepository<TEntity, TId>
        where TId : struct
        where TEntity : Entity<TId>
        where TDbContext : DbContext
    {
        private readonly DbSet<TEntity> _dbSet;

        protected TDbContext DbContext { get; }

        public Repository(TDbContext dbContext)
        {
            DbContext = dbContext;
            _dbSet = DbContext.Set<TEntity>();
        }

        public Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default)
            => _dbSet.FirstOrDefaultAsync(c => !((ISoftDelete)c).IsDeleted && c.Id.Equals(id), cancellationToken);

        public virtual async Task<TEntity> InsertAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            await _dbSet.AddAsync(entity, cancellationToken);

            return entity;
        }

        public Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            if (entity is ISoftDelete en)
            {
                en.IsDeleted = true;
            }
            else
            {
                _dbSet.Remove(entity);
            }

            return Task.CompletedTask;
        }
    }
}