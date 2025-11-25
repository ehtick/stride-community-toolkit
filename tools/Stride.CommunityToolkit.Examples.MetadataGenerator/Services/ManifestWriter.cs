using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;

namespace Stride.CommunityToolkit.Examples.MetadataGenerator.Services;

/// <summary>
/// Writes example metadata collections to JSON manifest files.
/// </summary>
public class ManifestWriter(ILogger<ManifestWriter> logger)
{
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
    };

    /// <summary>
    /// Writes the collection of example metadata to a JSON file.
    /// </summary>
    /// <param name="examples">The collection of example metadata to serialize.</param>
    /// <param name="outputPath">The full path where the JSON file should be written.</param>
    public async Task WriteManifestAsync(IEnumerable<ExampleMetadata> examples, string outputPath)
    {
        ArgumentNullException.ThrowIfNull(examples);
        ArgumentException.ThrowIfNullOrWhiteSpace(outputPath);

        logger.LogInformation("Writing manifest to: {OutputPath}", outputPath);

        EnsureOutputDirectoryExists(outputPath);

        var examplesList = examples.ToList();
        var json = JsonSerializer.Serialize(examplesList, _jsonOptions);

        await File.WriteAllTextAsync(outputPath, json, Encoding.UTF8);

        logger.LogInformation("Successfully wrote manifest with {Count} examples", examplesList.Count);
    }

    private void EnsureOutputDirectoryExists(string outputPath)
    {
        var outputDirectory = Path.GetDirectoryName(outputPath);

        if (string.IsNullOrEmpty(outputDirectory))
        {
            return;
        }

        if (!Directory.Exists(outputDirectory))
        {
            logger.LogInformation("Creating output directory: {Directory}", outputDirectory);

            Directory.CreateDirectory(outputDirectory);
        }
    }
}