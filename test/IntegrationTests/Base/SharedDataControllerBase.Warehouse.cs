using Inv.Domain.Warehouses;
using Inv.Infrastructure.Database.Context;

namespace IntegrationTests.Base
{
    public partial class SharedDataControllerBase<TFactory> where TFactory : ApiFactoryBase
    {
        protected async Task<Warehouse> AddWarehouseAsync(AppDbContext db,
            string? name = null,
            WarehouseStatus warehouseStatus = WarehouseStatus.Active,
            bool isDeleted = false,
            Guid? id = null)
        {
            var entity = new Warehouse
            {
                Id = id ?? Guid.NewGuid(),
                Name = name ?? "House",
                WarehouseStatus = warehouseStatus,
                IsDeleted = isDeleted
            };

            await db.Warehouses.AddAsync(entity);

            return entity;
        }
    }
}