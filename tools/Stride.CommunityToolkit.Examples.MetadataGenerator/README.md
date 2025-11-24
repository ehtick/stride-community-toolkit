# Stride Community Toolkit - Examples Metadata Generator

A command-line tool that scans Stride example projects and generates metadata JSON files for documentation and tooling purposes.

## Architecture

This project follows a modern C# 14/.NET 10 console application pattern that integrates:
- **System.CommandLine 2.0** for CLI parsing
- **Microsoft.Extensions.Hosting** for dependency injection
- **Clean separation of concerns** with command handlers as services

### Pattern: DI-Integrated Command Handlers

Since `System.CommandLine.Hosting` was deprecated, we use a custom integration pattern:

1. **Build the DI container once** at application startup
2. **Register command handlers as scoped services**
3. **Extract CLI configuration** to a dedicated `CommandLineConfiguration` class
4. **Inline action setup** that creates a DI scope per command invocation
5. **Handlers receive parsed arguments** and return exit codes

```csharp
// Program.cs - Clean and minimal
var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddScoped<ScanCommandHandler>();
using var host = builder.Build();

var rootCommand = CommandLineConfiguration.CreateRootCommand(host.Services);
var parseResult = rootCommand.Parse(args);
return parseResult.Invoke();
```

### Project Structure

```
Commands/
  ├─ ScanCommandHandler.cs           # Scans examples and displays metadata
  └─ GenerateCommandHandler.cs       # Generates JSON manifest file
Services/
  └─ ManifestService.cs              # Business logic for manifest generation
CommandLineConfiguration.cs         # CLI structure and command setup
MetadataScanner.cs                   # Core scanning logic
Program.cs                           # Entry point (DI setup + execution)
```

## Commands

### `scan <examples-root-path>`
Scans example directories and lists discovered metadata.

```bash
MetadataGenerator scan "../../examples/code-only"
# or
dotnet run -- scan "../../examples/code-only"
```

### `generate <examples-root-path>`
Generates a JSON manifest file from example metadata.

```bash
MetadataGenerator generate "../../examples/code-only"
# or
dotnet run -- generate "../../examples/code-only"
```

### Default Path
If no path is provided, defaults to `../../../../../examples/code-only` relative to the tool's location.

## Build Output

The project compiles to `MetadataGenerator.exe` (set via `<AssemblyName>` in the `.csproj`), keeping the namespace as `Stride.CommunityToolkit.Examples.MetadataGenerator`.

## Key Benefits of This Pattern

1. **Testable**: Handlers are injectable services that can be unit tested
2. **Type-safe**: Strong typing throughout with C# 14 primary constructors
3. **Clean**: Program.cs is minimal (~20 lines); configuration is separate
4. **Maintainable**: Clear separation between CLI setup, DI, and business logic
5. **Modern**: Leverages latest C# and .NET patterns
6. **Extensible**: Easy to add new commands by extending `CommandLineConfiguration`

## System.CommandLine 2.0 Notes

- Uses `SetAction` (not deprecated `SetHandler`)
- `Argument<T>` requires explicit name parameter
- `ParseResult.GetValue()` for type-safe value extraction
- `ParseResult.Invoke()` for synchronous command execution
- DI scope created per command invocation for proper resource management
