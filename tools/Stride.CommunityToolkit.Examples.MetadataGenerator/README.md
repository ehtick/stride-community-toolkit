# Stride Community Toolkit - Examples Metadata Generator

A command-line tool that scans Stride example projects and generates metadata JSON files for documentation and tooling purposes.

## Architecture

This project follows a modern C# 14/.NET 10 console application pattern that integrates:
- **System.CommandLine 2.0** for CLI parsing
- **Microsoft.Extensions.Hosting** for dependency injection
- **Clean separation of concerns** with command handlers as services
- **Constructor injection** throughout for testability

### Pattern: DI-Integrated Command Handlers

Since `System.CommandLine.Hosting` was deprecated, we use a custom integration pattern:

1. **Build the DI container once** at application startup
2. **Register command handlers as scoped services**
3. **Instantiate CLI configuration** with injected `IServiceProvider`
4. **Each command creates a DI scope** per invocation
5. **Handlers receive parsed arguments** and return exit codes

```csharp
// Program.cs - Clean and minimal
var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddScoped<ManifestService>();
using var host = builder.Build();

// Constructor injection for CLI configuration
var cliConfiguration = new CommandLineConfiguration(host.Services);
var rootCommand = cliConfiguration.CreateRootCommand();

var parseResult = rootCommand.Parse(args);
return parseResult.Invoke();
```

### Project Structure

```
Services/
  └─ ManifestService.cs             # Business logic for manifest generation
CommandLineConfiguration.cs         # CLI structure and command setup (instance-based)
MetadataScanner.cs                  # Core scanning logic
Program.cs                          # Entry point (DI setup + execution)
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

1. **Testable**: All classes use constructor injection and can be unit tested
2. **Type-safe**: Strong typing throughout with C# 14 primary constructors
3. **Clean**: Program.cs is minimal (~20 lines); configuration is separate
4. **Maintainable**: Clear separation between CLI setup, DI, and business logic
5. **Modern**: Leverages latest C# and .NET patterns
6. **Extensible**: Easy to add new commands or dependencies
7. **Consistent**: Uses constructor injection everywhere (no static utility classes)

## Design Decisions

### Why Instance-Based `CommandLineConfiguration`?

Instead of a static class, we use constructor injection because:
- **Testability**: Can mock `IServiceProvider` in unit tests
- **Consistency**: Matches the pattern used by command handlers
- **Flexibility**: Easy to inject additional dependencies if needed
- **Best Practice**: Follows SOLID principles and DI conventions

## System.CommandLine 2.0 Notes

- Uses `SetAction` (not deprecated `SetHandler`)
- `Argument<T>` requires explicit name parameter
- `ParseResult.GetValue()` for type-safe value extraction
- `ParseResult.Invoke()` for synchronous command execution
- DI scope created per command invocation for proper resource management
