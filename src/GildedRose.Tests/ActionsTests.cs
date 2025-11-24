using Xunit;
using GildedRose.Console.Rules.Actions;
using GildedRose.Console;

namespace GildedRose.Tests;

public class ActionsTests
{
    [Fact]
    public void DegradeQualityAction_ReducesQualityByDecrement()
    {
        // Arrange
        var item = new Item { Name = "Test Item", Quality = 10, SellIn = 5 };
        var action = new DegradeQualityAction(decrement: 2, minValue: 0);

        // Act
        action.Execute(item);

        // Assert
        Assert.Equal(8, item.Quality);
    }

    [Fact]
    public void DegradeQualityAction_ClampsQualityToMinValue()
    {
        // Arrange
        var item = new Item { Name = "Test Item", Quality = 3, SellIn = 5 };
        var action = new DegradeQualityAction(decrement: 5, minValue: 0);

        // Act
        action.Execute(item);

        // Assert
        Assert.Equal(0, item.Quality);
    }

    [Fact]
    public void DegradeQualityAction_RespectCustomMinValue()
    {
        // Arrange
        var item = new Item { Name = "Test Item", Quality = 3, SellIn = 5 };
        var action = new DegradeQualityAction(decrement: 2, minValue: 1);

        // Act
        action.Execute(item);

        // Assert
        Assert.Equal(1, item.Quality);
    }

    [Fact]
    public void DecreaseSellInAction_ReducesSellInByDays()
    {
        // Arrange
        var item = new Item { Name = "Test Item", Quality = 10, SellIn = 10 };
        var action = new DecreaseSellInAction(days: 1);

        // Act
        action.Execute(item);

        // Assert
        Assert.Equal(9, item.SellIn);
    }

    [Fact]
    public void DecreaseSellInAction_CanMakeSellInNegative()
    {
        // Arrange
        var item = new Item { Name = "Test Item", Quality = 10, SellIn = 2 };
        var action = new DecreaseSellInAction(days: 5);

        // Act
        action.Execute(item);

        // Assert
        Assert.Equal(-3, item.SellIn);
    }

    [Fact]
    public void IncreaseQualityAction_IncreasesQualityByIncrement()
    {
        // Arrange
        var item = new Item { Name = "Test Item", Quality = 10, SellIn = 5 };
        var action = new IncreaseQualityAction(increment: 2, maxValue: 50);

        // Act
        action.Execute(item);

        // Assert
        Assert.Equal(12, item.Quality);
    }

    [Fact]
    public void IncreaseQualityAction_ClampsQualityToMaxValue()
    {
        // Arrange
        var item = new Item { Name = "Test Item", Quality = 48, SellIn = 5 };
        var action = new IncreaseQualityAction(increment: 5, maxValue: 50);

        // Act
        action.Execute(item);

        // Assert
        Assert.Equal(50, item.Quality);
    }

    [Fact]
    public void IncreaseQualityAction_RespectCustomMaxValue()
    {
        // Arrange
        var item = new Item { Name = "Test Item", Quality = 8, SellIn = 5 };
        var action = new IncreaseQualityAction(increment: 3, maxValue: 10);

        // Act
        action.Execute(item);

        // Assert
        Assert.Equal(10, item.Quality);
    }

    [Fact]
    public void SetQualityToZeroAction_SetsQualityToZero()
    {
        // Arrange
        var item = new Item { Name = "Test Item", Quality = 50, SellIn = 5 };
        var action = new SetQualityToZeroAction();

        // Act
        action.Execute(item);

        // Assert
        Assert.Equal(0, item.Quality);
    }

    [Fact]
    public void SetQualityToZeroAction_WorksWhenQualityAlreadyZero()
    {
        // Arrange
        var item = new Item { Name = "Test Item", Quality = 0, SellIn = 5 };
        var action = new SetQualityToZeroAction();

        // Act
        action.Execute(item);

        // Assert
        Assert.Equal(0, item.Quality);
    }

    [Fact]
    public void DoNothingAction_LeavesItemUnchanged()
    {
        // Arrange
        var item = new Item { Name = "Test Item", Quality = 25, SellIn = 5 };
        var action = new DoNothingAction();

        // Act
        action.Execute(item);

        // Assert
        Assert.Equal(25, item.Quality);
        Assert.Equal(5, item.SellIn);
        Assert.Equal("Test Item", item.Name);
    }
}
