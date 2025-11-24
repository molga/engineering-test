namespace GildedRose.Console.Rules.Actions
{
    public class DegradeQualityAction : IAction
    {
        private readonly int _decrement;
        private readonly int _minValue;

        public DegradeQualityAction(int decrement, int minValue)
        {
            _decrement = decrement;
            _minValue = minValue;
        }
        public void Execute(Item item)
        {
            item.Quality -= _decrement;
            if (item.Quality < _minValue)
            {
                item.Quality = _minValue;
            }
        }
    }
}
