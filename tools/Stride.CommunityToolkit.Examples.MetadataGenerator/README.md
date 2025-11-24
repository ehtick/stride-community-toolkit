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
3. **Inline action setup** that creates a DI scope per command invocation
4. **Handlers receive parsed arguments** and return exit codes

```csharp
// Register handlers
builder.Services.AddScoped<ScanCommandHandler>();

// Setup command with DI integration
scanCommand.SetAction(async (parseResult, cancellationToken) =>
{
    using var scope = host.Services.CreateScope();
    var handler = scope.ServiceProvider.GetRequiredService<ScanCommandHandler>();
    var path = parseResult.GetValue(pathArgument);
    return await handler.HandleAsync(path);
});
```

### Project Structure

```
Commands/
  ├─ ScanCommandHandler.cs      # Scans examples and displays metadata
  └─ GenerateCommandHandler.cs  # Generates JSON manifest file
Services/
  └─ ManifestService.cs         # Business logic for manifest generation
MetadataScanner.cs              # Core scanning logic
Program.cs                      # Entry point with CLI setup
```

## Commands

### `scan <examples-root-path>`
Scans example directories and lists discovered metadata.

```bash
dotnet run -- scan "../../examples/code-only"
```

### `generate <examples-root-path>`
Generates a JSON manifest file from example metadata.

```bash
dotnet run -- generate "../../examples/code-only"
```

### Default Path
If no path is provided, defaults to `../../examples/code-only` relative to the tool's location.

## Key Benefits of This Pattern

1. **Testable**: Handlers are injectable services that can be unit tested
2. **Type-safe**: Strong typing throughout with C# 14 primary constructors
3. **Clean**: No custom base classes or complex extension methods
4. **Modern**: Leverages latest C# and .NET patterns
5. **Maintainable**: Clear separation between CLI setup and business logic

## System.CommandLine 2.0 Notes

- Uses `SetAction` (not deprecated `SetHandler`)
- `Argument<T>` requires explicit name parameter
- `ParseResult.GetValue()` for type-safe value extraction
- `ParseResult.Invoke()` for synchronous command execution
