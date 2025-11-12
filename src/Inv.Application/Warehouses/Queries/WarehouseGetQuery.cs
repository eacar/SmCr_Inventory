using Inv.Application.Warehouses.Responses;
using MediatR;

namespace Inv.Application.Warehouses.Queries
{
    public class WarehouseGetQuery : IRequest<WarehouseDetailResponse?>
    {
        public Guid Id { get; set; }
    }
}