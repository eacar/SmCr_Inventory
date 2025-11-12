using Inv.Domain.Warehouses;

namespace Inv.Application.Warehouses.Responses
{
    public class WarehouseDetailResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public WarehouseStatus WarehouseStatus { get; set; }
    }
}