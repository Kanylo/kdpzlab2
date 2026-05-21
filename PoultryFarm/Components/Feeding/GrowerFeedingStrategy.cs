namespace PoultryFarm.Components;

public class GrowerFeedingStrategy : IFeedingStrategy
{
    public string GetName() => "Ростовий (11-20 дн)";
    public bool IsApplicable(double ageInDays) => ageInDays > 10 && ageInDays <= 20;
}