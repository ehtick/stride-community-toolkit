using Microsoft.Extensions.Logging;
using Stride.CommunityToolkit.Examples.MetadataGenerator.Core;
using System.Text;
using YamlDotNet.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Stride.CommunityToolkit.Examples.MetadataGenerator.Services;

/// <summary>
/// Parses YAML metadata from example project files.
/// </summary>
public class MetadataParser(ILogger<MetadataParser> logger)
{
    private readonly IDeserializer _yamlDeserializer = new DeserializerBuilder()
        .WithNamingConvention(CamelCaseNamingConvention.Instance)
        .IgnoreUnmatchedProperties()
        .Build();

    /// <summary>
    /// Extracts and parses metadata from a Program.cs file.
    /// </summary>
    /// <param name="programFilePath">The full path to the Program.cs file.</param>
    /// <param name="examplesRootPath">The root examples directory for calculating relative paths.</param>
    /// <returns>The parsed metadata, or null if no metadata block was found.</returns>
    public async Task<ExampleMetadata?> ParseMetadataAsync(string programFilePath, string examplesRootPath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(programFilePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(examplesRootPath);

        var projectName = Path.GetFileName(Path.GetDirectoryName(programFilePath));
        logger.LogDebug("Parsing metadata from: {ProjectName}", projectName);

        var content = await File.ReadAllTextAsync(programFilePath);
        var match = YamlMetadataExtractor.MetadataBlockPattern().Match(content);

        if (!match.Success)
        {
            logger.LogDebug("No metadata block found in: {ProjectName}", projectName);
            return null;
        }

        var yamlContent = match.Groups[1].Value.Trim();

        try
        {
            var metadata = _yamlDeserializer.Deserialize<ExampleMetadata>(yamlContent);

            if (metadata is not null)
            {
                var projectDirectory = Path.GetDirectoryName(programFilePath);
                metadata.ProjectName = Path.GetFileName(projectDirectory);
                metadata.ProjectPath = Path.GetRelativePath(examplesRootPath, programFilePath);

                logger.LogDebug("Successfully parsed metadata for: {ProjectName}", metadata.ProjectName);
            }

            return metadata;
        }
        catch (YamlException ex)
        {
            var errorMessage = BuildYamlErrorMessage(programFilePath, yamlContent, ex);

            logger.LogError(ex, "YAML parsing failed for: {ProjectName}", projectName);

            throw new InvalidOperationException(errorMessage, ex);
        }
        catch (Exception ex)
        {
            var errorMessage = BuildGenericErrorMessage(programFilePath, yamlContent, ex);

            logger.LogError(ex, "Unexpected error parsing metadata for: {ProjectName}", projectName);

            throw new InvalidOperationException(errorMessage, ex);
        }
    }

    private static string BuildYamlErrorMessage(string programFilePath, string yamlContent, YamlException ex)
    {
        var errorMessage = new StringBuilder();
        errorMessage.AppendLine($"Failed to parse YAML metadata in {Path.GetFileName(programFilePath)}");
        errorMessage.AppendLine($"Error at Line {ex.End.Line}, Column {ex.End.Column}");
        errorMessage.AppendLine();
        errorMessage.AppendLine("YAML Content:");
        errorMessage.AppendLine("---");
        errorMessage.AppendLine(yamlContent);
        errorMessage.AppendLine("---");
        errorMessage.AppendLine();
        errorMessage.AppendLine($"Error: {ex.Message}");

        return errorMessage.ToString();
    }

    private static string BuildGenericErrorMessage(string programFilePath, string yamlContent, Exception ex)
    {
        var errorMessage = new StringBuilder();
        errorMessage.AppendLine($"Unexpected error while processing {Path.GetFileName(programFilePath)}");
        errorMessage.AppendLine();
        errorMessage.AppendLine("YAML Content:");
        errorMessage.AppendLine("---");
        errorMessage.AppendLine(yamlContent);
        errorMessage.AppendLine("---");
        errorMessage.AppendLine();
        errorMessage.AppendLine($"Error: {ex.Message}");

        return errorMessage.ToString();
    }
}