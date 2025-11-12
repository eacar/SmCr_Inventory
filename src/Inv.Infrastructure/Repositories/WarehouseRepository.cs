using Inv.Application.Contracts.Persistence;
using Inv.Domain.Warehouses;
using Inv.Infrastructure.Database.Context;

namespace Inv.Infrastructure.Repositories
{
    public class WarehouseRepository : Repository<AppDbContext, Warehouse, Guid>, IWarehouseRepository
    {
        public WarehouseRepository(AppDbContext dbContext) : base(dbContext)
        {
        }
    }
}