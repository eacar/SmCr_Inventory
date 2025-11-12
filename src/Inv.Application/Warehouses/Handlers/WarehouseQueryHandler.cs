using AutoMapper;
using FluentValidation;
using Inv.Application.Base;
using Inv.Application.Contracts.Persistence;
using Inv.Application.Warehouses.Queries;
using Inv.Application.Warehouses.Responses;
using MediatR;

namespace Inv.Application.Warehouses.Handlers
{
    public class WarehouseQueryHandler : HandlerBase
        , IRequestHandler<WarehouseGetQuery, WarehouseDetailResponse?>
    {
        private readonly IMapper _mapper;
        private readonly IWarehouseRepository _warehouseRepository;
        private readonly IValidator<WarehouseGetQuery> _getValidator;

        public WarehouseQueryHandler(
            IMapper mapper,
            IWarehouseRepository warehouseRepository,
            IValidator<WarehouseGetQuery> getValidator)
        {
            _mapper = mapper;
            _warehouseRepository = warehouseRepository;
            _getValidator = getValidator;
        }

        public async Task<WarehouseDetailResponse?> Handle(WarehouseGetQuery request, CancellationToken cancellationToken)
        {
            var validation = await _getValidator.ValidateAsync(request, cancellationToken);
            if (!validation.IsValid)
                throw new ValidationException(validation.Errors);

            var result = await _warehouseRepository.GetByIdAsync(request.Id, cancellationToken);

            return _mapper.Map<WarehouseDetailResponse>(result);
        }
    }
}
