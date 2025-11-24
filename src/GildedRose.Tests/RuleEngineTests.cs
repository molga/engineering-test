using Xunit;
using Moq;
using GildedRose.Console.Rules;
using GildedRose.Console.Rules.Conditions;
using GildedRose.Console.Rules.Actions;
using GildedRose.Console;

namespace GildedRose.Tests;

public class RuleEngineTests
{
    [Fact]
    public void RuleEngine_ProcessesEmptyItemList()
    {
        // Arrange
        var rules = new List<IRule>();
        var ruleEngine = new RuleEngine(rules);
        var items = new List<Item>();

        // Act
        ruleEngine.Process(items);

        // Assert - no exception thrown
        Assert.Empty(items);
    }

    [Fact]
    public void RuleEngine_ProcessesSingleItemWithNoMatchingRules()
    {
        // Arrange
        var item = new Item { Name = "Test Item", Quality = 10, SellIn = 5 };
        var items = new List<Item> { item };
        
        var mockRule = new Mock<IRule>();
        mockRule.Setup(r => r.Matches(It.IsAny<Item>())).Returns(false);
        var rules = new List<IRule> { mockRule.Object };
        
        var ruleEngine = new RuleEngine(rules);

        // Act
        ruleEngine.Process(items);

        // Assert
        Assert.Equal(10, item.Quality);
        Assert.Equal(5, item.SellIn);
        mockRule.Verify(r => r.Matches(item), Times.Once);
        mockRule.Verify(r => r.Apply(It.IsAny<Item>()), Times.Never);
    }

    [Fact]
    public void RuleEngine_AppliesRuleWhenConditionMatches()
    {
        // Arrange
        var item = new Item { Name = "Aged Brie", Quality = 10, SellIn = 5 };
        var items = new List<Item> { item };
        
        var mockRule = new Mock<IRule>();
        mockRule.Setup(r => r.Matches(It.IsAny<Item>())).Returns(true);
        var rules = new List<IRule> { mockRule.Object };
        
        var ruleEngine = new RuleEngine(rules);

        // Act
        ruleEngine.Process(items);

        // Assert
        mockRule.Verify(r => r.Matches(item), Times.Once);
        mockRule.Verify(r => r.Apply(item), Times.Once);
    }

    [Fact]
    public void RuleEngine_ProcessesMultipleItemsSequentially()
    {
        // Arrange
        var item1 = new Item { Name = "Item 1", Quality = 10, SellIn = 5 };
        var item2 = new Item { Name = "Item 2", Quality = 20, SellIn = 10 };
        var items = new List<Item> { item1, item2 };
        
        var mockRule = new Mock<IRule>();
        mockRule.Setup(r => r.Matches(It.IsAny<Item>())).Returns(true);
        var rules = new List<IRule> { mockRule.Object };
        
        var ruleEngine = new RuleEngine(rules);

        // Act
        ruleEngine.Process(items);

        // Assert
        mockRule.Verify(r => r.Matches(item1), Times.Once);
        mockRule.Verify(r => r.Matches(item2), Times.Once);
        mockRule.Verify(r => r.Apply(item1), Times.Once);
        mockRule.Verify(r => r.Apply(item2), Times.Once);
    }

    [Fact]
    public void RuleEngine_AppliesMultipleRulesToSameItem()
    {
        // Arrange
        var item = new Item { Name = "Aged Brie", Quality = 10, SellIn = 5 };
        var items = new List<Item> { item };
        
        var mockRule1 = new Mock<IRule>();
        mockRule1.Setup(r => r.Matches(It.IsAny<Item>())).Returns(true);
        
        var mockRule2 = new Mock<IRule>();
        mockRule2.Setup(r => r.Matches(It.IsAny<Item>())).Returns(true);
        
        var rules = new List<IRule> { mockRule1.Object, mockRule2.Object };
        var ruleEngine = new RuleEngine(rules);

        // Act
        ruleEngine.Process(items);

        // Assert
        mockRule1.Verify(r => r.Matches(item), Times.Once);
        mockRule1.Verify(r => r.Apply(item), Times.Once);
        mockRule2.Verify(r => r.Matches(item), Times.Once);
        mockRule2.Verify(r => r.Apply(item), Times.Once);
    }

    [Fact]
    public void RuleEngine_AppliesOnlyMatchingRulesFromMultipleRules()
    {
        // Arrange
        var item = new Item { Name = "Aged Brie", Quality = 10, SellIn = 5 };
        var items = new List<Item> { item };
        
        var matchingRule = new Mock<IRule>();
        matchingRule.Setup(r => r.Matches(It.IsAny<Item>())).Returns(true);
        
        var nonMatchingRule = new Mock<IRule>();
        nonMatchingRule.Setup(r => r.Matches(It.IsAny<Item>())).Returns(false);
        
        var rules = new List<IRule> { matchingRule.Object, nonMatchingRule.Object };
        var ruleEngine = new RuleEngine(rules);

        // Act
        ruleEngine.Process(items);

        // Assert
        matchingRule.Verify(r => r.Apply(item), Times.Once);
        nonMatchingRule.Verify(r => r.Apply(It.IsAny<Item>()), Times.Never);
    }

    [Fact]
    public void RuleEngine_WithConfigurableRule_AppliesActionWhenConditionMatches()
    {
        // Arrange
        var item = new Item { Name = "Aged Brie", Quality = 10, SellIn = 5 };
        var items = new List<Item> { item };
        
        var condition = new NameContainsCondition("Brie");
        var action = new IncreaseQualityAction(increment: 1, maxValue: 50);
        var rule = new ConfigurableRule("Brie Rule", condition, action);
        var rules = new List<IRule> { rule };
        
        var ruleEngine = new RuleEngine(rules);

        // Act
        ruleEngine.Process(items);

        // Assert
        Assert.Equal(11, item.Quality);
    }

    [Fact]
    public void RuleEngine_WithConfigurableRule_DoesNotApplyActionWhenConditionDoesNotMatch()
    {
        // Arrange
        var item = new Item { Name = "Regular Item", Quality = 10, SellIn = 5 };
        var items = new List<Item> { item };
        
        var condition = new NameContainsCondition("Brie");
        var action = new IncreaseQualityAction(increment: 1, maxValue: 50);
        var rule = new ConfigurableRule("Brie Rule", condition, action);
        var rules = new List<IRule> { rule };
        
        var ruleEngine = new RuleEngine(rules);

        // Act
        ruleEngine.Process(items);

        // Assert
        Assert.Equal(10, item.Quality);
    }

    [Fact]
    public void RuleEngine_AppliesRulesInOrder()
    {
        // Arrange
        var item = new Item { Name = "Test Item", Quality = 10, SellIn = 5 };
        var items = new List<Item> { item };
        
        var callOrder = new List<string>();
        
        var rule1 = new Mock<IRule>();
        rule1.Setup(r => r.Matches(It.IsAny<Item>())).Returns(true);
        rule1.Setup(r => r.Apply(It.IsAny<Item>())).Callback(() => callOrder.Add("Rule1"));
        
        var rule2 = new Mock<IRule>();
        rule2.Setup(r => r.Matches(It.IsAny<Item>())).Returns(true);
        rule2.Setup(r => r.Apply(It.IsAny<Item>())).Callback(() => callOrder.Add("Rule2"));
        
        var rules = new List<IRule> { rule1.Object, rule2.Object };
        var ruleEngine = new RuleEngine(rules);

        // Act
        ruleEngine.Process(items);

        // Assert
        Assert.Equal(new[] { "Rule1", "Rule2" }, callOrder);
    }

    [Fact]
    public void RuleEngine_WithMultipleConditionsAndActions_ExecutesCorrectly()
    {
        // Arrange
        var item = new Item { Name = "Aged Brie", Quality = 10, SellIn = 3 };
        var items = new List<Item> { item };
        
        var condition1 = new NameContainsCondition("Brie");
        var action1 = new IncreaseQualityAction(increment: 2, maxValue: 50);
        var rule1 = new ConfigurableRule("Brie Increase", condition1, action1);
        
        var condition2 = new RemainedLessThanNDaysToSellDateCondition(numberOfDays: 5);
        var action2 = new IncreaseQualityAction(increment: 1, maxValue: 50);
        var rule2 = new ConfigurableRule("Urgent Increase", condition2, action2);
        
        var rules = new List<IRule> { rule1, rule2 };
        var ruleEngine = new RuleEngine(rules);

        // Act
        ruleEngine.Process(items);

        // Assert
        Assert.Equal(13, item.Quality); // 10 + 2 (from rule1) + 1 (from rule2)
    }

    [Fact]
    public void RuleEngine_WithNegatedCondition_AppliesRuleCorrectly()
    {
        // Arrange
        var item = new Item { Name = "Sulfuras", Quality = 80, SellIn = 5 };
        var items = new List<Item> { item };
        
        var baseCondition = new NameContainsCondition("Brie");
        var notCondition = new NotCondition(baseCondition);
        var action = new DegradeQualityAction(decrement: 1, minValue: 0);
        var rule = new ConfigurableRule("Degrade Non-Brie", notCondition, action);
        
        var rules = new List<IRule> { rule };
        var ruleEngine = new RuleEngine(rules);

        // Act
        ruleEngine.Process(items);

        // Assert
        Assert.Equal(79, item.Quality);
    }

    [Fact]
    public void RuleEngine_WithAndCondition_AppliesRuleOnlyWhenAllConditionsMet()
    {
        // Arrange
        var item = new Item { Name = "Aged Brie", Quality = 10, SellIn = 3 };
        var items = new List<Item> { item };
        
        var condition1 = new NameContainsCondition("Brie");
        var condition2 = new RemainedLessThanNDaysToSellDateCondition(numberOfDays: 5);
        var andCondition = new AndCondition(condition1, condition2);
        var action = new IncreaseQualityAction(increment: 3, maxValue: 50);
        var rule = new ConfigurableRule("Urgent Brie", andCondition, action);
        
        var rules = new List<IRule> { rule };
        var ruleEngine = new RuleEngine(rules);

        // Act
        ruleEngine.Process(items);

        // Assert
        Assert.Equal(13, item.Quality);
    }

    [Fact]
    public void RuleEngine_WithComplexRuleSetSimulation()
    {
        // Arrange - Simulating the Gilded Rose business rules
        var items = new List<Item>
        {
            new Item { Name = "Aged Brie", Quality = 20, SellIn = 2 },
            new Item { Name = "Regular Item", Quality = 10, SellIn = 5 },
            new Item { Name = "Sulfuras, Hand of Ragnaros", Quality = 80, SellIn = 0 }
        };
        
        var agedBrieCondition = new NameContainsCondition("Aged Brie");
        var agedBrieRule = new ConfigurableRule("Aged Brie Quality", agedBrieCondition, new IncreaseQualityAction(increment: 1, maxValue: 50));
        
        var sulfurasCondition = new NameContainsCondition("Sulfuras");
        var sulfurasRule = new ConfigurableRule("Sulfuras No Change", sulfurasCondition, new DoNothingAction());
        
        var degradeRule = new ConfigurableRule("Default Degrade", new AndCondition(
                        new NameMatchesRegexCondition("^(?!.*Sulfuras)(?!.*Brie)(?!.*Backstage)(?!.*Conjured).*$"),
                        new RemainedMoreThanNDaysToSellDateCondition(0)
                    ), new DegradeQualityAction(decrement: 1, minValue: 0));
        
        var rules = new List<IRule> { agedBrieRule, sulfurasRule, degradeRule };
        var ruleEngine = new RuleEngine(rules);

        // Act
        ruleEngine.Process(items);

        // Assert
        Assert.Equal(21, items[0].Quality); // Aged Brie increased
        Assert.Equal(9, items[1].Quality);  // Regular item degraded
        Assert.Equal(80, items[2].Quality); // Sulfuras unchanged
    }
}
