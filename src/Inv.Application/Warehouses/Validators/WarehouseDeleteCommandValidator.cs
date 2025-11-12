using FluentValidation;
using Inv.Application.Warehouses.Commands;

namespace Inv.Application.Warehouses.Validators
{
    internal class WarehouseDeleteCommandValidator : AbstractValidator<WarehouseDeleteCommand>
    {
        public WarehouseDeleteCommandValidator()
        {
            RuleFor(c => c.Id)
                .NotEmpty().WithErrorCode("ErrorCode_35")
                ;
        }
    }
}