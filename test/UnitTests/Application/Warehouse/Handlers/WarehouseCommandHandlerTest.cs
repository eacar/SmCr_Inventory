using FluentValidation;
using FluentValidation.Results;
using Inv.Application.Base;
using Inv.Application.Contracts.Persistence;
using Inv.Application.Warehouses.Commands;
using Inv.Application.Warehouses.Handlers;
using Inv.Domain.Exceptions;
using Inv.Domain.Warehouses;
using Moq;

namespace UnitTests.Application.Warehouse.Handlers;

public sealed class WarehouseCommandHandlerTest
{
    #region Fields

    private readonly Mock<IUnitOfWork> _uow;
    private readonly Mock<IWarehouseRepository> _repo;

    private readonly Mock<IValidator<WarehouseAddCommand>> _addValidator;
    private readonly Mock<IValidator<WarehouseUpdateCommand>> _updateValidator;
    private readonly Mock<IValidator<WarehouseDeleteCommand>> _deleteValidator;

    private readonly WarehouseCommandHandler _testClass;

    private readonly CancellationToken _ct = CancellationToken.None;

    #endregion

    #region Constructors

    public WarehouseCommandHandlerTest()
    {
        _uow = new Mock<IUnitOfWork>();
        _repo = new Mock<IWarehouseRepository>();

        _addValidator = new Mock<IValidator<WarehouseAddCommand>>();
        _updateValidator = new Mock<IValidator<WarehouseUpdateCommand>>();
        _deleteValidator = new Mock<IValidator<WarehouseDeleteCommand>>();

        _addValidator.Setup(v => v.ValidateAsync(It.IsAny<WarehouseAddCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _updateValidator.Setup(v => v.ValidateAsync(It.IsAny<WarehouseUpdateCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _deleteValidator.Setup(v => v.ValidateAsync(It.IsAny<WarehouseDeleteCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _uow.Setup(u => u.BeginAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        _uow.Setup(u => u.CommitAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        _testClass = new WarehouseCommandHandler(
            _uow.Object, _repo.Object, _addValidator.Object, _updateValidator.Object, _deleteValidator.Object);
    }

    #endregion

    #region Helpers

    private static WarehouseAddCommand MakeAddCmd() => new WarehouseAddCommand
    {
        Name = "Main Warehouse",
        WarehouseStatus = WarehouseStatus.Active
    };

    private static WarehouseUpdateCommand MakeUpdateCmd(Guid? id = null) => new WarehouseUpdateCommand
    {
        Id = id ?? Guid.NewGuid(),
        Name = "Updated Name",
        WarehouseStatus = WarehouseStatus.Maintenance
    };

    private static WarehouseDeleteCommand MakeDeleteCmd(Guid? id = null) => new WarehouseDeleteCommand
    {
        Id = id ?? Guid.NewGuid()
    };

    #endregion

    #region Tests - Add

    [Fact]
    public async Task Add_Should_Throw_ValidationException_When_Invalid()
    {
        #region Setups

        var invalid = new ValidationResult(new[] { new ValidationFailure("Name", "required") });
        _addValidator.Setup(v => v.ValidateAsync(It.IsAny<WarehouseAddCommand>(), _ct))
            .ReturnsAsync(invalid);

        var cmd = MakeAddCmd();

        #endregion

        #region Acts & Asserts

        await Assert.ThrowsAsync<ValidationException>(() => _testClass.Handle(cmd, _ct));

        _uow.Verify(u => u.BeginAsync(It.IsAny<CancellationToken>()), Times.Never);
        _uow.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
        _repo.Verify(r => r.InsertAsync(It.IsAny<Inv.Domain.Warehouses.Warehouse>(), It.IsAny<CancellationToken>()), Times.Never);

        #endregion
    }

    [Fact]
    public async Task Add_Should_Insert_Entity_Commit_And_Return_Id()
    {
        #region Setups

        var cmd = MakeAddCmd();
        var createdId = Guid.NewGuid();
        Inv.Domain.Warehouses.Warehouse? captured = null;
        _repo.Setup(r => r.InsertAsync(It.IsAny<Inv.Domain.Warehouses.Warehouse>(), _ct))
            .Callback<Inv.Domain.Warehouses.Warehouse, CancellationToken>((e, _) =>
            {
                captured = e;
                e.GetType().GetProperty("Id")?.SetValue(e, createdId);
            });

        #endregion

        #region Acts

        var result = await _testClass.Handle(cmd, _ct);

        #endregion

        #region Asserts

        Assert.NotNull(captured);
        Assert.Equal(cmd.Name, captured!.Name);
        Assert.Equal(cmd.WarehouseStatus, captured.WarehouseStatus);

        Assert.IsType<AddResponseBase<Guid>>(result);
        Assert.Equal(createdId, result.Id);

        _uow.Verify(u => u.BeginAsync(_ct), Times.Once);
        _repo.Verify(r => r.InsertAsync(It.IsAny<Inv.Domain.Warehouses.Warehouse>(), _ct), Times.Once);
        _uow.Verify(u => u.CommitAsync(_ct), Times.Once);

        #endregion
    }

    #endregion

    #region Tests - Update

    [Fact]
    public async Task Update_Should_Throw_ValidationException_When_Invalid()
    {
        #region Setups

        var invalid = new ValidationResult(new[] { new ValidationFailure("Id", "required") });
        _updateValidator.Setup(v => v.ValidateAsync(It.IsAny<WarehouseUpdateCommand>(), _ct))
            .ReturnsAsync(invalid);

        var cmd = MakeUpdateCmd();

        #endregion

        #region Acts & Asserts

        await Assert.ThrowsAsync<ValidationException>(() => _testClass.Handle(cmd, _ct));

        _uow.Verify(u => u.BeginAsync(It.IsAny<CancellationToken>()), Times.Never);
        _repo.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _uow.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);

        #endregion
    }

    [Fact]
    public async Task Update_Should_Throw_BusinessException_When_NotFound()
    {
        #region Setups

        var cmd = MakeUpdateCmd();
        _repo.Setup(r => r.GetByIdAsync(cmd.Id, _ct)).ReturnsAsync((Inv.Domain.Warehouses.Warehouse)null!);

        #endregion

        #region Acts & Asserts

        var ex = await Assert.ThrowsAsync<BusinessException>(() => _testClass.Handle(cmd, _ct));
        Assert.Equal("ErrorCodes.40", ex.ErrorCode);

        _uow.Verify(u => u.BeginAsync(_ct), Times.Once);
        _uow.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);

        #endregion
    }

    [Fact]
    public async Task Update_Should_Modify_Entity_And_Commit()
    {
        #region Setups

        var cmd = MakeUpdateCmd();
        var entity = new Inv.Domain.Warehouses.Warehouse { Name = "Old", WarehouseStatus = (WarehouseStatus)1 };

        _repo.Setup(r => r.GetByIdAsync(cmd.Id, _ct)).ReturnsAsync(entity);

        #endregion

        #region Acts

        await _testClass.Handle(cmd, _ct);

        #endregion

        #region Asserts

        Assert.Equal(cmd.Name, entity.Name);
        Assert.Equal(cmd.WarehouseStatus, entity.WarehouseStatus);

        _uow.Verify(u => u.BeginAsync(_ct), Times.Once);
        _uow.Verify(u => u.CommitAsync(_ct), Times.Once);

        #endregion
    }

    #endregion

    #region Tests - Delete

    [Fact]
    public async Task Delete_Should_Throw_ValidationException_When_Invalid()
    {
        #region Setups

        var invalid = new ValidationResult(new[] { new ValidationFailure("Id", "required") });
        _deleteValidator.Setup(v => v.ValidateAsync(It.IsAny<WarehouseDeleteCommand>(), _ct))
            .ReturnsAsync(invalid);

        var cmd = MakeDeleteCmd();

        #endregion

        #region Acts & Asserts

        await Assert.ThrowsAsync<ValidationException>(() => _testClass.Handle(cmd, _ct));

        _uow.Verify(u => u.BeginAsync(It.IsAny<CancellationToken>()), Times.Never);
        _repo.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _repo.Verify(r => r.DeleteAsync(It.IsAny<Inv.Domain.Warehouses.Warehouse>(), It.IsAny<CancellationToken>()), Times.Never);
        _uow.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);

        #endregion
    }

    [Fact]
    public async Task Delete_Should_Throw_BusinessException_When_NotFound()
    {
        #region Setups

        var cmd = MakeDeleteCmd();
        _repo.Setup(r => r.GetByIdAsync(cmd.Id, _ct)).ReturnsAsync((Inv.Domain.Warehouses.Warehouse)null!);

        #endregion

        #region Acts & Asserts

        var ex = await Assert.ThrowsAsync<BusinessException>(() => _testClass.Handle(cmd, _ct));
        Assert.Equal("ErrorCodes.40", ex.ErrorCode);

        _uow.Verify(u => u.BeginAsync(_ct), Times.Once);
        _uow.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);

        #endregion
    }

    [Fact]
    public async Task Delete_Should_Remove_Entity_And_Commit()
    {
        #region Setups

        var cmd = MakeDeleteCmd();
        var entity = new Inv.Domain.Warehouses.Warehouse { Name = "Any", WarehouseStatus = (WarehouseStatus)1 };

        _repo.Setup(r => r.GetByIdAsync(cmd.Id, _ct)).ReturnsAsync(entity);
        _repo.Setup(r => r.DeleteAsync(entity, _ct)).Returns(Task.CompletedTask);

        #endregion

        #region Acts

        await _testClass.Handle(cmd, _ct);

        #endregion

        #region Asserts

        _repo.Verify(r => r.DeleteAsync(It.Is<Inv.Domain.Warehouses.Warehouse>(w => ReferenceEquals(w, entity)), _ct), Times.Once);
        _uow.Verify(u => u.CommitAsync(_ct), Times.Once);

        #endregion
    }

    #endregion
}
