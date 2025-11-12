using Inv.Application.Base;
using Inv.Domain.Warehouses;
using MediatR;

namespace Inv.Application.Warehouses.Commands
{
    public class WarehouseAddCommand : IRequest<AddResponseBase<Guid>>
    {
        public string Name { get; set; }
        public WarehouseStatus WarehouseStatus { get; set; }
    }
}