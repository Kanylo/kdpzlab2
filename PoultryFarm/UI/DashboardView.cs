using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Spectre.Console;
using PoultryFarm.Domain;
using PoultryFarm.Components;
using PoultryFarm.Engine; // Додано для доступу до SimulationEngine

namespace PoultryFarm.UI;

public class DashboardView
{
    private readonly List<Zone> _zones;
    private readonly User _currentUser;
    private readonly SimulationEngine _engine; // Двигун для керування часом

    // Стан для повідомлень
    private string _statusMessage = "[grey]Очікування команд...[/]";
    private DateTime _statusClearTime = DateTime.MinValue;

    public DashboardView(List<Zone> zones, User currentUser, SimulationEngine engine)
    {
        _zones = zones;
        _currentUser = currentUser;
        _engine = engine;
    }

    public async Task RenderDashboardAsync(CancellationTokenSource cts)
    {
        var table = new Table();
        UpdateTableHeaders(table);
        PopulateTable(table);

        AnsiConsole.Clear();

        try
        {
            await AnsiConsole.Live(table)
                .Overflow(VerticalOverflow.Visible)
                .StartAsync(async ctx =>
                {
                    while (!cts.Token.IsCancellationRequested)
                    {
                        PopulateTable(table);
                        
                        // Логіка зникнення повідомлень через 3 секунди
                        if (DateTime.Now > _statusClearTime)
                        {
                            _statusMessage = "[grey]Очікування команд...[/]";
                        }
                        // Додаємо статус-бар внизу таблиці
                        table.Caption = new TableTitle(_statusMessage);

                        ctx.Refresh();

                        try
                        {
                            if (Console.KeyAvailable)
                            {
                                var key = Console.ReadKey(intercept: true).Key;
                                ProcessInput(key, cts);
                            }
                        }
                        catch (InvalidOperationException) { }

                        await Task.Delay(300, cts.Token);
                    }
                });
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteException(ex);
        }
    }

    private void PopulateTable(Table table)
    {
        table.Rows.Clear();
        List<Zone> zonesCopy;
        
        lock (_zones) { zonesCopy = _zones.ToList(); }

        foreach (var zone in zonesCopy)
        {
            var snap = zone.GetSnapshot();
            string healthColor = snap.Health > 80 ? "green" : snap.Health > 40 ? "yellow" : "red";
            string resourceColor = (snap.Food < snap.Count * 0.5 || snap.Water < snap.Count * 1.0) ? "red" : "white";

            table.AddRow(
                $"[bold cyan]Зона {snap.Id}[/] ({snap.Purpose})",
                $"{snap.AgeInDays:F1}",
                $"[{healthColor}]{snap.Health:F0}%[/]",
                snap.FeedType,
                $"[{resourceColor}]{snap.Food:F1} кг / {snap.Water:F1} л[/]",
                snap.Purpose == ZonePurpose.Layers ? $"[gold1]{snap.Eggs}[/]" : "[grey]N/A[/]"
            );
        }
    }

    private void UpdateTableHeaders(Table table)
    {
        if (table.Columns.Count == 0)
        {
            table.AddColumn("Зона / Тип");
            table.AddColumn("Вік (Днів)");
            table.AddColumn("Здоров'я");
            table.AddColumn("Раціон");
            table.AddColumn("Їжа / Вода");
            table.AddColumn("Яйця");
        }
        
        string adminCmds = _currentUser.Role == UserRole.Admin ? "[green][[A]][/] Додати зону | " : "";
        table.Title = new TableTitle($"[bold yellow]Птахоферма. Користувач: {_currentUser.Username}[/]\n" +
                                     $"[grey]{adminCmds}[blue][[F]][/] Корм | [blue][[W]][/] Вода | [yellow][[-]][[+]][/] Швидкість | [red][[Q]][/] Вихід[/]");
    }

    // Метод для показу транзитних повідомлень
    private void ShowStatus(string message)
    {
        _statusMessage = message;
        _statusClearTime = DateTime.Now.AddSeconds(3); // Зникне через 3 сек
    }

    private void ProcessInput(ConsoleKey key, CancellationTokenSource cts)
    {
        if (key == ConsoleKey.Q) cts.Cancel();
        
        if (key == ConsoleKey.F)
        {
            lock (_zones) { foreach (var z in _zones) z.RefillResources(500, 0); }
            ShowStatus("[bold green]✔ Всі зони успішно нагодовані (+500 кг корму).[/]");
        }
        
        if (key == ConsoleKey.W)
        {
            lock (_zones) { foreach (var z in _zones) z.RefillResources(0, 1000); }
            ShowStatus("[bold blue]✔ Воду успішно поповнено (+1000 л).[/]");
        }

        if (key == ConsoleKey.A && _currentUser.Role == UserRole.Admin)
        {
            lock (_zones)
            {
                int newId = _zones.Count > 0 ? _zones.Max(z => z.Id) + 1 : 1;
                _zones.Add(new Zone(newId, ZonePurpose.Broilers, 200, new BasicClimateControl()));
            }
            ShowStatus($"[bold cyan]✔ Створено нову зону #{_zones.Count}[/]");
        }

        // Обробка швидкості часу (з обмеженнями, щоб не зламати математику)
        if (key == ConsoleKey.OemPlus || key == ConsoleKey.Add)
        {
            _engine.TimeScaleMultiplier = Math.Min(_engine.TimeScaleMultiplier * 2, 5000000);
            ShowStatus($"[bold yellow]⏩ Час ПРИСКОРЕНО! Множник: {_engine.TimeScaleMultiplier}[/]");
        }
        if (key == ConsoleKey.OemMinus || key == ConsoleKey.Subtract)
        {
            _engine.TimeScaleMultiplier = Math.Max(_engine.TimeScaleMultiplier / 2, 1000);
            ShowStatus($"[bold yellow]⏪ Час УПОВІЛЬНЕНО! Множник: {_engine.TimeScaleMultiplier}[/]");
        }
    }
}