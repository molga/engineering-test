namespace GildedRose.Console.Rules.Conditions
{
    public class NameContainsCondition : ICondition
    {

        private readonly string _substring;

        public NameContainsCondition(string substring) => _substring = substring;


        public bool IsSatisfiedBy(Item item)
            => item.Name.IndexOf(_substring, StringComparison.OrdinalIgnoreCase) >= 0;
    }
}
