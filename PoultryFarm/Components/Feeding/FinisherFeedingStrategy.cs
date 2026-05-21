namespace PoultryFarm.Components;

public class FinisherFeedingStrategy : IFeedingStrategy
{
    public string GetName() => "Фінішний (21+ дн)";
    public bool IsApplicable(double ageInDays) => ageInDays > 20;
}