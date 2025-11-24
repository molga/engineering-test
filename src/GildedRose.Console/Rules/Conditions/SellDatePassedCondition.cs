namespace GildedRose.Console.Rules.Conditions
{
    public class SellDatePassedCondition : ICondition
    {
        public bool IsSatisfiedBy(Item item)
           => item.SellIn < 0;
    }
}
