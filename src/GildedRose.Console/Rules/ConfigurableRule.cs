namespace GildedRose.Console.Rules;

public class ConfigurableRule : IRule
{
    public string Name { get; }
    private readonly ICondition _condition;
    private readonly IAction _action;

    public ConfigurableRule(string name, ICondition condition, IAction action)
    {
        Name = name;
        _condition = condition;
        _action = action;
    }

    public bool Matches(Item item) => _condition.IsSatisfiedBy(item);
    public void Apply(Item item)
    {
        //TODO: add logger
        //System.Console.WriteLine($"-> Applied Special Rule: {Name}");
        _action.Execute(item);
    }
}
