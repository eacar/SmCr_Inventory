using FluentValidation;
using Inv.Application.Warehouses.Queries;

namespace Inv.Application.Warehouses.Validators
{
    public class WarehouseGetQueryValidator : AbstractValidator<WarehouseGetQuery>
    {
        public WarehouseGetQueryValidator()
        {
            RuleFor(c => c.Id).NotEmpty().WithErrorCode("ErrorCode_1");
        }
    }
}