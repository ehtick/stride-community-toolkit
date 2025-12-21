using System.Text.RegularExpressions;

namespace Stride.CommunityToolkit.Examples.MetadataGenerator.Core;

/// <summary>
/// Provides pattern matching for extracting YAML metadata blocks from Program.cs files.
/// </summary>
/// <remarks>
/// Matches comment blocks in the format: /* ---example-metadata ... --- */
/// </remarks>
public static partial class YamlMetadataExtractor
{
    /// <summary>
    /// Regular expression pattern for extracting YAML metadata from multi-line comments.
    /// </summary>
    /// <returns>A compiled regex that matches YAML metadata blocks within C# multi-line comments.</returns>
    [GeneratedRegex(@"/\*\s*---example-metadata\s*(.*?)\s*---\s*\*/", RegexOptions.Singleline | RegexOptions.Compiled)]
    private static partial Regex MetadataBlockPatternRegex();

    /// <summary>
    /// Gets the compiled regex for matching YAML metadata blocks.
    /// </summary>
    public static Regex MetadataBlockPattern() => MetadataBlockPatternRegex();
}