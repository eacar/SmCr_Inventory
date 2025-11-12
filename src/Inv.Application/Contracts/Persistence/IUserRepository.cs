using Inv.Domain.Users;

namespace Inv.Application.Contracts.Persistence
{
    public interface IUserRepository : IRepository<User, Guid>
    {
        Task<(Guid UserId, string PasswordHash)?> GetAuthAsync(string email, CancellationToken cancellationToken = default);
    }
}