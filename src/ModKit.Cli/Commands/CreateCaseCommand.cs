using ModKit.Cli.Config;
using Scriban;
using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;

namespace ModKit.Cli.Commands;

public class CreateCaseSettings : CommandSettings
{
    [CommandOption("-m|--module <ModuleName>")]
    [Description("Nombre del módulo donde crear el caso de uso (opcional si se usa en modo interactivo)")]
    public string? ModuleName { get; set; }
}

public class CreateCaseCommand(ModKitConfig config) : Command<CreateCaseSettings>
{
    public override int Execute(CommandContext context, CreateCaseSettings settings)
    {
        var module = settings.ModuleName;

        var modules = Directory.Exists(config.ModulesPath)
            ? Directory.GetDirectories(config.ModulesPath).Select(Path.GetFileName).OrderBy(x => x).ToList()
            : [];

        modules.Add("➕ Crear uno nuevo...");

        if (string.IsNullOrWhiteSpace(module))
        {
            var selected = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("📦 Seleccioná un módulo:")
                    .PageSize(10)
                    .AddChoices(modules));

            if (selected == "➕ Crear uno nuevo...")
            {
                var newModule = AnsiConsole.Ask<string>("🆕 Ingresá el nombre del nuevo módulo:");
                AnsiConsole.MarkupLine($"[blue]⚙️ Ejecutando 'create-module {newModule}'...[/]");

                var createCommand = new CreateModuleCommand(config);
                var result = createCommand.Execute(context, new CreateModuleSettings { ModuleName = newModule });
                if (result != 0)
                    return result;

                module = newModule;
            }
            else
            {
                module = selected;
            }
        }

        var pascalModule = ToPascal(module!);

        var appNsRoot = $"{config.NamespaceRoot}.{pascalModule}.Application";
        var appRoot = Path.Combine(config.ModulesPath, pascalModule, $"{config.NamespaceRoot}.{pascalModule}.Application");
        var presRoot = Path.Combine(config.ModulesPath, pascalModule, $"{config.NamespaceRoot}.{pascalModule}.Presentation");

        var submodules = Directory.Exists(appRoot)
            ? Directory.GetDirectories(appRoot).Select(Path.GetFileName).OrderBy(x => x).ToList()
            : [];

        submodules.Add("➕ Crear uno nuevo...");

        var submodule = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title($"📁 Submódulos disponibles en [blue]{pascalModule}[/]:")
                .PageSize(10)
                .AddChoices(submodules));

        if (submodule == "➕ Crear uno nuevo...")
        {
            var nuevo = AnsiConsole.Ask<string>("🆕 Ingresá el nombre del nuevo submódulo:");
            submodule = ToPascal(nuevo);
            Directory.CreateDirectory(Path.Combine(appRoot, submodule));
            Directory.CreateDirectory(Path.Combine(presRoot, submodule));
            AnsiConsole.MarkupLine($"[green]🗂️ Submódulo '{submodule}' creado.[/]");
        }

        var useCase = ToPascal(AnsiConsole.Ask<string>("🧩 Nombre del caso de uso (ej: ConsumeSupply):"));
        var type = AnsiConsole.Prompt(new SelectionPrompt<string>()
            .Title("❓ Tipo")
            .AddChoices(["command", "query"]));

        var typeCapital = type[..1].ToUpper() + type[1..];

        var appPath = Path.Combine(appRoot, submodule, useCase);
        var presPath = Path.Combine(presRoot, submodule);
        Directory.CreateDirectory(appPath);
        Directory.CreateDirectory(presPath);

        var templateData = new
        {
            Namespace = $"{appNsRoot}.{submodule}.{useCase}",
            UseCase = useCase,
            MessagingNamespace = $"{appNsRoot}.Abstractions.Messaging",
            DomainNamespace = $"{config.NamespaceRoot}.{pascalModule}.Domain.Abstractions",
            Module = pascalModule,
            Submodule = submodule,
            Type = type.ToLower(),
            TypeCapital = typeCapital
        };

        var templateFiles = new[]
        {
            ($"{typeCapital}.cs.tpl", $"{useCase}{typeCapital}.cs", appPath),
            ($"{typeCapital}Handler.cs.tpl", $"{useCase}{typeCapital}Handler.cs", appPath),
            ("Validator.cs.tpl", $"{useCase}Validator.cs", appPath),
            ("Endpoint.cs.tpl", $"{useCase}.cs", presPath)
        };

        var missingTemplates = templateFiles
            .Where(t => !File.Exists(Path.Combine(config.UseCaseTemplatesPath, t.Item1)))
            .ToList();

        if (missingTemplates.Any())
        {
            AnsiConsole.MarkupLine("[red]❌ No se encontraron los siguientes templates necesarios:[/]");
            foreach (var (templateFile, _, _) in missingTemplates)
            {
                var path = Path.Combine(config.UseCaseTemplatesPath, templateFile);
                AnsiConsole.MarkupLine($"  [red]• {path}[/]");
            }

            return -1;
        }

        foreach (var (templateFile, outputFile, outputDir) in templateFiles)
        {
            var templatePath = Path.Combine(config.UseCaseTemplatesPath, templateFile);
            var templateText = File.ReadAllText(templatePath);
            var template = Template.Parse(templateText);
            var content = template.Render(templateData, member => member.Name);
            File.WriteAllText(Path.Combine(outputDir, outputFile), content);
        }

        AnsiConsole.MarkupLine($"[green]✅ Archivos generados para '{useCase}' en módulo '{pascalModule}/{submodule}'.[/]");
        return 0;
    }

    private static string ToPascal(string input)
    {
        return string.Concat(input.Split(new[] { '-', '_' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(word => char.ToUpperInvariant(word[0]) + word[1..]));
    }
}
