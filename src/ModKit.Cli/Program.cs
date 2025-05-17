// See https://aka.ms/new-console-template for more information

using ModKit.Cli;
using ModKit.Cli.Commands;
using ModKit.Cli.Config;
using Spectre.Console;
using Spectre.Console.Cli;

Console.OutputEncoding = System.Text.Encoding.UTF8;
// Buscar --config <archivo>
string configFile = "modkit.config.json";
var argsList = args.ToList();

int configIndex = argsList.IndexOf("--config");
if (configIndex >= 0 && argsList.Count > configIndex + 1)
{
    configFile = argsList[configIndex + 1];
    // Eliminar --config y su valor de los argumentos reales que recibe Spectre
    argsList.RemoveAt(configIndex + 1);
    argsList.RemoveAt(configIndex);
}

var config = ModKitConfig.Load(configFile);

var registrar = new TypeRegistrar();
registrar.RegisterInstance(typeof(ModKitConfig), config);

var app = new CommandApp(registrar);
app.Configure(cfg =>
{
    cfg.SetApplicationName("modkit");
    cfg.AddCommand<CreateModuleCommand>("create-module")
        .WithDescription("Crea un nuevo módulo con estructura estándar");
    cfg.AddCommand<DeleteModuleCommand>("delete-module")
        .WithDescription("Elimina un módulo y su carpeta");
    cfg.AddCommand<CreateCaseCommand>("create-case")
        .WithDescription("Crea un nuevo caso de uso con estructura estándar");
    cfg.AddCommand<DeleteCaseCommand>("delete-case")
        .WithDescription("Elimina un caso de uso y su carpeta");
});

// 🧭 Si no hay args → menú interactivo
return await InteractiveShell.RunAsync(app, config);

