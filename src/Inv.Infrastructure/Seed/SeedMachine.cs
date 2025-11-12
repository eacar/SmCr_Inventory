using Inv.Application.Contracts.Security;
using Inv.Infrastructure.Database.Context;
using Inv.Infrastructure.Seed.Base;
using Inv.Infrastructure.Seed.Seeders;

namespace Inv.Infrastructure.Seed
{
    public static class SeedMachine
    {
        public static void Seed(AppDbContext context, IPasswordHasher passwordHasher)
        {
            var seedBuilder = new SeederBase(context);
            seedBuilder.AddSeed(new UserSeeder(passwordHasher));
            context.SaveChanges();
        }
    }
}