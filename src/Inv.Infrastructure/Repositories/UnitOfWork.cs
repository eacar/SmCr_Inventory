using Inv.Application.Contracts.Persistence;
using Inv.Infrastructure.Database.Context;
using Microsoft.EntityFrameworkCore.Storage;

namespace Inv.Infrastructure.Repositories
{
    public sealed class UnitOfWork : IUnitOfWork
    {
        #region Constructors

        public UnitOfWork(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        #endregion

        #region Fields

        private readonly AppDbContext _dbContext;
        private IDbContextTransaction _currentTransaction;

        #endregion

        #region Methods - Public - IUnitOfWork

        public async Task BeginAsync(CancellationToken cancellationToken = default)
        {
            _currentTransaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        }

        public async Task CommitAsync(CancellationToken cancellationToken = default)
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
            await _currentTransaction.CommitAsync(cancellationToken);
        }

        public void Dispose()
        {
            _currentTransaction?.Dispose();
        }

        #endregion
    }
}