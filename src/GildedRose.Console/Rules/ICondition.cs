namespace GildedRose.Console.Rules
{
    public interface ICondition
    {
        bool IsSatisfiedBy(Item item);
    }
}
