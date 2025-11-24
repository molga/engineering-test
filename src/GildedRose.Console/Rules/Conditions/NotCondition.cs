namespace GildedRose.Console.Rules.Conditions;

public class NotCondition : ICondition
{
    private readonly ICondition _condition;

    public NotCondition(ICondition condition)
    {
        _condition = condition;
    }

    public bool IsSatisfiedBy(Item item)
    {
        return !_condition.IsSatisfiedBy(item);
    }
}
