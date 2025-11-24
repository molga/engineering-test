namespace GildedRose.Console.Rules.Actions;

public class SetQualityToZeroAction : IAction
{
    public void Execute(Item item) => item.Quality = 0;
}
