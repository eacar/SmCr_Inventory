using Inv.Domain.Base;

namespace Inv.Domain.Warehouses
{
    public class Warehouse : SoftDeleteBase<Guid>
    {
        public string Name { get; set; }
        public WarehouseStatus WarehouseStatus { get; set; }
    }
}