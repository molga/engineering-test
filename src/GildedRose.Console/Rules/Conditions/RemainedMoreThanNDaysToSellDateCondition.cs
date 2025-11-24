namespace GildedRose.Console.Rules.Conditions;

public class RemainedMoreThanNDaysToSellDateCondition : ICondition
{
    private readonly int _numberOfDays;
    public RemainedMoreThanNDaysToSellDateCondition(int numberOfDays) => _numberOfDays = numberOfDays;
    public bool IsSatisfiedBy(Item item)
    {
        return item.SellIn > _numberOfDays;
    }
}
