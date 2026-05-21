using System;

namespace PoultryFarm.Domain;

public class Flock
{
    public int Count { get; set; }
    public double AgeInDays { get; set; }
    public double HealthIndex { get; set; } = 100.0;
    public int EggsCollected { get; set; } = 0;
}