namespace GildedRose.Console.Rules.Actions
{
    public class DecreaseSellInAction : IAction
    {
        private readonly int _days;
        public DecreaseSellInAction(int days) => _days = days;
        public void Execute(Item item) => item.SellIn -= _days;
    }
}
