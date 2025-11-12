using Inv.Application.Contracts.Security;
using Inv.Domain.Users;
using Inv.Infrastructure.Database.Context;
using Inv.Infrastructure.Seed.Contracts;

namespace Inv.Infrastructure.Seed.Seeders
{
    public class UserSeeder : ISeeder
    {
        private readonly IPasswordHasher _passwordHasher;

        public UserSeeder(IPasswordHasher passwordHasher)
        {
            _passwordHasher = passwordHasher;
        }

        public void Seed(AppDbContext context)
        {
            //For interview reasons, we can use only 1 user for the moment just to login
            var ss = _passwordHasher.ToHashedPassword("password");
            var gg = _passwordHasher.ValidateHashedPassword("password", ss);
            context.Users.Add(new User
            {
                Id = Guid.NewGuid(),
                Email = "admin@admin.com",
                PasswordHash = ss
            });


        }
    }
}