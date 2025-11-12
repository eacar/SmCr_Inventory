using Inv.Domain.Warehouses;

namespace Inv.Application.Contracts.Persistence
{
    public interface IWarehouseRepository : IRepository<Warehouse, Guid>
    {
    }
}