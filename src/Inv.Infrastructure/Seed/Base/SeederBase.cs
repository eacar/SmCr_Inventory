using Inv.Infrastructure.Database.Context;
using Inv.Infrastructure.Seed.Contracts;

namespace Inv.Infrastructure.Seed.Base
{
    public class SeederBase
    {
        private readonly AppDbContext _context;

        public SeederBase(AppDbContext context)
        {
            _context = context;
        }

        public SeederBase AddSeed(ISeeder seeder)
        {
            seeder.Seed(_context);
            return this;
        }
    }
}