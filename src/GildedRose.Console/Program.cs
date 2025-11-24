using GildedRose.Console.Rules;
using GildedRose.Console.Rules.Actions;
using GildedRose.Console.Rules.Conditions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GildedRose.Console;

public class Program
{
    public static IList<Item> Items = new List<Item>();

    static void Main(string[] args)
    {
        System.Console.WriteLine("OMGHAI!");

        Items = new List<Item>
                                      {
                                          new Item {Name = "+5 Dexterity Vest", SellIn = 10, Quality = 20},
                                          new Item {Name = "Aged Brie", SellIn = 2, Quality = 0},
                                          new Item {Name = "Elixir of the Mongoose", SellIn = 5, Quality = 7},
                                          new Item {Name = "Sulfuras, Hand of Ragnaros", SellIn = 0, Quality = 80},
                                          new Item
                                              {
                                                  Name = "Backstage passes to a TAFKAL80ETC concert",
                                                  SellIn = 15,
                                                  Quality = 20
                                              },
                                          new Item {Name = "Conjured Mana Cake", SellIn = 3, Quality = 6}
                                      };

        var host = Host.CreateDefaultBuilder()
           .ConfigureServices((context, services) =>
           {
               // Register the Rule Engine
               services.AddTransient<RuleEngine>();

               // Register Rules
               // Rule A: Sulfuras never has to be sold or decreases in Quality
               services.AddSingleton<IRule>(sp => new ConfigurableRule(
                   "SulfurasProcessing",
                   new NameContainsCondition(SpecialItemNames.Sulfuras),
                   new DoNothingAction()
               ));

               // Rule B: Backstage passes increases in Quality by 2 when there are 10 days or less to sell date
               services.AddSingleton<IRule>(sp => new ConfigurableRule(
                   "BackstagePassesProcessingLessThan10Days",
                   new AndCondition(
                        new NameContainsCondition(SpecialItemNames.BackstagePasses),
                        new RemainedLessThanNDaysToSellDateCondition(11),
                        new RemainedMoreThanNDaysToSellDateCondition(5)
                    ),
                   new IncreaseQualityAction(2, 50)
               ));

               // Rule C: Backstage passes increases in Quality by 3 when there are 5 days or less to sell date
               services.AddSingleton<IRule>(sp => new ConfigurableRule(
                   "BackstagePassesProcessingLessThan5Days",
                   new AndCondition(
                        new NameContainsCondition(SpecialItemNames.BackstagePasses),
                        new RemainedLessThanNDaysToSellDateCondition(6),
                        new RemainedMoreThanNDaysToSellDateCondition(0)
                    ),
                   new IncreaseQualityAction(3, 50)
               ));


               // Rule D: Backstage passes increases in Quality by 1 when there are more than 10 days to sell date
               services.AddSingleton<IRule>(sp => new ConfigurableRule(
                   "BackstagePassesProcessingMoreThan10Days",
                   new AndCondition(
                        new NameContainsCondition(SpecialItemNames.BackstagePasses),
                        new RemainedMoreThanNDaysToSellDateCondition(10)
                    ),
                   new IncreaseQualityAction(1, 50)
               ));
               // Rule E: Decrease Quality by 1 before Sell Date for all except for Sulfuras,Aged Brie, Backstage Passes
               services.AddSingleton<IRule>(sp => new ConfigurableRule(
                   "DecreaseQualityBasicBeforeSellDate",
                   new AndCondition(
                        new NameMatchesRegexCondition("^(?!.*Sulfuras)(?!.*Brie)(?!.*Backstage)(?!.*Conjured).*$"),
                        new RemainedMoreThanNDaysToSellDateCondition(0)
                    ),

                   new DegradeQualityAction(1, 0)
               ));
               // Rule F: Decrease Quality by 2 after Sell Date for all except for Sulfuras,Aged Brie, Backstage Passes
               services.AddSingleton<IRule>(sp => new ConfigurableRule(
                   "DecreaseQualityBasicAfterSellDate",
                   new AndCondition(
                        new NameMatchesRegexCondition("^(?!.*Sulfuras)(?!.*Brie)(?!.*Backstage)(?!.*Conjured).*$"),
                        new RemainedLessThanNDaysToSellDateCondition(1)
                    ),

                   new DegradeQualityAction(2, 0)
               ));

               // Rule G:
               services.AddSingleton<IRule>(sp => new ConfigurableRule(
                   "ConjuredProcessingBeforeSellDate",
                   new AndCondition(
                       new NameContainsCondition(SpecialItemNames.Conjured),
                       new RemainedMoreThanNDaysToSellDateCondition(0)
                   ),
                   new DegradeQualityAction(2, 0)
               ));
               // Rule H:
               services.AddSingleton<IRule>(sp => new ConfigurableRule(
                   "ConjuredProcessingAfterSellDate",
                   new AndCondition(
                       new NameContainsCondition(SpecialItemNames.Conjured),
                       new RemainedLessThanNDaysToSellDateCondition(1)
                   ),
                   new DegradeQualityAction(4, 0)    //Normal items decrease with speed 2 after Sell date => Conjured twice as fast (4)
               ));

               // Rule I: Decrease SellIn by 1 except for Sulfuras
               services.AddSingleton<IRule>(sp => new ConfigurableRule(
                   "DecreaseSellInAllExceptSulfuras",
                   new NotCondition(
                        new NameContainsCondition(SpecialItemNames.Sulfuras)
                    ),
                   new DecreaseSellInAction(1)
               ));

               // Rule J: Aged Brie
               services.AddSingleton<IRule>(sp => new ConfigurableRule(
                   "AgedBrieProcessing",
                   new NameContainsCondition(SpecialItemNames.AgedBrie),
                   new IncreaseQualityAction(1, 50)
               ));

               // Rule K: Backstage passes: Quality drops to 0 after the concert
               services.AddSingleton<IRule>(sp => new ConfigurableRule(
                   "BackstagePassesProcessingAfterConcert",
                   new AndCondition(
                        new NameContainsCondition(SpecialItemNames.BackstagePasses),
                        new RemainedLessThanNDaysToSellDateCondition(0)
                    ),
                   new SetQualityToZeroAction()
               ));
           })
           .Build();

        var engine = host.Services.GetRequiredService<RuleEngine>();
        UpdateQuality(engine);

        System.Console.ReadKey();
    }

    public static void UpdateQuality(RuleEngine ruleEngine)
    {
        ruleEngine.Process(Items);
    }
}

public class Item
{
    public string Name { get; set; } = "";

    public int SellIn { get; set; }

    public int Quality { get; set; }
}
