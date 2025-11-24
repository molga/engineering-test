namespace GildedRose.Console.Rules.Actions;

public class IncreaseQualityAction : IAction
{
    private readonly int _increment;
    private readonly int _maxValue;

    public IncreaseQualityAction(int increment, int maxValue)
    {
        _increment = increment;
        _maxValue = maxValue;
    }

    public void Execute(Item item)
    {
        item.Quality += _increment;
        if (item.Quality > _maxValue)
        {
            item.Quality = _maxValue;
        }
    }
}
