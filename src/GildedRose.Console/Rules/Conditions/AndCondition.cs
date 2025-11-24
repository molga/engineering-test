namespace GildedRose.Console.Rules.Conditions;

public class AndCondition : ICondition
{
    private readonly IEnumerable<ICondition> _conditions;

    public AndCondition(params ICondition[] conditions)
    {
        _conditions = conditions;
    }

    public bool IsSatisfiedBy(Item item)
        => _conditions.All(c => c.IsSatisfiedBy(item));
}
