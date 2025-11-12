using AutoMapper;
using Inv.Application.Warehouses.Responses;
using Inv.Domain.Warehouses;

namespace Inv.Application.Warehouses.Profiles
{
    public class WarehouseProfile : Profile
    {
        public WarehouseProfile()
        {
            CreateMap<Warehouse, WarehouseDetailResponse>();
        }
    }
}