// Commands/CreateModuleCommand.cs
using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using ModKit.Cli.Config;

namespace ModKit.Cli.Commands;

public class CreateModuleSettings : CommandSettings
{
    [CommandArgument(0, "<ModuleName>")]
    [Description("Nombre del módulo a crear (ej: Recipes)")]
    public string ModuleName { get; set; } = default!;
}

public class CreateModuleCommand(ModKitConfig config) : Command<CreateModuleSettings>
{
    public override int Execute(CommandContext context, CreateModuleSettings settings)
    {
        var moduleName = settings.ModuleName;

        var modulesPath = config.ModulesPath;
        var solutionPath = config.SolutionPath;

        var modulePath = Path.Combine(modulesPath, moduleName);
        var appNamespace = $"{config.NamespaceRoot}.{moduleName}";

        // 🔍 Validar existencia de solution file
        if (!File.Exists(solutionPath))
        {
            AnsiConsole.MarkupLine($"[red]❌ No se encontró la solución en [bold]{solutionPath}[/].[/]");
            return -1;
        }

        // 🔍 Validar existencia de modulesPath
        if (!Directory.Exists(modulesPath))
        {
            AnsiConsole.MarkupLine($"[yellow]⚠️ No existe el directorio de módulos, creando: [bold]{modulesPath}[/][/]");
            Directory.CreateDirectory(modulesPath);
        }

        // ⚠️ Validar si ya existe el módulo
        if (Directory.Exists(modulePath))
        {
            AnsiConsole.MarkupLine($"[yellow]⚠️ El módulo '{moduleName}' ya existe en {modulePath}. Cancelando.[/]");
            return -1;
        }

        AnsiConsole.MarkupLine($"[green]📁 Creando módulo en:[/] {modulePath}");

        var layers = new[] { "Domain", "Application", "Presentation", "Infrastructure" };

        foreach (var layer in layers)
        {
            var fullPath = Path.Combine(modulePath, $"{appNamespace}.{layer}");
            Directory.CreateDirectory(fullPath);
            Run($"dotnet new classlib -n {appNamespace}.{layer} -o {fullPath}");
        }

        AnsiConsole.MarkupLine("[blue]➕ Agregando proyectos a la solución...[/]");

        foreach (var layer in layers)
        {
            var projPath = Path.Combine(modulePath, $"{appNamespace}.{layer}", $"{appNamespace}.{layer}.csproj");
            Run($"dotnet sln \"{solutionPath}\" add \"{projPath}\"");
        }

        AnsiConsole.MarkupLine("[yellow]🔗 Agregando referencias...[/]");
        string GetProj(string layer) =>
            Path.Combine(modulePath, $"{appNamespace}.{layer}", $"{appNamespace}.{layer}.csproj");

        Run($"dotnet add \"{GetProj("Application")}\" reference \"{GetProj("Domain")}\"");
        Run($"dotnet add \"{GetProj("Presentation")}\" reference \"{GetProj("Application")}\"");
        Run($"dotnet add \"{GetProj("Infrastructure")}\" reference \"{GetProj("Application")}\"");
        Run($"dotnet add \"{GetProj("Infrastructure")}\" reference \"{GetProj("Presentation")}\"");


        AnsiConsole.MarkupLine($"[green]✅ Módulo '{moduleName}' creado con éxito.[/]");
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
