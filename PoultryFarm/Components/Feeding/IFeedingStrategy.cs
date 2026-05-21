namespace PoultryFarm.Components;

public interface IFeedingStrategy 
{ 
    string GetName(); 
    bool IsApplicable(double ageInDays); 
}