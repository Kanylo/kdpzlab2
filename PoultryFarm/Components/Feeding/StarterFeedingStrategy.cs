namespace PoultryFarm.Components;

public class StarterFeedingStrategy : IFeedingStrategy
{
    public string GetName() => "Стартовий (1-10 дн)";
    public bool IsApplicable(double ageInDays) => ageInDays <= 10;
}