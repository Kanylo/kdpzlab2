using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PoultryFarm.Domain;
using PoultryFarm.Components;
using PoultryFarm.Engine;
using PoultryFarm.UI;

namespace PoultryFarm;

class Program
{
    static async Task Main(string[] args)
    {
        var auth = new AuthService();
        User? user = null;
        while (user == null) { user = auth.Login(); }

        var climate = new BasicClimateControl();
        var eggCollector = new AutomatedEggCollector();

        var zones = new List<Zone>
        {
            new Zone(1, ZonePurpose.Broilers, 500, climate),
            new Zone(2, ZonePurpose.Layers, 300, climate, eggCollector)
        };

        var engine = new SimulationEngine(zones, timeScaleMultiplier: 172800); 
        
        // 5. Передаємо engine в DashboardView
        var dashboard = new DashboardView(zones, user, engine);

        using var cts = new CancellationTokenSource();

        // 6. Запуск багатопоточної системи!
        Task simTask = engine.StartAsync(cts.Token);
        Task uiTask = dashboard.RenderDashboardAsync(cts);

        try
        {
            await Task.WhenAll(simTask, uiTask);
        }
        catch (TaskCanceledException)
        {
            // Нормальне завершення
        }
        catch (Exception ex)
        {
            Console.Clear();
            Console.WriteLine($"Критична помилка системи:\n{ex.Message}");
        }

        Console.Clear();
        Console.WriteLine("Система безпечно зупинена. Всі дані збережено в пам'яті (до вимкнення).");
}
}