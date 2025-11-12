using Inv.Infrastructure.Database.Context;

namespace Inv.Infrastructure.Seed.Contracts
{
    public interface ISeeder
    {
        void Seed(AppDbContext context);
    }
}