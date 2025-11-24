namespace GildedRose.Console.Rules;

public class RuleEngine
{
    private readonly IEnumerable<IRule> _rules;

    // DI injects all special rules and the basic/default fallback actions
    public RuleEngine(IEnumerable<IRule> rules)
    {
        _rules = rules;
    }

    public void Process(IEnumerable<Item> items)
    {
        foreach (var item in items)
        {
            //TODO: inject logger and use it here
            //System.Console.WriteLine($"Processing: {item.Name}   - {item.Quality}   -- {item.SellIn}");

            foreach (var rule in _rules)
            {
                if (rule.Matches(item))
                {
                    rule.Apply(item);
                }
            }
            //System.Console.WriteLine($"AFTER Processing: {item.Name}   - {item.Quality}   -- {item.SellIn}");            

            //System.Console.WriteLine("------------------------------------------------");
        }
    }
}
