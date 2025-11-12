using Inv.Domain.Warehouses;

namespace Inv.Api.Requests.Warehouses
{
    public class WarehouseAddRequest
    {
        public string Name { get; set; }
        public WarehouseStatus WarehouseStatus { get; set; }
    }
}