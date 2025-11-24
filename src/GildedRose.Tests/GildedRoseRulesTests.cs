using GildedRose.Console;
using GildedRose.Console.Rules;
using GildedRose.Console.Rules.Actions;
using GildedRose.Console.Rules.Conditions;
using Xunit;

namespace GildedRose.Tests;

public class GildedRoseRulesTests
{
    private readonly IEnumerable<IRule> _rules;
    private readonly RuleEngine _ruleEngine;

    public GildedRoseRulesTests()
    {
        _rules = new List<IRule>
        {           
           new ConfigurableRule(
                   "BackstagePassesProcessingLessThan10Days",
                   new AndCondition(
                        new NameContainsCondition(SpecialItemNames.BackstagePasses),
                        new RemainedLessThanNDaysToSellDateCondition(11),
                        new RemainedMoreThanNDaysToSellDateCondition(5)
                    ),
                   new IncreaseQualityAction(2, 50)
               ),
           new ConfigurableRule(
                   "BackstagePassesProcessingLessThan5Days",
                   new AndCondition(
                        new NameContainsCondition(SpecialItemNames.BackstagePasses),
                        new RemainedLessThanNDaysToSellDateCondition(6),
                        new RemainedMoreThanNDaysToSellDateCondition(0)
                    ),
                   new IncreaseQualityAction(3, 50)
               ),            
             new ConfigurableRule(
                   "BackstagePassesProcessingMoreThan10Days",
                   new AndCondition(
                        new NameContainsCondition(SpecialItemNames.BackstagePasses),
                        new RemainedMoreThanNDaysToSellDateCondition(10)
                    ),
                   new IncreaseQualityAction(1, 50)
               ),
              new ConfigurableRule(
                   "DecreaseQualityBasicBeforeSellDate",
                   new AndCondition(
                        new NameMatchesRegexCondition("^(?!.*Sulfuras)(?!.*Brie)(?!.*Backstage)(?!.*Conjured).*$"),
                        new RemainedMoreThanNDaysToSellDateCondition(0)
                    ),

                   new DegradeQualityAction(1, 0)
               ),
              new ConfigurableRule(
                   "DecreaseQualityBasicAfterSellDate",
                   new AndCondition(
                        new NameMatchesRegexCondition("^(?!.*Sulfuras)(?!.*Brie)(?!.*Backstage)(?!.*Conjured).*$"),
                        new RemainedLessThanNDaysToSellDateCondition(1)
                    ),

                   new DegradeQualityAction(2, 0)
               ),
              new ConfigurableRule(
                   "ConjuredProcessingBeforeSellDate",
                   new AndCondition(
                       new NameContainsCondition(SpecialItemNames.Conjured),
                       new RemainedMoreThanNDaysToSellDateCondition(0)
                   ),
                   new DegradeQualityAction(2, 0)
               ),
              new ConfigurableRule(
                   "ConjuredProcessingAfterSellDate",
                   new AndCondition(
                       new NameContainsCondition(SpecialItemNames.Conjured),
                       new RemainedLessThanNDaysToSellDateCondition(1)
                   ),
                   new DegradeQualityAction(4, 0)    //Normal items decrease with speed 2 after Sell date => Conjured twice as fast (4)
               ),
              new ConfigurableRule(
                   "DecreaseSellInAllExceptSulfuras",
                   new NotCondition(
                        new NameContainsCondition(SpecialItemNames.Sulfuras)
                    ),
                   new DecreaseSellInAction(1)
               ),
              new ConfigurableRule(
                   "AgedBrieProcessing",
                   new NameContainsCondition(SpecialItemNames.AgedBrie),
                   new IncreaseQualityAction(1, 50)
               ),
              new ConfigurableRule(
                   "BackstagePassesProcessingAfterConcert",
                   new AndCondition(
                        new NameContainsCondition(SpecialItemNames.BackstagePasses),
                        new RemainedLessThanNDaysToSellDateCondition(0)
                    ),
                   new SetQualityToZeroAction()
               )
        };
        _ruleEngine = new RuleEngine(_rules);
    }

    #region Aged Brie Tests

    [Fact]
    public void AgedBrie_IncreaseQualityBeforeSellDate()
    {
        // Arrange
        var items = new List<Item>
        {
            new Item { Name = SpecialItemNames.AgedBrie, Quality = 20, SellIn = 5 }
        };

        // Act
        _ruleEngine.Process(items);

        // Assert
        Assert.Equal(21, items[0].Quality);
    }

    [Fact]
    public void AgedBrie_IncreaseQualityAfterSellDate()
    {
        // Arrange
        var items = new List<Item>
        {
            new Item { Name = SpecialItemNames.AgedBrie, Quality = 30, SellIn = -1 }
        };

        // Act
        _ruleEngine.Process(items);

        // Assert
        Assert.Equal(31, items[0].Quality);
    }

    [Fact]
    public void AgedBrie_QualityDoesNotExceedMax()
    {
        // Arrange
        var items = new List<Item>
        {
            new Item { Name = SpecialItemNames.AgedBrie, Quality = 50, SellIn = 5 }
        };

        // Act
        _ruleEngine.Process(items);

        // Assert
        Assert.Equal(50, items[0].Quality);
    }

    [Fact]
    public void AgedBrie_SellInDecreases()
    {
        // Arrange
        var items = new List<Item>
        {
            new Item { Name = SpecialItemNames.AgedBrie, Quality = 20, SellIn = 5 }
        };

        // Act
        _ruleEngine.Process(items);

        // Assert
        Assert.Equal(4, items[0].SellIn);
    }

    [Fact]
    public void AgedBrie_NearMaxQuality_IncreasesWithinBound()
    {
        // Arrange
        var items = new List<Item>
        {
            new Item { Name = SpecialItemNames.AgedBrie, Quality = 49, SellIn = 10 }
        };

        // Act
        _ruleEngine.Process(items);

        // Assert
        Assert.Equal(50, items[0].Quality);
    }

    #endregion

    #region Backstage Passes Tests

    [Fact]
    public void BackstagePasses_IncreaseBy1WhenMoreThan10DaysLeft()
    {
        // Arrange
        var items = new List<Item>
        {
            new Item { Name = SpecialItemNames.BackstagePasses, Quality = 20, SellIn = 11 }
        };

        // Act
        _ruleEngine.Process(items);

        // Assert
        Assert.Equal(21, items[0].Quality);
    }

    [Fact]
    public void BackstagePasses_IncreaseBy2When6To10DaysLeft()
    {
        // Arrange
        var items = new List<Item>
        {
            new Item { Name = SpecialItemNames.BackstagePasses, Quality = 20, SellIn = 8 }
        };

        // Act
        _ruleEngine.Process(items);

        // Assert
        Assert.Equal(22, items[0].Quality);
    }

    [Fact]
    public void BackstagePasses_IncreaseBy3When1To5DaysLeft()
    {
        // Arrange
        var items = new List<Item>
        {
            new Item { Name = SpecialItemNames.BackstagePasses, Quality = 20, SellIn = 3 }
        };

        // Act
        _ruleEngine.Process(items);

        // Assert
        Assert.Equal(23, items[0].Quality);
    }

    [Fact]
    public void BackstagePasses_DropToZeroAfterConcert()
    {
        // Arrange
        var items = new List<Item>
        {
            new Item { Name = SpecialItemNames.BackstagePasses, Quality = 45, SellIn = -1 }
        };

        // Act
        _ruleEngine.Process(items);

        // Assert
        Assert.Equal(0, items[0].Quality);
    }

    [Fact]
    public void BackstagePasses_DropToZeroAtSellDate()
    {
        // Arrange
        var items = new List<Item>
        {
            new Item { Name = SpecialItemNames.BackstagePasses, Quality = 45, SellIn = 0 }
        };

        // Act
        _ruleEngine.Process(items);

        // Assert
        Assert.Equal(0, items[0].Quality);
    }

    [Fact]
    public void BackstagePasses_ExactlyTenDaysIncreaseBy2()
    {
        // Arrange
        var items = new List<Item>
        {
            new Item { Name = SpecialItemNames.BackstagePasses, Quality = 20, SellIn = 10 }
        };

        // Act
        _ruleEngine.Process(items);

        // Assert
        Assert.Equal(22, items[0].Quality);
    }

    [Fact]
    public void BackstagePasses_ExactlySixDaysIncreaseBy2()
    {
        // Arrange
        var items = new List<Item>
        {
            new Item { Name = SpecialItemNames.BackstagePasses, Quality = 20, SellIn = 6 }
        };

        // Act
        _ruleEngine.Process(items);

        // Assert
        Assert.Equal(22, items[0].Quality);
    }

    [Fact]
    public void BackstagePasses_ExactlyFiveDaysIncreaseBy3()
    {
        // Arrange
        var items = new List<Item>
        {
            new Item { Name = SpecialItemNames.BackstagePasses, Quality = 20, SellIn = 5 }
        };

        // Act
        _ruleEngine.Process(items);

        // Assert
        Assert.Equal(23, items[0].Quality);
    }

    [Fact]
    public void BackstagePasses_QualityNotExceedsMaxAt1Day()
    {
        // Arrange
        var items = new List<Item>
        {
            new Item { Name = SpecialItemNames.BackstagePasses, Quality = 48, SellIn = 1 }
        };

        // Act
        _ruleEngine.Process(items);

        // Assert
        Assert.Equal(50, items[0].Quality);
    }

    [Fact]
    public void BackstagePasses_SellInDecreases()
    {
        // Arrange
        var items = new List<Item>
        {
            new Item { Name = SpecialItemNames.BackstagePasses, Quality = 20, SellIn = 5 }
        };

        // Act
        _ruleEngine.Process(items);

        // Assert
        Assert.Equal(4, items[0].SellIn);
    }

    #endregion

    #region Sulfuras Tests

    [Fact]
    public void Sulfuras_QualityNeverChanges()
    {
        // Arrange
        var items = new List<Item>
        {
            new Item { Name = SpecialItemNames.Sulfuras, Quality = 80, SellIn = 10 }
        };

        // Act
        _ruleEngine.Process(items);

        // Assert
        Assert.Equal(80, items[0].Quality);
    }

    [Fact]
    public void Sulfuras_SellInNeverChanges()
    {
        // Arrange
        var items = new List<Item>
        {
            new Item { Name = SpecialItemNames.Sulfuras, Quality = 80, SellIn = 10 }
        };

        // Act
        _ruleEngine.Process(items);

        // Assert
        Assert.Equal(10, items[0].SellIn);
    }

    [Fact]
    public void Sulfuras_RemainsUnchangedAfterSellDate()
    {
        // Arrange
        var items = new List<Item>
        {
            new Item { Name = SpecialItemNames.Sulfuras, Quality = 80, SellIn = -5 }
        };

        // Act
        _ruleEngine.Process(items);

        // Assert
        Assert.Equal(80, items[0].Quality);
        Assert.Equal(-5, items[0].SellIn);
    }

    #endregion

    #region Conjured Items Tests

    [Fact]
    public void ConjuredManaCake_DecreaseBy2BeforeSellDate()
    {
        // Arrange
        var items = new List<Item>
        {
            new Item { Name = SpecialItemNames.Conjured, Quality = 10, SellIn = 1 }
        };

        // Act
        _ruleEngine.Process(items);

        // Assert
        Assert.Equal(8, items[0].Quality);
    }

    [Fact]
    public void ConjuredManaCake_DecreaseBy4AfterSellDate()
    {
        // Arrange
        var items = new List<Item>
        {
            new Item { Name = SpecialItemNames.Conjured, Quality = 10, SellIn = -1 }
        };

        // Act
        _ruleEngine.Process(items);

        // Assert
        Assert.Equal(6, items[0].Quality);
    }

    [Fact]
    public void ConjuredManaCake_DecreaseBy4AtSellDate()
    {
        // Arrange
        var items = new List<Item>
        {
            new Item { Name = SpecialItemNames.Conjured, Quality = 10, SellIn = 0 }
        };

        // Act
        _ruleEngine.Process(items);

        // Assert
        Assert.Equal(6, items[0].Quality);
    }

    [Fact]
    public void ConjuredManaCake_QualityNotBelowZeroBeforeSellDate()
    {
        // Arrange
        var items = new List<Item>
        {
            new Item { Name = SpecialItemNames.Conjured, Quality = 1, SellIn = 5 }
        };

        // Act
        _ruleEngine.Process(items);

        // Assert
        Assert.Equal(0, items[0].Quality);
    }

    [Fact]
    public void ConjuredManaCake_QualityNotBelowZeroAfterSellDate()
    {
        // Arrange
        var items = new List<Item>
        {
            new Item { Name = SpecialItemNames.Conjured, Quality = 2, SellIn = -1 }
        };

        // Act
        _ruleEngine.Process(items);

        // Assert
        Assert.Equal(0, items[0].Quality);
    }

    [Fact]
    public void ConjuredManaCake_SellInDecreases()
    {
        // Arrange
        var items = new List<Item>
        {
            new Item { Name = SpecialItemNames.Conjured, Quality = 10, SellIn = 5 }
        };

        // Act
        _ruleEngine.Process(items);

        // Assert
        Assert.Equal(4, items[0].SellIn);
    }

    #endregion

    #region Regular Items Tests

    [Fact]
    public void RegularItem_DecreaseQualityBy1BeforeSellDate()
    {
        // Arrange
        var items = new List<Item>
        {
            new Item { Name = "+5 Dexterity Vest", Quality = 20, SellIn = 10 }
        };

        // Act
        _ruleEngine.Process(items);

        // Assert
        Assert.Equal(19, items[0].Quality);
    }

    [Fact]
    public void RegularItem_DecreaseQualityBy2AfterSellDate()
    {
        // Arrange
        var items = new List<Item>
        {
            new Item { Name = "Elixir of the Mongoose", Quality = 20, SellIn = -1 }
        };

        // Act
        _ruleEngine.Process(items);

        // Assert
        Assert.Equal(18, items[0].Quality);
    }

    [Fact]
    public void RegularItem_DecreaseQualityBy2AtSellDate()
    {
        // Arrange
        var items = new List<Item>
        {
            new Item { Name = "Elixir of the Mongoose", Quality = 20, SellIn = 0 }
        };

        // Act
        _ruleEngine.Process(items);

        // Assert
        Assert.Equal(18, items[0].Quality);
    }

    [Fact]
    public void RegularItem_QualityNotBelowZeroBeforeSellDate()
    {
        // Arrange
        var items = new List<Item>
        {
            new Item { Name = "+5 Dexterity Vest", Quality = 0, SellIn = 5 }
        };

        // Act
        _ruleEngine.Process(items);

        // Assert
        Assert.Equal(0, items[0].Quality);
    }

    [Fact]
    public void RegularItem_QualityNotBelowZeroAfterSellDate()
    {
        // Arrange
        var items = new List<Item>
        {
            new Item { Name = "Elixir of the Mongoose", Quality = 1, SellIn = -1 }
        };

        // Act
        _ruleEngine.Process(items);

        // Assert
        Assert.Equal(0, items[0].Quality);
    }

    [Fact]
    public void RegularItem_SellInDecreases()
    {
        // Arrange
        var items = new List<Item>
        {
            new Item { Name = "+5 Dexterity Vest", Quality = 20, SellIn = 10 }
        };

        // Act
        _ruleEngine.Process(items);

        // Assert
        Assert.Equal(9, items[0].SellIn);
    }

    #endregion

    #region Edge Cases Tests

    [Fact]
    public void MultipleItems_AllTypesProcessedCorrectly()
    {
        // Arrange
        var items = new List<Item>
        {
            new Item { Name = SpecialItemNames.AgedBrie, Quality = 21, SellIn = 5 },
            new Item { Name = SpecialItemNames.BackstagePasses, Quality = 20, SellIn = 10 },
            new Item { Name = SpecialItemNames.Sulfuras, Quality = 80, SellIn = 0 },
            new Item { Name = SpecialItemNames.Conjured, Quality = 10, SellIn = 3 },
            new Item { Name = "+5 Dexterity Vest", Quality = 20, SellIn = 10 }
        };

        // Act
        _ruleEngine.Process(items);

        // Assert
        Assert.Equal(22, items[0].Quality);
        Assert.Equal(4, items[0].SellIn);

        Assert.Equal(22, items[1].Quality);
        Assert.Equal(9, items[1].SellIn);

        Assert.Equal(80, items[2].Quality);
        Assert.Equal(0, items[2].SellIn);

        Assert.Equal(8, items[3].Quality);
        Assert.Equal(2, items[3].SellIn);

        Assert.Equal(19, items[4].Quality);
        Assert.Equal(9, items[4].SellIn);
    }

    [Fact]
    public void AgedBrie_AtZeroQualityIncreases()
    {
        // Arrange
        var items = new List<Item>
        {
            new Item { Name = SpecialItemNames.AgedBrie, Quality = 0, SellIn = 5 }
        };

        // Act
        _ruleEngine.Process(items);

        // Assert
        Assert.Equal(1, items[0].Quality);
    }

    [Fact]
    public void BackstagePasses_At49QualityIncreaseRespects50Cap()
    {
        // Arrange
        var items = new List<Item>
        {
            new Item { Name = SpecialItemNames.BackstagePasses, Quality = 49, SellIn = 2 }
        };

        // Act
        _ruleEngine.Process(items);

        // Assert
        Assert.Equal(50, items[0].Quality);
    }

    [Fact]
    public void ConjuredManaCake_ZeroQualityRemains()
    {
        // Arrange
        var items = new List<Item>
        {
            new Item { Name = SpecialItemNames.Conjured, Quality = 0, SellIn = 5 }
        };

        // Act
        _ruleEngine.Process(items);

        // Assert
        Assert.Equal(0, items[0].Quality);
    }

    [Fact]
    public void RegularItem_ZeroQualityRemains()
    {
        // Arrange
        var items = new List<Item>
        {
            new Item { Name = "+5 Dexterity Vest", Quality = 0, SellIn = -5 }
        };

        // Act
        _ruleEngine.Process(items);

        // Assert
        Assert.Equal(0, items[0].Quality);
    }

    [Fact]
    public void BackstagePasses_ExactlyAtZeroSellInQualityDropsTo0()
    {
        // Arrange
        var items = new List<Item>
        {
            new Item { Name = SpecialItemNames.BackstagePasses, Quality = 40, SellIn = 0 }
        };

        // Act
        _ruleEngine.Process(items);

        // Assert
        Assert.Equal(0, items[0].Quality);
    }

    [Fact]
    public void Sulfuras_WithNegativeSellInRemainsUnchanged()
    {
        // Arrange
        var items = new List<Item>
        {
            new Item { Name = SpecialItemNames.Sulfuras, Quality = 80, SellIn = -10 }
        };

        // Act
        _ruleEngine.Process(items);

        // Assert
        Assert.Equal(80, items[0].Quality);
        Assert.Equal(-10, items[0].SellIn);
    }

    #endregion

    #region Boundary Tests

    [Fact]
    public void AgedBrie_MaxQuality50Boundary()
    {
        // Arrange
        var items = new List<Item>
        {
            new Item { Name = SpecialItemNames.AgedBrie, Quality = 49, SellIn = 5 }
        };

        // Act
        _ruleEngine.Process(items);

        // Assert
        Assert.Equal(50, items[0].Quality);
    }

    [Fact]
    public void ConjuredManaCake_MinQuality0Boundary()
    {
        // Arrange
        var items = new List<Item>
        {
            new Item { Name = SpecialItemNames.Conjured, Quality = 3, SellIn = -1 }
        };

        // Act
        _ruleEngine.Process(items);

        // Assert
        Assert.Equal(0, items[0].Quality);
    }

    [Fact]
    public void BackstagePasses_TransitionFrom11DaysTo10Days()
    {
        // Arrange - At 11 days, increase by 1
        var items1 = new List<Item>
        {
            new Item { Name = SpecialItemNames.BackstagePasses, Quality = 20, SellIn = 11 }
        };

        // Act
        _ruleEngine.Process(items1);

        // Assert
        Assert.Equal(21, items1[0].Quality);
    }

    [Fact]
    public void BackstagePasses_TransitionFrom5DaysTo4Days()
    {
        // Arrange - At 5 days, increase by 3
        var items1 = new List<Item>
        {
            new Item { Name = SpecialItemNames.BackstagePasses, Quality = 20, SellIn = 5 }
        };

        // Act
        _ruleEngine.Process(items1);

        // Assert
        Assert.Equal(23, items1[0].Quality);
    }

    [Fact]
    public void BackstagePasses_TransitionFrom1DayTo0Days()
    {
        // Arrange - At 1 day, increase by 3; at 0 days, drop to 0
        var items1 = new List<Item>
        {
            new Item { Name = SpecialItemNames.BackstagePasses, Quality = 20, SellIn = 1 }
        };

        // Act
        _ruleEngine.Process(items1);

        // Assert - Quality increases by 3, SellIn decreases to 0
        Assert.Equal(23, items1[0].Quality);
        Assert.Equal(0, items1[0].SellIn);
    }

    #endregion

    #region Rule Priority Tests

    [Fact]
    public void RuleOrder_AllRulesApplyCorrectlyForAgedBrie()
    {
        // Arrange - Aged Brie should only match AgedBrieProcessing and DecreaseSellInAllExceptSulfuras
        var items = new List<Item>
        {
            new Item { Name = SpecialItemNames.AgedBrie, Quality = 25, SellIn = 10 }
        };

        // Act
        _ruleEngine.Process(items);

        // Assert - Quality increases by 1, SellIn decreases by 1
        Assert.Equal(26, items[0].Quality);
        Assert.Equal(9, items[0].SellIn);
    }

    [Fact]
    public void RuleOrder_AllRulesApplyCorrectlyForBackstagePasses()
    {
        // Arrange - Backstage passes at exactly 8 days
        var items = new List<Item>
        {
            new Item { Name = SpecialItemNames.BackstagePasses, Quality = 30, SellIn = 8 }
        };

        // Act
        _ruleEngine.Process(items);

        // Assert - Quality increases by 2 (6-10 days rule), SellIn decreases by 1
        Assert.Equal(32, items[0].Quality);
        Assert.Equal(7, items[0].SellIn);
    }

    [Fact]
    public void RuleOrder_AllRulesApplyCorrectlyForConjured()
    {
        // Arrange - Conjured item before sell date
        var items = new List<Item>
        {
            new Item { Name = SpecialItemNames.Conjured, Quality = 12, SellIn = 3 }
        };

        // Act
        _ruleEngine.Process(items);

        // Assert - Quality decreases by 2 (before sell date), SellIn decreases by 1
        Assert.Equal(10, items[0].Quality);
        Assert.Equal(2, items[0].SellIn);
    }

    [Fact]
    public void RuleOrder_AllRulesApplyCorrectlyForRegularItem()
    {
        // Arrange
        var items = new List<Item>
        {
            new Item { Name = "Normal Item", Quality = 25, SellIn = 5 }
        };

        // Act
        _ruleEngine.Process(items);

        // Assert - Quality decreases by 1 (before sell date), SellIn decreases by 1
        Assert.Equal(24, items[0].Quality);
        Assert.Equal(4, items[0].SellIn);
    }

    #endregion
}
