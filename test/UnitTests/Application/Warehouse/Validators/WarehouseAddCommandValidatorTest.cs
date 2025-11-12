using FluentValidation.TestHelper;
using Inv.Application.Warehouses.Commands;
using Inv.Application.Warehouses.Validators;
using Inv.Domain.Warehouses;

namespace UnitTests.Application.Warehouse.Validators;

public sealed class WarehouseAddCommandValidatorTest
{
    #region Fields

    private readonly WarehouseAddCommandValidator _testClass;
    private readonly WarehouseAddCommand _command;

    #endregion

    #region Constructors

    public WarehouseAddCommandValidatorTest()
    {
        _testClass = new WarehouseAddCommandValidator();
        _command = new WarehouseAddCommand
        {
            Name = "Main Warehouse",
            WarehouseStatus = WarehouseStatus.Active
        };
    }

    #endregion

    #region Tests

    [Fact]
    public async Task Should_Pass_When_EverythingIsValid()
    {
        #region Acts

        var result = await _testClass.TestValidateAsync(_command);

        #endregion

        #region Asserts

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);

        #endregion
    }

    [Fact]
    public async Task Should_NotPass_When_Name_Is_TooShort()
    {
        #region Setups

        _command.Name = "A"; // length = 1

        #endregion

        #region Acts

        var result = await _testClass.TestValidateAsync(_command);

        #endregion

        #region Asserts

        var errors = result.ShouldHaveValidationErrorFor(x => x.Name).ToList();
        Assert.NotEmpty(errors);
        Assert.Equal("ErrorCode_3", errors.First().ErrorCode);

        #endregion
    }

    [Fact]
    public async Task Should_NotPass_When_Name_Is_TooLong()
    {
        #region Setups

        _command.Name = new string('x', 151); // > 150

        #endregion

        #region Acts

        var result = await _testClass.TestValidateAsync(_command);

        #endregion

        #region Asserts

        var errors = result.ShouldHaveValidationErrorFor(x => x.Name).ToList();
        Assert.NotEmpty(errors);
        Assert.Equal("ErrorCode_4", errors.First().ErrorCode);

        #endregion
    }

    [Theory]
    [InlineData(2)]
    [InlineData(150)]
    public async Task Should_Pass_When_Name_Length_OnBoundary(int length)
    {
        #region Setups

        _command.Name = new string('a', length);

        #endregion

        #region Acts

        var result = await _testClass.TestValidateAsync(_command);

        #endregion

        #region Asserts

        Assert.True(result.IsValid);

        #endregion
    }

    [Fact]
    public async Task Should_NotPass_When_WarehouseStatus_Is_Invalid()
    {
        #region Setups

        _command.WarehouseStatus = (WarehouseStatus)999;

        #endregion

        #region Acts

        var result = await _testClass.TestValidateAsync(_command);

        #endregion

        #region Asserts

        var errors = result.ShouldHaveValidationErrorFor(x => x.WarehouseStatus).ToList();
        Assert.NotEmpty(errors);
        Assert.Equal("ErrorCode_5", errors.First().ErrorCode);

        #endregion
    }

    #endregion
}