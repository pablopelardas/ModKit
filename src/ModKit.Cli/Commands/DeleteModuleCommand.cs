using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using ModKit.Cli.Config;

namespace ModKit.Cli.Commands;

public class DeleteModuleSettings : CommandSettings
{
    // En este caso no necesita argumentos
}
public class DeleteModuleCommand(ModKitConfig config) : Command<DeleteModuleSettings>
{
    public override int Execute(CommandContext context, DeleteModuleSettings settings)
    {
        // 📦 Listar módulos disponibles en ModulesPath
        if (!Directory.Exists(config.ModulesPath))
        {
            AnsiConsole.MarkupLine($"[red]❌ No se encontró el directorio de módulos en [bold]{config.ModulesPath}[/][/]");
            return -1;
        }

        var modules = Directory.GetDirectories(config.ModulesPath)
            .Select(Path.GetFileName)
            .OrderBy(x => x)
            .ToList();

        if (!modules.Any())
        {
            AnsiConsole.MarkupLine("[red]⚠️ No hay módulos para eliminar.[/]");
            return -1;
        }

        var selected = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[yellow]Seleccioná el módulo que querés eliminar:[/]")
                .PageSize(10)
                .AddChoices(modules));

        var modulePath = Path.Combine(config.ModulesPath, selected);

        if (!AnsiConsole.Confirm($"[red]¿Estás seguro que querés eliminar el módulo [bold]{selected}[/]? Esta acción es irreversible.[/]"))
        {
            AnsiConsole.MarkupLine("[grey]❌ Eliminación cancelada por el usuario.[/]");
            return 0;
        }

        // 🔧 Remover los proyectos del .sln
        var csprojs = Directory.GetFiles(modulePath, "*.csproj", SearchOption.AllDirectories);

        if (csprojs.Length > 0)
        {
            AnsiConsole.MarkupLine("[blue]🧹 Quitando proyectos de la solución...[/]");

            foreach (var proj in csprojs)
            {
                Run($"dotnet sln \"{config.SolutionPath}\" remove \"{proj}\"");
            }
        }

        // 🗑️ Eliminar el módulo
        Directory.Delete(modulePath, true);
        AnsiConsole.MarkupLine($"[green]✅ Módulo [bold]{selected}[/] eliminado correctamente.[/]");

        return 0;
    }

    private void Run(string command)
    {
        var parts = command.Split(' ');
        var process = new System.Diagnostics.Process
        {
            StartInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = parts[0],
                Arguments = string.Join(' ', parts[1..]),
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false
            }
        };

        process.Start();
        process.WaitForExit();
    }
}
