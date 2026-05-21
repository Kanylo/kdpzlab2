using System;
using System.Collections.Generic;
using PoultryFarm.Components;

namespace PoultryFarm.Domain;

public class Zone
{
    private readonly object _syncLock = new object();

    public int Id { get; }
    public ZonePurpose Purpose { get; }
    public Flock CurrentFlock { get; }
    
    public double FoodKg { get; private set; }
    public double WaterLiters { get; private set; }

    private readonly IClimateControl _climateControl;
    private readonly IEggCollectionSystem? _eggSystem;
    public IFeedingStrategy FeedingStrategy { get; private set; }

    public Zone(int id, ZonePurpose purpose, int birdCount, IClimateControl climate, IEggCollectionSystem? eggSystem = null)
    {
        Id = id;
        Purpose = purpose;
        CurrentFlock = new Flock { Count = birdCount, AgeInDays = 1 };
        FoodKg = birdCount * 0.5;
        WaterLiters = birdCount * 1.0;
        _climateControl = climate;
        _eggSystem = eggSystem;
        UpdateFeedingStrategy();
    }

    public void Tick(double daysPassed)
    {
        lock (_syncLock)
        {
            CurrentFlock.AgeInDays += daysPassed;
            
            double foodNeeded = CurrentFlock.Count * 0.1 * daysPassed;
            double waterNeeded = CurrentFlock.Count * 0.25 * daysPassed;

            if (FoodKg >= foodNeeded && WaterLiters >= waterNeeded)
            {
                FoodKg -= foodNeeded;
                WaterLiters -= waterNeeded;
                CurrentFlock.HealthIndex = Math.Min(100, CurrentFlock.HealthIndex + 2 * daysPassed); 
            }
            else
            {
                CurrentFlock.HealthIndex = Math.Max(0, CurrentFlock.HealthIndex - 10 * daysPassed); 
            }

            UpdateFeedingStrategy();
            _climateControl.Regulate(this);
            _eggSystem?.Collect(this);
        }
    }

    public ZoneSnapshot GetSnapshot()
    {
        lock (_syncLock)
        {
            return new ZoneSnapshot(Id, Purpose, CurrentFlock.Count, CurrentFlock.AgeInDays, CurrentFlock.HealthIndex, FoodKg, WaterLiters, CurrentFlock.EggsCollected, FeedingStrategy?.GetName() ?? "Немає");
        }
    }

    public void RefillResources(double food, double water)
    {
        lock (_syncLock) { FoodKg += food; WaterLiters += water; }
    }

    private void UpdateFeedingStrategy()
    {
        var strategies = new List<IFeedingStrategy> { new StarterFeedingStrategy(), new GrowerFeedingStrategy(), new FinisherFeedingStrategy() };
        foreach (var strategy in strategies)
        {
            if (strategy.IsApplicable(CurrentFlock.AgeInDays))
            {
                FeedingStrategy = strategy;
                break;
            }
        }
    }
}