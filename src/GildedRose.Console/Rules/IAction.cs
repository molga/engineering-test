namespace GildedRose.Console.Rules
{
    public interface IAction
    {
        void Execute(Item item);  // keeping it synchoronous as there is no I/O in actions logic at the moment
        // In case logic changes and rules require to call database or any external API (or any similar I/O related task),
        // should consider making this as async method that returns Task instead of void
    }
}
