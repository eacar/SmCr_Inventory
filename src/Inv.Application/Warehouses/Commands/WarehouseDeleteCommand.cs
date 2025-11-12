using MediatR;

namespace Inv.Application.Warehouses.Commands
{
    public class WarehouseDeleteCommand : IRequest
    {
        public Guid Id { get; set; }
    }
}