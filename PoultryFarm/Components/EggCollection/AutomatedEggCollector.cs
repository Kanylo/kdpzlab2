using PoultryFarm.Domain;

namespace PoultryFarm.Components;

public class AutomatedEggCollector : IEggCollectionSystem
{
    public void Collect(Zone zone)
    {
        if (zone.CurrentFlock.AgeInDays > 20 && zone.CurrentFlock.HealthIndex > 50)
        {
            int newEggs = (int)(zone.CurrentFlock.Count * 0.05); 
            zone.CurrentFlock.EggsCollected += newEggs;
        }
    }
}