using FluentValidation;
using Inv.Application.Base;
using Inv.Application.Contracts.Persistence;
using Inv.Application.Warehouses.Commands;
using Inv.Domain.Exceptions;
using Inv.Domain.Warehouses;
using MediatR;

namespace Inv.Application.Warehouses.Handlers
{
    public class WarehouseCommandHandler : HandlerBase
        , IRequestHandler<WarehouseAddCommand, AddResponseBase<Guid>>
        , IRequestHandler<WarehouseUpdateCommand>
        , IRequestHandler<WarehouseDeleteCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWarehouseRepository _warehouseRepository;
        private readonly IValidator<WarehouseAddCommand> _addValidator;
        private readonly IValidator<WarehouseUpdateCommand> _updateValidator;
        private readonly IValidator<WarehouseDeleteCommand> _deleteValidator;

        public WarehouseCommandHandler(
            IUnitOfWork unitOfWork,
            IWarehouseRepository warehouseRepository,
            IValidator<WarehouseAddCommand> addValidator,
            IValidator<WarehouseUpdateCommand> updateValidator,
            IValidator<WarehouseDeleteCommand> deleteValidator)
        {
            _unitOfWork = unitOfWork;
            _warehouseRepository = warehouseRepository;
            _addValidator = addValidator;
            _updateValidator = updateValidator;
            _deleteValidator = deleteValidator;
        }

        public async Task<AddResponseBase<Guid>> Handle(WarehouseAddCommand request, CancellationToken cancellationToken)
        {
            var validation = await _addValidator.ValidateAsync(request, cancellationToken);
            if (!validation.IsValid)
                throw new ValidationException(validation.Errors);

            using (_unitOfWork)
            {
                await _unitOfWork.BeginAsync(cancellationToken: cancellationToken);

                var entity = new Warehouse
                {
                    Name = request.Name,
                    WarehouseStatus = request.WarehouseStatus
                };

                await _warehouseRepository.InsertAsync(entity, cancellationToken);

                await _unitOfWork.CommitAsync(cancellationToken);

                return new AddResponseBase<Guid>
                {
                    Id = entity.Id
                };
            }
        }

        public async Task Handle(WarehouseUpdateCommand request, CancellationToken cancellationToken)
        {
            var validation = await _updateValidator.ValidateAsync(request, cancellationToken);
            if (!validation.IsValid)
                throw new ValidationException(validation.Errors);

            using (_unitOfWork)
            {
                await _unitOfWork.BeginAsync(cancellationToken: cancellationToken);

                var entity = await _warehouseRepository.GetByIdAsync(request.Id, cancellationToken);

                if (entity is null)
                    throw new BusinessException("ErrorCodes.40", "Entity not found");

                entity.WarehouseStatus = request.WarehouseStatus;
                entity.Name = request.Name;

                await _unitOfWork.CommitAsync(cancellationToken);
            }
        }

        public async Task Handle(WarehouseDeleteCommand request, CancellationToken cancellationToken)
        {
            var validation = await _deleteValidator.ValidateAsync(request, cancellationToken);
            if (!validation.IsValid) throw new ValidationException(validation.Errors);

            using (_unitOfWork)
            {
                await _unitOfWork.BeginAsync(cancellationToken);

                var entity = await _warehouseRepository.GetByIdAsync(request.Id, cancellationToken);

                if (entity is null) throw new BusinessException("ErrorCodes.40", "Entity not found");

                await _warehouseRepository.DeleteAsync(entity, cancellationToken);

                await _unitOfWork.CommitAsync(cancellationToken);
            }
        }
    }
}
