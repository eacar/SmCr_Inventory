using AutoMapper;
using Inv.Api.Requests.Warehouses;
using Inv.Application.Warehouses.Commands;

namespace Inv.Api.Profiles
{
    public class WarehouseProfile : Profile
    {
        public WarehouseProfile()
        {
            this.CreateMap<WarehouseAddRequest, WarehouseAddCommand>();
            this.CreateMap<WarehouseUpdateRequest, WarehouseUpdateCommand>();
        }
    }
}