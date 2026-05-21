using Spectre.Console;
using PoultryFarm.Domain;

namespace PoultryFarm.UI;

public class AuthService
{
    public User? Login()
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(new FigletText("Poultry OS").Centered().Color(Color.Green));

        var roleChoice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Оберіть роль для входу:")
                .AddChoices(new[] { "Admin", "Worker" }));

        var password = AnsiConsole.Prompt(
            new TextPrompt<string>("Введіть пароль:")
                .Secret());

        if (roleChoice == "Admin" && password == "admin") return new User("Головний Інженер", UserRole.Admin);
        if (roleChoice == "Worker" && password == "1234") return new User("Оператор Зони", UserRole.Worker);

        AnsiConsole.MarkupLine("[red]Невірний пароль![/]");
        return null;
    }
}