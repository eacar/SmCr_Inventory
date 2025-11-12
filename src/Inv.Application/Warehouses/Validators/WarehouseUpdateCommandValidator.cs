using FluentValidation;
using Inv.Application.Warehouses.Commands;

namespace Inv.Application.Warehouses.Validators
{
    public class WarehouseUpdateCommandValidator : AbstractValidator<WarehouseUpdateCommand>
    {
        public WarehouseUpdateCommandValidator()
        {
            RuleFor(c => c.Id).NotEmpty().WithErrorCode("ErrorCode_2");
            RuleFor(c => c.Name)
                .MinimumLength(2).WithErrorCode("ErrorCode_3")
                .MaximumLength(150).WithErrorCode("ErrorCode_4")
                ;
            RuleFor(c => c.WarehouseStatus)
                .IsInEnum()
                .WithErrorCode("ErrorCode_5");
        }
    }
}