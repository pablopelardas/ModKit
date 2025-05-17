using ModKit.Cli.Config;
using Spectre.Console;
using Spectre.Console.Cli;

namespace ModKit.Cli.Commands;

public class DeleteCaseSettings : CommandSettings
{
}
public class DeleteCaseCommand(ModKitConfig config) : Command<DeleteCaseSettings>
{
    public override int Execute(CommandContext context, DeleteCaseSettings settings)
    {
        // Módulos
        var modules = Directory.GetDirectories(config.ModulesPath).Select(Path.GetFileName).OrderBy(x => x).ToList();
        if (modules.Count == 0)
        {
            AnsiConsole.MarkupLine("[red]❌ No hay módulos disponibles.[/]");
            return -1;
        }

        var module = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("📦 Seleccioná un módulo:")
                .PageSize(10)
                .AddChoices(modules));

        var pascalModule = ToPascal(module);
        var appRoot = Path.Combine(config.ModulesPath, pascalModule, $"{config.NamespaceRoot}.{pascalModule}.Application");
        var presRoot = Path.Combine(config.ModulesPath, pascalModule, $"{config.NamespaceRoot}.{pascalModule}.Presentation");

        if (!Directory.Exists(appRoot))
        {
            AnsiConsole.MarkupLine($"[red]❌ No existe el Application path:[/] {appRoot}");
            return -1;
        }

        var submodules = Directory.GetDirectories(appRoot).Select(Path.GetFileName).OrderBy(x => x).ToList();
        if (submodules.Count == 0)
        {
            AnsiConsole.MarkupLine("[red]❌ No hay submódulos disponibles.[/]");
            return -1;
        }

        var submodule = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title($"📁 Submódulos en [blue]{pascalModule}[/]:")
                .PageSize(10)
                .AddChoices(submodules));

        var useCasePath = Path.Combine(appRoot, submodule);
        var useCases = Directory.GetDirectories(useCasePath).Select(Path.GetFileName).OrderBy(x => x).ToList();
        if (useCases.Count == 0)
        {
            AnsiConsole.MarkupLine("[red]❌ No hay casos de uso en el submódulo seleccionado.[/]");
            return -1;
        }

        var useCase = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("🧩 Casos de uso disponibles:")
                .PageSize(10)
                .AddChoices(useCases));

        var appCasePath = Path.Combine(useCasePath, useCase);
        var presCaseFile = Path.Combine(presRoot, submodule, $"{useCase}.cs");

        AnsiConsole.MarkupLine($"[yellow]⚠️ Vas a eliminar:[/]");
        AnsiConsole.MarkupLine($"[grey]📂 {appCasePath}[/]");
        AnsiConsole.MarkupLine($"[grey]📄 {presCaseFile}[/]");

        if (!AnsiConsole.Confirm("¿Estás seguro?"))
        {
            AnsiConsole.MarkupLine("[red]❌ Operación cancelada.[/]");
            return 0;
        }

        Directory.Delete(appCasePath, true);
        if (File.Exists(presCaseFile)) File.Delete(presCaseFile);

        AnsiConsole.MarkupLine($"[green]✅ Caso de uso '[bold]{useCase}[/]' eliminado correctamente de {pascalModule}/{submodule}.[/]");
        return 0;
    }

    private static string ToPascal(string input)
    {
        return string.Concat(input.Split(new[] { '-', '_' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(word => char.ToUpperInvariant(word[0]) + word[1..]));
    }
}
