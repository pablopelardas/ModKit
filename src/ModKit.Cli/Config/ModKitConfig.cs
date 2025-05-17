using System.Text.Json;
using System.Text.Json.Serialization;

namespace ModKit.Cli.Config;

public class ModKitConfig
{
    public string BasePath { get; set; } = ".";
    [JsonPropertyName("ModulesPath")] public string RawModulesPath { get; set; } = Path.Combine("src", "modules");
    [JsonPropertyName("SolutionPath")] public string RawSolutionPath { get; set; } = "Solution.sln";
    public string NamespaceRoot { get; set; } = "Solution.Modules";
    public string TemplatesPath { get; set; } = "../templates";
    [JsonIgnore] public string ModulesPath => Path.GetFullPath(Path.Combine(BasePath, RawModulesPath));
    [JsonIgnore] public string SolutionPath => Path.GetFullPath(Path.Combine(BasePath, RawSolutionPath));

    [JsonIgnore]
    public string UseCaseTemplatesPath => Path.GetFullPath(Path.Combine(BasePath, TemplatesPath, "UseCases"));

    public static ModKitConfig Load(string configPath = "modkit.config.json")
    {
        if (!File.Exists(configPath)) return new ModKitConfig();
        var json = File.ReadAllText(configPath);
        return JsonSerializer.Deserialize<ModKitConfig>(json) ?? new ModKitConfig();
    }
}