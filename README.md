# 🧰 ModKit - CLI for Modular .NET Projects

ModKit is a terminal-based CLI tool to scaffold and manage modules and use cases in clean architecture .NET solutions.
![image](https://github.com/user-attachments/assets/cfe677dc-06ed-44e1-9cb8-3bdcac6e79d1)

---

## 🚀 Features

- Create modules with standard structure (`Domain`, `Application`, `Presentation`, `Infrastructure`)
- Generate and remove use cases (Commands or Queries) with validators and endpoints
- Interactive terminal UI with keyboard navigation
- Flexible configuration via JSON file
- Template-based code generation using [Scriban](https://github.com/scriban/scriban)

---

## 📦 Structure of a Generated Module

```
📁 Modules/
  📁 Recipes/
    📁 Recipes.Application/
    📁 Recipes.Domain/
    📁 Recipes.Infrastructure/
    📁 Recipes.Presentation/
```

---

## ⚙️ Installation

### Option 1 - Run Locally

```bash
git clone https://github.com/your-user/modkit.git
cd modkit
dotnet run --project ./ModKit.Cli
```

### Option 2 - Executable

Build a self-contained executable:

```bash
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

The executable will be available in `ModKit.Cli/bin/Release/net8.0/win-x64/publish/`.

> 📌 You can create a shortcut with a predefined config path.

---

## 🔧 Configuration (`modkit.config.json`)

```json
{
  "BasePath": "D:/your-solution",
  "ModulesPath": "src/modules",
  "SolutionPath": "YourSolution.sln",
  "NamespaceRoot": "YourSolution.Modules",
  "TemplatesPath": "Templates"
}
```

All paths are relative to `BasePath` unless absolute.

---

## 🧩 Template System

ModKit uses Scriban templates to generate source files. Expected template files:

```
Templates/
  └── UseCases/
       ├── Command.cs.tpl
       ├── CommandHandler.cs.tpl
       ├── Query.cs.tpl
       ├── QueryHandler.cs.tpl
       ├── Validator.cs.tpl
       └── Endpoint.cs.tpl
```

### Template Variables

| Variable           | Description                             |
|--------------------|-----------------------------------------|
| `UseCase`          | Use case name in PascalCase             |
| `Type`             | `command` or `query`                    |
| `TypeCapital`      | `Command` or `Query`                    |
| `Namespace`        | Fully qualified namespace               |
| `MessagingNamespace` | Namespace of command/query interfaces |
| `DomainNamespace`  | Namespace of domain abstractions        |
| `Module`           | Name of the module                      |
| `Submodule`        | Name of the submodule                   |

---

## 🧪 Usage

```bash
modkit --config path/to/modkit.config.json
```

Use the interactive menu to:

- Create or delete modules
- Generate or remove use cases

---

## 📋 Requirements

- [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
- Windows OS (for .exe usage)

---

## 🛠️ Roadmap

- Cross-platform publishing
- Support for custom layer structures
- Style and naming convention settings

---

## 🧑‍💻 Author

Built with ❤️ by Pablo Pelardas — Licensed under MIT.
