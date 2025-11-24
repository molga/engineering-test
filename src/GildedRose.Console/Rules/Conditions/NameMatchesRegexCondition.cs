using System.Text.RegularExpressions;

namespace GildedRose.Console.Rules.Conditions;

public class NameMatchesRegexCondition : ICondition
{
    private readonly Regex _regex;

    public NameMatchesRegexCondition(string pattern)
    {
        _regex = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
    }

    public bool IsSatisfiedBy(Item item) => _regex.IsMatch(item.Name);
}
