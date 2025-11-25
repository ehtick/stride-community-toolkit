using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;
using YamlDotNet.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Stride.CommunityToolkit.Examples.MetadataGenerator.Services;

public class ManifestService(ILogger<ManifestService> logger)
{
    private readonly ILogger<ManifestService> _logger = logger;

    public Task<int> GenerateManifestAsync(DirectoryInfo? examplesRootPath)
    {
        if (examplesRootPath is null)
        {
            _logger.LogError("Path not provided");

            return Task.FromResult(0);
        }

        _logger.LogInformation("Executing generate command...");
        _logger.LogInformation("Examples root path: {Path}", examplesRootPath.FullName);
        _logger.LogInformation("Generating manifest...");

        return Task.FromResult(0);
    }

    public async Task<List<ExampleMetadata>> ScanExamplesAsync(DirectoryInfo? examplesRootPath)
    {
        _logger.LogInformation("Executing scan command...");
        _logger.LogInformation("Examples root path: {Path}, exists: {Exists}",
            examplesRootPath.FullName, examplesRootPath.Exists);

        if (!examplesRootPath.Exists)
        {
            logger.LogError("Examples directory does not exist: {Path}", examplesRootPath.FullName);

            return [];
        }

        var examplesRoot = examplesRootPath.FullName;
        var examples = new List<ExampleMetadata>();
        var programFiles = Directory.GetFiles(examplesRoot, "Program.cs", SearchOption.AllDirectories);

        _logger.LogInformation("Scanning {length} Program.cs files.", programFiles.Length);

        foreach (var programFile in programFiles)
        {
            _logger.LogInformation("Processing {exampleName}", Path.GetFileName(Path.GetDirectoryName(programFile)));

            try
            {
                var metadata = await ExtractMetadata(programFile, examplesRoot);

                if (metadata != null)
                {
                    examples.Add(metadata);
                    _logger.LogInformation(" ✅ {metadata}", metadata.ProjectName);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ✗ {Path.GetFileName(Path.GetDirectoryName(programFile))}: {ex.Message}");
            }
        }

        Console.WriteLine($"\nFound {examples.Count} examples with metadata.");

        return examples;
    }

    public async Task<int> ScanAndGenerateAsync(DirectoryInfo examplesRootPath, string outputPath)
    {
        _logger.LogInformation("Executing scan command...");
        _logger.LogInformation("Examples root path: {Path}, exists: {Exists}",
            examplesRootPath.FullName, examplesRootPath.Exists);

        if (!examplesRootPath.Exists)
        {
            logger.LogError("Directory does not exist: {Path}", examplesRootPath.FullName);

            return 1;
        }

        var examples = await ScanExamplesAsync(examplesRootPath);

        if (examples.Count > 0)
        {
            await WriteManifestAsync(examples, outputPath);

            Console.WriteLine($"Manifest written to: {outputPath}");
        }
        else
        {
            Console.WriteLine("No manifest written - no examples with metadata found.");
        }

        return 0;
    }



    private async Task WriteManifestAsync(List<ExampleMetadata> examples, string outputPath)
    {
        var outputDir = Path.GetDirectoryName(outputPath);

        if (!string.IsNullOrEmpty(outputDir) && !Directory.Exists(outputDir))
        {
            Directory.CreateDirectory(outputDir);
        }

        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };

        var json = JsonSerializer.Serialize(examples, options);

        await File.WriteAllTextAsync(outputPath, json, Encoding.UTF8);
    }

    private async Task<ExampleMetadata?> ExtractMetadata(string programFile, string examplesRoot)
    {
        var content = await File.ReadAllTextAsync(programFile);
        var match = MetadataScanner.YamlBlockRegex().Match(content);

        if (!match.Success) return null;

        var yamlContent = match.Groups[1].Value.Trim();

        try
        {
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .IgnoreUnmatchedProperties()
                .Build();

            var metadata = deserializer.Deserialize<ExampleMetadata>(yamlContent);

            if (metadata != null)
            {
                var projectDir = Path.GetDirectoryName(programFile);
                metadata.ProjectName = Path.GetFileName(projectDir);
                metadata.ProjectPath = Path.GetRelativePath(examplesRoot, programFile);
            }

            return metadata;
        }
        catch (YamlException ex)
        {
            var errorMessage = new StringBuilder();
            errorMessage.AppendLine($"Failed to parse YAML metadata in {Path.GetFileName(programFile)}");
            errorMessage.AppendLine($"Error at Line {ex.End.Line}, Column {ex.End.Column}");
            errorMessage.AppendLine();
            errorMessage.AppendLine("YAML Content:");
            errorMessage.AppendLine("---");
            errorMessage.AppendLine(yamlContent);
            errorMessage.AppendLine("---");
            errorMessage.AppendLine();
            errorMessage.AppendLine($"Error: {ex.Message}");

            throw new InvalidOperationException(errorMessage.ToString(), ex);
        }
        catch (Exception ex)
        {
            var errorMessage = new StringBuilder();
            errorMessage.AppendLine($"Unexpected error while processing {Path.GetFileName(programFile)}");
            errorMessage.AppendLine();
            errorMessage.AppendLine("YAML Content:");
            errorMessage.AppendLine("---");
            errorMessage.AppendLine(yamlContent);
            errorMessage.AppendLine("---");
            errorMessage.AppendLine();
            errorMessage.AppendLine($"Error: {ex.Message}");

            throw new InvalidOperationException(errorMessage.ToString(), ex);
        }
    }
}