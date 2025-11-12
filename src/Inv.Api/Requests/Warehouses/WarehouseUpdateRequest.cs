using Inv.Domain.Warehouses;

namespace Inv.Api.Requests.Warehouses
{
    public class WarehouseUpdateRequest
    {
        public string Name { get; set; }
        public WarehouseStatus WarehouseStatus { get; set; }
    }
}