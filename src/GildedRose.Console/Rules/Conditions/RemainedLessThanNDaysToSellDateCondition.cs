namespace GildedRose.Console.Rules.Conditions; 

public class RemainedLessThanNDaysToSellDateCondition : ICondition
{
    private readonly int _numberOfDays;
    public RemainedLessThanNDaysToSellDateCondition(int numberOfDays) => _numberOfDays = numberOfDays;        
    public bool IsSatisfiedBy(Item item)
    {
        return item.SellIn < _numberOfDays;
    }
}
