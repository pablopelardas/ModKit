using ModKit.Cli.Config;
using Spectre.Console;
using Spectre.Console.Cli;

namespace ModKit.Cli;

public static class InteractiveShell
{
   public static async Task<int> RunAsync(CommandApp app, ModKitConfig config)
{
    // Título
    AnsiConsole.Write(
        new FigletText("ModKit")
            .Centered()
            .Color(Color.MediumPurple1));

    AnsiConsole.MarkupLine("[bold silver]🔧 CLI para proyectos modulares en[/] [underline silver].NET[/]\n");

    var configStatus = ShowConfiguration(config);

    // ❗ Advertencia si no existe carpeta de templates
    if (!configStatus.TemplatesPathExists)
    {
        AnsiConsole.MarkupLine("[bold red]🚫 No se encontró el directorio de templates:[/] [red italic]{0}[/]", config.UseCaseTemplatesPath);
        AnsiConsole.MarkupLine("[bold red]👉 Los comandos de generación de casos de uso no funcionarán sin esta carpeta.[/]\n");
    }

    // ⚠️ Si no existe el directorio de módulos
    if (!configStatus.ModulesPathExists)
    {
        var create = AnsiConsole.Confirm($"[yellow]⚠️ El directorio de módulos no existe. ¿Querés crearlo en [bold]{config.ModulesPath}[/]?[/]");
        if (create)
        {
            Directory.CreateDirectory(config.ModulesPath);
            AnsiConsole.MarkupLine("[green]✅ Directorio creado correctamente.[/]\n");
        }
        else
        {
            AnsiConsole.MarkupLine("[red]🚫 No se puede continuar sin el directorio de módulos. Abortando...[/]");
            return -1;
        }
    }

    // Menú principal...
    while (true)
    {
        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[bold deepskyblue1]📋 ¿Qué querés hacer?[/]")
                .PageSize(10)
                .UseConverter(item => item)
                .HighlightStyle("bold fuchsia")
                .MoreChoicesText("[grey62](Usá ↑ ↓ para navegar, Enter para seleccionar)[/]")
                .AddChoices([
                    "🧱 Crear módulo",
                    "🗑️ Eliminar módulo",
                    "🧩 Crear caso de uso",
                    "❌ Eliminar caso de uso",
                    "🚪 Salir"
                ]));

        switch (choice)
        {
            case "🧱 Crear módulo":
                var moduleName = AnsiConsole.Ask<string>("[cyan]📦 Nombre del nuevo módulo:[/]");
                await app.RunAsync(["create-module", moduleName]);
                break;

            case "🗑️ Eliminar módulo":
                await app.RunAsync(["delete-module"]);
                break;

            case "🧩 Crear caso de uso":
                await app.RunAsync(["create-case"]);
                break;

            case "❌ Eliminar caso de uso":
                await app.RunAsync(["delete-case"]);
                break;

            case "🚪 Salir":
            default:
                AnsiConsole.MarkupLine("\n[green]👋 ¡Hasta luego![/]");
                return 0;
        }

        AnsiConsole.WriteLine();
    }
}

    private record ConfigStatus(bool ModulesPathExists, bool SolutionPathExists, bool TemplatesPathExists);

    private static ConfigStatus ShowConfiguration(ModKitConfig config)
    {
        var table = new Table()
            .Border(TableBorder.Rounded)
            .Title("[bold underline lime]🛠️ Configuración actual[/]")
            .AddColumn("[bold yellow]🔑 Clave[/]")
            .AddColumn("[bold white]📍 Valor[/]");

        var basePath = config.BasePath;
        var modulesPath = config.ModulesPath;
        var solutionPath = config.SolutionPath;
        var templatesPath = config.UseCaseTemplatesPath;
        var namespaceRoot = config.NamespaceRoot;

        bool modulesExists = Directory.Exists(modulesPath);
        bool solutionExists = File.Exists(solutionPath);
        bool templatesExists = Directory.Exists(templatesPath);

        table.AddRow("📁 BasePath", MarkPath(basePath));
        table.AddRow("📂 ModulesPath", MarkPath(modulesPath));
        table.AddRow("📄 SolutionPath", MarkPath(solutionPath));
        table.AddRow("🧩 TemplatesPath", MarkPath(templatesPath));
        table.AddRow("🧪 NamespaceRoot", $"[cyan]{namespaceRoot}[/]");

        AnsiConsole.Write(table);
        AnsiConsole.WriteLine();

        return new ConfigStatus(modulesExists, solutionExists, templatesExists);
    }

    private static string MarkPath(string path)
    {
        return File.Exists(path) || Directory.Exists(path)
            ? $"[green]{path}[/]"
            : $"[red]⚠️ {path}[/]";
    }
}
