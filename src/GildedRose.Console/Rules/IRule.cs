namespace GildedRose.Console.Rules
{
    public interface IRule
    {
        string Name { get; }
        bool Matches(Item item);
        void Apply(Item item);
    }
}
