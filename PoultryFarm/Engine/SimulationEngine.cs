using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PoultryFarm.Domain;

namespace PoultryFarm.Engine;

public class SimulationEngine
{
    private readonly List<Zone> _zones;
    
    public double TimeScaleMultiplier { get; set; } 

    public SimulationEngine(List<Zone> zones, double timeScaleMultiplier)
    {
        _zones = zones;
        TimeScaleMultiplier = timeScaleMultiplier;
    }

    public async Task StartAsync(CancellationToken token)
    {
        DateTime lastUpdate = DateTime.Now;

        while (!token.IsCancellationRequested)
        {
            DateTime now = DateTime.Now;
            TimeSpan delta = now - lastUpdate;
            lastUpdate = now;

            // Використовуємо поточний множник часу
            double simulatedDaysPassed = delta.TotalDays * TimeScaleMultiplier;

            List<Zone> currentZones;
            lock (_zones) { currentZones = _zones.ToList(); }

            foreach (var zone in currentZones)
            {
                zone.Tick(simulatedDaysPassed);
            }

            await Task.Delay(100, token);
        }
    }
}