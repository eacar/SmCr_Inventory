using Inv.Application.Contracts.Persistence;
using Inv.Domain.Users;
using Inv.Infrastructure.Database.Context;
using Microsoft.EntityFrameworkCore;

namespace Inv.Infrastructure.Repositories
{
    public class UserRepository : Repository<AppDbContext, User, Guid>, IUserRepository
    {
        public UserRepository(AppDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<(Guid UserId, string PasswordHash)?> GetAuthAsync(string email, CancellationToken cancellationToken = default)
        {
            var row = await DbContext.Users
                .AsNoTracking()
                .Where(u => !u.IsDeleted && u.Email.Equals(email))
                .Select(u => new { u.Id, u.PasswordHash })
                .SingleOrDefaultAsync(cancellationToken);

            return row is null ? null : (row.Id, row.PasswordHash);
        }
    }
}