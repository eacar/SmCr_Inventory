using Inv.Domain.Warehouses;
using MediatR;

namespace Inv.Application.Warehouses.Commands
{
    public class WarehouseUpdateCommand : IRequest
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public WarehouseStatus WarehouseStatus { get; set; }
    }
}